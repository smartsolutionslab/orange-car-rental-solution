import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { ReservationsComponent } from './reservations.component';
import { ReservationService } from '../../services/reservation.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError, Observable } from 'rxjs';
import type { ReservationStatus } from '@orange-car-rental/reservation-api';
import type { Reservation } from '../../types';
import { API_CONFIG } from '@orange-car-rental/shared';
import type { Price, ISODateString } from '@orange-car-rental/shared';
import {
  TEST_RESERVATION_IDS,
  TEST_CUSTOMER_IDS,
  TEST_VEHICLE_IDS,
  TEST_LOCATION_CODES,
  getFutureDate,
  getPastDate,
} from '@orange-car-rental/shared/testing';

describe('ReservationsComponent', () => {
  let component: ReservationsComponent;
  let fixture: ComponentFixture<ReservationsComponent>;
  let mockReservationService: jasmine.SpyObj<ReservationService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: { queryParams: Observable<Record<string, string>> };

  // Use shared test IDs where possible
  const mockReservations: Reservation[] = [
    {
      reservationId: TEST_RESERVATION_IDS.CONFIRMED,
      vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
      customerId: TEST_CUSTOMER_IDS.HANS_MUELLER,
      pickupDate: getFutureDate(10),
      returnDate: getFutureDate(14),
      pickupLocationCode: TEST_LOCATION_CODES.MUNICH_AIRPORT,
      dropoffLocationCode: TEST_LOCATION_CODES.MUNICH_AIRPORT,
      rentalDays: 4,
      totalPriceNet: 336.13 as Price,
      totalPriceVat: 63.87 as Price,
      totalPriceGross: 400.0 as Price,
      currency: 'EUR',
      status: 'Confirmed' as ReservationStatus,
      createdAt: getPastDate(10),
    },
    {
      reservationId: TEST_RESERVATION_IDS.PENDING,
      vehicleId: TEST_VEHICLE_IDS.BMW_3ER,
      customerId: TEST_CUSTOMER_IDS.ANNA_SCHMIDT,
      pickupDate: getFutureDate(5),
      returnDate: getFutureDate(7),
      pickupLocationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      dropoffLocationCode: TEST_LOCATION_CODES.BERLIN_HBF,
      rentalDays: 2,
      totalPriceNet: 168.07 as Price,
      totalPriceVat: 31.93 as Price,
      totalPriceGross: 200.0 as Price,
      currency: 'EUR',
      status: 'Pending' as ReservationStatus,
      createdAt: getPastDate(10),
    },
    {
      reservationId: TEST_RESERVATION_IDS.COMPLETED,
      vehicleId: TEST_VEHICLE_IDS.AUDI_A4,
      customerId: TEST_CUSTOMER_IDS.MAX_WEBER,
      pickupDate: getPastDate(60),
      returnDate: getPastDate(58),
      pickupLocationCode: TEST_LOCATION_CODES.FRANKFURT_CITY,
      dropoffLocationCode: TEST_LOCATION_CODES.FRANKFURT_CITY,
      rentalDays: 2,
      totalPriceNet: 252.1 as Price,
      totalPriceVat: 47.9 as Price,
      totalPriceGross: 300.0 as Price,
      currency: 'EUR',
      status: 'Completed' as ReservationStatus,
      createdAt: getPastDate(65),
    },
  ];

  beforeEach(async () => {
    const reservationServiceSpy = jasmine.createSpyObj('ReservationService', [
      'searchReservations',
      'confirmReservation',
      'cancelReservation',
    ]);

    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    mockActivatedRoute = {
      queryParams: of({}),
    };

    await TestBed.configureTestingModule({
      imports: [ReservationsComponent, TranslateModule.forRoot()],
      providers: [
        { provide: ReservationService, useValue: reservationServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    mockReservationService = TestBed.inject(
      ReservationService,
    ) as jasmine.SpyObj<ReservationService>;
    mockRouter = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    fixture = TestBed.createComponent(ReservationsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Initialization', () => {
    it('should load reservations on init', () => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 3,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );

      component.ngOnInit();

      expect(mockReservationService.searchReservations).toHaveBeenCalled();
      expect(component['reservations']().length).toBe(3);
      expect(component['totalCount']()).toBe(3);
    });

    it('should load filters from URL parameters', () => {
      mockActivatedRoute.queryParams = of({
        status: 'Confirmed',
        customerId: TEST_CUSTOMER_IDS.HANS_MUELLER as string,
        dateFrom: '2025-11-01',
        dateTo: '2025-11-30',
        location: TEST_LOCATION_CODES.MUNICH_AIRPORT as string,
        minPrice: '100',
        maxPrice: '500',
        sortBy: 'Price',
        sortOrder: 'asc',
        groupBy: 'status',
        page: '2',
        pageSize: '50',
      });

      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: [],
          totalCount: 0,
          pageNumber: 2,
          pageSize: 50,
          totalPages: 1,
        }),
      );

      component.ngOnInit();

      expect(component['searchStatus']()).toBe('Confirmed');
      expect(component['searchCustomerId']()).toBe(TEST_CUSTOMER_IDS.HANS_MUELLER);
      expect(component['searchPickupDateFrom']()).toBe('2025-11-01');
      expect(component['searchPickupDateTo']()).toBe('2025-11-30');
      expect(component['searchLocation']()).toBe(TEST_LOCATION_CODES.MUNICH_AIRPORT);
      expect(component['searchMinPrice']()).toBe(100);
      expect(component['searchMaxPrice']()).toBe(500);
      expect(component['sortBy']()).toBe('Price');
      expect(component['sortOrder']()).toBe('asc');
      expect(component['groupBy']()).toBe('status');
      expect(component['currentPage']()).toBe(2);
      expect(component['pageSize']()).toBe(50);
    });

    it('should handle loading error', () => {
      mockReservationService.searchReservations.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      component.ngOnInit();

      expect(component['error']()).toBe('errors.generic');
      expect(component['loading']()).toBe(false);
    });
  });

  describe('Filtering', () => {
    beforeEach(() => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 3,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );
    });

    it('should apply status filter', () => {
      component['searchStatus'].set('Confirmed');
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          status: 'Confirmed',
        }),
      );
    });

    it('should apply customer ID filter', () => {
      component['searchCustomerId'].set(TEST_CUSTOMER_IDS.HANS_MUELLER as string);
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          customerId: TEST_CUSTOMER_IDS.HANS_MUELLER,
        }),
      );
    });

    it('should apply date range filter', () => {
      component['searchPickupDateFrom'].set('2025-11-01');
      component['searchPickupDateTo'].set('2025-11-30');
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          pickupDateFrom: '2025-11-01',
          pickupDateTo: '2025-11-30',
        }),
      );
    });

    it('should apply location filter', () => {
      component['searchLocation'].set(TEST_LOCATION_CODES.MUNICH_AIRPORT as string);
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          locationCode: TEST_LOCATION_CODES.MUNICH_AIRPORT,
        }),
      );
    });

    it('should apply price range filter', () => {
      component['searchMinPrice'].set(100);
      component['searchMaxPrice'].set(500);
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          minPrice: 100,
          maxPrice: 500,
        }),
      );
    });

    it('should count active filters', () => {
      expect(component['activeFiltersCount']()).toBe(0);

      component['searchStatus'].set('Confirmed');
      expect(component['activeFiltersCount']()).toBe(1);

      component['searchCustomerId'].set(TEST_CUSTOMER_IDS.HANS_MUELLER as string);
      expect(component['activeFiltersCount']()).toBe(2);

      component['searchPickupDateFrom'].set('2025-11-01');
      expect(component['activeFiltersCount']()).toBe(3);
    });

    it('should clear all filters', () => {
      component['searchStatus'].set('Confirmed');
      component['searchCustomerId'].set(TEST_CUSTOMER_IDS.HANS_MUELLER as string);
      component['searchPickupDateFrom'].set('2025-11-01');
      component['searchLocation'].set(TEST_LOCATION_CODES.MUNICH_AIRPORT as string);
      component['sortBy'].set('Price');
      component['groupBy'].set('status');

      component['clearFilters']();

      expect(component['searchStatus']()).toBe('');
      expect(component['searchCustomerId']()).toBe('');
      expect(component['searchPickupDateFrom']()).toBe('');
      expect(component['searchLocation']()).toBe('');
      expect(component['sortBy']()).toBe('PickupDate');
      expect(component['sortOrder']()).toBe('desc');
      expect(component['groupBy']()).toBe('none');
      expect(component['currentPage']()).toBe(1);
    });

    it('should reset to page 1 when applying filters', () => {
      component['currentPage'].set(3);
      component['applyFilters']();

      expect(component['currentPage']()).toBe(1);
    });
  });

  describe('Sorting', () => {
    beforeEach(() => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 3,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );
    });

    it('should change sort field', () => {
      component['changeSortBy']('Price');

      expect(component['sortBy']()).toBe('Price');
      expect(component['sortOrder']()).toBe('desc');
    });

    it('should toggle sort order when clicking same field', () => {
      component['sortBy'].set('Price');
      component['sortOrder'].set('desc');

      component['changeSortBy']('Price');

      expect(component['sortBy']()).toBe('Price');
      expect(component['sortOrder']()).toBe('asc');

      component['changeSortBy']('Price');

      expect(component['sortOrder']()).toBe('desc');
    });

    it('should apply sorting to search query', () => {
      component['sortBy'].set('Price');
      component['sortOrder'].set('asc');
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          sortBy: 'Price',
          sortOrder: 'asc',
        }),
      );
    });
  });

  describe('Grouping', () => {
    beforeEach(() => {
      component['reservations'].set(mockReservations);
    });

    it('should not group when groupBy is none', () => {
      component['groupBy'].set('none');
      const grouped = component['groupedReservations']();

      expect(Object.keys(grouped).length).toBe(1);
      expect(grouped['all'].length).toBe(3);
    });

    it('should group by status', () => {
      component['groupBy'].set('status');
      const grouped = component['groupedReservations']();

      expect(grouped['Confirmed'].length).toBe(1);
      expect(grouped['Pending'].length).toBe(1);
      expect(grouped['Completed'].length).toBe(1);
    });

    it('should group by location', () => {
      component['groupBy'].set('location');
      const grouped = component['groupedReservations']();

      expect(grouped[TEST_LOCATION_CODES.MUNICH_AIRPORT].length).toBe(1);
      expect(grouped[TEST_LOCATION_CODES.BERLIN_HBF].length).toBe(1);
      expect(grouped[TEST_LOCATION_CODES.FRANKFURT_CITY].length).toBe(1);
    });

    it('should group by pickup date', () => {
      component['groupBy'].set('pickupDate');
      const grouped = component['groupedReservations']();

      expect(Object.keys(grouped).length).toBeGreaterThan(0);
    });

    it('should return group keys', () => {
      component['groupBy'].set('status');
      const keys = component['groupKeys']();

      expect(keys).toContain('Confirmed');
      expect(keys).toContain('Pending');
      expect(keys).toContain('Completed');
    });
  });

  describe('Pagination', () => {
    beforeEach(() => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 150,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 6,
        }),
      );
    });

    it('should calculate total pages', () => {
      component['totalCount'].set(150);
      component['pageSize'].set(25);

      expect(component['totalPages']()).toBe(6);
    });

    it('should go to specific page', () => {
      // First, initialize the totalCount so totalPages is calculated
      component['totalCount'].set(150);
      component['pageSize'].set(25);
      // Now goToPage will work since totalPages = 6
      component['goToPage'](3);

      expect(component['currentPage']()).toBe(3);
      expect(mockReservationService.searchReservations).toHaveBeenCalled();
    });

    it('should not go to page less than 1', () => {
      component['currentPage'].set(1);
      component['goToPage'](0);

      expect(component['currentPage']()).toBe(1);
    });

    it('should not go to page greater than total pages', () => {
      component['totalCount'].set(150);
      component['pageSize'].set(25);
      component['currentPage'].set(6);
      component['goToPage'](7);

      expect(component['currentPage']()).toBe(6);
    });

    it('should go to next page', () => {
      component['currentPage'].set(2);
      component['totalCount'].set(150);
      component['pageSize'].set(25);

      component['nextPage']();

      expect(component['currentPage']()).toBe(3);
    });

    it('should go to previous page', () => {
      // Initialize totalCount so totalPages check in goToPage passes
      component['totalCount'].set(150);
      component['pageSize'].set(25);
      component['currentPage'].set(3);
      component['previousPage']();

      expect(component['currentPage']()).toBe(2);
    });

    it('should change page size', () => {
      component['pageSize'].set(50);
      component['applyFilters']();

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          pageSize: 50,
        }),
      );
    });
  });

  describe('Statistics', () => {
    it("should calculate today's reservations", () => {
      const today = new Date().toISOString().split('T')[0] as ISODateString;
      const reservationsToday = [
        { ...mockReservations[0], createdAt: today },
        { ...mockReservations[1], createdAt: today },
      ];

      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: reservationsToday,
          totalCount: 2,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );

      component.ngOnInit();

      expect(component['todayReservations']()).toBe(2);
    });

    it('should calculate active reservations', () => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 3,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );

      component.ngOnInit();

      expect(component['activeReservations']()).toBe(1); // Only "Confirmed" status
    });

    it('should calculate pending reservations', () => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 3,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );

      component.ngOnInit();

      expect(component['pendingReservations']()).toBe(1); // Only "Pending" status
    });
  });

  describe('Reservation Actions', () => {
    beforeEach(() => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 3,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );
    });

    it('should check if reservation can be confirmed', () => {
      const pendingReservation = mockReservations[1];
      const confirmedReservation = mockReservations[0];

      expect(component['canConfirm'](pendingReservation)).toBe(true);
      expect(component['canConfirm'](confirmedReservation)).toBe(false);
    });

    it('should check if reservation can be cancelled', () => {
      const confirmedReservation = mockReservations[0];
      const pendingReservation = mockReservations[1];
      const completedReservation = mockReservations[2];

      expect(component['canCancel'](confirmedReservation)).toBe(true);
      expect(component['canCancel'](pendingReservation)).toBe(true);
      expect(component['canCancel'](completedReservation)).toBe(false);
    });

    it('should open confirmation modal when confirming reservation', () => {
      component['confirmReservation'](mockReservations[1]);

      expect(component['showConfirmModal']()).toBe(true);
      expect(component['selectedReservation']()).toEqual(mockReservations[1]);
    });

    it('should confirm reservation successfully via modal', () => {
      mockReservationService.confirmReservation.and.returnValue(of(void 0));

      component['selectedReservation'].set(mockReservations[1]);
      component['executeConfirmation']();

      expect(mockReservationService.confirmReservation).toHaveBeenCalledWith(
        mockReservations[1].reservationId,
      );
      expect(component['successMessage']()).toBe('common.messages.success');
      expect(component['showConfirmModal']()).toBe(false);
    });

    it('should close confirmation modal without confirming', () => {
      component['selectedReservation'].set(mockReservations[1]);
      component['showConfirmModal'].set(true);

      component['closeConfirmModal']();

      expect(component['showConfirmModal']()).toBe(false);
      expect(mockReservationService.confirmReservation).not.toHaveBeenCalled();
    });

    it('should handle confirm error', () => {
      mockReservationService.confirmReservation.and.returnValue(
        throwError(() => new Error('Confirmation failed')),
      );

      component['selectedReservation'].set(mockReservations[1]);
      component['executeConfirmation']();

      expect(component['error']()).toBe('errors.generic');
    });

    it('should open cancel dialog', () => {
      component['showCancelDialog'](mockReservations[0]);

      expect(component['selectedReservation']()).toEqual(mockReservations[0]);
      expect(component['showCancelModal']()).toBe(true);
      expect(component['cancelReason']()).toBe('');
    });

    it('should close cancel dialog', () => {
      component['selectedReservation'].set(mockReservations[0]);
      component['showCancelModal'].set(true);
      component['cancelReason'].set('Test');

      component['closeCancelModal']();

      expect(component['showCancelModal']()).toBe(false);
      expect(component['cancelReason']()).toBe('');
    });

    it('should cancel reservation successfully', () => {
      mockReservationService.cancelReservation.and.returnValue(of(void 0));

      component['selectedReservation'].set(mockReservations[0]);
      component['cancelReason'].set('Test reason');

      component['cancelReservation']();

      expect(mockReservationService.cancelReservation).toHaveBeenCalledWith(
        mockReservations[0].reservationId,
        'Test reason',
      );
      expect(component['successMessage']()).toBe('common.messages.success');
    });

    it('should not cancel without reason', () => {
      component['selectedReservation'].set(mockReservations[0]);
      component['cancelReason'].set('');

      component['cancelReservation']();

      expect(mockReservationService.cancelReservation).not.toHaveBeenCalled();
      expect(component['error']()).toBe('common.messages.required');
    });
  });

  describe('Details Modal', () => {
    it('should open details modal', () => {
      component['viewDetails'](mockReservations[0]);

      expect(component['selectedReservation']()).toEqual(mockReservations[0]);
      expect(component['showDetails']()).toBe(true);
    });

    it('should close details modal', () => {
      component['selectedReservation'].set(mockReservations[0]);
      component['showDetails'].set(true);

      component['closeDetails']();

      expect(component['showDetails']()).toBe(false);
      expect(component['selectedReservation']()).toBeNull();
    });
  });

  describe('Helper Methods', () => {
    it('should format date correctly', () => {
      const dateString = '2025-12-01';
      const formatted = component['formatDate'](dateString);

      expect(formatted).toMatch(/\d{2}\.\d{2}\.\d{4}/);
    });

    it('should format price correctly', () => {
      const price = 400.0;
      const formatted = component['formatPrice'](price);

      expect(formatted).toContain('400');
      expect(formatted).toContain('€');
    });

    it('should return correct status class', () => {
      expect(component['getStatusClass']('Confirmed')).toBe('status-success');
      expect(component['getStatusClass']('Pending')).toBe('status-warning');
      expect(component['getStatusClass']('Cancelled')).toBe('status-error');
      expect(component['getStatusClass']('Completed')).toBe('status-completed');
    });

    it('should return correct status label', () => {
      expect(component['getStatusLabel']('Confirmed')).toBe('Bestätigt');
      expect(component['getStatusLabel']('Pending')).toBe('Ausstehend');
      expect(component['getStatusLabel']('Active')).toBe('Aktiv');
      expect(component['getStatusLabel']('Completed')).toBe('Abgeschlossen');
      expect(component['getStatusLabel']('Cancelled')).toBe('Storniert');
    });
  });

  describe('URL Parameter Sync', () => {
    it('should update URL when filters change', () => {
      component['searchStatus'].set('Confirmed');
      component['searchCustomerId'].set(TEST_CUSTOMER_IDS.HANS_MUELLER as string);
      component['sortBy'].set('Price');

      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 0,
        }),
      );

      component['applyFilters']();

      expect(mockRouter.navigate).toHaveBeenCalled();
    });
  });
});
