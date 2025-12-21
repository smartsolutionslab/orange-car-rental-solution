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
  VehicleName,
  VehicleSearchResult,
  VehicleSearchQuery,
  CategoryCode,
  CategoryName,
  SeatingCapacity,
  DailyRate,
  FuelType,
  TransmissionType,
  VehicleStatus,
  LicensePlate,
  Manufacturer,
  VehicleModel,
  ManufacturingYear,
} from '@orange-car-rental/vehicle-api';
import type { LocationCode, CityName } from '@orange-car-rental/location-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import type { Currency, ISODateString } from '@orange-car-rental/shared';

// Register German locale for DecimalPipe
registerLocaleData(localeDe);

describe('VehicleListComponent', () => {
  let component: VehicleListComponent;
  let fixture: ComponentFixture<VehicleListComponent>;
  let vehicleService: jasmine.SpyObj<VehicleService>;
  let router: jasmine.SpyObj<Router>;

  const mockVehicle: Vehicle = {
    id: '123e4567-e89b-12d3-a456-426614174000' as VehicleId,
    name: 'VW Golf' as VehicleName,
    categoryCode: 'MITTEL' as CategoryCode,
    categoryName: 'Mittelklasse' as CategoryName,
    locationCode: 'BER-HBF' as LocationCode,
    city: 'Berlin' as CityName,
    dailyRateNet: 50.0 as DailyRate,
    dailyRateVat: 9.5 as DailyRate,
    dailyRateGross: 59.5 as DailyRate,
    currency: 'EUR' as Currency,
    seats: 5 as SeatingCapacity,
    fuelType: 'Petrol' as FuelType,
    transmissionType: 'Manual' as TransmissionType,
    status: 'Available' as VehicleStatus,
    licensePlate: 'B-AB 1234' as LicensePlate,
    manufacturer: 'Volkswagen' as Manufacturer,
    model: 'Golf 8' as VehicleModel,
    year: 2023 as ManufacturingYear,
    imageUrl: null,
  };

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

    component['onSearch']({ locationCode: 'BER-HBF' as LocationCode });

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
        locationCode: 'BER-HBF' as LocationCode,
      };

      component['onSearch'](query);

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith(query);
    });

    it('should search vehicles with date range filter', () => {
      const query: VehicleSearchQuery = {
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
      };

      component['onSearch'](query);

      expect(vehicleService.searchVehicles).toHaveBeenCalledWith(query);
    });

    it('should search vehicles with multiple filters', () => {
      const query: VehicleSearchQuery = {
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
        locationCode: 'BER-HBF' as LocationCode,
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
        locationCode: 'BER-HBF' as LocationCode,
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
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
      const query: VehicleSearchQuery = {
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
        locationCode: 'BER-HBF' as LocationCode,
      };

      component['currentSearchQuery'].set(query);
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: '123e4567-e89b-12d3-a456-426614174000',
          categoryCode: 'MITTEL' as CategoryCode,
          pickupDate: '2024-01-15' as ISODateString,
          returnDate: '2024-01-20' as ISODateString,
          locationCode: 'BER-HBF' as LocationCode,
        },
      });
    });

    it('should navigate to booking with vehicle location when search has no location', () => {
      const query: VehicleSearchQuery = {
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
      };

      component['currentSearchQuery'].set(query);
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: '123e4567-e89b-12d3-a456-426614174000',
          categoryCode: 'MITTEL' as CategoryCode,
          pickupDate: '2024-01-15' as ISODateString,
          returnDate: '2024-01-20' as ISODateString,
          locationCode: 'BER-HBF' as LocationCode,
        },
      });
    });

    it('should navigate to booking with empty dates when search has no dates', () => {
      const query: VehicleSearchQuery = {
        locationCode: 'BER-HBF' as LocationCode,
      };

      component['currentSearchQuery'].set(query);
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: '123e4567-e89b-12d3-a456-426614174000',
          categoryCode: 'MITTEL' as CategoryCode,
          pickupDate: '',
          returnDate: '',
          locationCode: 'BER-HBF' as LocationCode,
        },
      });
    });

    it('should handle booking with no search query', () => {
      component['currentSearchQuery'].set({});
      component['onBookVehicle'](mockVehicle);

      expect(router.navigate).toHaveBeenCalledWith(['/booking'], {
        queryParams: {
          vehicleId: '123e4567-e89b-12d3-a456-426614174000',
          categoryCode: 'MITTEL' as CategoryCode,
          pickupDate: '',
          returnDate: '',
          locationCode: 'BER-HBF' as LocationCode,
        },
      });
    });
  });

  describe('State Management', () => {
    it('should create component through TestBed with proper dependencies', () => {
      // These initialization tests are covered by the 'should create' test
      // Cannot test with 'new VehicleListComponent()' due to inject() usage
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

      component['onSearch']({ locationCode: 'BER-HBF' as LocationCode });
      component['onSearch']({ locationCode: 'MUC-FLG' as LocationCode });
      component['onSearch']({ locationCode: 'HAM-CTY' as LocationCode });

      expect(vehicleService.searchVehicles).toHaveBeenCalledTimes(4); // 1 from init + 3 manual
    });
  });
});
