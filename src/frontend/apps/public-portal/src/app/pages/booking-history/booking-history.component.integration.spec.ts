import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TranslateModule } from '@ngx-translate/core';
import { BookingHistoryComponent } from './booking-history.component';
import { ReservationService } from '../../services/reservation.service';
import { AuthService } from '../../services/auth.service';
import { ConfigService } from '../../services/config.service';
import type { Reservation, ReservationStatus } from '@orange-car-rental/reservation-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import { API_CONFIG, ToastService } from '@orange-car-rental/shared';
import type { Price, Currency } from '@orange-car-rental/shared';
import {
  getFutureDate,
  getPastDate,
  TEST_RESERVATION_IDS,
  TEST_CUSTOMER_IDS,
  TEST_VEHICLE_IDS,
} from '@orange-car-rental/shared/testing';

/**
 * Integration Tests for Booking History Component
 * Tests the full interaction between component, service, and HTTP layer
 */
describe('BookingHistoryComponent (Integration)', () => {
  let component: BookingHistoryComponent;
  let fixture: ComponentFixture<BookingHistoryComponent>;
  let httpMock: HttpTestingController;
  let authService: jasmine.SpyObj<AuthService>;
  let toastService: jasmine.SpyObj<ToastService>;

  const apiUrl = 'http://localhost:5000';

  // Use shared test fixtures with short location codes for integration tests
  const mockReservations: Reservation[] = [
    {
      id: TEST_RESERVATION_IDS.CONFIRMED,
      vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
      customerId: TEST_CUSTOMER_IDS.HANS_MUELLER,
      pickupDate: getFutureDate(7), // 7 days from now - upcoming
      returnDate: getFutureDate(11), // 11 days from now
      pickupLocationCode: 'MUC' as LocationCode,
      dropoffLocationCode: 'MUC' as LocationCode,
      rentalDays: 4,
      totalPriceNet: 336.13 as Price,
      totalPriceVat: 63.87 as Price,
      totalPriceGross: 400.0 as Price,
      currency: 'EUR' as Currency,
      status: 'Confirmed' as ReservationStatus,
      cancellationReason: null,
      createdAt: getPastDate(15),
      confirmedAt: getPastDate(14),
      cancelledAt: null,
      completedAt: null,
    },
    {
      id: TEST_RESERVATION_IDS.PENDING,
      vehicleId: TEST_VEHICLE_IDS.BMW_3ER,
      customerId: TEST_CUSTOMER_IDS.HANS_MUELLER,
      pickupDate: getFutureDate(14), // 14 days from now - pending
      returnDate: getFutureDate(16), // 16 days from now
      pickupLocationCode: 'BER' as LocationCode,
      dropoffLocationCode: 'BER' as LocationCode,
      rentalDays: 2,
      totalPriceNet: 168.07 as Price,
      totalPriceVat: 31.93 as Price,
      totalPriceGross: 200.0 as Price,
      currency: 'EUR' as Currency,
      status: 'Pending' as ReservationStatus,
      cancellationReason: null,
      createdAt: getPastDate(15),
      confirmedAt: null,
      cancelledAt: null,
      completedAt: null,
    },
  ];

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', [
      'isAuthenticated',
      'getUserProfile',
    ]);

    const toastServiceSpy = jasmine.createSpyObj('ToastService', [
      'success',
      'error',
      'info',
      'warning',
    ]);

    const configServiceSpy = jasmine.createSpyObj('ConfigService', [], {
      apiUrl: apiUrl,
    });

    await TestBed.configureTestingModule({
      imports: [BookingHistoryComponent, HttpClientTestingModule, TranslateModule.forRoot()],
      providers: [
        ReservationService,
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ConfigService, useValue: configServiceSpy },
        { provide: ToastService, useValue: toastServiceSpy },
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    toastService = TestBed.inject(ToastService) as jasmine.SpyObj<ToastService>;

    fixture = TestBed.createComponent(BookingHistoryComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('Complete Authenticated User Flow', () => {
    it('should load and display user reservations with real HTTP calls', fakeAsync(() => {
      // Arrange
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({
          id: TEST_CUSTOMER_IDS.HANS_MUELLER as string,
          username: 'testuser',
          email: 'test@example.com',
          firstName: 'Test',
          lastName: 'User',
        }),
      );

      // Act
      component.ngOnInit();
      tick();

      // Assert - HTTP Request
      const req = httpMock.expectOne(
        (request) =>
          request.url.includes('/api/reservations/search') &&
          request.params.get('customerId') === TEST_CUSTOMER_IDS.HANS_MUELLER,
      );
      expect(req.request.method).toBe('GET');

      // Respond with mock data
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      // Verify component state
      expect(component.isAuthenticated()).toBe(true);
      expect(component.groupedReservations().upcoming.length).toBe(1);
      expect(component.groupedReservations().pending.length).toBe(1);
      expect(component.isLoading()).toBe(false);
    }));

    it('should handle HTTP error gracefully', fakeAsync(() => {
      // Arrange
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({ id: TEST_CUSTOMER_IDS.HANS_MUELLER as string, username: 'testuser' }),
      );

      // Act
      component.ngOnInit();
      tick();

      // Assert - HTTP Request fails
      const req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
      tick();

      // Verify error handling
      expect(component.error()).toBe('bookingHistory.errors.loadFailed');
      expect(component.isLoading()).toBe(false);
    }));

    it('should retry loading after error', fakeAsync(() => {
      // Arrange
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({ id: TEST_CUSTOMER_IDS.HANS_MUELLER as string, username: 'testuser' }),
      );

      component.ngOnInit();
      tick();

      // First request fails
      let req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush('Error', { status: 500, statusText: 'Server Error' });
      tick();

      expect(component.error()).toBeTruthy();

      // Retry
      component['loadCustomerReservations']();
      tick();

      // Second request succeeds
      req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      // Verify recovery
      expect(component.error()).toBeNull();
      expect(component.groupedReservations().upcoming.length).toBeGreaterThan(0);
    }));
  });

  describe('Guest Lookup Flow with Real HTTP', () => {
    beforeEach(() => {
      authService.isAuthenticated.and.returnValue(false);
    });

    it('should lookup guest reservation via HTTP', fakeAsync(() => {
      // Arrange
      component.ngOnInit();
      tick();
      fixture.detectChanges();

      component.guestLookupForm.reservationId = TEST_RESERVATION_IDS.CONFIRMED as string;
      component.guestLookupForm.email = 'guest@example.com';

      // Act
      component.onGuestLookup();
      tick();

      // Assert - HTTP Request
      const req = httpMock.expectOne(
        (request) =>
          request.url.includes('/api/reservations/lookup') &&
          request.params.get('reservationId') === TEST_RESERVATION_IDS.CONFIRMED &&
          request.params.get('email') === 'guest@example.com',
      );
      expect(req.request.method).toBe('GET');

      // Respond with reservation
      req.flush(mockReservations[0]!);
      tick();

      // Verify result
      expect(component.guestReservation()).toEqual(mockReservations[0]);
      expect(component.guestLookupError()).toBeNull();
      expect(component.isLoading()).toBe(false);
    }));

    it('should handle guest lookup not found', fakeAsync(() => {
      // Arrange
      component.ngOnInit();
      tick();

      component.guestLookupForm.reservationId = 'invalid-id';
      component.guestLookupForm.email = 'wrong@example.com';

      // Act
      component.onGuestLookup();
      tick();

      // Assert - HTTP Request fails with 404
      const req = httpMock.expectOne((request) => request.url.includes('/api/reservations/lookup'));
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
      tick();

      // Verify error
      expect(component.guestLookupError()).toBe('bookingHistory.guestLookup.notFound');
      expect(component.guestReservation()).toBeNull();
    }));
  });

  describe('Cancellation Flow with Real HTTP', () => {
    beforeEach(fakeAsync(() => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({ id: TEST_CUSTOMER_IDS.HANS_MUELLER as string, username: 'testuser' }),
      );

      component.ngOnInit();
      tick();

      const req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();
    }));

    it('should cancel reservation via HTTP', fakeAsync(() => {
      // Arrange
      const reservation = component.groupedReservations().upcoming[0];
      component.selectedReservation.set(reservation);
      component.cancellationReason = 'Change of plans';

      // Act
      component.confirmCancellation();
      tick();

      // Assert - HTTP Request
      const req = httpMock.expectOne(
        (request) =>
          request.url.includes(`/api/reservations/${reservation.id}/cancel`) &&
          request.method === 'PUT',
      );
      expect(req.request.body).toEqual({ cancellationReason: 'Change of plans' });

      // Respond with success
      req.flush(null);
      tick();

      // Verify reload request
      const reloadReq = httpMock.expectOne((request) =>
        request.url.includes('/api/reservations/search'),
      );
      reloadReq.flush({
        items: mockReservations.map((r) =>
          r.id === reservation.id ? { ...r, status: 'Cancelled' } : r,
        ),
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      // Verify success
      expect(toastService.success).toHaveBeenCalledWith('bookingHistory.cancel.success');
      expect(component.showCancelModal()).toBe(false);
    }));

    it('should handle cancellation HTTP error', fakeAsync(() => {
      // Arrange
      const reservation = component.groupedReservations().upcoming[0];
      component.selectedReservation.set(reservation);
      component.cancellationReason = 'Test';

      // Act
      component.confirmCancellation();
      tick();

      // Assert - HTTP Request fails
      const req = httpMock.expectOne((request) =>
        request.url.includes(`/api/reservations/${reservation.id}/cancel`),
      );
      req.flush('Cancellation not allowed', { status: 400, statusText: 'Bad Request' });
      tick();

      // Verify error
      expect(toastService.error).toHaveBeenCalledWith('bookingHistory.cancel.error');
      expect(component.isLoading()).toBe(false);
    }));
  });

  describe('End-to-End Booking History Scenario', () => {
    it('should complete full user journey: login -> view bookings -> cancel -> verify', fakeAsync(() => {
      // Step 1: User logs in
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({
          id: TEST_CUSTOMER_IDS.HANS_MUELLER as string,
          username: 'testuser',
          email: 'user@example.com',
        }),
      );

      component.ngOnInit();
      tick();

      // Step 2: Load initial bookings
      let req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      // Verify initial state
      expect(component.groupedReservations().upcoming.length).toBe(1);
      expect(component.groupedReservations().pending.length).toBe(1);

      // Step 3: User views reservation details
      const reservation = component.groupedReservations().upcoming[0];
      component.viewDetails(reservation);
      expect(component.showDetailModal()).toBe(true);
      expect(component.detailReservation()).toEqual(reservation);

      // Step 4: User decides to cancel
      component.closeDetailModal();
      component.openCancelModal(reservation);
      expect(component.showCancelModal()).toBe(true);

      // Step 5: User confirms cancellation
      component.cancellationReason = 'Trip cancelled';
      component.confirmCancellation();
      tick();

      // Step 6: Cancel HTTP request
      req = httpMock.expectOne((request) =>
        request.url.includes(`/api/reservations/${reservation.id}/cancel`),
      );
      req.flush(null);
      tick();

      // Step 7: Reload bookings
      req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations.map((r) =>
          r.id === reservation.id ? { ...r, status: 'Cancelled' } : r,
        ),
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      // Step 8: Verify final state
      expect(toastService.success).toHaveBeenCalledWith('bookingHistory.cancel.success');
      expect(
        component
          .groupedReservations()
          .past.some((r) => r.id === reservation.id && r.status === 'Cancelled'),
      ).toBe(true);
      expect(component.showCancelModal()).toBe(false);
    }));
  });

  describe('Performance and Concurrency', () => {
    it('should handle multiple concurrent requests', fakeAsync(() => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({ id: TEST_CUSTOMER_IDS.HANS_MUELLER as string, username: 'testuser' }),
      );

      // Start multiple operations
      component.ngOnInit();
      tick();

      const req1 = httpMock.expectOne((request) =>
        request.url.includes('/api/reservations/search'),
      );

      // Simulate user navigating away and back quickly
      component['loadCustomerReservations']();
      tick();

      // Both requests complete
      req1.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });

      const req2 = httpMock.expectOne((request) =>
        request.url.includes('/api/reservations/search'),
      );
      req2.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      // Should handle gracefully
      expect(component.isLoading()).toBe(false);
      expect(component.groupedReservations().upcoming.length).toBeGreaterThan(0);
    }));

    it('should handle slow network gracefully', fakeAsync(() => {
      authService.isAuthenticated.and.returnValue(true);
      authService.getUserProfile.and.returnValue(
        Promise.resolve({ id: TEST_CUSTOMER_IDS.HANS_MUELLER as string, username: 'testuser' }),
      );

      component.ngOnInit();
      tick();

      expect(component.isLoading()).toBe(true);

      // Simulate slow network (no response yet)
      tick(3000);
      expect(component.isLoading()).toBe(true);

      // Finally respond
      const req = httpMock.expectOne((request) => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1,
      });
      tick();

      expect(component.isLoading()).toBe(false);
    }));
  });
});
