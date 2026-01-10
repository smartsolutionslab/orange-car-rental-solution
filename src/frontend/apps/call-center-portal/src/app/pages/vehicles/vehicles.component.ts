import { Component, inject, signal, computed, DestroyRef } from '@angular/core';
import type { OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import type {
  Vehicle,
  DailyRate,
  AddVehicleRequest,
  CategoryCode,
} from '@orange-car-rental/vehicle-api';
import {
  VehicleStatus,
  VehicleCategoryLabel,
  FuelType,
  TransmissionType,
  VehicleStatusLabel,
} from '@orange-car-rental/vehicle-api';
import type { Location, LocationCode } from '@orange-car-rental/location-api';
import { LocationService } from '@orange-car-rental/location-api';
import { logError } from '@orange-car-rental/util';
import {
  SelectVehicleStatusComponent,
  SelectLocationComponent,
  SelectCategoryComponent,
  StatusBadgeComponent,
  ModalComponent,
  ErrorStateComponent,
  StatCardComponent,
  IconComponent,
  DataTableComponent,
  DataTableColumnDirective,
  getVehicleStatusClass,
  getVehicleStatusLabel,
} from '@orange-car-rental/ui-components';
import { VehicleService } from '../../services/vehicle.service';
import {
  UI_TIMING,
  VEHICLE_DEFAULTS,
  VEHICLE_CONSTRAINTS,
  GERMAN_VAT_MULTIPLIER,
} from '../../constants/app.constants';

/**
 * Vehicles management page for call center
 * Displays vehicle inventory and management tools
 */
@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    SelectVehicleStatusComponent,
    SelectLocationComponent,
    SelectCategoryComponent,
    StatusBadgeComponent,
    ModalComponent,
    ErrorStateComponent,
    StatCardComponent,
    IconComponent,
    DataTableComponent,
    DataTableColumnDirective,
  ],
  templateUrl: './vehicles.component.html',
  styleUrl: './vehicles.component.css',
})
export class VehiclesComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly locationService = inject(LocationService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  private readonly translate = inject(TranslateService);

  protected readonly vehicles = signal<Vehicle[]>([]);
  protected readonly locations = signal<Location[]>([]);
  protected readonly loading = signal(false);
  protected readonly loadingLocations = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedVehicle = signal<Vehicle | null>(null);
  protected readonly showDetails = signal(false);
  protected readonly showAddModal = signal(false);
  protected readonly showStatusModal = signal(false);
  protected readonly showLocationModal = signal(false);
  protected readonly showPricingModal = signal(false);
  protected readonly actionInProgress = signal(false);
  protected readonly successMessage = signal<string | null>(null);

  // Search filters
  protected readonly searchStatus = signal<string>('');
  protected readonly searchLocation = signal<string>('');
  protected readonly searchCategory = signal<string>('');

  // Reference data for dropdowns
  protected readonly categories = Object.entries(VehicleCategoryLabel).map(([code, name]) => ({
    code,
    name,
  }));
  protected readonly fuelTypes = Object.values(FuelType);
  protected readonly transmissionTypes = Object.values(TransmissionType);
  protected readonly statuses = Object.entries(VehicleStatusLabel).map(([code, label]) => ({
    code,
    label,
  }));

  // Forms
  protected addVehicleForm!: FormGroup;
  protected statusForm!: FormGroup;
  protected locationForm!: FormGroup;
  protected pricingForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForms();
    this.loadVehicles();
    this.loadLocations();
  }

  /**
   * Initialize all forms
   */
  private initializeForms(): void {
    this.addVehicleForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      manufacturer: [''],
      model: [''],
      year: [
        new Date().getFullYear(),
        [
          Validators.min(VEHICLE_CONSTRAINTS.MIN_YEAR),
          Validators.max(new Date().getFullYear() + VEHICLE_CONSTRAINTS.MAX_YEAR_OFFSET),
        ],
      ],
      imageUrl: [''],
      category: ['', Validators.required],
      seats: [
        VEHICLE_DEFAULTS.SEATS,
        [
          Validators.required,
          Validators.min(VEHICLE_CONSTRAINTS.MIN_SEATS),
          Validators.max(VEHICLE_CONSTRAINTS.MAX_SEATS),
        ],
      ],
      fuelType: ['Petrol', Validators.required],
      transmissionType: ['Manual', Validators.required],
      locationCode: ['', Validators.required],
      dailyRateNet: [VEHICLE_DEFAULTS.DAILY_RATE_NET, [Validators.required, Validators.min(1)]],
      licensePlate: [''],
    });

    this.statusForm = this.fb.group({
      status: [VehicleStatus.Available, Validators.required],
    });

    this.locationForm = this.fb.group({
      locationCode: ['', Validators.required],
    });

    this.pricingForm = this.fb.group({
      dailyRateNet: [VEHICLE_DEFAULTS.DAILY_RATE_NET, [Validators.required, Validators.min(1)]],
    });
  }

  /**
   * Load all locations
   */
  private loadLocations(): void {
    this.loadingLocations.set(true);
    this.locationService
      .getAllLocations()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (locations) => {
          this.locations.set(locations);
          this.loadingLocations.set(false);
        },
        error: (err) => {
          logError('VehiclesComponent', 'Error loading locations', err);
          this.loadingLocations.set(false);
        },
      });
  }

  /**
   * Load all vehicles
   */
  protected loadVehicles(): void {
    this.loading.set(true);
    this.error.set(null);

    const query: {
      status?: VehicleStatus;
      locationCode?: LocationCode;
      categoryCode?: CategoryCode;
    } = {};

    if (this.searchStatus()) query.status = this.searchStatus() as VehicleStatus;
    if (this.searchLocation()) query.locationCode = this.searchLocation() as LocationCode;
    if (this.searchCategory()) query.categoryCode = this.searchCategory() as CategoryCode;

    this.vehicleService
      .searchVehicles(query)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.vehicles.set(result.vehicles);
          this.loading.set(false);
        },
        error: (err) => {
          logError('VehiclesComponent', 'Error loading vehicles', err);
          this.error.set(this.translate.instant('vehicles.errors.loading'));
          this.loading.set(false);
        },
      });
  }

  /**
   * Apply filters and reload vehicles
   */
  protected applyFilters(): void {
    this.loadVehicles();
  }

  /**
   * Clear all filters
   */
  protected clearFilters(): void {
    this.searchStatus.set('');
    this.searchLocation.set('');
    this.searchCategory.set('');
    this.loadVehicles();
  }

  /**
   * View vehicle details
   */
  protected viewDetails(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.showDetails.set(true);
  }

  /**
   * Close details modal
   */
  protected closeDetails(): void {
    this.showDetails.set(false);
    this.selectedVehicle.set(null);
  }

  /**
   * Track by function for DataTable
   */
  protected readonly trackByVehicleId = (_index: number, vehicle: Vehicle) => vehicle.id;

  /**
   * Total number of vehicles
   */
  protected readonly totalVehicles = computed(() => this.vehicles().length);

  /**
   * Number of available vehicles
   */
  protected readonly availableVehicles = computed(
    () => this.vehicles().filter((v) => v.status === VehicleStatus.Available).length,
  );

  /**
   * Number of vehicles in maintenance
   */
  protected readonly maintenanceVehicles = computed(
    () => this.vehicles().filter((v) => v.status === VehicleStatus.Maintenance).length,
  );

  /**
   * Number of rented vehicles
   */
  protected readonly rentedVehicles = computed(
    () => this.vehicles().filter((v) => v.status === VehicleStatus.Rented).length,
  );

  /**
   * Get status badge class
   */
  protected getStatusClass = getVehicleStatusClass;

  /**
   * Get status display text
   */
  protected getStatusText = getVehicleStatusLabel;

  /**
   * Unique locations from vehicles
   */
  protected readonly uniqueLocations = computed(() => {
    const locations = new Set(this.vehicles().map((v) => v.locationCode));
    return Array.from(locations).sort();
  });

  /**
   * Unique categories from vehicles
   */
  protected readonly uniqueCategories = computed(() => {
    const categories = new Set(this.vehicles().map((v) => v.categoryCode));
    return Array.from(categories).sort();
  });

  // ============================================================================
  // FLEET MANAGEMENT ACTIONS
  // ============================================================================

  /**
   * Show add vehicle modal
   */
  protected showAddVehicleModal(): void {
    this.addVehicleForm.reset({
      year: new Date().getFullYear(),
      seats: VEHICLE_DEFAULTS.SEATS,
      fuelType: 'Petrol',
      transmissionType: 'Manual',
      dailyRateNet: VEHICLE_DEFAULTS.DAILY_RATE_NET,
    });
    this.showAddModal.set(true);
    this.error.set(null);
  }

  /**
   * Close add vehicle modal
   */
  protected closeAddVehicleModal(): void {
    this.showAddModal.set(false);
    this.addVehicleForm.reset();
  }

  /**
   * Add new vehicle to fleet
   */
  protected addNewVehicle(): void {
    if (this.addVehicleForm.invalid) {
      this.error.set(this.translate.instant('vehicles.errors.validation'));
      return;
    }

    const formValue = this.addVehicleForm.value;
    const request: AddVehicleRequest = {
      basicInfo: {
        name: formValue.name,
        manufacturer: formValue.manufacturer || undefined,
        model: formValue.model || undefined,
        year: formValue.year || undefined,
        imageUrl: formValue.imageUrl || undefined,
      },
      specifications: {
        category: formValue.category,
        seats: formValue.seats,
        fuelType: formValue.fuelType,
        transmissionType: formValue.transmissionType,
      },
      locationAndPricing: {
        locationCode: formValue.locationCode,
        dailyRateNet: formValue.dailyRateNet,
      },
      registration: formValue.licensePlate ? { licensePlate: formValue.licensePlate } : undefined,
    };

    this.actionInProgress.set(true);
    this.error.set(null);

    this.vehicleService
      .addVehicle(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (result) => {
          this.actionInProgress.set(false);
          this.successMessage.set(
            this.translate.instant('vehicles.success.added', { name: result.name }),
          );
          this.closeAddVehicleModal();
          this.loadVehicles();
          setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_DURATION);
        },
        error: (err) => {
          logError('VehiclesComponent', 'Error adding vehicle', err);
          this.actionInProgress.set(false);
          this.error.set(this.translate.instant('vehicles.errors.addVehicle'));
        },
      });
  }

  /**
   * Show status update modal
   */
  protected showStatusUpdateModal(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.statusForm.patchValue({ status: vehicle.status });
    this.showStatusModal.set(true);
    this.error.set(null);
  }

  /**
   * Close status modal
   */
  protected closeStatusModal(): void {
    this.showStatusModal.set(false);
    this.selectedVehicle.set(null);
  }

  /**
   * Update vehicle status
   */
  protected updateStatus(): void {
    const vehicle = this.selectedVehicle();
    if (!vehicle || this.statusForm.invalid) return;

    const newStatus = this.statusForm.value.status as VehicleStatus;
    this.actionInProgress.set(true);
    this.error.set(null);

    this.vehicleService
      .updateVehicleStatus(vehicle.id, newStatus)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.actionInProgress.set(false);
          this.successMessage.set(
            this.translate.instant('vehicles.success.statusUpdated', {
              name: vehicle.name,
              status: this.getStatusText(newStatus),
            }),
          );
          this.closeStatusModal();
          this.loadVehicles();
          setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_DURATION);
        },
        error: (err) => {
          logError('VehiclesComponent', 'Error updating status', err);
          this.actionInProgress.set(false);
          this.error.set(this.translate.instant('vehicles.errors.updateStatus'));
        },
      });
  }

  /**
   * Show location update modal
   */
  protected showLocationUpdateModal(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.locationForm.patchValue({ locationCode: vehicle.locationCode });
    this.showLocationModal.set(true);
    this.error.set(null);
  }

  /**
   * Close location modal
   */
  protected closeLocationModal(): void {
    this.showLocationModal.set(false);
    this.selectedVehicle.set(null);
  }

  /**
   * Update vehicle location
   */
  protected updateLocation(): void {
    const vehicle = this.selectedVehicle();
    if (!vehicle || this.locationForm.invalid) return;

    const newLocation = this.locationForm.value.locationCode as LocationCode;
    this.actionInProgress.set(true);
    this.error.set(null);

    this.vehicleService
      .updateVehicleLocation(vehicle.id, newLocation)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.actionInProgress.set(false);
          this.successMessage.set(
            this.translate.instant('vehicles.success.locationUpdated', { name: vehicle.name }),
          );
          this.closeLocationModal();
          this.loadVehicles();
          setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_DURATION);
        },
        error: (err) => {
          logError('VehiclesComponent', 'Error updating location', err);
          this.actionInProgress.set(false);
          this.error.set(this.translate.instant('vehicles.errors.updateLocation'));
        },
      });
  }

  /**
   * Show pricing update modal
   */
  protected showPricingUpdateModal(vehicle: Vehicle): void {
    this.selectedVehicle.set(vehicle);
    this.pricingForm.patchValue({ dailyRateNet: vehicle.dailyRateNet });
    this.showPricingModal.set(true);
    this.error.set(null);
  }

  /**
   * Close pricing modal
   */
  protected closePricingModal(): void {
    this.showPricingModal.set(false);
    this.selectedVehicle.set(null);
  }

  /**
   * Update vehicle pricing
   */
  protected updatePricing(): void {
    const vehicle = this.selectedVehicle();
    if (!vehicle || this.pricingForm.invalid) return;

    const newRate = this.pricingForm.value.dailyRateNet as DailyRate;
    this.actionInProgress.set(true);
    this.error.set(null);

    this.vehicleService
      .updateVehicleDailyRate(vehicle.id, newRate)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.actionInProgress.set(false);
          this.successMessage.set(
            this.translate.instant('vehicles.success.priceUpdated', { name: vehicle.name }),
          );
          this.closePricingModal();
          this.loadVehicles();
          setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_DURATION);
        },
        error: (err) => {
          logError('VehiclesComponent', 'Error updating pricing', err);
          this.actionInProgress.set(false);
          this.error.set(this.translate.instant('vehicles.errors.updatePrice'));
        },
      });
  }

  /**
   * Calculate daily rate gross with VAT
   */
  protected calculateGrossRate(netRate: number): number {
    return netRate * GERMAN_VAT_MULTIPLIER;
  }
}
