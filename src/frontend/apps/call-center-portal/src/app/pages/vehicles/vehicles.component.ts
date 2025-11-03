import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle } from '../../services/vehicle.model';

/**
 * Vehicles management page for call center
 * Displays vehicle inventory and management tools
 */
@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vehicles.component.html',
  styleUrl: './vehicles.component.css'
})
export class VehiclesComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedVehicle = signal<Vehicle | null>(null);
  protected readonly showDetails = signal(false);

  // Search filters
  protected readonly searchStatus = signal<string>('');
  protected readonly searchLocation = signal<string>('');
  protected readonly searchCategory = signal<string>('');

  ngOnInit(): void {
    this.loadVehicles();
  }

  /**
   * Load all vehicles
   */
  protected loadVehicles(): void {
    this.loading.set(true);
    this.error.set(null);

    const query: any = {};

    if (this.searchStatus()) {
      query.status = this.searchStatus();
    }
    if (this.searchLocation()) {
      query.locationCode = this.searchLocation();
    }
    if (this.searchCategory()) {
      query.categoryCode = this.searchCategory();
    }

    this.vehicleService.searchVehicles(query).subscribe({
      next: (result) => {
        this.vehicles.set(result.vehicles);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading vehicles:', err);
        this.error.set('Fehler beim Laden der Fahrzeuge');
        this.loading.set(false);
      }
    });
  }

  /**
   * Apply filters and reload vehicles
   */
  protected applyFilters(): void {
    this.loadVehicles();
  }

  /**
   * Clear all filters
   */
  protected clearFilters(): void {
    this.searchStatus.set('');
    this.searchLocation.set('');
    this.searchCategory.set('');
    this.loadVehicles();
  }

  /**
   * View vehicle details
   */
  protected viewDetails(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.showDetails.set(true);
  }

  /**
   * Close details modal
   */
  protected closeDetails(): void {
    this.showDetails.set(false);
    this.selectedVehicle.set(null);
  }

  /**
   * Get total number of vehicles
   */
  protected get totalVehicles(): number {
    return this.vehicles().length;
  }

  /**
   * Get number of available vehicles
   */
  protected get availableVehicles(): number {
    return this.vehicles().filter(v => v.status === 'Available').length;
  }

  /**
   * Get number of vehicles in maintenance
   */
  protected get maintenanceVehicles(): number {
    return this.vehicles().filter(v => v.status === 'Maintenance').length;
  }

  /**
   * Get number of rented vehicles
   */
  protected get rentedVehicles(): number {
    return this.vehicles().filter(v => v.status === 'Rented').length;
  }

  /**
   * Get status badge class
   */
  protected getStatusClass(status: string): string {
    switch (status) {
      case 'Available':
        return 'status-success';
      case 'Rented':
        return 'status-info';
      case 'Maintenance':
        return 'status-warning';
      case 'OutOfService':
        return 'status-error';
      default:
        return '';
    }
  }

  /**
   * Get status display text
   */
  protected getStatusText(status: string): string {
    switch (status) {
      case 'Available':
        return 'Verfügbar';
      case 'Rented':
        return 'Vermietet';
      case 'Maintenance':
        return 'Wartung';
      case 'OutOfService':
        return 'Außer Betrieb';
      default:
        return status;
    }
  }

  /**
   * Get unique locations from vehicles
   */
  protected get uniqueLocations(): string[] {
    const locations = new Set(this.vehicles().map(v => v.locationCode));
    return Array.from(locations).sort();
  }

  /**
   * Get unique categories from vehicles
   */
  protected get uniqueCategories(): string[] {
    const categories = new Set(this.vehicles().map(v => v.categoryCode));
    return Array.from(categories).sort();
  }
}
