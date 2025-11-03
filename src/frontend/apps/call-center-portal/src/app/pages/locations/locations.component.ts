import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocationService } from '../../services/location.service';
import { VehicleService } from '../../services/vehicle.service';
import { Location } from '../../services/location.model';
import { Vehicle } from '../../services/vehicle.model';

/**
 * Locations management page for call center
 * Displays rental locations and management tools
 */
@Component({
  selector: 'app-locations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.css'
})
export class LocationsComponent implements OnInit {
  private readonly locationService = inject(LocationService);
  private readonly vehicleService = inject(VehicleService);

  protected readonly locations = signal<Location[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedLocation = signal<Location | null>(null);
  protected readonly showDetails = signal(false);
  protected readonly locationVehicles = signal<Vehicle[]>([]);
  protected readonly loadingVehicles = signal(false);

  // Map to store vehicle counts by location
  protected readonly vehicleCountByLocation = signal<Map<string, number>>(new Map());

  ngOnInit(): void {
    this.loadLocations();
  }

  private loadLocations(): void {
    this.loading.set(true);
    this.error.set(null);

    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations.set(locations);
        this.loading.set(false);
        this.loadVehicleCounts();
      },
      error: (error) => {
        console.error('Error loading locations:', error);
        this.error.set('Fehler beim Laden der Standorte');
        this.loading.set(false);
      }
    });
  }

  /**
   * Load vehicle counts for all locations
   */
  private loadVehicleCounts(): void {
    this.vehicleService.searchVehicles({}).subscribe({
      next: (result) => {
        const countMap = new Map<string, number>();
        result.vehicles.forEach(vehicle => {
          const count = countMap.get(vehicle.locationCode) || 0;
          countMap.set(vehicle.locationCode, count + 1);
        });
        this.vehicleCountByLocation.set(countMap);
      },
      error: (err) => {
        console.error('Error loading vehicle counts:', err);
      }
    });
  }

  /**
   * Get vehicle count for a location
   */
  protected getVehicleCount(locationCode: string): number {
    return this.vehicleCountByLocation().get(locationCode) || 0;
  }

  /**
   * View location details
   */
  protected viewDetails(location: Location): void {
    this.selectedLocation.set(location);
    this.showDetails.set(true);
    this.loadLocationVehicles(location.code);
  }

  /**
   * Close details modal
   */
  protected closeDetails(): void {
    this.showDetails.set(false);
    this.selectedLocation.set(null);
    this.locationVehicles.set([]);
  }

  /**
   * Load vehicles for a specific location
   */
  private loadLocationVehicles(locationCode: string): void {
    this.loadingVehicles.set(true);

    this.vehicleService.searchVehicles({ locationCode }).subscribe({
      next: (result) => {
        this.locationVehicles.set(result.vehicles);
        this.loadingVehicles.set(false);
      },
      error: (err) => {
        console.error('Error loading location vehicles:', err);
        this.loadingVehicles.set(false);
      }
    });
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

  protected get totalLocations(): number {
    return this.locations().length;
  }

  protected get activeLocations(): number {
    // For now, assume all locations are active
    return this.locations().length;
  }

  /**
   * Get total vehicles across all locations
   */
  protected get totalVehicles(): number {
    let total = 0;
    this.vehicleCountByLocation().forEach(count => total += count);
    return total;
  }
}
