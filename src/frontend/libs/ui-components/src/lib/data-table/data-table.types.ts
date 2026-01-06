import { TemplateRef } from "@angular/core";

/**
 * Sort direction for columns
 */
export type SortDirection = "asc" | "desc" | null;

/**
 * Column definition for the data table
 */
export interface DataTableColumn<T = unknown> {
  /** Unique identifier/key for the column (usually property name) */
  key: string;

  /** Display header text */
  header: string;

  /** Whether the column is sortable */
  sortable?: boolean;

  /** Whether the column is filterable */
  filterable?: boolean;

  /** Custom cell template */
  cellTemplate?: TemplateRef<DataTableCellContext<T>>;

  /** Custom header template */
  headerTemplate?: TemplateRef<DataTableHeaderContext<T>>;

  /** Width of the column (CSS value) */
  width?: string;

  /** Minimum width of the column (CSS value) */
  minWidth?: string;

  /** Text alignment */
  align?: "left" | "center" | "right";

  /** Whether to hide on mobile */
  hideOnMobile?: boolean;

  /** Custom sort function */
  sortFn?: (a: T, b: T, direction: SortDirection) => number;

  /** Custom filter function */
  filterFn?: (item: T, filterValue: string) => boolean;

  /** Value accessor for nested properties */
  valueAccessor?: (item: T) => unknown;
}

/**
 * Context provided to cell templates
 */
export interface DataTableCellContext<T = unknown> {
  /** The data item for this row */
  $implicit: T;

  /** The column definition */
  column: DataTableColumn<T>;

  /** The cell value */
  value: unknown;

  /** Row index */
  rowIndex: number;
}

/**
 * Context provided to header templates
 */
export interface DataTableHeaderContext<T = unknown> {
  /** The column definition */
  $implicit: DataTableColumn<T>;

  /** Current sort direction for this column */
  sortDirection: SortDirection;

  /** Column index */
  columnIndex: number;
}

/**
 * Sort state for the table
 */
export interface DataTableSort {
  /** Column key being sorted */
  column: string;

  /** Sort direction */
  direction: SortDirection;
}

/**
 * Selection state for the table
 */
export interface DataTableSelection<T = unknown> {
  /** Selected items */
  items: T[];

  /** Whether all items are selected */
  allSelected: boolean;

  /** Whether some (but not all) items are selected */
  indeterminate: boolean;
}

/**
 * Event emitted when sort changes
 */
export interface DataTableSortEvent {
  /** Column key */
  column: string;

  /** New sort direction */
  direction: SortDirection;
}

/**
 * Event emitted when selection changes
 */
export interface DataTableSelectionEvent<T = unknown> {
  /** Currently selected items */
  selected: T[];

  /** Item that was just selected/deselected (if single action) */
  item?: T;

  /** Whether the item was selected or deselected */
  checked?: boolean;
}

/**
 * Event emitted when row is clicked
 */
export interface DataTableRowClickEvent<T = unknown> {
  /** The clicked item */
  item: T;

  /** Row index */
  index: number;

  /** Original mouse event */
  event: MouseEvent;
}

/**
 * Pagination state
 */
export interface DataTablePagination {
  /** Current page (1-indexed) */
  page: number;

  /** Items per page */
  pageSize: number;

  /** Total number of items */
  totalItems: number;

  /** Total number of pages */
  totalPages: number;
}
