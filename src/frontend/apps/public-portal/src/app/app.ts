import { Component, signal, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DecimalPipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';

// Vehicle models (inline)
export interface Vehicle {
  id: string;
  name: string;
  categoryCode: string;
  categoryName: string;
  locationCode: string;
  city: string;
  seats: number;
  fuelType: string;
  transmissionType: string;
  dailyRateNet: number;
  dailyRateVat: number;
  dailyRateGross: number;
  currency: string;
  status: string;
  licensePlate: string | null;
  manufacturer: string;
  model: string;
  year: number;
  imageUrl: string | null;
}

export interface VehicleSearchResult {
  vehicles: Vehicle[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, DecimalPipe],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5046/api/vehicles';

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  private selectedLocation = '';
  private selectedCategory = '';
  private selectedFuel = '';
  private selectedTransmission = '';

  ngOnInit() {
    // Load all vehicles on init
    this.searchVehicles();
  }

  protected onFilterChange() {
    // Get values from select elements
    const locationSelect = document.getElementById('location') as HTMLSelectElement;
    const categorySelect = document.getElementById('category') as HTMLSelectElement;
    const fuelSelect = document.getElementById('fuel') as HTMLSelectElement;
    const transmissionSelect = document.getElementById('transmission') as HTMLSelectElement;

    this.selectedLocation = locationSelect?.value || '';
    this.selectedCategory = categorySelect?.value || '';
    this.selectedFuel = fuelSelect?.value || '';
    this.selectedTransmission = transmissionSelect?.value || '';
  }

  protected searchVehicles() {
    this.loading.set(true);
    this.error.set(null);

    let params = new HttpParams();
    if (this.selectedLocation) params = params.set('locationCode', this.selectedLocation);
    if (this.selectedCategory) params = params.set('categoryCode', this.selectedCategory);
    if (this.selectedFuel) params = params.set('fuelType', this.selectedFuel);
    if (this.selectedTransmission) params = params.set('transmissionType', this.selectedTransmission);

    this.http.get<VehicleSearchResult>(this.apiUrl, { params }).subscribe({
      next: (result) => {
        this.vehicles.set(result.vehicles);
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
