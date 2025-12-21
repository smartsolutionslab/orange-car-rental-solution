import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { of, throwError, Observable } from 'rxjs';
import { ConfirmationComponent } from './confirmation.component';
import { ReservationService } from '../../services/reservation.service';
import type {
  Reservation,
  CustomerId,
  ReservationId,
  ReservationStatus,
} from '@orange-car-rental/reservation-api';
import type { VehicleId } from '@orange-car-rental/vehicle-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import { API_CONFIG } from '@orange-car-rental/shared';
import type { Price, Currency, ISODateString } from '@orange-car-rental/shared';

describe('ConfirmationComponent', () => {
  let component: ConfirmationComponent;
  let fixture: ComponentFixture<ConfirmationComponent>;
  let reservationService: jasmine.SpyObj<ReservationService>;
  let router: jasmine.SpyObj<Router>;
  let activatedRoute: { queryParams: Observable<Record<string, string>> };

  const mockReservation: Reservation = {
    id: '987e6543-e89b-12d3-a456-426614174000' as ReservationId,
    vehicleId: '123e4567-e89b-12d3-a456-426614174000' as VehicleId,
    customerId: '111e2222-e89b-12d3-a456-426614174000' as CustomerId,
    pickupDate: '2024-01-15T00:00:00Z' as ISODateString,
    returnDate: '2024-01-20T00:00:00Z' as ISODateString,
    pickupLocationCode: 'BER-HBF' as LocationCode,
    dropoffLocationCode: 'BER-HBF' as LocationCode,
    totalPriceNet: 250.0 as Price,
    totalPriceVat: 47.5 as Price,
    totalPriceGross: 297.5 as Price,
    currency: 'EUR' as Currency,
    status: 'Pending' as ReservationStatus,
  };

  beforeEach(async () => {
    const reservationServiceSpy = jasmine.createSpyObj('ReservationService', ['getReservation']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    activatedRoute = {
      queryParams: of({}),
    };

    await TestBed.configureTestingModule({
      imports: [ConfirmationComponent, TranslateModule.forRoot()],
      providers: [
        { provide: ReservationService, useValue: reservationServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRoute },
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    reservationService = TestBed.inject(ReservationService) as jasmine.SpyObj<ReservationService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    fixture = TestBed.createComponent(ConfirmationComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load reservation when reservationId is in query params', () => {
    reservationService.getReservation.and.returnValue(of(mockReservation));
    activatedRoute.queryParams = of({
      reservationId: '987e6543-e89b-12d3-a456-426614174000',
      customerId: '111e2222-e89b-12d3-a456-426614174000',
    });

    fixture.detectChanges();

    expect(reservationService.getReservation).toHaveBeenCalledWith(
      '987e6543-e89b-12d3-a456-426614174000' as ReservationId,
    );
    expect(component['reservation']()).toEqual(mockReservation);
    expect(component['loading']()).toBeFalse();
  });

  it('should show error when reservationId is missing', () => {
    activatedRoute.queryParams = of({});

    fixture.detectChanges();

    expect(component['error']()).toBe('errors.notFound');
    expect(component['loading']()).toBeFalse();
    expect(reservationService.getReservation).not.toHaveBeenCalled();
  });

  it('should handle reservation loading error', () => {
    const error = new Error('Not found');
    reservationService.getReservation.and.returnValue(throwError(() => error));
    activatedRoute.queryParams = of({ reservationId: '987e6543-e89b-12d3-a456-426614174000' });

    fixture.detectChanges();

    expect(component['reservation']()).toBeNull();
    expect(component['loading']()).toBeFalse();
    expect(component['error']()).toBe('confirmation.error');
  });

  it('should navigate to home page', () => {
    component['goToHome']();
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should call window.print on printConfirmation', () => {
    spyOn(window, 'print');
    component['printConfirmation']();
    expect(window.print).toHaveBeenCalled();
  });

  it('should calculate rental days correctly', () => {
    component['reservation'].set(mockReservation);
    const days = component['getRentalDays']();
    expect(days).toBe(5);
  });

  it('should return 0 rental days when reservation is null', () => {
    component['reservation'].set(null);
    const days = component['getRentalDays']();
    expect(days).toBe(0);
  });

  it('should format date correctly', () => {
    const formatted = component['formatDate']('2024-01-15T00:00:00Z');
    expect(typeof formatted).toBe('string');
    expect(formatted.length).toBeGreaterThan(0);
  });
});
