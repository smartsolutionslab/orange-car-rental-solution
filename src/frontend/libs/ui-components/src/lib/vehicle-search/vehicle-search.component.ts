import {
  Component,
  output,
  signal,
  inject,
  DestroyRef,
  type OnInit,
} from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormBuilder, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { TranslateModule } from "@ngx-translate/core";
import type { ISODateString } from "@orange-car-rental/shared";
import type { LocationCode } from "@orange-car-rental/location-api";
import type {
  CategoryCode,
  SeatingCapacity,
  FuelType,
  TransmissionType,
} from "@orange-car-rental/vehicle-api";
import type { VehicleSearchQuery } from "./types";
import { SelectCategoryComponent } from "../select-category/select-category.component";
import { SelectFuelTypeComponent } from "../select-fuel-type/select-fuel-type.component";
import { SelectTransmissionComponent } from "../select-transmission/select-transmission.component";
import { SelectLocationComponent } from "../select-location/select-location.component";
import { SelectSeatsComponent } from "../select-seats/select-seats.component";
import { IconComponent } from "../icon/icon.component";

export type { VehicleSearchQuery } from "./types";

/** Default pickup/return time for vehicle search */
const DEFAULT_SEARCH_TIME = "10:00";
/** Default rental duration in days */
const DEFAULT_RENTAL_DAYS = 3;

@Component({
  selector: "ocr-vehicle-search",
  standalone: true,
  imports: [
    ReactiveFormsModule,
    TranslateModule,
    SelectCategoryComponent,
    SelectFuelTypeComponent,
    SelectTransmissionComponent,
    SelectLocationComponent,
    SelectSeatsComponent,
    IconComponent,
  ],
  templateUrl: "./vehicle-search.component.html",
  styleUrl: "./vehicle-search.component.css",
})
export class VehicleSearchComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  search = output<VehicleSearchQuery>();

  protected readonly searchForm: FormGroup;
  protected readonly searching = signal(false);

  protected readonly today = new Date().toISOString().split("T")[0];
  protected readonly minReturnDate = signal(this.today);

  constructor() {
    this.searchForm = this.fb.group({
      pickupDate: [""],
      pickupTime: [DEFAULT_SEARCH_TIME],
      returnDate: [""],
      returnTime: [DEFAULT_SEARCH_TIME],
      locationCode: [""],
      categoryCode: [""],
      fuelType: [""],
      transmissionType: [""],
      minSeats: [null],
    });
  }

  ngOnInit(): void {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    const returnDate = new Date(tomorrow);
    returnDate.setDate(returnDate.getDate() + DEFAULT_RENTAL_DAYS);

    this.searchForm.patchValue({
      pickupDate: tomorrow.toISOString().split("T")[0],
      returnDate: returnDate.toISOString().split("T")[0],
    });

    this.searchForm
      .get("pickupDate")
      ?.valueChanges.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((pickupDate) => {
        if (pickupDate) {
          const pickup = new Date(pickupDate);
          pickup.setDate(pickup.getDate() + 1);
          this.minReturnDate.set(pickup.toISOString().split("T")[0]);

          const currentReturnDate = this.searchForm.get("returnDate")?.value;
          if (currentReturnDate && currentReturnDate <= pickupDate) {
            this.searchForm.patchValue({
              returnDate: pickup.toISOString().split("T")[0],
            });
          }
        }
      });
  }

  protected onSearch(): void {
    if (this.searchForm.invalid) return;

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
      pickupDate: pickupDateTime
        ? (pickupDateTime as ISODateString)
        : undefined,
      returnDate: returnDateTime
        ? (returnDateTime as ISODateString)
        : undefined,
      locationCode: formValue.locationCode
        ? (formValue.locationCode as LocationCode)
        : undefined,
      categoryCode: formValue.categoryCode
        ? (formValue.categoryCode as CategoryCode)
        : undefined,
      fuelType: formValue.fuelType
        ? (formValue.fuelType as FuelType)
        : undefined,
      transmissionType: formValue.transmissionType
        ? (formValue.transmissionType as TransmissionType)
        : undefined,
      minSeats: formValue.minSeats
        ? (formValue.minSeats as SeatingCapacity)
        : undefined,
    };

    const cleanQuery = Object.fromEntries(
      Object.entries(query).filter(([, value]) => value !== undefined),
    ) as VehicleSearchQuery;

    this.search.emit(cleanQuery);
  }

  protected onReset(): void {
    this.searchForm.reset({
      pickupDate: "",
      pickupTime: DEFAULT_SEARCH_TIME,
      returnDate: "",
      returnTime: DEFAULT_SEARCH_TIME,
      locationCode: "",
      categoryCode: "",
      fuelType: "",
      transmissionType: "",
      minSeats: null,
    });

    this.ngOnInit();
  }
}
