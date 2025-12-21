import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { VehicleService } from './vehicle.service';
import { ConfigService } from './config.service';
import type {
  Vehicle,
  VehicleId,
  VehicleName,
  VehicleSearchQuery,
  VehicleSearchResult,
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
import type { ISODateString, Currency } from '@orange-car-rental/shared';
import type { LocationCode, CityName } from '@orange-car-rental/location-api';

describe('VehicleService', () => {
  let service: VehicleService;
  let httpMock: HttpTestingController;
  let configService: ConfigService;

  const mockApiUrl = 'https://api.example.com';

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
        expect(result.vehicles.length).toBe(1);
        expect(result.totalCount).toBe(1);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/vehicles`);
      expect(req.request.method).toBe('GET');
      expect(req.request.params.keys().length).toBe(0);
      req.flush(mockSearchResult);
    });

    it('should search vehicles with location filter', () => {
      const query: VehicleSearchQuery = {
        locationCode: 'BER-HBF' as LocationCode,
      };

      service.searchVehicles(query).subscribe((result) => {
        expect(result).toEqual(mockSearchResult);
      });

      const req = httpMock.expectOne(
        (req) =>
          req.url === `${mockApiUrl}/api/vehicles` && req.params.get('locationCode') === 'BER-HBF',
      );
      expect(req.request.method).toBe('GET');
      req.flush(mockSearchResult);
    });

    it('should search vehicles with date range filter', () => {
      const query: VehicleSearchQuery = {
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
      };

      service.searchVehicles(query).subscribe((result) => {
        expect(result).toEqual(mockSearchResult);
      });

      const req = httpMock.expectOne(
        (req) =>
          req.url === `${mockApiUrl}/api/vehicles` &&
          req.params.get('pickupDate') === '2024-01-15' &&
          req.params.get('returnDate') === '2024-01-20',
      );
      expect(req.request.method).toBe('GET');
      req.flush(mockSearchResult);
    });

    it('should search vehicles with all filters', () => {
      const query: VehicleSearchQuery = {
        pickupDate: '2024-01-15' as ISODateString,
        returnDate: '2024-01-20' as ISODateString,
        locationCode: 'BER-HBF' as LocationCode,
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
          req.params.get('locationCode') === 'BER-HBF' &&
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
        vehicles: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0,
      };

      service.searchVehicles().subscribe((result) => {
        expect(result.vehicles.length).toBe(0);
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
      const vehicleId = '123e4567-e89b-12d3-a456-426614174000' as VehicleId;

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
