import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VehicleService } from '../../services/vehicle.service';
import { LocationService } from '../../services/location.service';
import {
  Vehicle,
  VehicleId,
  VehicleStatus,
  LocationCode,
  DailyRate,
  AddVehicleRequest,
  VEHICLE_CATEGORIES,
  FUEL_TYPES,
  TRANSMISSION_TYPES,
  VEHICLE_STATUSES
} from '../../services/vehicle.model';
import { Location } from '../../services/location.model';

/**
 * Vehicles management page for call center
 * Displays vehicle inventory and management tools
 */
@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './vehicles.component.html',
  styleUrl: './vehicles.component.css'
})
export class VehiclesComponent implements OnInit {
  private readonly vehicleService = inject(VehicleService);
  private readonly locationService = inject(LocationService);
  private readonly fb = inject(FormBuilder);

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
  protected readonly categories = VEHICLE_CATEGORIES;
  protected readonly fuelTypes = FUEL_TYPES;
  protected readonly transmissionTypes = TRANSMISSION_TYPES;
  protected readonly statuses = VEHICLE_STATUSES;

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
      year: [new Date().getFullYear(), [Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      imageUrl: [''],
      category: ['', Validators.required],
      seats: [5, [Validators.required, Validators.min(1), Validators.max(9)]],
      fuelType: ['Petrol', Validators.required],
      transmissionType: ['Manual', Validators.required],
      locationCode: ['', Validators.required],
      dailyRateNet: [50, [Validators.required, Validators.min(1)]],
      licensePlate: ['']
    });

    this.statusForm = this.fb.group({
      status: ['Available', Validators.required]
    });

    this.locationForm = this.fb.group({
      locationCode: ['', Validators.required]
    });

    this.pricingForm = this.fb.group({
      dailyRateNet: [50, [Validators.required, Validators.min(1)]]
    });
  }

  /**
   * Load all locations
   */
  private loadLocations(): void {
    this.loadingLocations.set(true);
    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations.set(locations);
        this.loadingLocations.set(false);
      },
      error: (err) => {
        console.error('Error loading locations:', err);
        this.loadingLocations.set(false);
      }
    });
  }

  /**
   * Load all vehicles
   */
  protected loadVehicles(): void {
    this.loading.set(true);
    this.error.set(null);

    const query: any = {};

    if (this.searchStatus()) {
      query.status = this.searchStatus();
    }
    if (this.searchLocation()) {
      query.locationCode = this.searchLocation();
    }
    if (this.searchCategory()) {
      query.categoryCode = this.searchCategory();
    }

    this.vehicleService.searchVehicles(query).subscribe({
      next: (result) => {
        this.vehicles.set(result.vehicles);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading vehicles:', err);
        this.error.set('Fehler beim Laden der Fahrzeuge');
        this.loading.set(false);
      }
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
   * Get total number of vehicles
   */
  protected get totalVehicles(): number {
    return this.vehicles().length;
  }

  /**
   * Get number of available vehicles
   */
  protected get availableVehicles(): number {
    return this.vehicles().filter(v => v.status === 'Available').length;
  }

  /**
   * Get number of vehicles in maintenance
   */
  protected get maintenanceVehicles(): number {
    return this.vehicles().filter(v => v.status === 'Maintenance').length;
  }

  /**
   * Get number of rented vehicles
   */
  protected get rentedVehicles(): number {
    return this.vehicles().filter(v => v.status === 'Rented').length;
  }

  /**
   * Get status badge class
   */
  protected getStatusClass(status: string): string {
    switch (status) {
      case 'Available':
        return 'status-success';
      case 'Rented':
        return 'status-info';
      case 'Maintenance':
        return 'status-warning';
      case 'OutOfService':
        return 'status-error';
      default:
        return '';
    }
  }

  /**
   * Get status display text
   */
  protected getStatusText(status: string): string {
    switch (status) {
      case 'Available':
        return 'Verfügbar';
      case 'Rented':
        return 'Vermietet';
      case 'Maintenance':
        return 'Wartung';
      case 'OutOfService':
        return 'Außer Betrieb';
      default:
        return status;
    }
  }

  /**
   * Get unique locations from vehicles
   */
  protected get uniqueLocations(): string[] {
    const locations = new Set(this.vehicles().map(v => v.locationCode));
    return Array.from(locations).sort();
  }

  /**
   * Get unique categories from vehicles
   */
  protected get uniqueCategories(): string[] {
    const categories = new Set(this.vehicles().map(v => v.categoryCode));
    return Array.from(categories).sort();
  }

  // ============================================================================
  // FLEET MANAGEMENT ACTIONS
  // ============================================================================

  /**
   * Show add vehicle modal
   */
  protected showAddVehicleModal(): void {
    this.addVehicleForm.reset({
      year: new Date().getFullYear(),
      seats: 5,
      fuelType: 'Petrol',
      transmissionType: 'Manual',
      dailyRateNet: 50
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
      this.error.set('Bitte füllen Sie alle Pflichtfelder korrekt aus');
      return;
    }

    const formValue = this.addVehicleForm.value;
    const request: AddVehicleRequest = {
      basicInfo: {
        name: formValue.name,
        manufacturer: formValue.manufacturer || undefined,
        model: formValue.model || undefined,
        year: formValue.year || undefined,
        imageUrl: formValue.imageUrl || undefined
      },
      specifications: {
        category: formValue.category,
        seats: formValue.seats,
        fuelType: formValue.fuelType,
        transmissionType: formValue.transmissionType
      },
      locationAndPricing: {
        locationCode: formValue.locationCode,
        dailyRateNet: formValue.dailyRateNet
      },
      registration: formValue.licensePlate ? { licensePlate: formValue.licensePlate } : undefined
    };

    this.actionInProgress.set(true);
    this.error.set(null);

    this.vehicleService.addVehicle(request).subscribe({
      next: (result) => {
        this.actionInProgress.set(false);
        this.successMessage.set(`Fahrzeug "${result.name}" erfolgreich hinzugefügt`);
        this.closeAddVehicleModal();
        this.loadVehicles();
        setTimeout(() => this.successMessage.set(null), 5000);
      },
      error: (err) => {
        console.error('Error adding vehicle:', err);
        this.actionInProgress.set(false);
        this.error.set('Fehler beim Hinzufügen des Fahrzeugs');
      }
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

    this.vehicleService.updateVehicleStatus(vehicle.id, newStatus).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.successMessage.set(`Status von "${vehicle.name}" auf "${this.getStatusText(newStatus)}" aktualisiert`);
        this.closeStatusModal();
        this.loadVehicles();
        setTimeout(() => this.successMessage.set(null), 5000);
      },
      error: (err) => {
        console.error('Error updating status:', err);
        this.actionInProgress.set(false);
        this.error.set('Fehler beim Aktualisieren des Status');
      }
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

    this.vehicleService.updateVehicleLocation(vehicle.id, newLocation).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.successMessage.set(`Standort von "${vehicle.name}" erfolgreich aktualisiert`);
        this.closeLocationModal();
        this.loadVehicles();
        setTimeout(() => this.successMessage.set(null), 5000);
      },
      error: (err) => {
        console.error('Error updating location:', err);
        this.actionInProgress.set(false);
        this.error.set('Fehler beim Aktualisieren des Standorts');
      }
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

    this.vehicleService.updateVehicleDailyRate(vehicle.id, newRate).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.successMessage.set(`Tagespreis von "${vehicle.name}" erfolgreich aktualisiert`);
        this.closePricingModal();
        this.loadVehicles();
        setTimeout(() => this.successMessage.set(null), 5000);
      },
      error: (err) => {
        console.error('Error updating pricing:', err);
        this.actionInProgress.set(false);
        this.error.set('Fehler beim Aktualisieren des Preises');
      }
    });
  }

  /**
   * Calculate daily rate gross with VAT
   */
  protected calculateGrossRate(netRate: number): number {
    return netRate * 1.19; // 19% German VAT
  }
}
