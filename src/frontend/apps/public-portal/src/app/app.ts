import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { VehicleService } from './services/vehicle.service';
import { Vehicle } from './services/vehicle.model';
import { VehicleSearchComponent, VehicleSearchQuery } from './components/vehicle-search/vehicle-search.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, DecimalPipe, VehicleSearchComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly vehicleService = inject(VehicleService);

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly totalCount = signal(0);

  ngOnInit() {
    // Load all vehicles on init with empty search
    this.onSearch({});
  }

  /**
   * Handle search event from vehicle search component
   */
  protected onSearch(query: VehicleSearchQuery) {
    this.loading.set(true);
    this.error.set(null);

    this.vehicleService.searchVehicles(query).subscribe({
      next: (result) => {
        this.vehicles.set(result.vehicles);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading vehicles:', err);
        this.error.set('Fehler beim Laden der Fahrzeuge. Bitte versuchen Sie es später erneut.');
        this.loading.set(false);
      }
    });
  }
}
