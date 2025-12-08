import { Component, signal, inject } from '@angular/core';
import type { OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import type { Vehicle, VehicleSearchQuery } from '@orange-car-rental/vehicle-api';
import { logError } from '@orange-car-rental/util';
import {
  LoadingStateComponent,
  EmptyStateComponent,
  ErrorStateComponent,
} from '@orange-car-rental/ui-components';
import { VehicleService } from '../../services/vehicle.service';
import { VehicleSearchComponent } from '../../components/vehicle-search/vehicle-search.component';

/**
 * Vehicle list page component
 * Displays vehicle search functionality and results
 */
@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [
    DecimalPipe,
    VehicleSearchComponent,
    LoadingStateComponent,
    EmptyStateComponent,
    ErrorStateComponent,
  ],
  templateUrl: './vehicle-list.component.html',
  styleUrl: './vehicle-list.component.css'
})
export class VehicleListComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly router = inject(Router);

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly totalCount = signal(0);
  protected readonly currentSearchQuery = signal<VehicleSearchQuery>({});

  ngOnInit() {
    // Load all vehicles on init with empty search
    this.onSearch({});
  }

  /**
   * Handle search event from vehicle search component
   */
  protected onSearch(query: VehicleSearchQuery) {
    this.currentSearchQuery.set(query);
    this.loading.set(true);
    this.error.set(null);

    this.vehicleService.searchVehicles(query).subscribe({
      next: (result) => {
        this.vehicles.set(result.vehicles);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        logError('VehicleListComponent', 'Error loading vehicles', err);
        this.error.set('Fehler beim Laden der Fahrzeuge. Bitte versuchen Sie es später erneut.');
        this.loading.set(false);
      }
    });
  }

  /**
   * Navigate to booking page with vehicle and search details
   */
  protected onBookVehicle(vehicle: Vehicle): void {
    const query = this.currentSearchQuery();

    this.router.navigate(['/booking'], {
      queryParams: {
        vehicleId: vehicle.id,
        categoryCode: vehicle.categoryCode,
        pickupDate: query.pickupDate || '',
        returnDate: query.returnDate || '',
        locationCode: query.locationCode || vehicle.locationCode
      }
    });
  }
}
