import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ReservationService } from './reservation.service';
import { ConfigService } from './config.service';
import type {
  GuestReservationRequest,
  GuestReservationResponse,
  Reservation,
  ReservationId,
  LicenseNumber,
} from '@orange-car-rental/reservation-api';
import type {
  Price,
  Currency,
  ISODateString,
  EmailAddress,
  PhoneNumber,
  FirstName,
  LastName,
  PostalCode,
  CountryCode,
} from '@orange-car-rental/shared';
import type { StreetAddress, CityName } from '@orange-car-rental/location-api';
import type { CategoryCode } from '@orange-car-rental/vehicle-api';
import {
  MOCK_RESERVATIONS,
  TEST_RESERVATION_IDS,
  TEST_CUSTOMER_IDS,
  TEST_VEHICLE_IDS,
  TEST_LOCATION_CODES,
  getFutureDate,
  getPastDate,
} from '@orange-car-rental/shared/testing';

describe('ReservationService', () => {
  let service: ReservationService;
  let httpMock: HttpTestingController;
  let configService: ConfigService;

  const mockApiUrl = 'https://api.example.com';

  const mockGuestReservationRequest: GuestReservationRequest = {
    reservation: {
      vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
      categoryCode: 'MITTEL' as CategoryCode,
      pickupDate: getFutureDate(7),
      returnDate: getFutureDate(12),
      pickupLocationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      dropoffLocationCode: TEST_LOCATION_CODES.BERLIN_HBF,
    },
    customer: {
      firstName: 'Max' as FirstName,
      lastName: 'Mustermann' as LastName,
      email: 'max.mustermann@example.com' as EmailAddress,
      phoneNumber: '+49 30 12345678' as PhoneNumber,
      dateOfBirth: '1990-01-15' as ISODateString,
    },
    address: {
      street: 'Musterstraße 123' as StreetAddress,
      city: 'Berlin' as CityName,
      postalCode: '10115' as PostalCode,
      country: 'DE' as CountryCode,
    },
    driversLicense: {
      licenseNumber: 'B123456789' as LicenseNumber,
      licenseIssueCountry: 'DE' as CountryCode,
      licenseIssueDate: getPastDate(365 * 14), // 14 years ago
      licenseExpiryDate: getFutureDate(365 * 5), // 5 years from now
    },
  };

  const mockGuestReservationResponse: GuestReservationResponse = {
    reservationId: TEST_RESERVATION_IDS.PENDING,
    customerId: TEST_CUSTOMER_IDS.HANS_MUELLER,
    totalPriceNet: 250.0 as Price,
    totalPriceVat: 47.5 as Price,
    totalPriceGross: 297.5 as Price,
    currency: 'EUR' as Currency,
  };

  // Use shared mock reservation
  const mockReservation: Reservation = MOCK_RESERVATIONS.PENDING;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ReservationService, ConfigService],
    });

    service = TestBed.inject(ReservationService);
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

  describe('createGuestReservation', () => {
    it('should create a guest reservation successfully', () => {
      service.createGuestReservation(mockGuestReservationRequest).subscribe((response) => {
        expect(response).toEqual(mockGuestReservationResponse);
        expect(response.reservationId).toBe(TEST_RESERVATION_IDS.PENDING);
        expect(response.customerId).toBe(TEST_CUSTOMER_IDS.HANS_MUELLER);
        expect(response.totalPriceGross).toBe(297.5 as Price);
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockGuestReservationRequest);
      req.flush(mockGuestReservationResponse);
    });

    it('should send correct nested structure in request body', () => {
      service.createGuestReservation(mockGuestReservationRequest).subscribe();

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);

      // Verify nested structure
      expect(req.request.body.reservation).toBeDefined();
      expect(req.request.body.customer).toBeDefined();
      expect(req.request.body.address).toBeDefined();
      expect(req.request.body.driversLicense).toBeDefined();

      // Verify reservation details
      expect(req.request.body.reservation.vehicleId).toBe(TEST_VEHICLE_IDS.VW_GOLF);
      expect(req.request.body.reservation.categoryCode).toBe('MITTEL');

      // Verify customer details
      expect(req.request.body.customer.firstName).toBe('Max');
      expect(req.request.body.customer.lastName).toBe('Mustermann');
      expect(req.request.body.customer.email).toBe('max.mustermann@example.com');

      req.flush(mockGuestReservationResponse);
    });

    it('should handle validation errors (400 Bad Request)', () => {
      const errorResponse = {
        message: 'Invalid driver license expiry date',
      };

      service.createGuestReservation(mockGuestReservationRequest).subscribe({
        next: () => fail('should have failed with 400 error'),
        error: (error) => {
          expect(error.status).toBe(400);
          expect(error.error.message).toBe('Invalid driver license expiry date');
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      req.flush(errorResponse, { status: 400, statusText: 'Bad Request' });
    });

    it('should handle conflict errors (409 Conflict)', () => {
      const errorResponse = {
        message: 'Email already exists',
      };

      service.createGuestReservation(mockGuestReservationRequest).subscribe({
        next: () => fail('should have failed with 409 error'),
        error: (error) => {
          expect(error.status).toBe(409);
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      req.flush(errorResponse, { status: 409, statusText: 'Conflict' });
    });

    it('should handle server errors (500 Internal Server Error)', () => {
      service.createGuestReservation(mockGuestReservationRequest).subscribe({
        next: () => fail('should have failed with 500 error'),
        error: (error) => {
          expect(error.status).toBe(500);
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('getReservation', () => {
    it('should get reservation by ID', () => {
      const reservationId = TEST_RESERVATION_IDS.PENDING;

      service.getReservation(reservationId).subscribe((reservation) => {
        expect(reservation).toEqual(mockReservation);
        expect(reservation.id).toBe(reservationId);
        expect(reservation.status).toBe('Pending');
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/${reservationId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockReservation);
    });

    it('should handle 404 when reservation not found', () => {
      const reservationId = 'non-existent-id' as ReservationId;

      service.getReservation(reservationId).subscribe({
        next: () => fail('should have failed with 404 error'),
        error: (error) => {
          expect(error.status).toBe(404);
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/${reservationId}`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });

    it('should handle malformed reservation ID (400 Bad Request)', () => {
      const invalidId = 'invalid-guid' as ReservationId;

      service.getReservation(invalidId).subscribe({
        next: () => fail('should have failed with 400 error'),
        error: (error) => {
          expect(error.status).toBe(400);
        },
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/${invalidId}`);
      req.flush('Invalid ID format', { status: 400, statusText: 'Bad Request' });
    });
  });
});
