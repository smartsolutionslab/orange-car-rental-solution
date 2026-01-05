import { Directive, input, contentChild, TemplateRef } from '@angular/core';
import type { SortDirection, DataTableCellContext, DataTableHeaderContext } from './data-table.types';

/**
 * Directive for defining a data table column
 *
 * @example
 * <ocr-data-table [data]="items">
 *   <ng-container *ocrDataTableColumn="'name'; header: 'Name'; sortable: true">
 *     <ng-template let-item let-value="value">
 *       <strong>{{ value }}</strong>
 *     </ng-template>
 *   </ng-container>
 * </ocr-data-table>
 */
@Directive({
  selector: '[ocrDataTableColumn]',
  standalone: true,
})
export class DataTableColumnDirective<T = unknown> {
  /**
   * Column key (property name to display)
   */
  readonly ocrDataTableColumn = input.required<string>();

  /**
   * Column header text
   */
  readonly header = input.required<string>();

  /**
   * Whether the column is sortable
   */
  readonly sortable = input(false);

  /**
   * Whether the column is filterable
   */
  readonly filterable = input(false);

  /**
   * Column width (CSS value)
   */
  readonly width = input<string | undefined>(undefined);

  /**
   * Minimum column width (CSS value)
   */
  readonly minWidth = input<string | undefined>(undefined);

  /**
   * Text alignment
   */
  readonly align = input<'left' | 'center' | 'right'>('left');

  /**
   * Whether to hide on mobile
   */
  readonly hideOnMobile = input(false);

  /**
   * Cell template
   */
  readonly cellTemplate = contentChild<TemplateRef<DataTableCellContext<T>>>('cell');

  /**
   * Header template
   */
  readonly headerTemplate = contentChild<TemplateRef<DataTableHeaderContext<T>>>('header');

  /**
   * Get the column key
   */
  get key(): string {
    return this.ocrDataTableColumn();
  }
}
