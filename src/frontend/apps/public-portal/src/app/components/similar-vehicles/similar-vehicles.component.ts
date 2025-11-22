import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Vehicle } from '../../services/vehicle.model';

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
  imports: [CommonModule],
  templateUrl: './similar-vehicles.component.html',
  styleUrl: './similar-vehicles.component.css'
})
export class SimilarVehiclesComponent {
  @Input() currentVehicle: Vehicle | null = null;
  @Input() similarVehicles: Vehicle[] = [];
  @Input() showUnavailableWarning = false;
  @Output() vehicleSelected = new EventEmitter<Vehicle>();

  /**
   * Calculate price difference between current and alternative vehicle
   */
  protected getPriceDifference(vehicle: Vehicle): { amount: number; text: string } {
    if (!this.currentVehicle) {
      return { amount: 0, text: '' };
    }

    const currentPrice = this.currentVehicle.dailyRateGross;
    const alternativePrice = vehicle.dailyRateGross;
    const difference = currentPrice - alternativePrice;
    const absDifference = Math.abs(difference);

    if (difference > 0) {
      return {
        amount: difference,
        text: `€${absDifference.toFixed(2)}/Tag günstiger`
      };
    } else if (difference < 0) {
      return {
        amount: difference,
        text: `€${absDifference.toFixed(2)}/Tag teurer`
      };
    } else {
      return {
        amount: 0,
        text: 'Gleicher Preis'
      };
    }
  }

  /**
   * Get similarity reason for the vehicle
   */
  protected getSimilarityReason(vehicle: Vehicle): string {
    if (!this.currentVehicle) {
      return '';
    }

    const reasons: string[] = [];

    // Same category
    if (vehicle.categoryCode === this.currentVehicle.categoryCode) {
      reasons.push('Gleiche Kategorie');
    } else {
      reasons.push('Ähnliche Kategorie');
    }

    // Price comparison
    const priceDiff = this.getPriceDifference(vehicle);
    if (priceDiff.amount > 0) {
      reasons.push('Günstiger');
    } else if (priceDiff.amount === 0) {
      reasons.push('Gleicher Preis');
    }

    // Same fuel type
    if (vehicle.fuelType === this.currentVehicle.fuelType) {
      reasons.push(`${vehicle.fuelType}`);
    }

    // Same transmission
    if (vehicle.transmissionType === this.currentVehicle.transmissionType) {
      reasons.push(vehicle.transmissionType);
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
   * Format fuel type in German
   */
  protected formatFuelType(fuelType: string): string {
    const fuelTypes: Record<string, string> = {
      'Petrol': 'Benzin',
      'Diesel': 'Diesel',
      'Electric': 'Elektro',
      'Hybrid': 'Hybrid'
    };
    return fuelTypes[fuelType] || fuelType;
  }

  /**
   * Format transmission type in German
   */
  protected formatTransmissionType(transmissionType: string): string {
    const transmissionTypes: Record<string, string> = {
      'Manual': 'Manuell',
      'Automatic': 'Automatik'
    };
    return transmissionTypes[transmissionType] || transmissionType;
  }
}
