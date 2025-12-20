import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { VehiclesComponent } from './vehicles.component';
import { VehicleService } from '../../services/vehicle.service';
import { LocationService } from '@orange-car-rental/location-api';
import { of, throwError } from 'rxjs';
import type { Vehicle } from '@orange-car-rental/vehicle-api';
import { VehicleStatus } from '@orange-car-rental/vehicle-api';
import type { Location } from '@orange-car-rental/location-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import { UI_TIMING, GERMAN_VAT_MULTIPLIER } from '../../constants/app.constants';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;
  let mockVehicleService: jasmine.SpyObj<VehicleService>;
  let mockLocationService: jasmine.SpyObj<LocationService>;

  const mockVehicles: Vehicle[] = [
    {
      id: 'veh-001',
      name: 'BMW 3er',
      manufacturer: 'BMW',
      model: '320i',
      year: 2024,
      categoryCode: 'MITTEL',
      categoryName: 'Mittelklasse',
      seats: 5,
      fuelType: 'Petrol',
      transmissionType: 'Automatic',
      locationCode: 'MUC',
      city: 'München',
      licensePlate: 'M-AB 1234',
      dailyRateNet: 84.03,
      dailyRateVat: 15.97,
      dailyRateGross: 100.0,
      currency: 'EUR',
      status: VehicleStatus.Available,
      imageUrl: 'https://example.com/bmw.jpg',
    },
    {
      id: 'veh-002',
      name: 'Audi A4',
      manufacturer: 'Audi',
      model: 'A4 Avant',
      year: 2023,
      categoryCode: 'KOMBI',
      categoryName: 'Kombi',
      seats: 5,
      fuelType: 'Diesel',
      transmissionType: 'Automatic',
      locationCode: 'BER',
      city: 'Berlin',
      licensePlate: 'B-CD 5678',
      dailyRateNet: 100.84,
      dailyRateVat: 19.16,
      dailyRateGross: 120.0,
      currency: 'EUR',
      status: VehicleStatus.Rented,
      imageUrl: 'https://example.com/audi.jpg',
    },
    {
      id: 'veh-003',
      name: 'VW Golf',
      manufacturer: 'Volkswagen',
      model: 'Golf 8',
      year: 2024,
      categoryCode: 'KOMPAKT',
      categoryName: 'Kompaktklasse',
      seats: 5,
      fuelType: 'Hybrid',
      transmissionType: 'Manual',
      locationCode: 'MUC',
      city: 'München',
      licensePlate: 'M-EF 9012',
      dailyRateNet: 58.82,
      dailyRateVat: 11.18,
      dailyRateGross: 70.0,
      currency: 'EUR',
      status: VehicleStatus.Maintenance,
      imageUrl: 'https://example.com/vw.jpg',
    },
  ];

  const mockLocations: Location[] = [
    {
      code: 'MUC',
      name: 'München Flughafen',
      city: 'München',
      street: 'Flughafenstraße 1',
      postalCode: '85356',
      fullAddress: 'Flughafenstraße 1, 85356 München',
    },
    {
      code: 'BER',
      name: 'Berlin Brandenburg',
      city: 'Berlin',
      street: 'Willy Brandt Platz 1',
      postalCode: '12529',
      fullAddress: 'Willy Brandt Platz 1, 12529 Berlin',
    },
  ];

  beforeEach(async () => {
    const vehicleServiceSpy = jasmine.createSpyObj('VehicleService', [
      'searchVehicles',
      'addVehicle',
      'updateVehicleStatus',
      'updateVehicleLocation',
      'updateVehicleDailyRate',
    ]);
    const locationServiceSpy = jasmine.createSpyObj('LocationService', ['getAllLocations']);

    await TestBed.configureTestingModule({
      imports: [VehiclesComponent],
      providers: [
        { provide: VehicleService, useValue: vehicleServiceSpy },
        { provide: LocationService, useValue: locationServiceSpy },
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    mockVehicleService = TestBed.inject(VehicleService) as jasmine.SpyObj<VehicleService>;
    mockLocationService = TestBed.inject(LocationService) as jasmine.SpyObj<LocationService>;

    // Default mock responses
    mockVehicleService.searchVehicles.and.returnValue(
      of({
        vehicles: mockVehicles,
        totalCount: 3,
        pageNumber: 1,
        pageSize: 25,
        totalPages: 1,
      }),
    );
    mockLocationService.getAllLocations.and.returnValue(of(mockLocations));

    fixture = TestBed.createComponent(VehiclesComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should load vehicles on init', () => {
      component.ngOnInit();

      expect(mockVehicleService.searchVehicles).toHaveBeenCalled();
      expect(component['vehicles']().length).toBe(3);
    });

    it('should load locations on init', () => {
      component.ngOnInit();

      expect(mockLocationService.getAllLocations).toHaveBeenCalled();
      expect(component['locations']().length).toBe(2);
    });

    it('should initialize forms on init', () => {
      component.ngOnInit();

      expect(component['addVehicleForm']).toBeDefined();
      expect(component['statusForm']).toBeDefined();
      expect(component['locationForm']).toBeDefined();
      expect(component['pricingForm']).toBeDefined();
    });

    it('should handle vehicle loading error', () => {
      mockVehicleService.searchVehicles.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      component.ngOnInit();

      expect(component['error']()).toBe('Fehler beim Laden der Fahrzeuge');
      expect(component['loading']()).toBe(false);
    });
  });

  describe('Statistics', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should calculate total vehicles', () => {
      expect(component['totalVehicles']()).toBe(3);
    });

    it('should calculate available vehicles', () => {
      expect(component['availableVehicles']()).toBe(1);
    });

    it('should calculate rented vehicles', () => {
      expect(component['rentedVehicles']()).toBe(1);
    });

    it('should calculate maintenance vehicles', () => {
      expect(component['maintenanceVehicles']()).toBe(1);
    });
  });

  describe('Filtering', () => {
    beforeEach(() => {
      component.ngOnInit();
      mockVehicleService.searchVehicles.calls.reset();
    });

    it('should filter by status', () => {
      component['searchStatus'].set(VehicleStatus.Available);
      component['applyFilters']();

      expect(mockVehicleService.searchVehicles).toHaveBeenCalledWith(
        jasmine.objectContaining({
          status: VehicleStatus.Available,
        }),
      );
    });

    it('should filter by location', () => {
      component['searchLocation'].set('MUC');
      component['applyFilters']();

      expect(mockVehicleService.searchVehicles).toHaveBeenCalledWith(
        jasmine.objectContaining({
          locationCode: 'MUC',
        }),
      );
    });

    it('should filter by category', () => {
      component['searchCategory'].set('MITTEL');
      component['applyFilters']();

      expect(mockVehicleService.searchVehicles).toHaveBeenCalledWith(
        jasmine.objectContaining({
          categoryCode: 'MITTEL',
        }),
      );
    });

    it('should clear all filters', () => {
      component['searchStatus'].set(VehicleStatus.Available);
      component['searchLocation'].set('MUC');
      component['searchCategory'].set('MITTEL');

      component['clearFilters']();

      expect(component['searchStatus']()).toBe('');
      expect(component['searchLocation']()).toBe('');
      expect(component['searchCategory']()).toBe('');
      expect(mockVehicleService.searchVehicles).toHaveBeenCalled();
    });
  });

  describe('Unique Values', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should get unique locations from vehicles', () => {
      const locations = component['uniqueLocations']();
      expect(locations).toContain('MUC');
      expect(locations).toContain('BER');
      expect(locations.length).toBe(2);
    });

    it('should get unique categories from vehicles', () => {
      const categories = component['uniqueCategories']();
      expect(categories).toContain('MITTEL');
      expect(categories).toContain('KOMBI');
      expect(categories).toContain('KOMPAKT');
    });
  });

  describe('View Details', () => {
    it('should open details modal', () => {
      const vehicle = mockVehicles[0];
      component['viewDetails'](vehicle);

      expect(component['selectedVehicle']()).toEqual(vehicle);
      expect(component['showDetails']()).toBe(true);
    });

    it('should close details modal', () => {
      component['selectedVehicle'].set(mockVehicles[0]);
      component['showDetails'].set(true);

      component['closeDetails']();

      expect(component['showDetails']()).toBe(false);
      expect(component['selectedVehicle']()).toBeNull();
    });
  });

  describe('Add Vehicle', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should open add vehicle modal', () => {
      component['showAddVehicleModal']();

      expect(component['showAddModal']()).toBe(true);
      expect(component['error']()).toBeNull();
    });

    it('should reset form when opening add modal', () => {
      component['addVehicleForm'].patchValue({ name: 'Test' });
      component['showAddVehicleModal']();

      expect(component['addVehicleForm'].value.name).toBeNull();
      expect(component['addVehicleForm'].value.seats).toBe(5);
      expect(component['addVehicleForm'].value.fuelType).toBe('Petrol');
    });

    it('should close add vehicle modal', () => {
      component['showAddModal'].set(true);
      component['closeAddVehicleModal']();

      expect(component['showAddModal']()).toBe(false);
    });

    it('should not add vehicle with invalid form', () => {
      component['showAddVehicleModal']();
      component['addNewVehicle']();

      expect(component['error']()).toBe('Bitte füllen Sie alle Pflichtfelder korrekt aus');
      expect(mockVehicleService.addVehicle).not.toHaveBeenCalled();
    });

    it('should add vehicle successfully', fakeAsync(() => {
      const addVehicleResult = {
        vehicleId: 'new-id',
        name: 'New Car',
        category: 'MITTEL',
        location: 'MUC',
        dailyRateGross: 95.2,
        status: VehicleStatus.Available,
      };
      mockVehicleService.addVehicle.and.returnValue(of(addVehicleResult));

      component['showAddVehicleModal']();
      component['addVehicleForm'].patchValue({
        name: 'New Car',
        category: 'MITTEL',
        seats: 5,
        fuelType: 'Petrol',
        transmissionType: 'Manual',
        locationCode: 'MUC',
        dailyRateNet: 80,
      });

      component['addNewVehicle']();

      expect(mockVehicleService.addVehicle).toHaveBeenCalled();
      expect(component['showAddModal']()).toBe(false);
      expect(component['successMessage']()).toContain('erfolgreich hinzugefügt');

      tick(UI_TIMING.SUCCESS_MESSAGE_DURATION);
      expect(component['successMessage']()).toBeNull();
    }));

    it('should handle add vehicle error', () => {
      mockVehicleService.addVehicle.and.returnValue(throwError(() => new Error('Add failed')));

      component['showAddVehicleModal']();
      component['addVehicleForm'].patchValue({
        name: 'New Car',
        category: 'MITTEL',
        seats: 5,
        fuelType: 'Petrol',
        transmissionType: 'Manual',
        locationCode: 'MUC',
        dailyRateNet: 80,
      });

      component['addNewVehicle']();

      expect(component['error']()).toBe('Fehler beim Hinzufügen des Fahrzeugs');
      expect(component['actionInProgress']()).toBe(false);
    });
  });

  describe('Update Status', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should open status update modal', () => {
      const vehicle = mockVehicles[0];
      component['showStatusUpdateModal'](vehicle);

      expect(component['selectedVehicle']()).toEqual(vehicle);
      expect(component['showStatusModal']()).toBe(true);
      expect(component['statusForm'].value.status).toBe(vehicle.status);
    });

    it('should close status modal', () => {
      component['showStatusModal'].set(true);
      component['selectedVehicle'].set(mockVehicles[0]);

      component['closeStatusModal']();

      expect(component['showStatusModal']()).toBe(false);
      expect(component['selectedVehicle']()).toBeNull();
    });

    it('should update status successfully', fakeAsync(() => {
      mockVehicleService.updateVehicleStatus.and.returnValue(of(undefined));

      component['showStatusUpdateModal'](mockVehicles[0]);
      component['statusForm'].patchValue({ status: VehicleStatus.Maintenance });

      component['updateStatus']();

      expect(mockVehicleService.updateVehicleStatus).toHaveBeenCalledWith(
        mockVehicles[0].id,
        VehicleStatus.Maintenance,
      );
      expect(component['showStatusModal']()).toBe(false);
      expect(component['successMessage']()).toContain('aktualisiert');

      tick(UI_TIMING.SUCCESS_MESSAGE_DURATION);
      expect(component['successMessage']()).toBeNull();
    }));

    it('should handle status update error', () => {
      mockVehicleService.updateVehicleStatus.and.returnValue(
        throwError(() => new Error('Update failed')),
      );

      component['showStatusUpdateModal'](mockVehicles[0]);
      component['updateStatus']();

      expect(component['error']()).toBe('Fehler beim Aktualisieren des Status');
      expect(component['actionInProgress']()).toBe(false);
    });
  });

  describe('Update Location', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should open location update modal', () => {
      const vehicle = mockVehicles[0];
      component['showLocationUpdateModal'](vehicle);

      expect(component['selectedVehicle']()).toEqual(vehicle);
      expect(component['showLocationModal']()).toBe(true);
      expect(component['locationForm'].value.locationCode).toBe(vehicle.locationCode);
    });

    it('should close location modal', () => {
      component['showLocationModal'].set(true);
      component['selectedVehicle'].set(mockVehicles[0]);

      component['closeLocationModal']();

      expect(component['showLocationModal']()).toBe(false);
      expect(component['selectedVehicle']()).toBeNull();
    });

    it('should update location successfully', fakeAsync(() => {
      mockVehicleService.updateVehicleLocation.and.returnValue(of(undefined));

      component['showLocationUpdateModal'](mockVehicles[0]);
      component['locationForm'].patchValue({ locationCode: 'BER' });

      component['updateLocation']();

      expect(mockVehicleService.updateVehicleLocation).toHaveBeenCalledWith(
        mockVehicles[0].id,
        'BER',
      );
      expect(component['showLocationModal']()).toBe(false);

      tick(UI_TIMING.SUCCESS_MESSAGE_DURATION);
    }));

    it('should handle location update error', () => {
      mockVehicleService.updateVehicleLocation.and.returnValue(
        throwError(() => new Error('Update failed')),
      );

      component['showLocationUpdateModal'](mockVehicles[0]);
      component['updateLocation']();

      expect(component['error']()).toBe('Fehler beim Aktualisieren des Standorts');
    });
  });

  describe('Update Pricing', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should open pricing update modal', () => {
      const vehicle = mockVehicles[0];
      component['showPricingUpdateModal'](vehicle);

      expect(component['selectedVehicle']()).toEqual(vehicle);
      expect(component['showPricingModal']()).toBe(true);
      expect(component['pricingForm'].value.dailyRateNet).toBe(vehicle.dailyRateNet);
    });

    it('should close pricing modal', () => {
      component['showPricingModal'].set(true);
      component['selectedVehicle'].set(mockVehicles[0]);

      component['closePricingModal']();

      expect(component['showPricingModal']()).toBe(false);
      expect(component['selectedVehicle']()).toBeNull();
    });

    it('should update pricing successfully', fakeAsync(() => {
      mockVehicleService.updateVehicleDailyRate.and.returnValue(of(undefined));

      component['showPricingUpdateModal'](mockVehicles[0]);
      component['pricingForm'].patchValue({ dailyRateNet: 100 });

      component['updatePricing']();

      expect(mockVehicleService.updateVehicleDailyRate).toHaveBeenCalledWith(
        mockVehicles[0].id,
        100,
      );
      expect(component['showPricingModal']()).toBe(false);

      tick(UI_TIMING.SUCCESS_MESSAGE_DURATION);
    }));

    it('should handle pricing update error', () => {
      mockVehicleService.updateVehicleDailyRate.and.returnValue(
        throwError(() => new Error('Update failed')),
      );

      component['showPricingUpdateModal'](mockVehicles[0]);
      component['updatePricing']();

      expect(component['error']()).toBe('Fehler beim Aktualisieren des Preises');
    });
  });

  describe('Helper Methods', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should calculate gross rate with VAT', () => {
      const netRate = 100;
      const grossRate = component['calculateGrossRate'](netRate);

      expect(grossRate).toBe(netRate * GERMAN_VAT_MULTIPLIER);
    });

    it('should return correct status class for Available', () => {
      expect(component['getStatusClass'](VehicleStatus.Available)).toBe('status-success');
    });

    it('should return correct status class for Rented', () => {
      expect(component['getStatusClass'](VehicleStatus.Rented)).toBe('status-info');
    });

    it('should return correct status class for Maintenance', () => {
      expect(component['getStatusClass'](VehicleStatus.Maintenance)).toBe('status-warning');
    });

    it('should return correct status text for Available', () => {
      expect(component['getStatusText'](VehicleStatus.Available)).toBe('Verfügbar');
    });

    it('should return correct status text for Rented', () => {
      expect(component['getStatusText'](VehicleStatus.Rented)).toBe('Vermietet');
    });
  });

  describe('Form Validation', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should require name with minimum length', () => {
      const nameControl = component['addVehicleForm'].get('name');
      nameControl?.setValue('A');

      expect(nameControl?.invalid).toBe(true);
      expect(nameControl?.errors?.['minlength']).toBeTruthy();
    });

    it('should require category', () => {
      const categoryControl = component['addVehicleForm'].get('category');
      categoryControl?.setValue('');

      expect(categoryControl?.invalid).toBe(true);
      expect(categoryControl?.errors?.['required']).toBeTruthy();
    });

    it('should validate seats range', () => {
      const seatsControl = component['addVehicleForm'].get('seats');

      seatsControl?.setValue(0);
      expect(seatsControl?.errors?.['min']).toBeTruthy();

      seatsControl?.setValue(20);
      expect(seatsControl?.errors?.['max']).toBeTruthy();

      seatsControl?.setValue(5);
      expect(seatsControl?.valid).toBe(true);
    });

    it('should require positive daily rate', () => {
      const rateControl = component['addVehicleForm'].get('dailyRateNet');
      rateControl?.setValue(0);

      expect(rateControl?.invalid).toBe(true);
      expect(rateControl?.errors?.['min']).toBeTruthy();
    });
  });
});
