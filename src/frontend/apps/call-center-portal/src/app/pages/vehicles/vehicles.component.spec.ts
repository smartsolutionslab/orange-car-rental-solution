import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { VehiclesComponent } from './vehicles.component';
import { VehicleService } from '../../services/vehicle.service';
import { LocationService } from '@orange-car-rental/location-api';
import { of, throwError } from 'rxjs';
import type {
  Vehicle,
  VehicleId,
  VehicleName,
  CategoryCode,
  CategoryName,
  SeatingCapacity,
  DailyRate,
  FuelType,
  TransmissionType,
  LicensePlate,
  Manufacturer,
  VehicleModel,
  ManufacturingYear,
  ImageUrl,
} from '@orange-car-rental/vehicle-api';
import { VehicleStatus } from '@orange-car-rental/vehicle-api';
import type {
  Location,
  LocationCode,
  LocationName,
  CityName,
  StreetAddress,
} from '@orange-car-rental/location-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import type { Currency, PostalCode } from '@orange-car-rental/shared';
import { UI_TIMING, GERMAN_VAT_MULTIPLIER } from '../../constants/app.constants';
import { TEST_VEHICLE_IDS, SHORT_LOCATION_CODES } from '@orange-car-rental/shared/testing';

describe('VehiclesComponent', () => {
  let component: VehiclesComponent;
  let fixture: ComponentFixture<VehiclesComponent>;
  let mockVehicleService: jasmine.SpyObj<VehicleService>;
  let mockLocationService: jasmine.SpyObj<LocationService>;

  // Use shared test fixtures
  const mockVehicles: Vehicle[] = [
    {
      id: TEST_VEHICLE_IDS.BMW_3ER,
      name: 'BMW 3er' as VehicleName,
      manufacturer: 'BMW' as Manufacturer,
      model: '320i' as VehicleModel,
      year: 2024 as ManufacturingYear,
      categoryCode: 'MITTEL' as CategoryCode,
      categoryName: 'Mittelklasse' as CategoryName,
      seats: 5 as SeatingCapacity,
      fuelType: 'Petrol' as FuelType,
      transmissionType: 'Automatic' as TransmissionType,
      locationCode: SHORT_LOCATION_CODES.MUNICH,
      city: 'München' as CityName,
      licensePlate: 'M-AB 1234' as LicensePlate,
      dailyRateNet: 84.03 as DailyRate,
      dailyRateVat: 15.97 as DailyRate,
      dailyRateGross: 100.0 as DailyRate,
      currency: 'EUR' as Currency,
      status: VehicleStatus.Available,
      imageUrl: 'https://example.com/bmw.jpg' as ImageUrl,
    },
    {
      id: TEST_VEHICLE_IDS.AUDI_A4,
      name: 'Audi A4' as VehicleName,
      manufacturer: 'Audi' as Manufacturer,
      model: 'A4 Avant' as VehicleModel,
      year: 2023 as ManufacturingYear,
      categoryCode: 'KOMBI' as CategoryCode,
      categoryName: 'Kombi' as CategoryName,
      seats: 5 as SeatingCapacity,
      fuelType: 'Diesel' as FuelType,
      transmissionType: 'Automatic' as TransmissionType,
      locationCode: SHORT_LOCATION_CODES.BERLIN,
      city: 'Berlin' as CityName,
      licensePlate: 'B-CD 5678' as LicensePlate,
      dailyRateNet: 100.84 as DailyRate,
      dailyRateVat: 19.16 as DailyRate,
      dailyRateGross: 120.0 as DailyRate,
      currency: 'EUR' as Currency,
      status: VehicleStatus.Rented,
      imageUrl: 'https://example.com/audi.jpg' as ImageUrl,
    },
    {
      id: TEST_VEHICLE_IDS.VW_GOLF,
      name: 'VW Golf' as VehicleName,
      manufacturer: 'Volkswagen' as Manufacturer,
      model: 'Golf 8' as VehicleModel,
      year: 2024 as ManufacturingYear,
      categoryCode: 'KOMPAKT' as CategoryCode,
      categoryName: 'Kompaktklasse' as CategoryName,
      seats: 5 as SeatingCapacity,
      fuelType: 'Hybrid' as FuelType,
      transmissionType: 'Manual' as TransmissionType,
      locationCode: SHORT_LOCATION_CODES.MUNICH,
      city: 'München' as CityName,
      licensePlate: 'M-EF 9012' as LicensePlate,
      dailyRateNet: 58.82 as DailyRate,
      dailyRateVat: 11.18 as DailyRate,
      dailyRateGross: 70.0 as DailyRate,
      currency: 'EUR' as Currency,
      status: VehicleStatus.Maintenance,
      imageUrl: 'https://example.com/vw.jpg' as ImageUrl,
    },
  ];

  const mockLocations: Location[] = [
    {
      code: SHORT_LOCATION_CODES.MUNICH,
      name: 'München Flughafen' as LocationName,
      city: 'München' as CityName,
      street: 'Flughafenstraße 1' as StreetAddress,
      postalCode: '85356' as PostalCode,
      fullAddress: 'Flughafenstraße 1, 85356 München',
    },
    {
      code: SHORT_LOCATION_CODES.BERLIN,
      name: 'Berlin Brandenburg' as LocationName,
      city: 'Berlin' as CityName,
      street: 'Willy Brandt Platz 1' as StreetAddress,
      postalCode: '12529' as PostalCode,
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
    const translateServiceSpy = jasmine.createSpyObj('TranslateService', ['instant']);
    translateServiceSpy.instant.and.callFake((key: string) => key);

    await TestBed.configureTestingModule({
      imports: [VehiclesComponent, TranslateModule.forRoot()],
      providers: [
        { provide: VehicleService, useValue: vehicleServiceSpy },
        { provide: LocationService, useValue: locationServiceSpy },
        { provide: TranslateService, useValue: translateServiceSpy },
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

      expect(component['error']()).toBe('vehicles.errors.loading');
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
      expect(locations).toContain('MUC' as LocationCode);
      expect(locations).toContain('BER' as LocationCode);
      expect(locations.length).toBe(2);
    });

    it('should get unique categories from vehicles', () => {
      const categories = component['uniqueCategories']();
      expect(categories).toContain('MITTEL' as CategoryCode);
      expect(categories).toContain('KOMBI' as CategoryCode);
      expect(categories).toContain('KOMPAKT' as CategoryCode);
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

      expect(component['error']()).toBe('vehicles.errors.validation');
      expect(mockVehicleService.addVehicle).not.toHaveBeenCalled();
    });

    it('should add vehicle successfully', fakeAsync(() => {
      const addVehicleResult = {
        vehicleId: 'new-id' as VehicleId,
        name: 'New Car' as VehicleName,
        category: 'MITTEL' as CategoryCode,
        location: 'MUC' as LocationCode,
        dailyRateGross: 95.2 as DailyRate,
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
      expect(component['successMessage']()).toContain('vehicles.success.added');

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

      expect(component['error']()).toBe('vehicles.errors.addVehicle');
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
      expect(component['successMessage']()).toContain('vehicles.success.statusUpdated');

      tick(UI_TIMING.SUCCESS_MESSAGE_DURATION);
      expect(component['successMessage']()).toBeNull();
    }));

    it('should handle status update error', () => {
      mockVehicleService.updateVehicleStatus.and.returnValue(
        throwError(() => new Error('Update failed')),
      );

      component['showStatusUpdateModal'](mockVehicles[0]);
      component['updateStatus']();

      expect(component['error']()).toBe('vehicles.errors.updateStatus');
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
        'BER' as LocationCode,
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

      expect(component['error']()).toBe('vehicles.errors.updateLocation');
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
        100 as DailyRate,
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

      expect(component['error']()).toBe('vehicles.errors.updatePrice');
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
