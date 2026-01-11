import { Component, signal, inject, DestroyRef } from '@angular/core';
import type { OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import type { Vehicle, VehicleSearchQuery } from '@orange-car-rental/vehicle-api';
import { logError } from '@orange-car-rental/util';
import {
  LoadingStateComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  IconComponent,
  VehicleCardComponent,
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
    CommonModule,
    TranslateModule,
    VehicleSearchComponent,
    LoadingStateComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    IconComponent,
    VehicleCardComponent,
  ],
  templateUrl: './vehicle-list.component.html',
  styleUrl: './vehicle-list.component.css',
})
export class VehicleListComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly translate = inject(TranslateService);

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

    this.vehicleService
      .searchVehicles(query)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.vehicles.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
        },
        error: (err) => {
          logError('VehicleListComponent', 'Error loading vehicles', err);
          this.error.set(this.translate.instant('errors.generic'));
          this.loading.set(false);
        },
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
        locationCode: query.locationCode || vehicle.locationCode,
      },
    });
  }
}
