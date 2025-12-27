import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TranslateModule } from '@ngx-translate/core';
import { registerLocaleData } from '@angular/common';
import localeDe from '@angular/common/locales/de';
import { of, throwError } from 'rxjs';
import { VehicleListComponent } from './vehicle-list.component';
import { VehicleService } from '../../services/vehicle.service';
import type {
  Vehicle,
  VehicleId,
  VehicleSearchResult,
  VehicleSearchQuery,
  CategoryCode,
  SeatingCapacity,
  FuelType,
  TransmissionType,
} from '@orange-car-rental/vehicle-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import {
  MOCK_VEHICLES,
  TEST_VEHICLE_IDS,
  TEST_LOCATION_CODES,
  getFutureDate,
} from '@orange-car-rental/shared/testing';

// Register German locale for DecimalPipe
registerLocaleData(localeDe);

describe('VehicleListComponent', () => {
  let component: VehicleListComponent;
  let fixture: ComponentFixture<VehicleListComponent>;
  let vehicleService: jasmine.SpyObj<VehicleService>;
  let router: jasmine.SpyObj<Router>;

  // Use shared mock vehicle
  const mockVehicle: Vehicle = MOCK_VEHICLES.VW_GOLF;

  const mockSearchResult: VehicleSearchResult = {
    vehicles: [mockVehicle],
    totalCount: 1,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
  };

  beforeEach(async () => {
    const vehicleServiceSpy = jasmine.createSpyObj('VehicleService', ['searchVehicles']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [VehicleListComponent, TranslateModule.forRoot()],
      providers: [
        { provide: VehicleService, useValue: vehicleServiceSpy },
        { provide: Router, useValue: routerSpy },
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    vehicleService = TestBed.inject(VehicleService) as jasmine.SpyObj<VehicleService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    vehicleService.searchVehicles.and.returnValue(of(mockSearchResult));

    fixture = TestBed.createComponent(VehicleListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load vehicles on init', () => {
    fixture.detectChanges();

    expect(vehicleService.searchVehicles).toHaveBeenCalledWith({});
    expect(component['vehicles']().length).toBe(1);
    expect(component['totalCount']()).toBe(1);
  });

  it('should set loading state while searching', () => {
    fixture.detectChanges();

    component['onSearch']({ locationCode: TEST_LOCATION_CODES.BERLIN_HBF });

    expect(component['loading']()).toBeFalse(); // After subscribe completes
  });

  describe('Search Functionality', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should search vehicles with empty query', () => {
      const query: VehicleSearchQuery = {};

      component['onSearch'](query);

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith(query);
      expect(component['vehicles']().length).toBe(1);
    });

    it('should search vehicles with location filter', () => {
      const query: VehicleSearchQuery = {
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      };

      component['onSearch'](query);

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith(query);
    });

    it('should search vehicles with date range filter', () => {
      const query: VehicleSearchQuery = {
        pickupDate: getFutureDate(7),
        returnDate: getFutureDate(12),
      };

      component['onSearch'](query);

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith(query);
    });

    it('should search vehicles with multiple filters', () => {
      const query: VehicleSearchQuery = {
        pickupDate: getFutureDate(7),
        returnDate: getFutureDate(12),
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
        categoryCode: 'MITTEL' as CategoryCode,
        minSeats: 5 as SeatingCapacity,
        fuelType: 'Petrol' as FuelType,
        transmissionType: 'Manual' as TransmissionType,
      };

      component['onSearch'](query);

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith(query);
      expect(component['currentSearchQuery']()).toEqual(query);
    });

    it('should update current search query on search', () => {
      const query: VehicleSearchQuery = {
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
        pickupDate: getFutureDate(7),
        returnDate: getFutureDate(12),
      };

      component['onSearch'](query);

      expect(component['currentSearchQuery']()).toEqual(query);
    });

    it('should handle empty search results', () => {
      const emptyResult: VehicleSearchResult = {
        vehicles: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0,
      };

      vehicleService.searchVehicles.and.returnValue(of(emptyResult));

      component['onSearch']({});

      expect(component['vehicles']().length).toBe(0);
      expect(component['totalCount']()).toBe(0);
    });

    it('should handle search error', () => {
      const error = new Error('Network error');
      vehicleService.searchVehicles.and.returnValue(throwError(() => error));

      component['onSearch']({});

      expect(component['loading']()).toBeFalse();
      expect(component['error']()).toBeTruthy();
      expect(component['error']()).toBe('errors.generic');
    });

    it('should clear error on new search', () => {
      component['error'].set('Previous error');

      component['onSearch']({});

      expect(component['error']()).toBeNull();
    });
  });

  describe('Vehicle Booking', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should navigate to booking page with vehicle details', () => {
      const pickupDate = getFutureDate(7);
      const returnDate = getFutureDate(12);
      const query: VehicleSearchQuery = {
        pickupDate,
        returnDate,
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      };

      component['currentSearchQuery'].set(query);
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
          categoryCode: mockVehicle.categoryCode,
          pickupDate,
          returnDate,
          locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
        },
      });
    });

    it('should navigate to booking with vehicle location when search has no location', () => {
      const pickupDate = getFutureDate(7);
      const returnDate = getFutureDate(12);
      const query: VehicleSearchQuery = {
        pickupDate,
        returnDate,
      };

      component['currentSearchQuery'].set(query);
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
          categoryCode: mockVehicle.categoryCode,
          pickupDate,
          returnDate,
          locationCode: mockVehicle.locationCode,
        },
      });
    });

    it('should navigate to booking with empty dates when search has no dates', () => {
      const query: VehicleSearchQuery = {
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      };

      component['currentSearchQuery'].set(query);
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
          categoryCode: mockVehicle.categoryCode,
          pickupDate: '',
          returnDate: '',
          locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
        },
      });
    });

    it('should handle booking with no search query', () => {
      component['currentSearchQuery'].set({});
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
          categoryCode: mockVehicle.categoryCode,
          pickupDate: '',
          returnDate: '',
          locationCode: mockVehicle.locationCode,
        },
      });
    });
  });

  describe('State Management', () => {
    it('should create component through TestBed with proper dependencies', () => {
      expect(component).toBeTruthy();
    });

    it('should update vehicles array on successful search', () => {
      const multiVehicleResult: VehicleSearchResult = {
        vehicles: [mockVehicle, { ...mockVehicle, id: 'different-id' as VehicleId }],
        totalCount: 2,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
      };

      vehicleService.searchVehicles.and.returnValue(of(multiVehicleResult));

      component['onSearch']({});

      expect(component['vehicles']().length).toBe(2);
      expect(component['totalCount']()).toBe(2);
    });
  });

  describe('Integration', () => {
    it('should perform initial search on component initialization', () => {
      const freshFixture = TestBed.createComponent(VehicleListComponent);
      freshFixture.detectChanges();

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith({});
    });

    it('should handle rapid sequential searches', () => {
      fixture.detectChanges();

      component['onSearch']({ locationCode: TEST_LOCATION_CODES.BERLIN_HBF });
      component['onSearch']({ locationCode: TEST_LOCATION_CODES.MUNICH_AIRPORT });
      component['onSearch']({ locationCode: TEST_LOCATION_CODES.HAMBURG_CITY });

      expect(vehicleService.searchVehicles).toHaveBeenCalledTimes(4); // 1 from init + 3 manual
    });
  });
});
