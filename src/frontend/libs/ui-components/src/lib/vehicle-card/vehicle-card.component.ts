import { Component, input, output, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import type { Vehicle } from '@orange-car-rental/vehicle-api';
import { IconComponent } from '../icon';

export type VehicleCardVariant = 'browse' | 'similar' | 'compact';

export interface PriceDifference {
  amount: number;
  text: string;
  isCheaper: boolean;
}

/**
 * Reusable Vehicle Card Component
 *
 * Displays vehicle information in a card format with optional features:
 * - Image with placeholder fallback
 * - Name, category, location
 * - Specs (seats, transmission, fuel type)
 * - Pricing with optional comparison
 * - Similarity reason (for alternative vehicles)
 * - Action button (book/select)
 *
 * @example
 * <ocr-vehicle-card
 *   [vehicle]="vehicle"
 *   variant="browse"
 *   [actionLabel]="'Book Now'"
 *   (action)="onBookVehicle($event)"
 * />
 */
@Component({
  selector: 'ocr-vehicle-card',
  standalone: true,
  imports: [CommonModule, TranslateModule, IconComponent],
  template: `
    <div class="vehicle-card" [class.compact]="variant() === 'compact'">
      <!-- Vehicle Image -->
      <div class="vehicle-image">
        @if (vehicle().imageUrl) {
          <img [src]="vehicle().imageUrl" [alt]="vehicle().name" />
        } @else {
          <div class="placeholder-image">
            <lib-icon name="car-filled" variant="filled" size="lg" />
          </div>
        }
      </div>

      <!-- Vehicle Details -->
      <div class="vehicle-details">
        <!-- Header -->
        <div class="vehicle-header">
          <h3 class="vehicle-name">{{ vehicle().name }}</h3>
          <span class="vehicle-category">{{ vehicle().categoryName }}</span>
        </div>

        <!-- Location (browse variant) -->
        @if (showLocation() && vehicle().city) {
          <div class="vehicle-location">
            <lib-icon name="map-pin" variant="outline" size="xs" />
            <span>{{ vehicle().city }}</span>
          </div>
        }

        <!-- Price Comparison (similar variant) -->
        @if (showPriceComparison() && priceDiff()) {
          <div class="price-comparison">
            <span class="current-price">
              {{ vehicle().dailyRateGross | number: '1.2-2' : 'de-DE' }} €
              <span class="price-period">{{ 'vehicles.card.perDay' | translate }}</span>
            </span>
            @if (priceDiff()!.text) {
              <span
                class="price-difference"
                [class.cheaper]="priceDiff()!.isCheaper"
                [class.more-expensive]="!priceDiff()!.isCheaper && priceDiff()!.amount !== 0"
              >
                {{ priceDiff()!.text }}
              </span>
            }
          </div>
        }

        <!-- Simple Pricing (browse variant) -->
        @if (!showPriceComparison()) {
          <div class="vehicle-pricing">
            <div class="price-main">
              <span class="price-amount">
                {{ vehicle().dailyRateGross | number: '1.2-2' : 'de-DE' }} €
              </span>
              <span class="price-period">{{ 'vehicles.card.perDay' | translate }}</span>
            </div>
            <div class="price-details">
              <span>{{ 'vehicles.card.inclVat' | translate }}</span>
            </div>
          </div>
        }

        <!-- Specifications -->
        <div class="vehicle-specs">
          <span class="spec">
            <lib-icon name="users" variant="outline" size="xs" />
            {{ 'vehicles.card.seats' | translate: { count: vehicle().seats } }}
          </span>
          <span class="spec">
            <lib-icon name="cog" variant="outline" size="xs" />
            {{ formatTransmission(vehicle().transmissionType) }}
          </span>
          <span class="spec">
            <lib-icon name="fuel" variant="outline" size="xs" />
            {{ formatFuelType(vehicle().fuelType) }}
          </span>
        </div>

        <!-- Similarity Reason -->
        @if (similarityReason()) {
          <div class="similarity-reason">
            <lib-icon name="info" variant="outline" size="xs" />
            <span>{{ similarityReason() }}</span>
          </div>
        }

        <!-- Action Button -->
        <button type="button" class="action-button" [class.secondary]="variant() === 'similar'" (click)="onAction()">
          @if (variant() === 'similar') {
            <lib-icon name="chevron-right" variant="outline" size="sm" />
          }
          {{ actionLabel() || ('common.actions.bookNow' | translate) }}
        </button>
      </div>
    </div>
  `,
  styles: [
    `
      .vehicle-card {
        display: flex;
        flex-direction: column;
        background: white;
        border-radius: 0.75rem;
        border: 1px solid #e5e7eb;
        overflow: hidden;
        transition: box-shadow 0.2s ease, transform 0.2s ease;
      }

      .vehicle-card:hover {
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        transform: translateY(-2px);
      }

      .vehicle-image {
        position: relative;
        height: 160px;
        background: #f3f4f6;
        overflow: hidden;
      }

      .vehicle-image img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }

      .placeholder-image {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 100%;
        color: #9ca3af;
      }

      .vehicle-details {
        padding: 1rem;
        display: flex;
        flex-direction: column;
        gap: 0.75rem;
        flex: 1;
      }

      .vehicle-header {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .vehicle-name {
        margin: 0;
        font-size: 1.125rem;
        font-weight: 600;
        color: #111827;
        line-height: 1.3;
      }

      .vehicle-category {
        font-size: 0.875rem;
        color: #6b7280;
      }

      .vehicle-location {
        display: flex;
        align-items: center;
        gap: 0.25rem;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .vehicle-pricing {
        display: flex;
        flex-direction: column;
        gap: 0.125rem;
      }

      .price-main {
        display: flex;
        align-items: baseline;
        gap: 0.25rem;
      }

      .price-amount {
        font-size: 1.5rem;
        font-weight: 700;
        color: #f97316;
      }

      .price-period {
        font-size: 0.875rem;
        color: #6b7280;
      }

      .price-details {
        font-size: 0.75rem;
        color: #9ca3af;
      }

      .price-comparison {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .price-comparison .current-price {
        font-size: 1.25rem;
        font-weight: 600;
        color: #111827;
      }

      .price-comparison .current-price .price-period {
        font-size: 0.75rem;
        font-weight: 400;
      }

      .price-difference {
        font-size: 0.75rem;
        font-weight: 500;
        padding: 0.125rem 0.5rem;
        border-radius: 9999px;
        width: fit-content;
      }

      .price-difference.cheaper {
        background: #dcfce7;
        color: #166534;
      }

      .price-difference.more-expensive {
        background: #fef2f2;
        color: #991b1b;
      }

      .vehicle-specs {
        display: flex;
        flex-wrap: wrap;
        gap: 0.75rem;
      }

      .spec {
        display: flex;
        align-items: center;
        gap: 0.25rem;
        font-size: 0.75rem;
        color: #6b7280;
      }

      .similarity-reason {
        display: flex;
        align-items: flex-start;
        gap: 0.375rem;
        padding: 0.5rem;
        background: #f9fafb;
        border-radius: 0.375rem;
        font-size: 0.75rem;
        color: #6b7280;
      }

      .action-button {
        width: 100%;
        padding: 0.75rem 1rem;
        font-size: 0.875rem;
        font-weight: 600;
        color: white;
        background: #f97316;
        border: none;
        border-radius: 0.5rem;
        cursor: pointer;
        transition: background-color 0.15s ease;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        margin-top: auto;
      }

      .action-button:hover {
        background: #ea580c;
      }

      .action-button.secondary {
        background: transparent;
        color: #f97316;
        border: 2px solid #f97316;
      }

      .action-button.secondary:hover {
        background: #fff7ed;
      }

      /* Compact variant */
      .vehicle-card.compact .vehicle-image {
        height: 120px;
      }

      .vehicle-card.compact .vehicle-details {
        padding: 0.75rem;
        gap: 0.5rem;
      }

      .vehicle-card.compact .vehicle-name {
        font-size: 1rem;
      }

      .vehicle-card.compact .price-amount {
        font-size: 1.25rem;
      }

      .vehicle-card.compact .action-button {
        padding: 0.5rem 0.75rem;
        font-size: 0.8125rem;
      }
    `,
  ],
})
export class VehicleCardComponent {
  private readonly translate = inject(TranslateService);

  /** The vehicle to display */
  readonly vehicle = input.required<Vehicle>();

  /** Card variant: browse (default), similar, or compact */
  readonly variant = input<VehicleCardVariant>('browse');

  /** Show location (city) */
  readonly showLocation = input(true);

  /** Show price comparison with another vehicle */
  readonly showPriceComparison = input(false);

  /** Vehicle to compare price with (for similar variant) */
  readonly comparisonVehicle = input<Vehicle | null>(null);

  /** Custom similarity reason text */
  readonly similarityReason = input<string | null>(null);

  /** Custom action button label */
  readonly actionLabel = input<string | null>(null);

  /** Event emitted when action button is clicked */
  readonly action = output<Vehicle>();

  /** Computed price difference */
  readonly priceDiff = computed<PriceDifference | null>(() => {
    const comparison = this.comparisonVehicle();
    if (!comparison || !this.showPriceComparison()) {
      return null;
    }

    const currentPrice = comparison.dailyRateGross;
    const thisPrice = this.vehicle().dailyRateGross;
    const difference = currentPrice - thisPrice;
    const absDifference = Math.abs(difference);

    if (difference > 0) {
      return {
        amount: difference,
        isCheaper: true,
        text: this.translate.instant('similarVehicles.cheaper', {
          amount: absDifference.toFixed(2),
        }),
      };
    } else if (difference < 0) {
      return {
        amount: difference,
        isCheaper: false,
        text: this.translate.instant('similarVehicles.moreExpensive', {
          amount: absDifference.toFixed(2),
        }),
      };
    } else {
      return {
        amount: 0,
        isCheaper: false,
        text: this.translate.instant('similarVehicles.samePrice'),
      };
    }
  });

  protected onAction(): void {
    this.action.emit(this.vehicle());
  }

  protected formatFuelType(fuelType: string): string {
    const key = `vehicles.fuelType.${fuelType.toLowerCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : fuelType;
  }

  protected formatTransmission(transmissionType: string): string {
    const key = `vehicles.transmission.${transmissionType.toLowerCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : transmissionType;
  }
}
