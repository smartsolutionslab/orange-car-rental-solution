import { Component, output, signal, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LocationService, Location } from '@ocr/data-access';
import { CommonModule } from '@angular/common';

/**
 * Vehicle search query interface
 */
export interface VehicleSearchQuery {
  pickupDate?: string;
  returnDate?: string;
  locationCode?: string;
  categoryCode?: string;
  minSeats?: number;
  fuelType?: string;
  transmissionType?: string;
  maxDailyRateGross?: number;
  pageNumber?: number;
  pageSize?: number;
}

/**
 * Reusable vehicle search component with date range picker
 *
 * Features:
 * - Pickup and return date selection
 * - Location filter
 * - Vehicle category filter
 * - Fuel type filter
 * - Transmission type filter
 * - Min seats filter
 *
 * Emits search parameters when user clicks search button
 */
@Component({
  selector: 'ocr-vehicle-search',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './vehicle-search.component.html',
  styleUrl: './vehicle-search.component.css'
})
export class VehicleSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly locationService = inject(LocationService);

  // Output event for search
  search = output<VehicleSearchQuery>();

  // Form state
  protected readonly searchForm: FormGroup;
  protected readonly searching = signal(false);

  // Location data
  protected readonly locations = signal<Location[]>([]);
  protected readonly locationsLoading = signal(false);
  protected readonly locationsError = signal<string | null>(null);

  // Minimum dates for validation
  protected readonly today = new Date().toISOString().split('T')[0];
  protected readonly minReturnDate = signal(this.today);

  constructor() {
    // Initialize form with default values (day-wise rentals)
    this.searchForm = this.fb.group({
      pickupDate: [''],
      returnDate: [''],
      locationCode: [''],
      categoryCode: [''],
      fuelType: [''],
      transmissionType: [''],
      minSeats: [null]
    });
  }

  ngOnInit(): void {
    // Fetch locations from API
    this.loadLocations();

    // Set default dates (tomorrow for pickup, +3 days for return)
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    const returnDate = new Date(tomorrow);
    returnDate.setDate(returnDate.getDate() + 3);

    this.searchForm.patchValue({
      pickupDate: tomorrow.toISOString().split('T')[0],
      returnDate: returnDate.toISOString().split('T')[0]
    });

    // Update min return date when pickup date changes
    this.searchForm.get('pickupDate')?.valueChanges.subscribe((pickupDate: string) => {
      if (pickupDate) {
        const pickup = new Date(pickupDate);
        pickup.setDate(pickup.getDate() + 1);
        this.minReturnDate.set(pickup.toISOString().split('T')[0]);

        // Ensure return date is after pickup date
        const currentReturnDate = this.searchForm.get('returnDate')?.value;
        if (currentReturnDate && currentReturnDate <= pickupDate) {
          this.searchForm.patchValue({
            returnDate: pickup.toISOString().split('T')[0]
          });
        }
      }
    });
  }

  /**
   * Load locations from API
   */
  private loadLocations(): void {
    this.locationsLoading.set(true);
    this.locationsError.set(null);

    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations.set(locations);
        this.locationsLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading locations:', error);
        this.locationsError.set('Failed to load locations');
        this.locationsLoading.set(false);
      }
    });
  }

  /**
   * Handle search form submission (day-wise rentals)
   */
  protected onSearch(): void {
    if (this.searchForm.invalid) {
      return;
    }

    const formValue = this.searchForm.value;

    // Build search query with dates only (YYYY-MM-DD format)
    const query: VehicleSearchQuery = {
      pickupDate: formValue.pickupDate || undefined,
      returnDate: formValue.returnDate || undefined,
      locationCode: formValue.locationCode || undefined,
      categoryCode: formValue.categoryCode || undefined,
      fuelType: formValue.fuelType || undefined,
      transmissionType: formValue.transmissionType || undefined,
      minSeats: formValue.minSeats || undefined
    };

    // Remove undefined values
    const cleanQuery = Object.fromEntries(
      Object.entries(query).filter(([_, value]) => value !== undefined)
    ) as VehicleSearchQuery;

    this.search.emit(cleanQuery);
  }

  /**
   * Reset form to default values
   */
  protected onReset(): void {
    this.searchForm.reset({
      pickupDate: '',
      returnDate: '',
      locationCode: '',
      categoryCode: '',
      fuelType: '',
      transmissionType: '',
      minSeats: null
    });

    this.ngOnInit(); // Reinitialize default dates
  }
}
