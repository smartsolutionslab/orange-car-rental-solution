import { Component, output, signal, inject, DestroyRef } from '@angular/core';
import type { OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import type {
  VehicleSearchQuery,
  FuelType,
  TransmissionType,
} from '@orange-car-rental/vehicle-api';
import {
  SelectLocationComponent,
  SelectCategoryComponent,
  SelectFuelTypeComponent,
  SelectTransmissionComponent,
  getTodayDateString,
  addDays,
  IconComponent,
} from '@orange-car-rental/ui-components';

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
  selector: 'app-vehicle-search',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    SelectLocationComponent,
    SelectCategoryComponent,
    SelectFuelTypeComponent,
    SelectTransmissionComponent,
    IconComponent,
  ],
  templateUrl: './vehicle-search.component.html',
  styleUrl: './vehicle-search.component.css',
})
export class VehicleSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  // Output event for search
  searchSubmit = output<VehicleSearchQuery>();

  // Form state
  protected readonly searchForm: FormGroup;
  protected readonly searching = signal(false);

  // Minimum dates for validation
  protected readonly today = getTodayDateString();
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
      minSeats: [null],
    });
  }

  ngOnInit(): void {
    // Set default dates (tomorrow for pickup, +3 days for return)
    const tomorrow = addDays(new Date(), 1);
    const defaultReturn = addDays(tomorrow, 3);

    this.searchForm.patchValue({
      pickupDate: tomorrow,
      returnDate: defaultReturn,
    });

    // Update min return date when pickup date changes
    this.searchForm
      .get('pickupDate')
      ?.valueChanges.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((pickupDate: string) => {
        if (pickupDate) {
          const minReturn = addDays(pickupDate, 1);
          this.minReturnDate.set(minReturn);

          // Ensure return date is after pickup date
          const currentReturnDate = this.searchForm.get('returnDate')?.value;
          if (currentReturnDate && currentReturnDate <= pickupDate) {
            this.searchForm.patchValue({
              returnDate: minReturn,
            });
          }
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
    // Only include properties that have truthy values
    const query: VehicleSearchQuery = {
      ...(formValue.pickupDate && { pickupDate: formValue.pickupDate }),
      ...(formValue.returnDate && { returnDate: formValue.returnDate }),
      ...(formValue.locationCode && { locationCode: formValue.locationCode }),
      ...(formValue.categoryCode && { categoryCode: formValue.categoryCode }),
      ...(formValue.fuelType && { fuelType: formValue.fuelType as FuelType }),
      ...(formValue.transmissionType && {
        transmissionType: formValue.transmissionType as TransmissionType,
      }),
      ...(formValue.minSeats && { minSeats: formValue.minSeats }),
    };

    this.searchSubmit.emit(query);
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
      minSeats: null,
    });

    this.ngOnInit(); // Reinitialize default dates
  }
}
