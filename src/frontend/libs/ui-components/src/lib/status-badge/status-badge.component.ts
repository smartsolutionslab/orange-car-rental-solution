import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  getVehicleStatusClass,
  getVehicleStatusLabel,
  getReservationStatusClass,
  getReservationStatusLabel,
} from '../utils/status-display.utils';

/**
 * Status Badge Component
 * Displays a styled badge for vehicle or reservation status
 *
 * @example
 * <ui-status-badge type="vehicle" status="Available"></ui-status-badge>
 * <ui-status-badge type="reservation" status="Confirmed"></ui-status-badge>
 */
@Component({
  selector: 'ui-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="status-badge" [ngClass]="statusClass">
      {{ statusLabel }}
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-flex;
      align-items: center;
      padding: 0.25rem 0.75rem;
      border-radius: 9999px;
      font-size: 0.75rem;
      font-weight: 500;
      text-transform: capitalize;
      white-space: nowrap;
    }

    .status-success {
      background-color: #dcfce7;
      color: #166534;
    }

    .status-info {
      background-color: #dbeafe;
      color: #1e40af;
    }

    .status-warning {
      background-color: #fef3c7;
      color: #92400e;
    }

    .status-error {
      background-color: #fee2e2;
      color: #991b1b;
    }

    .status-completed {
      background-color: #e0e7ff;
      color: #3730a3;
    }

    .status-default {
      background-color: #f3f4f6;
      color: #374151;
    }
  `]
})
export class StatusBadgeComponent {
  @Input({ required: true }) status!: string;
  @Input({ required: true }) type!: 'vehicle' | 'reservation';

  get statusClass(): string {
    if (this.type === 'vehicle') {
      return getVehicleStatusClass(this.status) || 'status-default';
    }
    return getReservationStatusClass(this.status) || 'status-default';
  }

  get statusLabel(): string {
    if (this.type === 'vehicle') {
      return getVehicleStatusLabel(this.status);
    }
    return getReservationStatusLabel(this.status);
  }
}
