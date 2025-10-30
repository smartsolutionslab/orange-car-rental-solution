import { Component, output, signal, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

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
  imports: [ReactiveFormsModule],
  templateUrl: './vehicle-search.component.html',
  styleUrl: './vehicle-search.component.css'
})
export class VehicleSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  // Output event for search
  search = output<VehicleSearchQuery>();

  // Form state
  protected readonly searchForm: FormGroup;
  protected readonly searching = signal(false);

  // Minimum dates for validation
  protected readonly today = new Date().toISOString().split('T')[0];
  protected readonly minReturnDate = signal(this.today);

  constructor() {
    // Initialize form with default values
    this.searchForm = this.fb.group({
      pickupDate: [''],
      pickupTime: ['10:00'],
      returnDate: [''],
      returnTime: ['10:00'],
      locationCode: [''],
      categoryCode: [''],
      fuelType: [''],
      transmissionType: [''],
      minSeats: [null]
    });
  }

  ngOnInit(): void {
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
    this.searchForm.get('pickupDate')?.valueChanges.subscribe(pickupDate => {
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
   * Handle search form submission
   */
  protected onSearch(): void {
    if (this.searchForm.invalid) {
      return;
    }

    const formValue = this.searchForm.value;

    // Combine date and time into ISO 8601 format
    const pickupDateTime = formValue.pickupDate && formValue.pickupTime
      ? `${formValue.pickupDate}T${formValue.pickupTime}:00`
      : undefined;

    const returnDateTime = formValue.returnDate && formValue.returnTime
      ? `${formValue.returnDate}T${formValue.returnTime}:00`
      : undefined;

    // Build search query
    const query: VehicleSearchQuery = {
      pickupDate: pickupDateTime,
      returnDate: returnDateTime,
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
      pickupTime: '10:00',
      returnDate: '',
      returnTime: '10:00',
      locationCode: '',
      categoryCode: '',
      fuelType: '',
      transmissionType: '',
      minSeats: null
    });

    this.ngOnInit(); // Reinitialize default dates
  }
}
