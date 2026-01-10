import {
  Component,
  input,
  output,
  signal,
  computed,
  contentChildren,
  TemplateRef,
} from "@angular/core";
import type { TrackByFunction } from "@angular/core";
import { CommonModule, NgTemplateOutlet } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { DataTableColumnDirective } from "./data-table-column.directive";
import { PaginationComponent } from "../pagination/pagination.component";
import { LoadingStateComponent } from "../state-display/loading-state.component";
import { EmptyStateComponent } from "../state-display/empty-state.component";
import type {
  DataTableColumn,
  DataTableSort,
  DataTableSortEvent,
  DataTableSelectionEvent,
  DataTableRowClickEvent,
  SortDirection,
  DataTableCellContext,
  DataTableHeaderContext,
} from "./data-table.types";

/**
 * Data Table Component
 *
 * A feature-rich data table with sorting, filtering, pagination, and selection.
 *
 * @example
 * <ocr-data-table
 *   [data]="vehicles"
 *   [columns]="columns"
 *   [selectable]="true"
 *   [paginated]="true"
 *   (sortChange)="onSort($event)"
 *   (selectionChange)="onSelect($event)"
 * >
 * </ocr-data-table>
 */
@Component({
  selector: "ocr-data-table",
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgTemplateOutlet,
    PaginationComponent,
    LoadingStateComponent,
    EmptyStateComponent,
  ],
  template: `
    <div class="data-table" [class.data-table--loading]="loading()">
      <!-- Filter input -->
      @if (filterable() && !loading()) {
        <div class="data-table__filter">
          <input
            type="text"
            class="data-table__filter-input"
            [placeholder]="filterPlaceholder()"
            [ngModel]="filterValue()"
            (ngModelChange)="onFilterChange($event)"
          />
          @if (filterValue()) {
            <button
              type="button"
              class="data-table__filter-clear"
              (click)="clearFilter()"
              aria-label="Filter löschen"
            >
              <svg viewBox="0 0 24 24" fill="currentColor">
                <path
                  d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"
                />
              </svg>
            </button>
          }
        </div>
      }

      <!-- Loading state -->
      @if (loading()) {
        <ui-loading-state [message]="loadingMessage()"></ui-loading-state>
      }

      <!-- Empty state -->
      @else if (displayData().length === 0) {
        <ui-empty-state
          [icon]="emptyIcon()"
          [title]="emptyTitle()"
          [description]="emptyDescription()"
        ></ui-empty-state>
      }

      <!-- Table (desktop) -->
      @else {
        <div class="data-table__wrapper">
          <table class="data-table__table" role="grid">
            <thead>
              <tr>
                @if (selectable()) {
                  <th class="data-table__th data-table__th--checkbox">
                    <input
                      type="checkbox"
                      [checked]="allSelected()"
                      [indeterminate]="someSelected() && !allSelected()"
                      (change)="toggleSelectAll()"
                      aria-label="Alle auswählen"
                    />
                  </th>
                }
                @for (col of effectiveColumns(); track col.key) {
                  <th
                    class="data-table__th"
                    [class.data-table__th--sortable]="col.sortable"
                    [class.data-table__th--sorted]="
                      currentSort()?.column === col.key
                    "
                    [class.data-table__th--hide-mobile]="col.hideOnMobile"
                    [style.width]="col.width"
                    [style.min-width]="col.minWidth"
                    [style.text-align]="col.align || 'left'"
                    [attr.aria-sort]="getAriaSort(col.key)"
                    (click)="col.sortable ? toggleSort(col.key) : null"
                    (keydown.enter)="col.sortable ? toggleSort(col.key) : null"
                    (keydown.space)="col.sortable ? toggleSort(col.key) : null"
                    [attr.tabindex]="col.sortable ? 0 : null"
                  >
                    @if (col.headerTemplate) {
                      <ng-container
                        *ngTemplateOutlet="
                          col.headerTemplate;
                          context: getHeaderContext(col)
                        "
                      ></ng-container>
                    } @else {
                      <span class="data-table__th-content">
                        {{ col.header }}
                        @if (col.sortable) {
                          <span class="data-table__sort-icon">
                            @if (currentSort()?.column === col.key) {
                              @if (currentSort()?.direction === "asc") {
                                <svg viewBox="0 0 24 24" fill="currentColor">
                                  <path d="M7 14l5-5 5 5z" />
                                </svg>
                              } @else {
                                <svg viewBox="0 0 24 24" fill="currentColor">
                                  <path d="M7 10l5 5 5-5z" />
                                </svg>
                              }
                            } @else {
                              <svg
                                viewBox="0 0 24 24"
                                fill="currentColor"
                                style="opacity: 0.3"
                              >
                                <path
                                  d="M12 5.83L15.17 9l1.41-1.41L12 3 7.41 7.59 8.83 9 12 5.83zm0 12.34L8.83 15l-1.41 1.41L12 21l4.59-4.59L15.17 15 12 18.17z"
                                />
                              </svg>
                            }
                          </span>
                        }
                      </span>
                    }
                  </th>
                }
              </tr>
            </thead>
            <tbody>
              @for (
                item of displayData();
                track trackByFn()(i, item);
                let i = $index
              ) {
                <tr
                  class="data-table__row"
                  [class.data-table__row--selected]="isSelected(item)"
                  [class.data-table__row--clickable]="rowClickable()"
                  (click)="onRowClick(item, i, $event)"
                >
                  @if (selectable()) {
                    <td class="data-table__td data-table__td--checkbox">
                      <input
                        type="checkbox"
                        [checked]="isSelected(item)"
                        (change)="toggleSelect(item)"
                        (click)="$event.stopPropagation()"
                        aria-label="Zeile auswählen"
                      />
                    </td>
                  }
                  @for (col of effectiveColumns(); track col.key) {
                    <td
                      class="data-table__td"
                      [class.data-table__td--hide-mobile]="col.hideOnMobile"
                      [style.text-align]="col.align || 'left'"
                    >
                      @if (col.cellTemplate) {
                        <ng-container
                          *ngTemplateOutlet="
                            col.cellTemplate;
                            context: getCellContext(item, col, i)
                          "
                        ></ng-container>
                      } @else {
                        {{ getCellValue(item, col) }}
                      }
                    </td>
                  }
                </tr>
              }
            </tbody>
          </table>
        </div>

        <!-- Mobile card view -->
        <div class="data-table__cards">
          @for (
            item of displayData();
            track trackByFn()(i, item);
            let i = $index
          ) {
            <div
              class="data-table__card"
              [class.data-table__card--selected]="isSelected(item)"
              [class.data-table__card--clickable]="rowClickable()"
              (click)="onRowClick(item, i, $event)"
            >
              @if (selectable()) {
                <div class="data-table__card-checkbox">
                  <input
                    type="checkbox"
                    [checked]="isSelected(item)"
                    (change)="toggleSelect(item)"
                    (click)="$event.stopPropagation()"
                  />
                </div>
              }
              <div class="data-table__card-content">
                @for (col of effectiveColumns(); track col.key) {
                  <div class="data-table__card-row">
                    <span class="data-table__card-label">{{ col.header }}</span>
                    <span class="data-table__card-value">
                      @if (col.cellTemplate) {
                        <ng-container
                          *ngTemplateOutlet="
                            col.cellTemplate;
                            context: getCellContext(item, col, i)
                          "
                        ></ng-container>
                      } @else {
                        {{ getCellValue(item, col) }}
                      }
                    </span>
                  </div>
                }
              </div>
            </div>
          }
        </div>

        <!-- Pagination -->
        @if (paginated() && totalPages() > 1) {
          <div class="data-table__pagination">
            <ui-pagination
              [currentPage]="currentPage()"
              [totalPages]="totalPages()"
              [totalItems]="totalItems()"
              [pageSize]="pageSize()"
              [showItemRange]="true"
              [itemLabel]="itemLabel()"
              [itemLabelPlural]="itemLabelPlural()"
              (pageChange)="onPageChange($event)"
            ></ui-pagination>
          </div>
        }
      }
    </div>
  `,
  styles: [
    `
      .data-table {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .data-table--loading {
        min-height: 200px;
      }

      .data-table__filter {
        position: relative;
        max-width: 20rem;
      }

      .data-table__filter-input {
        width: 100%;
        padding: 0.5rem 2.5rem 0.5rem 0.75rem;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        font-size: 0.875rem;
        transition: border-color 0.15s ease;
      }

      .data-table__filter-input:focus {
        outline: none;
        border-color: #f97316;
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
      }

      .data-table__filter-clear {
        position: absolute;
        right: 0.5rem;
        top: 50%;
        transform: translateY(-50%);
        display: flex;
        align-items: center;
        justify-content: center;
        width: 1.5rem;
        height: 1.5rem;
        padding: 0;
        background: none;
        border: none;
        cursor: pointer;
        color: #9ca3af;
        border-radius: 0.25rem;
      }

      .data-table__filter-clear:hover {
        color: #6b7280;
        background-color: #f3f4f6;
      }

      .data-table__filter-clear svg {
        width: 1rem;
        height: 1rem;
      }

      .data-table__wrapper {
        overflow-x: auto;
        border: 1px solid #e5e7eb;
        border-radius: 0.5rem;
      }

      .data-table__table {
        width: 100%;
        border-collapse: collapse;
        font-size: 0.875rem;
      }

      .data-table__th {
        padding: 0.75rem 1rem;
        text-align: left;
        font-weight: 600;
        color: #374151;
        background-color: #f9fafb;
        border-bottom: 1px solid #e5e7eb;
        white-space: nowrap;
      }

      .data-table__th--sortable {
        cursor: pointer;
        user-select: none;
      }

      .data-table__th--sortable:hover {
        background-color: #f3f4f6;
      }

      .data-table__th--sortable:focus-visible {
        outline: 2px solid #f97316;
        outline-offset: -2px;
      }

      .data-table__th--sorted {
        color: #f97316;
      }

      .data-table__th--checkbox {
        width: 3rem;
        text-align: center;
      }

      .data-table__th-content {
        display: flex;
        align-items: center;
        gap: 0.5rem;
      }

      .data-table__sort-icon {
        display: flex;
        align-items: center;
      }

      .data-table__sort-icon svg {
        width: 1rem;
        height: 1rem;
      }

      .data-table__row {
        transition: background-color 0.15s ease;
      }

      .data-table__row:hover {
        background-color: #f9fafb;
      }

      .data-table__row--selected {
        background-color: #fff7ed !important;
      }

      .data-table__row--clickable {
        cursor: pointer;
      }

      .data-table__td {
        padding: 0.75rem 1rem;
        border-bottom: 1px solid #e5e7eb;
        color: #374151;
      }

      .data-table__td--checkbox {
        width: 3rem;
        text-align: center;
      }

      /* Mobile card view */
      .data-table__cards {
        display: none;
        flex-direction: column;
        gap: 0.75rem;
      }

      .data-table__card {
        display: flex;
        gap: 0.75rem;
        padding: 1rem;
        background: white;
        border: 1px solid #e5e7eb;
        border-radius: 0.5rem;
        transition: background-color 0.15s ease;
      }

      .data-table__card--selected {
        background-color: #fff7ed;
        border-color: #fed7aa;
      }

      .data-table__card--clickable {
        cursor: pointer;
      }

      .data-table__card--clickable:hover {
        background-color: #f9fafb;
      }

      .data-table__card-checkbox {
        padding-top: 0.125rem;
      }

      .data-table__card-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
      }

      .data-table__card-row {
        display: flex;
        justify-content: space-between;
        gap: 1rem;
      }

      .data-table__card-label {
        font-size: 0.75rem;
        color: #6b7280;
        flex-shrink: 0;
      }

      .data-table__card-value {
        font-size: 0.875rem;
        color: #111827;
        text-align: right;
      }

      .data-table__pagination {
        padding-top: 0.5rem;
      }

      /* Responsive */
      @media (max-width: 768px) {
        .data-table__wrapper {
          display: none;
        }

        .data-table__cards {
          display: flex;
        }

        .data-table__th--hide-mobile,
        .data-table__td--hide-mobile {
          display: none;
        }
      }

      /* Checkbox styling */
      input[type="checkbox"] {
        width: 1rem;
        height: 1rem;
        cursor: pointer;
        accent-color: #f97316;
      }
    `,
  ],
})
export class DataTableComponent<T = unknown> {
  /**
   * Data array to display
   */
  readonly data = input.required<T[]>();

  /**
   * Column definitions (alternative to using directives)
   */
  readonly columns = input<DataTableColumn<T>[]>([]);

  /**
   * Whether the table is loading
   */
  readonly loading = input(false);

  /**
   * Loading message
   */
  readonly loadingMessage = input("Lade Daten...");

  /**
   * Whether rows are selectable
   */
  readonly selectable = input(false);

  /**
   * Selection mode
   */
  readonly selectionMode = input<"single" | "multiple">("multiple");

  /**
   * Whether rows are clickable
   */
  readonly rowClickable = input(false);

  /**
   * Whether the table is filterable
   */
  readonly filterable = input(false);

  /**
   * Filter placeholder text
   */
  readonly filterPlaceholder = input("Suchen...");

  /**
   * Whether to enable client-side pagination
   */
  readonly paginated = input(false);

  /**
   * Items per page
   */
  readonly pageSize = input(10);

  /**
   * Current page (for external pagination control)
   */
  readonly page = input(1);

  /**
   * Total items (for server-side pagination)
   */
  readonly total = input<number | undefined>(undefined);

  /**
   * Item label for pagination
   */
  readonly itemLabel = input("Eintrag");

  /**
   * Item label plural for pagination
   */
  readonly itemLabelPlural = input("Einträge");

  /**
   * Empty state icon
   */
  readonly emptyIcon = input<
    "default" | "car" | "reservation" | "location" | "search" | "document"
  >("search");

  /**
   * Empty state title
   */
  readonly emptyTitle = input("Keine Daten gefunden");

  /**
   * Empty state description
   */
  readonly emptyDescription = input("");

  /**
   * Track by function for ngFor
   */
  readonly trackByFn = input<TrackByFunction<T>>(
    (index: number, _item: T) => index,
  );

  /**
   * Emitted when sort changes
   */
  readonly sortChange = output<DataTableSortEvent>();

  /**
   * Emitted when selection changes
   */
  readonly selectionChange = output<DataTableSelectionEvent<T>>();

  /**
   * Emitted when a row is clicked
   */
  readonly rowClick = output<DataTableRowClickEvent<T>>();

  /**
   * Emitted when page changes
   */
  readonly pageChange = output<number>();

  /**
   * Emitted when filter changes
   */
  readonly filterChange = output<string>();

  /**
   * Column directives from content
   */
  readonly columnDirectives = contentChildren(DataTableColumnDirective);

  /**
   * Current sort state
   */
  readonly currentSort = signal<DataTableSort | null>(null);

  /**
   * Current filter value
   */
  readonly filterValue = signal("");

  /**
   * Current page (internal)
   */
  readonly currentPage = signal(1);

  /**
   * Selected items
   */
  readonly selectedItems = signal<Set<T>>(new Set());

  /**
   * Effective columns (from input or directives)
   */
  readonly effectiveColumns = computed<DataTableColumn<T>[]>(() => {
    const inputColumns = this.columns();
    if (inputColumns.length > 0) return inputColumns;

    // Convert directives to column definitions
    return this.columnDirectives().map((dir) => ({
      key: dir.key,
      header: dir.header(),
      sortable: dir.sortable(),
      filterable: dir.filterable(),
      width: dir.width(),
      minWidth: dir.minWidth(),
      align: dir.align(),
      hideOnMobile: dir.hideOnMobile(),
      cellTemplate: dir.cellTemplate() as
        | TemplateRef<DataTableCellContext<T>>
        | undefined,
      headerTemplate: dir.headerTemplate(),
    }));
  });

  /**
   * Filtered data
   */
  readonly filteredData = computed(() => {
    const data = this.data();
    const filter = this.filterValue().toLowerCase().trim();

    if (!filter) return data;

    return data.filter((item) => {
      return this.effectiveColumns().some((col) => {
        if (col.filterFn) return col.filterFn(item, filter);
        const value = this.getCellValue(item, col);
        return String(value).toLowerCase().includes(filter);
      });
    });
  });

  /**
   * Sorted data
   */
  readonly sortedData = computed(() => {
    const data = [...this.filteredData()];
    const sort = this.currentSort();

    if (!sort || !sort.direction) return data;

    const column = this.effectiveColumns().find((c) => c.key === sort.column);
    if (!column) return data;

    return data.sort((a, b) => {
      if (column.sortFn) return column.sortFn(a, b, sort.direction);

      const aValue = this.getCellValue(a, column);
      const bValue = this.getCellValue(b, column);

      let comparison = 0;
      // Handle comparison of unknown values by converting to comparable types
      const aComp =
        aValue == null
          ? ""
          : typeof aValue === "number"
            ? aValue
            : String(aValue);
      const bComp =
        bValue == null
          ? ""
          : typeof bValue === "number"
            ? bValue
            : String(bValue);

      if (aComp < bComp) comparison = -1;
      else if (aComp > bComp) comparison = 1;

      return sort.direction === "desc" ? -comparison : comparison;
    });
  });

  /**
   * Data to display (with pagination)
   */
  readonly displayData = computed(() => {
    const data = this.sortedData();

    if (!this.paginated()) return data;

    const page = this.currentPage();
    const size = this.pageSize();
    const start = (page - 1) * size;
    const end = start + size;

    return data.slice(start, end);
  });

  /**
   * Total items for pagination
   */
  readonly totalItems = computed(() => {
    return this.total() ?? this.filteredData().length;
  });

  /**
   * Total pages
   */
  readonly totalPages = computed(() => {
    return Math.ceil(this.totalItems() / this.pageSize());
  });

  /**
   * Whether all visible items are selected
   */
  readonly allSelected = computed(() => {
    const display = this.displayData();
    if (display.length === 0) return false;
    return display.every((item) => this.selectedItems().has(item));
  });

  /**
   * Whether some (but not all) items are selected
   */
  readonly someSelected = computed(() => {
    const display = this.displayData();
    const selected = this.selectedItems();
    return display.some((item) => selected.has(item));
  });

  /**
   * Get cell value for an item and column
   */
  getCellValue(item: T, column: DataTableColumn<T>): unknown {
    if (column.valueAccessor) return column.valueAccessor(item);
    return (item as Record<string, unknown>)[column.key];
  }

  /**
   * Get cell template context
   */
  getCellContext(
    item: T,
    column: DataTableColumn<T>,
    rowIndex: number,
  ): DataTableCellContext<T> {
    return {
      $implicit: item,
      column,
      value: this.getCellValue(item, column),
      rowIndex,
    };
  }

  /**
   * Get header template context
   */
  getHeaderContext(column: DataTableColumn<T>): DataTableHeaderContext<T> {
    const sort = this.currentSort();
    return {
      $implicit: column,
      sortDirection: sort?.column === column.key ? sort.direction : null,
      columnIndex: this.effectiveColumns().indexOf(column),
    };
  }

  /**
   * Get ARIA sort attribute value
   */
  getAriaSort(columnKey: string): "ascending" | "descending" | "none" | null {
    const sort = this.currentSort();
    if (sort?.column !== columnKey) return "none";
    return sort.direction === "asc" ? "ascending" : "descending";
  }

  /**
   * Toggle sort on a column
   */
  toggleSort(columnKey: string): void {
    const current = this.currentSort();
    let newDirection: SortDirection;

    if (current?.column !== columnKey) {
      newDirection = "asc";
    } else if (current.direction === "asc") {
      newDirection = "desc";
    } else {
      newDirection = null;
    }

    const newSort = newDirection
      ? { column: columnKey, direction: newDirection }
      : null;
    this.currentSort.set(newSort);

    this.sortChange.emit({
      column: columnKey,
      direction: newDirection,
    });
  }

  /**
   * Check if an item is selected
   */
  isSelected(item: T): boolean {
    return this.selectedItems().has(item);
  }

  /**
   * Toggle selection of an item
   */
  toggleSelect(item: T): void {
    const selected = new Set(this.selectedItems());
    const isNowSelected = !selected.has(item);

    if (this.selectionMode() === "single") {
      selected.clear();
      if (isNowSelected) {
        selected.add(item);
      }
    } else {
      if (isNowSelected) {
        selected.add(item);
      } else {
        selected.delete(item);
      }
    }

    this.selectedItems.set(selected);

    this.selectionChange.emit({
      selected: Array.from(selected),
      item,
      checked: isNowSelected,
    });
  }

  /**
   * Toggle select all
   */
  toggleSelectAll(): void {
    const display = this.displayData();
    const allCurrentlySelected = this.allSelected();

    const selected = new Set(this.selectedItems());

    if (allCurrentlySelected) {
      // Deselect all displayed items
      display.forEach((item) => selected.delete(item));
    } else {
      // Select all displayed items
      display.forEach((item) => selected.add(item));
    }

    this.selectedItems.set(selected);

    this.selectionChange.emit({
      selected: Array.from(selected),
    });
  }

  /**
   * Handle row click
   */
  onRowClick(item: T, index: number, event: MouseEvent): void {
    if (!this.rowClickable()) return;

    this.rowClick.emit({
      item,
      index,
      event,
    });
  }

  /**
   * Handle filter change
   */
  onFilterChange(value: string): void {
    this.filterValue.set(value);
    this.currentPage.set(1); // Reset to first page
    this.filterChange.emit(value);
  }

  /**
   * Clear filter
   */
  clearFilter(): void {
    this.onFilterChange("");
  }

  /**
   * Handle page change
   */
  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.pageChange.emit(page);
  }

  /**
   * Clear selection
   */
  clearSelection(): void {
    this.selectedItems.set(new Set());
    this.selectionChange.emit({ selected: [] });
  }

  /**
   * Get selected items as array
   */
  getSelectedItems(): T[] {
    return Array.from(this.selectedItems());
  }
}
