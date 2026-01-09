import { Component, input, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import type { Vehicle } from '@orange-car-rental/vehicle-api';
import { VehicleCardComponent, IconComponent } from '@orange-car-rental/ui-components';

/**
 * Similar Vehicles Component
 *
 * Displays a list of similar vehicles as alternatives
 * Shows up to 4 vehicles with comparison details
 * Allows user to select an alternative vehicle
 */
@Component({
  selector: 'app-similar-vehicles',
  standalone: true,
  imports: [CommonModule, TranslateModule, VehicleCardComponent, IconComponent],
  templateUrl: './similar-vehicles.component.html',
  styleUrl: './similar-vehicles.component.css',
})
export class SimilarVehiclesComponent {
  private readonly translate = inject(TranslateService);

  readonly currentVehicle = input<Vehicle | null>(null);
  readonly similarVehicles = input<Vehicle[]>([]);
  readonly showUnavailableWarning = input(false);
  readonly vehicleSelected = output<Vehicle>();

  /**
   * Calculate price difference between current and alternative vehicle
   */
  protected getPriceDifference(vehicle: Vehicle): { amount: number; text: string } {
    if (!this.currentVehicle()) {
      return { amount: 0, text: '' };
    }

    const currentPrice = this.currentVehicle()!.dailyRateGross;
    const alternativePrice = vehicle.dailyRateGross;
    const difference = currentPrice - alternativePrice;
    const absDifference = Math.abs(difference);

    if (difference > 0) {
      return {
        amount: difference,
        text: this.translate.instant('similarVehicles.cheaper', {
          amount: absDifference.toFixed(2),
        }),
      };
    } else if (difference < 0) {
      return {
        amount: difference,
        text: this.translate.instant('similarVehicles.moreExpensive', {
          amount: absDifference.toFixed(2),
        }),
      };
    } else {
      return {
        amount: 0,
        text: this.translate.instant('similarVehicles.samePrice'),
      };
    }
  }

  /**
   * Get similarity reason for the vehicle
   */
  protected getSimilarityReason(vehicle: Vehicle): string {
    if (!this.currentVehicle()) {
      return '';
    }

    const reasons: string[] = [];

    // Same category
    if (vehicle.categoryCode === this.currentVehicle()!.categoryCode) {
      reasons.push(this.translate.instant('similarVehicles.sameCategory'));
    } else {
      reasons.push(this.translate.instant('similarVehicles.similarCategory'));
    }

    // Price comparison
    const priceDiff = this.getPriceDifference(vehicle);
    if (priceDiff.amount > 0) {
      reasons.push(this.translate.instant('similarVehicles.cheaperLabel'));
    } else if (priceDiff.amount === 0) {
      reasons.push(this.translate.instant('similarVehicles.samePrice'));
    }

    // Same fuel type
    if (vehicle.fuelType === this.currentVehicle()!.fuelType) {
      reasons.push(this.translate.instant(`vehicles.fuelType.${vehicle.fuelType.toLowerCase()}`));
    }

    // Same transmission
    if (vehicle.transmissionType === this.currentVehicle()!.transmissionType) {
      reasons.push(
        this.translate.instant(`vehicles.transmission.${vehicle.transmissionType.toLowerCase()}`),
      );
    }

    return reasons.join(' • ');
  }

  /**
   * Handle vehicle selection
   */
  protected selectVehicle(vehicle: Vehicle): void {
    this.vehicleSelected.emit(vehicle);
  }

  /**
   * Format fuel type using translation
   */
  protected formatFuelType(fuelType: string): string {
    const key = `vehicles.fuelType.${fuelType.toLowerCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : fuelType;
  }

  /**
   * Format transmission type using translation
   */
  protected formatTransmissionType(transmissionType: string): string {
    const key = `vehicles.transmission.${transmissionType.toLowerCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : transmissionType;
  }
}
