import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { VehicleService } from './vehicle.service';
import { ConfigService } from './config.service';
import type {
  Vehicle,
  VehicleId,
  VehicleSearchQuery,
  VehicleSearchResult,
  CategoryCode,
  SeatingCapacity,
  DailyRate,
  FuelType,
  TransmissionType,
} from '@orange-car-rental/vehicle-api';
import {
  MOCK_VEHICLES,
  TEST_VEHICLE_IDS,
  TEST_LOCATION_CODES,
  getFutureDate,
} from '@orange-car-rental/shared/testing';

describe('VehicleService', () => {
  let service: VehicleService;
  let httpMock: HttpTestingController;
  let configService: ConfigService;

  const mockApiUrl = 'https://api.example.com';

  // Use shared mock vehicle
  const mockVehicle: Vehicle = MOCK_VEHICLES.VW_GOLF;

  const mockSearchResult: VehicleSearchResult = {
    items: [mockVehicle],
    totalCount: 1,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
    hasPreviousPage: false,
    hasNextPage: false,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [VehicleService, ConfigService],
    });

    service = TestBed.inject(VehicleService);
    httpMock = TestBed.inject(HttpTestingController);
    configService = TestBed.inject(ConfigService);
    configService.setConfig({ apiUrl: mockApiUrl });
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('searchVehicles', () => {
    it('should search vehicles without query parameters', () => {
      service.searchVehicles().subscribe((result) => {
        expect(result).toEqual(mockSearchResult);
        expect(result.items.length).toBe(1);
        expect(result.totalCount).toBe(1);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/vehicles`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.keys().length).toBe(0);
      req.flush(mockSearchResult);
    });

    it('should search vehicles with location filter', () => {
      const query: VehicleSearchQuery = {
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      };

      service.searchVehicles(query).subscribe((result) => {
        expect(result).toEqual(mockSearchResult);
      });

      const req = httpMock.expectOne(
        (req) =>
          req.url === `${mockApiUrl}/api/vehicles` &&
          req.params.get('locationCode') === TEST_LOCATION_CODES.BERLIN_HBF,
      );
      expect(req.request.method).toBe('GET');
      req.flush(mockSearchResult);
    });

    it('should search vehicles with date range filter', () => {
      const pickupDate = getFutureDate(7);
      const returnDate = getFutureDate(12);
      const query: VehicleSearchQuery = {
        pickupDate,
        returnDate,
      };

      service.searchVehicles(query).subscribe((result) => {
        expect(result).toEqual(mockSearchResult);
      });

      const req = httpMock.expectOne(
        (req) =>
          req.url === `${mockApiUrl}/api/vehicles` &&
          req.params.get('pickupDate') === pickupDate &&
          req.params.get('returnDate') === returnDate,
      );
      expect(req.request.method).toBe('GET');
      req.flush(mockSearchResult);
    });

    it('should search vehicles with all filters', () => {
      const pickupDate = getFutureDate(7);
      const returnDate = getFutureDate(12);
      const query: VehicleSearchQuery = {
        pickupDate,
        returnDate,
        locationCode: TEST_LOCATION_CODES.BERLIN_HBF,
        categoryCode: 'MITTEL' as CategoryCode,
        minSeats: 5 as SeatingCapacity,
        fuelType: 'Petrol' as FuelType,
        transmissionType: 'Manual' as TransmissionType,
        maxDailyRateGross: 100.0 as DailyRate,
        pageNumber: 1,
        pageSize: 20,
      };

      service.searchVehicles(query).subscribe((result) => {
        expect(result).toEqual(mockSearchResult);
      });

      const req = httpMock.expectOne(
        (req) =>
          req.url === `${mockApiUrl}/api/vehicles` &&
          req.params.get('locationCode') === TEST_LOCATION_CODES.BERLIN_HBF &&
          req.params.get('categoryCode') === 'MITTEL' &&
          req.params.get('minSeats') === '5' &&
          req.params.get('fuelType') === 'Petrol' &&
          req.params.get('transmissionType') === 'Manual' &&
          req.params.get('maxDailyRateGross') === '100' &&
          req.params.get('pageNumber') === '1' &&
          req.params.get('pageSize') === '20',
      );
      expect(req.request.method).toBe('GET');
      req.flush(mockSearchResult);
    });

    it('should handle empty search results', () => {
      const emptyResult: VehicleSearchResult = {
        items: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      };

      service.searchVehicles().subscribe((result) => {
        expect(result.items.length).toBe(0);
        expect(result.totalCount).toBe(0);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/vehicles`);
      req.flush(emptyResult);
    });

    it('should handle HTTP error', () => {
      service.searchVehicles().subscribe({
        next: () => fail('should have failed with 500 error'),
        error: (error) => {
          expect(error.status).toBe(500);
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/vehicles`);
      req.flush('Server error', { status: 500, statusText: 'Server Error' });
    });
  });

  describe('getVehicleById', () => {
    it('should get vehicle by ID', () => {
      const vehicleId = TEST_VEHICLE_IDS.VW_GOLF;

      service.getVehicleById(vehicleId).subscribe((vehicle) => {
        expect(vehicle).toEqual(mockVehicle);
        expect(vehicle.id).toBe(vehicleId);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/vehicles/${vehicleId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockVehicle);
    });

    it('should handle 404 when vehicle not found', () => {
      const vehicleId = 'non-existent-id' as VehicleId;

      service.getVehicleById(vehicleId).subscribe({
        next: () => fail('should have failed with 404 error'),
        error: (error) => {
          expect(error.status).toBe(404);
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/vehicles/${vehicleId}`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });
  });
});
