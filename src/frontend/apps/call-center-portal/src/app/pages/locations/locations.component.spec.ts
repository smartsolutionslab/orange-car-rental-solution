import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { LocationsComponent } from './locations.component';
import { LocationService } from '@orange-car-rental/location-api';
import { VehicleService } from '../../services/vehicle.service';
import { of, throwError } from 'rxjs';
import type {
  Location,
  LocationCode,
  LocationName,
  LocationStatus,
  CityName,
  StreetAddress,
} from '@orange-car-rental/location-api';
import type { PhoneNumber, EmailAddress } from '@orange-car-rental/shared';
import type {
  Vehicle,
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
} from '@orange-car-rental/vehicle-api';
import { VehicleStatus } from '@orange-car-rental/vehicle-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import type { Currency, PostalCode } from '@orange-car-rental/shared';
import { UTILIZATION_THRESHOLDS } from '../../constants/app.constants';
import { TEST_VEHICLE_IDS, SHORT_LOCATION_CODES } from '@orange-car-rental/shared/testing';

describe('LocationsComponent', () => {
  let component: LocationsComponent;
  let fixture: ComponentFixture<LocationsComponent>;
  let mockLocationService: jasmine.SpyObj<LocationService>;
  let mockVehicleService: jasmine.SpyObj<VehicleService>;

  // Use shared test fixtures
  const mockLocations: Location[] = [
    {
      code: SHORT_LOCATION_CODES.MUNICH,
      name: 'München Flughafen' as LocationName,
      city: 'München' as CityName,
      street: 'Flughafenstraße 1' as StreetAddress,
      postalCode: '85356' as PostalCode,
      fullAddress: 'Flughafenstraße 1, 85356 München',
      openingHours: 'Mo-So: 06:00-23:00',
      phone: '+49 89 123456' as PhoneNumber,
      email: 'muc@orange-car-rental.de' as EmailAddress,
      status: 'Active' as LocationStatus,
    },
    {
      code: SHORT_LOCATION_CODES.BERLIN,
      name: 'Berlin Brandenburg' as LocationName,
      city: 'Berlin' as CityName,
      street: 'Willy Brandt Platz 1' as StreetAddress,
      postalCode: '12529' as PostalCode,
      fullAddress: 'Willy Brandt Platz 1, 12529 Berlin',
      openingHours: 'Mo-So: 06:00-22:00',
      phone: '+49 30 123456' as PhoneNumber,
      email: 'ber@orange-car-rental.de' as EmailAddress,
      status: 'Active' as LocationStatus,
    },
    {
      code: SHORT_LOCATION_CODES.FRANKFURT,
      name: 'Frankfurt Flughafen' as LocationName,
      city: 'Frankfurt' as CityName,
      street: 'Flughafenring 1' as StreetAddress,
      postalCode: '60549' as PostalCode,
      fullAddress: 'Flughafenring 1, 60549 Frankfurt',
      openingHours: 'Mo-So: 05:00-00:00',
      phone: '+49 69 123456' as PhoneNumber,
      email: 'fra@orange-car-rental.de' as EmailAddress,
      status: 'Active' as LocationStatus,
    },
  ];

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
      imageUrl: null,
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
      locationCode: SHORT_LOCATION_CODES.MUNICH,
      city: 'München' as CityName,
      licensePlate: 'M-CD 5678' as LicensePlate,
      dailyRateNet: 100.84 as DailyRate,
      dailyRateVat: 19.16 as DailyRate,
      dailyRateGross: 120.0 as DailyRate,
      currency: 'EUR' as Currency,
      status: VehicleStatus.Rented,
      imageUrl: null,
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
      locationCode: SHORT_LOCATION_CODES.BERLIN,
      city: 'Berlin' as CityName,
      licensePlate: 'B-EF 9012' as LicensePlate,
      dailyRateNet: 58.82 as DailyRate,
      dailyRateVat: 11.18 as DailyRate,
      dailyRateGross: 70.0 as DailyRate,
      currency: 'EUR' as Currency,
      status: VehicleStatus.Maintenance,
      imageUrl: null,
    },
    {
      id: TEST_VEHICLE_IDS.OPEL_ASTRA,
      name: 'Mercedes C-Klasse' as VehicleName,
      manufacturer: 'Mercedes-Benz' as Manufacturer,
      model: 'C 200' as VehicleModel,
      year: 2024 as ManufacturingYear,
      categoryCode: 'MITTEL' as CategoryCode,
      categoryName: 'Mittelklasse' as CategoryName,
      seats: 5 as SeatingCapacity,
      fuelType: 'Petrol' as FuelType,
      transmissionType: 'Automatic' as TransmissionType,
      locationCode: SHORT_LOCATION_CODES.BERLIN,
      city: 'Berlin' as CityName,
      licensePlate: 'B-GH 3456' as LicensePlate,
      dailyRateNet: 92.44 as DailyRate,
      dailyRateVat: 17.56 as DailyRate,
      dailyRateGross: 110.0 as DailyRate,
      currency: 'EUR' as Currency,
      status: VehicleStatus.Available,
      imageUrl: null,
    },
  ];

  beforeEach(async () => {
    const locationServiceSpy = jasmine.createSpyObj('LocationService', ['getAllLocations']);
    const vehicleServiceSpy = jasmine.createSpyObj('VehicleService', ['searchVehicles']);

    await TestBed.configureTestingModule({
      imports: [LocationsComponent, TranslateModule.forRoot()],
      providers: [
        { provide: LocationService, useValue: locationServiceSpy },
        { provide: VehicleService, useValue: vehicleServiceSpy },
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    mockLocationService = TestBed.inject(LocationService) as jasmine.SpyObj<LocationService>;
    mockVehicleService = TestBed.inject(VehicleService) as jasmine.SpyObj<VehicleService>;

    // Default mock responses
    mockLocationService.getAllLocations.and.returnValue(of(mockLocations));
    mockVehicleService.searchVehicles.and.returnValue(
      of({
        items: mockVehicles,
        totalCount: 4,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      }),
    );

    fixture = TestBed.createComponent(LocationsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should load locations on init', () => {
      component.ngOnInit();

      expect(mockLocationService.getAllLocations).toHaveBeenCalled();
      expect(component['locations']().length).toBe(3);
    });

    it('should load vehicles after locations', () => {
      component.ngOnInit();

      expect(mockVehicleService.searchVehicles).toHaveBeenCalled();
      expect(component['allVehicles']().length).toBe(4);
    });

    it('should handle location loading error', () => {
      mockLocationService.getAllLocations.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      component.ngOnInit();

      expect(component['error']()).toBe('locations.error');
      expect(component['loading']()).toBe(false);
    });

    it('should handle vehicle loading error gracefully', () => {
      mockVehicleService.searchVehicles.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      component.ngOnInit();

      // Should not set error - vehicle loading failure is not critical
      expect(component['error']()).toBeNull();
    });
  });

  describe('Statistics', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should calculate total locations', () => {
      expect(component['totalLocations']).toBe(3);
    });

    it('should calculate active locations', () => {
      expect(component['activeLocations']).toBe(2); // MUC and BER have vehicles
    });

    it('should calculate total vehicles', () => {
      expect(component['totalVehicles']).toBe(4);
    });

    it('should calculate total available vehicles', () => {
      expect(component['totalAvailableVehicles']).toBe(2); // veh-001 and veh-004
    });

    it('should calculate total rented vehicles', () => {
      expect(component['totalRentedVehicles']).toBe(1); // veh-002
    });

    it('should calculate overall utilization rate', () => {
      // 1 rented out of 4 total = 25%
      expect(component['overallUtilizationRate']).toBe(25);
    });

    it('should handle zero vehicles for utilization', () => {
      mockVehicleService.searchVehicles.and.returnValue(
        of({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 100,
          totalPages: 0,
          hasPreviousPage: false,
          hasNextPage: false,
        }),
      );

      component.ngOnInit();

      expect(component['overallUtilizationRate']).toBe(0);
    });
  });

  describe('Location Statistics', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should calculate stats per location', () => {
      const mucStats = component['getLocationStats']('MUC');

      expect(mucStats).toBeDefined();
      expect(mucStats?.totalVehicles).toBe(2);
      expect(mucStats?.availableVehicles).toBe(1);
      expect(mucStats?.rentedVehicles).toBe(1);
    });

    it('should calculate utilization rate per location', () => {
      const mucStats = component['getLocationStats']('MUC');

      // 1 rented out of 2 total = 50%
      expect(mucStats?.utilizationRate).toBe(50);
    });

    it('should return undefined for unknown location', () => {
      const stats = component['getLocationStats']('UNKNOWN');
      expect(stats).toBeUndefined();
    });

    it('should get vehicle distribution for location', () => {
      const distribution = component['getVehicleDistribution']('MUC');

      expect(distribution.available).toBe(1);
      expect(distribution.rented).toBe(1);
      expect(distribution.maintenance).toBe(0);
    });

    it('should return zero distribution for unknown location', () => {
      const distribution = component['getVehicleDistribution']('UNKNOWN');

      expect(distribution.available).toBe(0);
      expect(distribution.rented).toBe(0);
      expect(distribution.maintenance).toBe(0);
    });
  });

  describe('Most Utilized Location', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should find most utilized location', () => {
      const mostUtilized = component['mostUtilizedLocation'];

      expect(mostUtilized).toBeDefined();
      expect(mostUtilized?.locationCode).toBe('MUC' as LocationCode); // 50% utilization
    });

    it('should return null when no locations', () => {
      mockLocationService.getAllLocations.and.returnValue(of([]));
      component.ngOnInit();

      expect(component['mostUtilizedLocation']).toBeNull();
    });
  });

  describe('Filtering', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should filter locations by city', () => {
      component['searchCity'].set('München');

      const filtered = component['filteredLocations']();
      expect(filtered.length).toBe(1);
      expect(filtered[0].code).toBe('MUC');
    });

    it('should filter locations by name', () => {
      component['searchCity'].set('Flughafen');

      const filtered = component['filteredLocations']();
      expect(filtered.length).toBe(2); // MUC and FRA
    });

    it('should filter locations by code', () => {
      component['searchCity'].set('BER');

      const filtered = component['filteredLocations']();
      expect(filtered.length).toBe(1);
      expect(filtered[0].code).toBe('BER');
    });

    it('should be case insensitive', () => {
      component['searchCity'].set('münchen');

      const filtered = component['filteredLocations']();
      expect(filtered.length).toBe(1);
    });

    it('should return all locations when search is empty', () => {
      component['searchCity'].set('');

      const filtered = component['filteredLocations']();
      expect(filtered.length).toBe(3);
    });

    it('should clear search', () => {
      component['searchCity'].set('München');
      component['clearSearch']();

      expect(component['searchCity']()).toBe('');
    });
  });

  describe('View Details', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should open details modal', () => {
      const location = mockLocations[0];
      component['viewDetails'](location);

      expect(component['selectedLocation']()).toEqual(location);
      expect(component['showDetails']()).toBe(true);
    });

    it('should load location vehicles when viewing details', () => {
      const location = mockLocations[0];
      component['viewDetails'](location);

      expect(mockVehicleService.searchVehicles).toHaveBeenCalledWith(
        jasmine.objectContaining({
          locationCode: 'MUC',
        }),
      );
    });

    it('should close details modal and clear state', () => {
      component['selectedLocation'].set(mockLocations[0]);
      component['showDetails'].set(true);
      component['locationVehicles'].set(mockVehicles);

      component['closeDetails']();

      expect(component['showDetails']()).toBe(false);
      expect(component['selectedLocation']()).toBeNull();
      expect(component['locationVehicles']().length).toBe(0);
    });

    it('should handle vehicle loading error for location', () => {
      mockVehicleService.searchVehicles.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      component['viewDetails'](mockLocations[0]);

      expect(component['loadingVehicles']()).toBe(false);
    });
  });

  describe('Utilization Class', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should return high class for high utilization', () => {
      expect(component['getUtilizationClass'](UTILIZATION_THRESHOLDS.HIGH)).toBe(
        'utilization-high',
      );
      expect(component['getUtilizationClass'](90)).toBe('utilization-high');
    });

    it('should return medium class for medium utilization', () => {
      expect(component['getUtilizationClass'](UTILIZATION_THRESHOLDS.MEDIUM)).toBe(
        'utilization-medium',
      );
      expect(component['getUtilizationClass'](60)).toBe('utilization-medium');
    });

    it('should return low class for low utilization', () => {
      expect(component['getUtilizationClass'](UTILIZATION_THRESHOLDS.MEDIUM - 1)).toBe(
        'utilization-low',
      );
      expect(component['getUtilizationClass'](20)).toBe('utilization-low');
    });
  });

  describe('Helper Methods', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should format percentage', () => {
      expect(component['formatPercentage'](75)).toBe('75%');
      expect(component['formatPercentage'](0)).toBe('0%');
      expect(component['formatPercentage'](100)).toBe('100%');
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

    it('should return correct status text', () => {
      expect(component['getStatusText'](VehicleStatus.Available)).toBe('Verfügbar');
      expect(component['getStatusText'](VehicleStatus.Rented)).toBe('Vermietet');
      expect(component['getStatusText'](VehicleStatus.Maintenance)).toBe('Wartung');
    });
  });

  describe('Edge Cases', () => {
    it('should handle empty locations array', () => {
      mockLocationService.getAllLocations.and.returnValue(of([]));
      component.ngOnInit();

      expect(component['totalLocations']).toBe(0);
      expect(component['activeLocations']).toBe(0);
      expect(component['filteredLocations']().length).toBe(0);
    });

    it('should handle empty vehicles array', () => {
      mockVehicleService.searchVehicles.and.returnValue(
        of({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 100,
          totalPages: 0,
          hasPreviousPage: false,
          hasNextPage: false,
        }),
      );

      component.ngOnInit();

      expect(component['totalVehicles']).toBe(0);
      expect(component['totalAvailableVehicles']).toBe(0);
      expect(component['overallUtilizationRate']).toBe(0);
    });

    it('should handle location with no vehicles', () => {
      component.ngOnInit();

      const fraStats = component['getLocationStats']('FRA');
      expect(fraStats?.totalVehicles).toBe(0);
      expect(fraStats?.utilizationRate).toBe(0);
    });
  });
});
