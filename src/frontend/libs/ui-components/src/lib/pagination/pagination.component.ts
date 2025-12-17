import { Component, Input, Output, EventEmitter, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';

/**
 * Reusable pagination component
 * Displays page navigation with ellipsis for large page ranges
 */
@Component({
  selector: 'ui-pagination',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css'
})
export class PaginationComponent {
  /** Number of pages to display before and after the current page */
  private static readonly PAGES_AROUND_CURRENT = 2;

  /**
   * Current page number (1-indexed)
   */
  @Input({ required: true })
  set currentPage(value: number) {
    this._currentPage.set(value);
  }
  private readonly _currentPage = signal(1);

  /**
   * Total number of pages
   */
  @Input({ required: true })
  set totalPages(value: number) {
    this._totalPages.set(value);
  }
  private readonly _totalPages = signal(1);

  /**
   * Total count of items (optional, for display)
   */
  @Input() totalItems?: number;

  /**
   * Items per page (optional, for display)
   */
  @Input() pageSize?: number;

  /**
   * Label for previous button
   */
  @Input() previousLabel = 'Zurück';

  /**
   * Label for next button
   */
  @Input() nextLabel = 'Weiter';

  /**
   * Show item range info (e.g., "Showing 1-10 of 100")
   */
  @Input() showItemRange = false;

  /**
   * Item label for range display (singular)
   */
  @Input() itemLabel = 'Eintrag';

  /**
   * Item label for range display (plural)
   */
  @Input() itemLabelPlural = 'Einträge';

  /**
   * Emitted when page changes
   */
  @Output() pageChange = new EventEmitter<number>();

  /**
   * Current page signal for template
   */
  protected readonly page = computed(() => this._currentPage());

  /**
   * Total pages signal for template
   */
  protected readonly total = computed(() => this._totalPages());

  /**
   * Calculate visible page numbers
   */
  protected readonly visiblePages = computed(() => {
    const current = this._currentPage();
    const total = this._totalPages();
    const pages: number[] = [];

    for (let i = current - PaginationComponent.PAGES_AROUND_CURRENT; i <= current + PaginationComponent.PAGES_AROUND_CURRENT; i++) {
      if (i > 0 && i <= total) {
        pages.push(i);
      }
    }

    return pages;
  });

  /**
   * Check if we should show first page and leading ellipsis
   */
  protected readonly showLeadingEllipsis = computed(() => this._currentPage() > PaginationComponent.PAGES_AROUND_CURRENT + 1);
  protected readonly showLeadingFirstPage = computed(() => this._currentPage() > PaginationComponent.PAGES_AROUND_CURRENT + 1);
  protected readonly showLeadingDots = computed(() => this._currentPage() > PaginationComponent.PAGES_AROUND_CURRENT + 2);

  /**
   * Check if we should show last page and trailing ellipsis
   */
  protected readonly showTrailingEllipsis = computed(() => this._currentPage() < this._totalPages() - PaginationComponent.PAGES_AROUND_CURRENT);
  protected readonly showTrailingLastPage = computed(() => this._currentPage() < this._totalPages() - PaginationComponent.PAGES_AROUND_CURRENT);
  protected readonly showTrailingDots = computed(() => this._currentPage() < this._totalPages() - PaginationComponent.PAGES_AROUND_CURRENT - 1);

  /**
   * Calculate range start
   */
  protected readonly rangeStart = computed(() => {
    if (!this.pageSize) return 0;
    return (this._currentPage() - 1) * this.pageSize + 1;
  });

  /**
   * Calculate range end
   */
  protected readonly rangeEnd = computed(() => {
    if (!this.pageSize || !this.totalItems) return 0;
    return Math.min(this._currentPage() * this.pageSize, this.totalItems);
  });

  /**
   * Go to previous page
   */
  protected previousPage(): void {
    if (this._currentPage() > 1) {
      this.pageChange.emit(this._currentPage() - 1);
    }
  }

  /**
   * Go to next page
   */
  protected nextPage(): void {
    if (this._currentPage() < this._totalPages()) {
      this.pageChange.emit(this._currentPage() + 1);
    }
  }

  /**
   * Go to specific page
   */
  protected goToPage(page: number): void {
    if (page >= 1 && page <= this._totalPages() && page !== this._currentPage()) {
      this.pageChange.emit(page);
    }
  }
}
