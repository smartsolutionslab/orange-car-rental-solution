import { Component, inject, signal, computed, DestroyRef } from '@angular/core';
import type { OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import type { Location } from '@orange-car-rental/location-api';
import { LocationService } from '@orange-car-rental/location-api';
import type { Vehicle } from '@orange-car-rental/vehicle-api';
import { VehicleStatus } from '@orange-car-rental/vehicle-api';
import { logError } from '@orange-car-rental/util';
import {
  StatusBadgeComponent,
  ModalComponent,
  LoadingStateComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  StatCardComponent,
  IconComponent,
  getVehicleStatusClass,
  getVehicleStatusLabel,
} from '@orange-car-rental/ui-components';
import { VehicleService } from '../../services/vehicle.service';
import { DEFAULT_PAGE_SIZE, UTILIZATION_THRESHOLDS } from '../../constants/app.constants';
import type { LocationStatistics, VehicleDistribution } from '../../types';
/**
 * Locations management page for call center
 * Displays rental locations and management tools
 */
@Component({
  selector: 'app-locations',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    StatusBadgeComponent,
    ModalComponent,
    LoadingStateComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    StatCardComponent,
    IconComponent,
  ],
  templateUrl: './locations.component.html',
  styleUrl: './locations.component.css'
})
export class LocationsComponent implements OnInit {
  private readonly locationService = inject(LocationService);
  private readonly vehicleService = inject(VehicleService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly locations = signal<Location[]>([]);
  protected readonly allVehicles = signal<Vehicle[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedLocation = signal<Location | null>(null);
  protected readonly showDetails = signal(false);
  protected readonly locationVehicles = signal<Vehicle[]>([]);
  protected readonly loadingVehicles = signal(false);

  // Search filter
  protected readonly searchCity = signal<string>('');

  // Computed filtered locations
  protected readonly filteredLocations = computed(() => {
    const search = this.searchCity().toLowerCase();
    if (!search) return this.locations();

    return this.locations().filter(loc =>
      loc.city.toLowerCase().includes(search) ||
      loc.name.toLowerCase().includes(search) ||
      loc.code.toLowerCase().includes(search)
    );
  });

  // Statistics by location
  protected readonly locationStats = computed(() => {
    const stats = new Map<string, LocationStatistics>();
    const vehicles = this.allVehicles();

    this.locations().forEach(location => {
      const locationVehicles = vehicles.filter(v => v.locationCode === location.code);
      const available = locationVehicles.filter(v => v.status === VehicleStatus.Available).length;
      const rented = locationVehicles.filter(v => v.status === VehicleStatus.Rented).length;
      const maintenance = locationVehicles.filter(v => v.status === VehicleStatus.Maintenance).length;
      const outOfService = locationVehicles.filter(v => v.status === VehicleStatus.OutOfService).length;
      const reserved = locationVehicles.filter(v => v.status === VehicleStatus.Reserved).length;
      const total = locationVehicles.length;

      stats.set(location.code, {
        locationCode: location.code,
        locationName: location.name,
        totalVehicles: total,
        availableVehicles: available,
        rentedVehicles: rented,
        maintenanceVehicles: maintenance,
        outOfServiceVehicles: outOfService,
        utilizationRate: total > 0 ? Math.round(((rented + reserved) / total) * 100) : 0
      });
    });

    return stats;
  });

  ngOnInit(): void {
    this.loadLocations();
  }

  private loadLocations(): void {
    this.loading.set(true);
    this.error.set(null);

    this.locationService.getAllLocations().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (locations) => {
        this.locations.set(locations);
        this.loading.set(false);
        this.loadVehicleCounts();
      },
      error: (err) => {
        logError('LocationsComponent', 'Error loading locations', err);
        this.error.set('Fehler beim Laden der Standorte');
        this.loading.set(false);
      }
    });
  }

  /**
   * Load all vehicles for statistics
   */
  private loadVehicleCounts(): void {
    this.vehicleService.searchVehicles({ pageSize: DEFAULT_PAGE_SIZE.VEHICLES }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (result) => {
        this.allVehicles.set(result.vehicles);
      },
      error: (err) => {
        logError('LocationsComponent', 'Error loading vehicles', err);
      }
    });
  }

  /**
   * Get statistics for a location
   */
  protected getLocationStats(locationCode: string): LocationStatistics | undefined {
    return this.locationStats().get(locationCode);
  }

  /**
   * Get vehicle distribution for a location
   */
  protected getVehicleDistribution(locationCode: string): VehicleDistribution {
    const stats = this.locationStats().get(locationCode);
    if (!stats) {
      return { available: 0, rented: 0, maintenance: 0, outOfService: 0, reserved: 0 };
    }

    return {
      available: stats.availableVehicles,
      rented: stats.rentedVehicles,
      maintenance: stats.maintenanceVehicles,
      outOfService: stats.outOfServiceVehicles,
      reserved: 0 // Reserved is included in utilizationRate calculation
    };
  }

  /**
   * Clear search filter
   */
  protected clearSearch(): void {
    this.searchCity.set('');
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

    this.vehicleService.searchVehicles({ locationCode }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: (result) => {
        this.locationVehicles.set(result.vehicles);
        this.loadingVehicles.set(false);
      },
      error: (err) => {
        logError('LocationsComponent', 'Error loading location vehicles', err);
        this.loadingVehicles.set(false);
      }
    });
  }

  /**
   * Get status badge class
   */
  protected getStatusClass = getVehicleStatusClass;

  /**
   * Get status display text
   */
  protected getStatusText = getVehicleStatusLabel;

  /**
   * Get total number of locations
   */
  protected get totalLocations(): number {
    return this.locations().length;
  }

  /**
   * Get number of active locations (locations with vehicles)
   */
  protected get activeLocations(): number {
    return Array.from(this.locationStats().values())
      .filter(stat => stat.totalVehicles > 0).length;
  }

  /**
   * Get total vehicles across all locations
   */
  protected get totalVehicles(): number {
    return this.allVehicles().length;
  }

  /**
   * Get total available vehicles across all locations
   */
  protected get totalAvailableVehicles(): number {
    return this.allVehicles().filter(v => v.status === VehicleStatus.Available).length;
  }

  /**
   * Get total rented vehicles across all locations
   */
  protected get totalRentedVehicles(): number {
    return this.allVehicles().filter(v => v.status === VehicleStatus.Rented).length;
  }

  /**
   * Get overall fleet utilization rate
   */
  protected get overallUtilizationRate(): number {
    const total = this.totalVehicles;
    if (total === 0) return 0;
    const rented = this.totalRentedVehicles;
    return Math.round((rented / total) * 100);
  }

  /**
   * Get the most utilized location
   */
  protected get mostUtilizedLocation(): LocationStatistics | null {
    const stats = Array.from(this.locationStats().values());
    if (stats.length === 0) return null;

    return stats.reduce((prev, current) =>
      (current.utilizationRate > prev.utilizationRate) ? current : prev
    );
  }

  /**
   * Get utilization rate class for styling
   */
  protected getUtilizationClass(rate: number): string {
    if (rate >= UTILIZATION_THRESHOLDS.HIGH) return 'utilization-high';
    if (rate >= UTILIZATION_THRESHOLDS.MEDIUM) return 'utilization-medium';
    return 'utilization-low';
  }

  /**
   * Format percentage
   */
  protected formatPercentage(value: number): string {
    return `${value}%`;
  }
}
