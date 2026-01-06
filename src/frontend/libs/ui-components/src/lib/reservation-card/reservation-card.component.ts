import { Component, input, output, computed } from "@angular/core";
import { CommonModule } from "@angular/common";
import { StatusBadgeComponent } from "../status-badge/status-badge.component";
import { formatDateDE, formatPriceDE } from "../utils/status-display.utils";

export type ReservationCardVariant = "upcoming" | "pending" | "past" | "guest";

export interface ReservationCardData {
  id: string;
  status: string;
  pickupDate: string;
  returnDate: string;
  pickupLocationCode?: string;
  dropoffLocationCode?: string;
  totalPriceGross: number;
  currency?: string;
}

/**
 * Reusable reservation card component
 * Displays reservation information with variant-specific content
 */
@Component({
  selector: "ui-reservation-card",
  standalone: true,
  imports: [CommonModule, StatusBadgeComponent],
  template: `
    <div class="reservation-card" [ngClass]="variant()">
      <div class="card-header">
        <span class="reservation-id"
          >{{ reservation().id.substring(0, 8) }}...</span
        >
        <ui-status-badge
          type="reservation"
          [status]="reservation().status"
        ></ui-status-badge>
      </div>
      <div class="card-body">
        <div class="detail-row">
          <span class="label">Abholtermin:</span>
          <span class="value">{{ formatDate(reservation().pickupDate) }}</span>
        </div>
        <div class="detail-row">
          <span class="label">Rückgabetermin:</span>
          <span class="value">{{ formatDate(reservation().returnDate) }}</span>
        </div>
        @if (showLocations() && reservation().pickupLocationCode) {
          <div class="detail-row">
            <span class="label">Abholort:</span>
            <span class="value">{{ reservation().pickupLocationCode }}</span>
          </div>
        }
        @if (showLocations() && reservation().dropoffLocationCode) {
          <div class="detail-row">
            <span class="label">Rückgabeort:</span>
            <span class="value">{{ reservation().dropoffLocationCode }}</span>
          </div>
        }
        <div class="detail-row price">
          <span class="label">Gesamtpreis:</span>
          <span class="value">{{
            formatPrice(reservation().totalPriceGross)
          }}</span>
        </div>
      </div>
      <div class="card-footer">
        @if (showDetails()) {
          <button class="btn-secondary" (click)="onViewDetails()">
            Details
          </button>
        }
        @if (showPrint()) {
          <button class="btn-secondary" (click)="onPrint()">Drucken</button>
        }
        @if (showCancel() && canCancel()) {
          <button class="btn-danger" (click)="onCancel()">Stornieren</button>
        }
      </div>
    </div>
  `,
  styles: [
    `
      .reservation-card {
        background: white;
        border: 1px solid #dee2e6;
        border-radius: 8px;
        overflow: hidden;
        transition: box-shadow 0.2s;
      }

      .reservation-card:hover {
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      }

      .reservation-card.pending {
        border-left: 4px solid #ffc107;
      }

      .reservation-card.past {
        opacity: 0.8;
        border-left: 4px solid #6c757d;
      }

      .card-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem;
        background: #f8f9fa;
        border-bottom: 1px solid #dee2e6;
      }

      .reservation-id {
        font-family: monospace;
        font-size: 0.875rem;
        color: #6c757d;
      }

      .card-body {
        padding: 1rem;
      }

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

      .detail-row.price {
        font-weight: 500;
        margin-top: 0.5rem;
        padding-top: 0.75rem;
        border-top: 1px solid #dee2e6;
      }

      .label {
        color: #6c757d;
        font-size: 0.875rem;
      }

      .value {
        color: #212529;
      }

      .card-footer {
        display: flex;
        gap: 0.5rem;
        padding: 1rem;
        background: #f8f9fa;
        border-top: 1px solid #dee2e6;
      }

      .btn-secondary,
      .btn-danger {
        padding: 0.5rem 1rem;
        border-radius: 4px;
        font-weight: 500;
        cursor: pointer;
        transition: all 0.2s;
      }

      .btn-secondary {
        background: white;
        border: 1px solid #dee2e6;
        color: #495057;
      }

      .btn-secondary:hover {
        background: #f8f9fa;
        border-color: #ff7f00;
        color: #ff7f00;
      }

      .btn-danger {
        background: #dc3545;
        border: 1px solid #dc3545;
        color: white;
      }

      .btn-danger:hover {
        background: #c82333;
        border-color: #bd2130;
      }
    `,
  ],
})
export class ReservationCardComponent {
  readonly reservation = input.required<ReservationCardData>();
  readonly variant = input<ReservationCardVariant>("upcoming");
  readonly canCancel = input(false);
  readonly showDetails = input(true);
  readonly showPrint = input(false);
  readonly showCancel = input(true);

  readonly viewDetails = output<ReservationCardData>();
  readonly cancel = output<ReservationCardData>();
  readonly print = output<ReservationCardData>();

  protected readonly formatDate = formatDateDE;
  protected readonly formatPrice = formatPriceDE;

  protected readonly showLocations = computed(
    () => this.variant() === "upcoming" || this.variant() === "guest",
  );

  protected onViewDetails(): void {
    this.viewDetails.emit(this.reservation());
  }

  protected onCancel(): void {
    this.cancel.emit(this.reservation());
  }

  protected onPrint(): void {
    this.print.emit(this.reservation());
  }
}
