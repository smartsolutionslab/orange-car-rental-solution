import { Component, input } from "@angular/core";
import { CommonModule } from "@angular/common";

/**
 * Reusable detail row component for displaying label-value pairs
 * Commonly used in cards, modals, and detail views
 *
 * Usage:
 * - Simple: <ui-detail-row label="Name" [value]="customer.name"></ui-detail-row>
 * - With content: <ui-detail-row label="Status"><ui-status-badge [status]="item.status"></ui-status-badge></ui-detail-row>
 */
@Component({
  selector: "ui-detail-row",
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="detail-row"
      [class.highlight]="highlight()"
      [class.price]="isPrice()"
    >
      <span class="label">{{ label() }}</span>
      <span class="value" [class.strong]="strong()">
        @if (value() !== undefined && value() !== null) {
          {{ value() }}
        } @else {
          <ng-content></ng-content>
        }
      </span>
    </div>
  `,
  styles: [
    `
      .detail-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0.5rem 0;
        border-bottom: 1px solid #eee;
      }

      .detail-row:last-child {
        border-bottom: none;
      }

      .detail-row.highlight {
        background-color: #f8f9fa;
        padding: 0.5rem;
        margin: 0 -0.5rem;
        border-radius: 4px;
      }

      .detail-row.price {
        font-weight: 500;
      }

      .label {
        color: #6c757d;
        font-size: 0.875rem;
      }

      .value {
        color: #212529;
        text-align: right;
      }

      .value.strong {
        font-weight: 600;
      }
    `,
  ],
})
export class DetailRowComponent {
  readonly label = input.required<string>();
  readonly value = input<string | number | undefined>(undefined);
  readonly highlight = input(false);
  readonly isPrice = input(false);
  readonly strong = input(false);
}
