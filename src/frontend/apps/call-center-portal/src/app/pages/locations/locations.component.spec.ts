import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LocationsComponent } from './locations.component';
import { LocationService } from '@orange-car-rental/location-api';
import { VehicleService } from '../../services/vehicle.service';
import { of, throwError } from 'rxjs';
import type { Location } from '@orange-car-rental/location-api';
import type { Vehicle } from '@orange-car-rental/vehicle-api';
import { VehicleStatus } from '@orange-car-rental/vehicle-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import { UTILIZATION_THRESHOLDS } from '../../constants/app.constants';

describe('LocationsComponent', () => {
  let component: LocationsComponent;
  let fixture: ComponentFixture<LocationsComponent>;
  let mockLocationService: jasmine.SpyObj<LocationService>;
  let mockVehicleService: jasmine.SpyObj<VehicleService>;

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
    {
      code: 'FRA',
      name: 'Frankfurt Flughafen',
      city: 'Frankfurt',
      street: 'Flughafenring 1',
      postalCode: '60549',
      fullAddress: 'Flughafenring 1, 60549 Frankfurt',
    },
  ];

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
      imageUrl: null,
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
      locationCode: 'MUC',
      city: 'München',
      licensePlate: 'M-CD 5678',
      dailyRateNet: 100.84,
      dailyRateVat: 19.16,
      dailyRateGross: 120.0,
      currency: 'EUR',
      status: VehicleStatus.Rented,
      imageUrl: null,
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
      locationCode: 'BER',
      city: 'Berlin',
      licensePlate: 'B-EF 9012',
      dailyRateNet: 58.82,
      dailyRateVat: 11.18,
      dailyRateGross: 70.0,
      currency: 'EUR',
      status: VehicleStatus.Maintenance,
      imageUrl: null,
    },
    {
      id: 'veh-004',
      name: 'Mercedes C-Klasse',
      manufacturer: 'Mercedes-Benz',
      model: 'C 200',
      year: 2024,
      categoryCode: 'MITTEL',
      categoryName: 'Mittelklasse',
      seats: 5,
      fuelType: 'Petrol',
      transmissionType: 'Automatic',
      locationCode: 'BER',
      city: 'Berlin',
      licensePlate: 'B-GH 3456',
      dailyRateNet: 92.44,
      dailyRateVat: 17.56,
      dailyRateGross: 110.0,
      currency: 'EUR',
      status: VehicleStatus.Available,
      imageUrl: null,
    },
  ];

  beforeEach(async () => {
    const locationServiceSpy = jasmine.createSpyObj('LocationService', ['getAllLocations']);
    const vehicleServiceSpy = jasmine.createSpyObj('VehicleService', ['searchVehicles']);

    await TestBed.configureTestingModule({
      imports: [LocationsComponent],
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
        vehicles: mockVehicles,
        totalCount: 4,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
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

      expect(component['error']()).toBe('Fehler beim Laden der Standorte');
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
          vehicles: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 100,
          totalPages: 0,
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
      expect(mostUtilized?.locationCode).toBe('MUC'); // 50% utilization
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
      expect(component['getUtilizationClass'](UTILIZATION_THRESHOLDS.HIGH)).toBe('utilization-high');
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
          vehicles: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 100,
          totalPages: 0,
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
