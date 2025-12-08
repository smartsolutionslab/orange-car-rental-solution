import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ReservationService } from './reservation.service';
import { ConfigService } from './config.service';
import type { GuestReservationRequest, GuestReservationResponse, Reservation, CustomerId } from '@orange-car-rental/reservation-api';

describe('ReservationService', () => {
  let service: ReservationService;
  let httpMock: HttpTestingController;
  let configService: ConfigService;

  const mockApiUrl = 'https://api.example.com';

  const mockGuestReservationRequest: GuestReservationRequest = {
    reservation: {
      vehicleId: '123e4567-e89b-12d3-a456-426614174000',
      categoryCode: 'MITTEL',
      pickupDate: '2024-01-15',
      returnDate: '2024-01-20',
      pickupLocationCode: 'BER-HBF',
      dropoffLocationCode: 'BER-HBF'
    },
    customer: {
      firstName: 'Max',
      lastName: 'Mustermann',
      email: 'max.mustermann@example.com',
      phoneNumber: '+49 30 12345678',
      dateOfBirth: '1990-01-15'
    },
    address: {
      street: 'Musterstraße 123',
      city: 'Berlin',
      postalCode: '10115',
      country: 'Germany'
    },
    driversLicense: {
      licenseNumber: 'B123456789',
      licenseIssueCountry: 'Germany',
      licenseIssueDate: '2010-06-01',
      licenseExpiryDate: '2030-06-01'
    }
  };

  const mockGuestReservationResponse: GuestReservationResponse = {
    reservationId: '987e6543-e89b-12d3-a456-426614174000',
    customerId: '111e2222-e89b-12d3-a456-426614174000' as CustomerId,
    totalPriceNet: 250.00,
    totalPriceVat: 47.50,
    totalPriceGross: 297.50,
    currency: 'EUR'
  };

  const mockReservation: Reservation = {
    id: '987e6543-e89b-12d3-a456-426614174000',
    vehicleId: '123e4567-e89b-12d3-a456-426614174000',
    customerId: '111e2222-e89b-12d3-a456-426614174000' as CustomerId,
    pickupDate: '2024-01-15T00:00:00Z',
    returnDate: '2024-01-20T00:00:00Z',
    pickupLocationCode: 'BER-HBF',
    dropoffLocationCode: 'BER-HBF',
    totalPriceNet: 250.00,
    totalPriceVat: 47.50,
    totalPriceGross: 297.50,
    currency: 'EUR',
    status: 'Pending'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ReservationService, ConfigService]
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
      service.createGuestReservation(mockGuestReservationRequest).subscribe(response => {
        expect(response).toEqual(mockGuestReservationResponse);
        expect(response.reservationId).toBe('987e6543-e89b-12d3-a456-426614174000');
        expect(response.customerId).toBe('111e2222-e89b-12d3-a456-426614174000');
        expect(response.totalPriceGross).toBe(297.50);
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
      expect(req.request.body.reservation.vehicleId).toBe('123e4567-e89b-12d3-a456-426614174000');
      expect(req.request.body.reservation.categoryCode).toBe('MITTEL');

      // Verify customer details
      expect(req.request.body.customer.firstName).toBe('Max');
      expect(req.request.body.customer.lastName).toBe('Mustermann');
      expect(req.request.body.customer.email).toBe('max.mustermann@example.com');

      req.flush(mockGuestReservationResponse);
    });

    it('should handle validation errors (400 Bad Request)', () => {
      const errorResponse = {
        message: 'Invalid driver license expiry date'
      };

      service.createGuestReservation(mockGuestReservationRequest).subscribe({
        next: () => fail('should have failed with 400 error'),
        error: (error) => {
          expect(error.status).toBe(400);
          expect(error.error.message).toBe('Invalid driver license expiry date');
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      req.flush(errorResponse, { status: 400, statusText: 'Bad Request' });
    });

    it('should handle conflict errors (409 Conflict)', () => {
      const errorResponse = {
        message: 'Email already exists'
      };

      service.createGuestReservation(mockGuestReservationRequest).subscribe({
        next: () => fail('should have failed with 409 error'),
        error: (error) => {
          expect(error.status).toBe(409);
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      req.flush(errorResponse, { status: 409, statusText: 'Conflict' });
    });

    it('should handle server errors (500 Internal Server Error)', () => {
      service.createGuestReservation(mockGuestReservationRequest).subscribe({
        next: () => fail('should have failed with 500 error'),
        error: (error) => {
          expect(error.status).toBe(500);
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/guest`);
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('getReservation', () => {
    it('should get reservation by ID', () => {
      const reservationId = '987e6543-e89b-12d3-a456-426614174000';

      service.getReservation(reservationId).subscribe(reservation => {
        expect(reservation).toEqual(mockReservation);
        expect(reservation.id).toBe(reservationId);
        expect(reservation.status).toBe('Pending');
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/${reservationId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockReservation);
    });

    it('should handle 404 when reservation not found', () => {
      const reservationId = 'non-existent-id';

      service.getReservation(reservationId).subscribe({
        next: () => fail('should have failed with 404 error'),
        error: (error) => {
          expect(error.status).toBe(404);
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/${reservationId}`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });

    it('should handle malformed reservation ID (400 Bad Request)', () => {
      const invalidId = 'invalid-guid';

      service.getReservation(invalidId).subscribe({
        next: () => fail('should have failed with 400 error'),
        error: (error) => {
          expect(error.status).toBe(400);
        }
      });

      const req = httpMock.expectOne(`${mockApiUrl}/api/reservations/${invalidId}`);
      req.flush('Invalid ID format', { status: 400, statusText: 'Bad Request' });
    });
  });
});
