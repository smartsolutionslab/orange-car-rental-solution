import { Component, output, signal, inject, type OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import type { VehicleSearchQuery } from './types';
import { SelectCategoryComponent } from '../select-category/select-category.component';
import { SelectFuelTypeComponent } from '../select-fuel-type/select-fuel-type.component';
import { SelectTransmissionComponent } from '../select-transmission/select-transmission.component';
import { SelectLocationComponent } from '../select-location/select-location.component';
import { SelectSeatsComponent } from '../select-seats/select-seats.component';
import { IconComponent } from '../icon/icon.component';

export type { VehicleSearchQuery } from './types';

@Component({
  selector: 'ocr-vehicle-search',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    SelectCategoryComponent,
    SelectFuelTypeComponent,
    SelectTransmissionComponent,
    SelectLocationComponent,
    SelectSeatsComponent,
    IconComponent,
  ],
  templateUrl: './vehicle-search.component.html',
  styleUrl: './vehicle-search.component.css',
})
export class VehicleSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);

  search = output<VehicleSearchQuery>();

  protected readonly searchForm: FormGroup;
  protected readonly searching = signal(false);

  protected readonly today = new Date().toISOString().split('T')[0];
  protected readonly minReturnDate = signal(this.today);

  constructor() {
    this.searchForm = this.fb.group({
      pickupDate: [''],
      pickupTime: ['10:00'],
      returnDate: [''],
      returnTime: ['10:00'],
      locationCode: [''],
      categoryCode: [''],
      fuelType: [''],
      transmissionType: [''],
      minSeats: [null],
    });
  }

  ngOnInit(): void {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    const returnDate = new Date(tomorrow);
    returnDate.setDate(returnDate.getDate() + 3);

    this.searchForm.patchValue({
      pickupDate: tomorrow.toISOString().split('T')[0],
      returnDate: returnDate.toISOString().split('T')[0],
    });

    this.searchForm.get('pickupDate')?.valueChanges.subscribe((pickupDate) => {
      if (pickupDate) {
        const pickup = new Date(pickupDate);
        pickup.setDate(pickup.getDate() + 1);
        this.minReturnDate.set(pickup.toISOString().split('T')[0]);

        const currentReturnDate = this.searchForm.get('returnDate')?.value;
        if (currentReturnDate && currentReturnDate <= pickupDate) {
          this.searchForm.patchValue({
            returnDate: pickup.toISOString().split('T')[0],
          });
        }
      }
    });
  }

  protected onSearch(): void {
    if (this.searchForm.invalid) {
      return;
    }

    const formValue = this.searchForm.value;

    const pickupDateTime =
      formValue.pickupDate && formValue.pickupTime
        ? `${formValue.pickupDate}T${formValue.pickupTime}:00`
        : undefined;

    const returnDateTime =
      formValue.returnDate && formValue.returnTime
        ? `${formValue.returnDate}T${formValue.returnTime}:00`
        : undefined;

    const query: VehicleSearchQuery = {
      pickupDate: pickupDateTime,
      returnDate: returnDateTime,
      locationCode: formValue.locationCode || undefined,
      categoryCode: formValue.categoryCode || undefined,
      fuelType: formValue.fuelType || undefined,
      transmissionType: formValue.transmissionType || undefined,
      minSeats: formValue.minSeats || undefined,
    };

    const cleanQuery = Object.fromEntries(
      Object.entries(query).filter(([, value]) => value !== undefined)
    ) as VehicleSearchQuery;

    this.search.emit(cleanQuery);
  }

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
      minSeats: null,
    });

    this.ngOnInit();
  }
}
