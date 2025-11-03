import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocationService } from '../../services/location.service';
import { Location } from '../../services/location.model';

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

  protected readonly locations = signal<Location[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

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
      },
      error: (error) => {
        console.error('Error loading locations:', error);
        this.error.set('Fehler beim Laden der Standorte');
        this.loading.set(false);
      }
    });
  }

  protected get totalLocations(): number {
    return this.locations().length;
  }

  protected get activeLocations(): number {
    // For now, assume all locations are active
    return this.locations().length;
  }
}
