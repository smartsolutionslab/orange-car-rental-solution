import { signal, computed } from "@angular/core";
import type { Signal, WritableSignal } from "@angular/core";

/**
 * State manager for pagination.
 * Reduces boilerplate for common pagination patterns.
 *
 * @example
 * ```typescript
 * // In component
 * protected readonly pagination = new PaginationState(25);
 *
 * loadData() {
 *   this.service.getData({
 *     page: this.pagination.currentPage(),
 *     pageSize: this.pagination.pageSize()
 *   }).subscribe(result => {
 *     this.data.set(result.items);
 *     this.pagination.setTotalCount(result.totalCount);
 *   });
 * }
 *
 * // In template
 * <button (click)="pagination.previousPage()" [disabled]="!pagination.hasPreviousPage()">
 *   Zurück
 * </button>
 * <span>Seite {{ pagination.currentPage() }} von {{ pagination.totalPages() }}</span>
 * <button (click)="pagination.nextPage()" [disabled]="!pagination.hasNextPage()">
 *   Weiter
 * </button>
 * ```
 */
export class PaginationState {
  private readonly _currentPage: WritableSignal<number>;
  private readonly _pageSize: WritableSignal<number>;
  private readonly _totalCount: WritableSignal<number>;

  constructor(defaultPageSize = 25) {
    this._currentPage = signal(1);
    this._pageSize = signal(defaultPageSize);
    this._totalCount = signal(0);
  }

  /** Current page number (1-indexed) */
  readonly currentPage: Signal<number> = computed(() => this._currentPage());

  /** Number of items per page */
  readonly pageSize: Signal<number> = computed(() => this._pageSize());

  /** Total number of items across all pages */
  readonly totalCount: Signal<number> = computed(() => this._totalCount());

  /** Total number of pages */
  readonly totalPages: Signal<number> = computed(() =>
    Math.max(1, Math.ceil(this._totalCount() / this._pageSize())),
  );

  /** Whether there is a next page */
  readonly hasNextPage: Signal<boolean> = computed(
    () => this._currentPage() < this.totalPages(),
  );

  /** Whether there is a previous page */
  readonly hasPreviousPage: Signal<boolean> = computed(
    () => this._currentPage() > 1,
  );

  /** Starting item number for current page (1-indexed) */
  readonly startItem: Signal<number> = computed(() => {
    if (this._totalCount() === 0) return 0;
    return (this._currentPage() - 1) * this._pageSize() + 1;
  });

  /** Ending item number for current page */
  readonly endItem: Signal<number> = computed(() => {
    const end = this._currentPage() * this._pageSize();
    return Math.min(end, this._totalCount());
  });

  /** Whether there are any items */
  readonly hasItems: Signal<boolean> = computed(() => this._totalCount() > 0);

  /**
   * Go to a specific page
   * @param page - The page number (1-indexed)
   * @returns true if the page was changed
   */
  goToPage(page: number): boolean {
    const validPage = Math.max(1, Math.min(page, this.totalPages()));
    if (validPage !== this._currentPage()) {
      this._currentPage.set(validPage);
      return true;
    }
    return false;
  }

  /**
   * Go to the next page
   * @returns true if the page was changed
   */
  nextPage(): boolean {
    if (this.hasNextPage()) {
      this._currentPage.update((p) => p + 1);
      return true;
    }
    return false;
  }

  /**
   * Go to the previous page
   * @returns true if the page was changed
   */
  previousPage(): boolean {
    if (this.hasPreviousPage()) {
      this._currentPage.update((p) => p - 1);
      return true;
    }
    return false;
  }

  /**
   * Go to the first page
   * @returns true if the page was changed
   */
  firstPage(): boolean {
    return this.goToPage(1);
  }

  /**
   * Go to the last page
   * @returns true if the page was changed
   */
  lastPage(): boolean {
    return this.goToPage(this.totalPages());
  }

  /**
   * Set the total count of items
   * @param count - Total number of items
   */
  setTotalCount(count: number): void {
    this._totalCount.set(count);
    // Adjust current page if it's now beyond the total
    if (this._currentPage() > this.totalPages()) {
      this._currentPage.set(Math.max(1, this.totalPages()));
    }
  }

  /**
   * Set the page size
   * @param size - Number of items per page
   * @param resetToFirstPage - Whether to reset to page 1 (default: true)
   */
  setPageSize(size: number, resetToFirstPage = true): void {
    this._pageSize.set(size);
    if (resetToFirstPage) {
      this._currentPage.set(1);
    } else if (this._currentPage() > this.totalPages()) {
      this._currentPage.set(Math.max(1, this.totalPages()));
    }
  }

  /**
   * Reset pagination to initial state
   * @param preservePageSize - Whether to keep the current page size
   */
  reset(preservePageSize = true): void {
    this._currentPage.set(1);
    this._totalCount.set(0);
    if (!preservePageSize) {
      this._pageSize.set(25);
    }
  }

  /**
   * Get pagination parameters for API calls
   */
  getQueryParams(): { page: number; pageSize: number } {
    return {
      page: this._currentPage(),
      pageSize: this._pageSize(),
    };
  }

  /**
   * Generate an array of page numbers for pagination UI
   * @param maxVisible - Maximum number of page buttons to show
   */
  getPageNumbers(maxVisible = 5): number[] {
    const total = this.totalPages();
    const current = this._currentPage();

    if (total <= maxVisible) return Array.from({ length: total }, (_, i) => i + 1);

    const half = Math.floor(maxVisible / 2);
    let start = Math.max(1, current - half);
    const end = Math.min(total, start + maxVisible - 1);

    if (end - start < maxVisible - 1) start = Math.max(1, end - maxVisible + 1);

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }
}
