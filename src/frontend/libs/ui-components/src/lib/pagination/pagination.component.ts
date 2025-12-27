import { Component, input, output, computed } from '@angular/core';
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
  readonly currentPage = input.required<number>();

  /**
   * Total number of pages
   */
  readonly totalPages = input.required<number>();

  /**
   * Total count of items (optional, for display)
   */
  readonly totalItems = input<number | undefined>(undefined);

  /**
   * Items per page (optional, for display)
   */
  readonly pageSize = input<number | undefined>(undefined);

  /**
   * Label for previous button
   */
  readonly previousLabel = input('Zurück');

  /**
   * Label for next button
   */
  readonly nextLabel = input('Weiter');

  /**
   * Show item range info (e.g., "Showing 1-10 of 100")
   */
  readonly showItemRange = input(false);

  /**
   * Item label for range display (singular)
   */
  readonly itemLabel = input('Eintrag');

  /**
   * Item label for range display (plural)
   */
  readonly itemLabelPlural = input('Einträge');

  /**
   * Emitted when page changes
   */
  readonly pageChange = output<number>();

  /**
   * Calculate visible page numbers
   */
  protected readonly visiblePages = computed(() => {
    const current = this.currentPage();
    const total = this.totalPages();
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
  protected readonly showLeadingFirstPage = computed(() => this.currentPage() > PaginationComponent.PAGES_AROUND_CURRENT + 1);
  protected readonly showLeadingDots = computed(() => this.currentPage() > PaginationComponent.PAGES_AROUND_CURRENT + 2);

  /**
   * Check if we should show last page and trailing ellipsis
   */
  protected readonly showTrailingLastPage = computed(() => this.currentPage() < this.totalPages() - PaginationComponent.PAGES_AROUND_CURRENT);
  protected readonly showTrailingDots = computed(() => this.currentPage() < this.totalPages() - PaginationComponent.PAGES_AROUND_CURRENT - 1);

  /**
   * Calculate range start
   */
  protected readonly rangeStart = computed(() => {
    const size = this.pageSize();
    if (!size) return 0;
    return (this.currentPage() - 1) * size + 1;
  });

  /**
   * Calculate range end
   */
  protected readonly rangeEnd = computed(() => {
    const size = this.pageSize();
    const total = this.totalItems();
    if (!size || !total) return 0;
    return Math.min(this.currentPage() * size, total);
  });

  /**
   * Go to previous page
   */
  protected previousPage(): void {
    if (this.currentPage() > 1) {
      this.pageChange.emit(this.currentPage() - 1);
    }
  }

  /**
   * Go to next page
   */
  protected nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.pageChange.emit(this.currentPage() + 1);
    }
  }

  /**
   * Go to specific page
   */
  protected goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages() && page !== this.currentPage()) {
      this.pageChange.emit(page);
    }
  }
}
