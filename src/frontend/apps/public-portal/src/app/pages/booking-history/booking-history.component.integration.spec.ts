import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BookingHistoryComponent } from './booking-history.component';
import { ReservationService } from '../../services/reservation.service';
import { AuthService } from '../../services/auth.service';
import { ConfigService } from '../../services/config.service';

/**
 * Integration Tests for Booking History Component
 * Tests the full interaction between component, service, and HTTP layer
 */
describe('BookingHistoryComponent (Integration)', () => {
  let component: BookingHistoryComponent;
  let fixture: ComponentFixture<BookingHistoryComponent>;
  let httpMock: HttpTestingController;
  let authService: jasmine.SpyObj<AuthService>;
  let configService: jasmine.SpyObj<ConfigService>;

  const apiUrl = 'http://localhost:5000/api';

  const mockReservations = [
    {
      id: '123e4567-e89b-12d3-a456-426614174000',
      vehicleId: 'veh-001',
      customerId: 'cust-001',
      pickupDate: '2025-12-01T10:00:00Z',
      returnDate: '2025-12-05T10:00:00Z',
      pickupLocationCode: 'MUC',
      dropoffLocationCode: 'MUC',
      totalPriceNet: 336.13,
      totalPriceVat: 63.87,
      totalPriceGross: 400.00,
      currency: 'EUR',
      status: 'Confirmed',
      createdAt: '2025-11-20T09:00:00Z'
    },
    {
      id: '223e4567-e89b-12d3-a456-426614174001',
      vehicleId: 'veh-002',
      customerId: 'cust-001',
      pickupDate: '2025-11-25T10:00:00Z',
      returnDate: '2025-11-27T10:00:00Z',
      pickupLocationCode: 'BER',
      dropoffLocationCode: 'BER',
      totalPriceNet: 168.07,
      totalPriceVat: 31.93,
      totalPriceGross: 200.00,
      currency: 'EUR',
      status: 'Pending',
      createdAt: '2025-11-20T09:00:00Z'
    }
  ];

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', [
      'isAuthenticated',
      'getUserProfile'
    ]);

    const configServiceSpy = jasmine.createSpyObj('ConfigService', [], {
      apiUrl: apiUrl
    });

    await TestBed.configureTestingModule({
      imports: [
        BookingHistoryComponent,
        HttpClientTestingModule
      ],
      providers: [
        ReservationService,
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ConfigService, useValue: configServiceSpy }
      ]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    configService = TestBed.inject(ConfigService) as jasmine.SpyObj<ConfigService>;

    fixture = TestBed.createComponent(BookingHistoryComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('Complete Authenticated User Flow', () => {
    it('should load and display user reservations with real HTTP calls', fakeAsync(() => {
      // Arrange
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({
        sub: 'cust-001',
        email: 'test@example.com',
        name: 'Test User'
      });

      // Act
      component.ngOnInit();
      tick();
      fixture.detectChanges();

      // Assert - HTTP Request
      const req = httpMock.expectOne(request =>
        request.url.includes('/api/reservations/search') &&
        request.params.get('customerId') === 'cust-001'
      );
      expect(req.request.method).toBe('GET');

      // Respond with mock data
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1
      });
      tick();
      fixture.detectChanges();

      // Verify component state
      expect(component.isAuthenticated()).toBe(true);
      expect(component.groupedReservations().upcoming.length).toBe(1);
      expect(component.groupedReservations().pending.length).toBe(1);
      expect(component.isLoading()).toBe(false);
    }));

    it('should handle HTTP error gracefully', fakeAsync(() => {
      // Arrange
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({ sub: 'cust-001' });

      // Act
      component.ngOnInit();
      tick();

      // Assert - HTTP Request fails
      const req = httpMock.expectOne(request =>
        request.url.includes('/api/reservations/search')
      );
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
      tick();

      // Verify error handling
      expect(component.error()).toBe('Failed to load your booking history. Please try again later.');
      expect(component.isLoading()).toBe(false);
    }));

    it('should retry loading after error', fakeAsync(() => {
      // Arrange
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({ sub: 'cust-001' });

      component.ngOnInit();
      tick();

      // First request fails
      let req = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req.flush('Error', { status: 500, statusText: 'Server Error' });
      tick();

      expect(component.error()).toBeTruthy();

      // Retry
      component['loadCustomerReservations']();
      tick();

      // Second request succeeds
      req = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1
      });
      tick();

      // Verify recovery
      expect(component.error()).toBeNull();
      expect(component.groupedReservations().upcoming.length).toBeGreaterThan(0);
    }));
  });

  describe('Guest Lookup Flow with Real HTTP', () => {
    beforeEach(() => {
      authService.isAuthenticated.and.returnValue(Promise.resolve(false));
    });

    it('should lookup guest reservation via HTTP', fakeAsync(() => {
      // Arrange
      component.ngOnInit();
      tick();
      fixture.detectChanges();

      component.guestLookupForm.reservationId = '123e4567-e89b-12d3-a456-426614174000';
      component.guestLookupForm.email = 'guest@example.com';

      // Act
      component.onGuestLookup();
      tick();

      // Assert - HTTP Request
      const req = httpMock.expectOne(request =>
        request.url.includes('/api/reservations/lookup') &&
        request.params.get('reservationId') === '123e4567-e89b-12d3-a456-426614174000' &&
        request.params.get('email') === 'guest@example.com'
      );
      expect(req.request.method).toBe('GET');

      // Respond with reservation
      req.flush(mockReservations[0]);
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
      const req = httpMock.expectOne(request => request.url.includes('/api/reservations/lookup'));
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
      tick();

      // Verify error
      expect(component.guestLookupError()).toBe('Reservation not found. Please check your Reservation ID and Email.');
      expect(component.guestReservation()).toBeNull();
    }));
  });

  describe('Cancellation Flow with Real HTTP', () => {
    beforeEach(fakeAsync(() => {
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({ sub: 'cust-001' });

      component.ngOnInit();
      tick();

      const req = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1
      });
      tick();
    }));

    it('should cancel reservation via HTTP', fakeAsync(() => {
      // Arrange
      const reservation = component.groupedReservations().upcoming[0];
      component.selectedReservation.set(reservation);
      component.cancellationReason = 'Change of plans';

      spyOn(window, 'alert');

      // Act
      component.confirmCancellation();
      tick();

      // Assert - HTTP Request
      const req = httpMock.expectOne(request =>
        request.url.includes(`/api/reservations/${reservation.id}/cancel`) &&
        request.method === 'PUT'
      );
      expect(req.request.body).toEqual({ reason: 'Change of plans' });

      // Respond with success
      req.flush(null);
      tick();

      // Verify reload request
      const reloadReq = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      reloadReq.flush({
        items: mockReservations.map(r =>
          r.id === reservation.id ? { ...r, status: 'Cancelled' } : r
        ),
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1
      });
      tick();

      // Verify success
      expect(window.alert).toHaveBeenCalledWith('Reservation cancelled successfully!');
      expect(component.showCancelModal()).toBe(false);
    }));

    it('should handle cancellation HTTP error', fakeAsync(() => {
      // Arrange
      const reservation = component.groupedReservations().upcoming[0];
      component.selectedReservation.set(reservation);
      component.cancellationReason = 'Test';

      spyOn(window, 'alert');

      // Act
      component.confirmCancellation();
      tick();

      // Assert - HTTP Request fails
      const req = httpMock.expectOne(request =>
        request.url.includes(`/api/reservations/${reservation.id}/cancel`)
      );
      req.flush('Cancellation not allowed', { status: 400, statusText: 'Bad Request' });
      tick();

      // Verify error
      expect(window.alert).toHaveBeenCalledWith('Failed to cancel reservation. Please try again or contact support.');
      expect(component.isLoading()).toBe(false);
    }));
  });

  describe('End-to-End Booking History Scenario', () => {
    it('should complete full user journey: login -> view bookings -> cancel -> verify', fakeAsync(() => {
      // Step 1: User logs in
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({
        sub: 'cust-001',
        email: 'user@example.com'
      });

      component.ngOnInit();
      tick();

      // Step 2: Load initial bookings
      let req = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations,
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1
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
      spyOn(window, 'alert');
      component.confirmCancellation();
      tick();

      // Step 6: Cancel HTTP request
      req = httpMock.expectOne(request =>
        request.url.includes(`/api/reservations/${reservation.id}/cancel`)
      );
      req.flush(null);
      tick();

      // Step 7: Reload bookings
      req = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req.flush({
        items: mockReservations.map(r =>
          r.id === reservation.id ? { ...r, status: 'Cancelled' } : r
        ),
        totalCount: 2,
        pageNumber: 1,
        pageSize: 100,
        totalPages: 1
      });
      tick();

      // Step 8: Verify final state
      expect(window.alert).toHaveBeenCalledWith('Reservation cancelled successfully!');
      expect(component.groupedReservations().past.some(r => r.id === reservation.id && r.status === 'Cancelled')).toBe(true);
      expect(component.showCancelModal()).toBe(false);
    }));
  });

  describe('Performance and Concurrency', () => {
    it('should handle multiple concurrent requests', fakeAsync(() => {
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({ sub: 'cust-001' });

      // Start multiple operations
      component.ngOnInit();
      tick();

      const req1 = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));

      // Simulate user navigating away and back quickly
      component['loadCustomerReservations']();
      tick();

      // Both requests complete
      req1.flush({ items: mockReservations, totalCount: 2, pageNumber: 1, pageSize: 100, totalPages: 1 });

      const req2 = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req2.flush({ items: mockReservations, totalCount: 2, pageNumber: 1, pageSize: 100, totalPages: 1 });
      tick();

      // Should handle gracefully
      expect(component.isLoading()).toBe(false);
      expect(component.groupedReservations().upcoming.length).toBeGreaterThan(0);
    }));

    it('should handle slow network gracefully', fakeAsync(() => {
      authService.isAuthenticated.and.returnValue(Promise.resolve(true));
      authService.getUserProfile.and.returnValue({ sub: 'cust-001' });

      component.ngOnInit();
      tick();

      expect(component.isLoading()).toBe(true);

      // Simulate slow network (no response yet)
      tick(3000);
      expect(component.isLoading()).toBe(true);

      // Finally respond
      const req = httpMock.expectOne(request => request.url.includes('/api/reservations/search'));
      req.flush({ items: mockReservations, totalCount: 2, pageNumber: 1, pageSize: 100, totalPages: 1 });
      tick();

      expect(component.isLoading()).toBe(false);
    }));
  });
});
