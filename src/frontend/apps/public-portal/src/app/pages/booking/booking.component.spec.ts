import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { BookingComponent } from './booking.component';
import { LocationService } from '../../services/location.service';
import { VehicleService } from '../../services/vehicle.service';
import { ReservationService } from '../../services/reservation.service';
import { Location } from '../../services/location.model';
import { Vehicle } from '../../services/vehicle.model';
import { GuestReservationResponse } from '../../services/reservation.model';

describe('BookingComponent', () => {
  let component: BookingComponent;
  let fixture: ComponentFixture<BookingComponent>;
  let locationService: jasmine.SpyObj<LocationService>;
  let vehicleService: jasmine.SpyObj<VehicleService>;
  let reservationService: jasmine.SpyObj<ReservationService>;
  let router: jasmine.SpyObj<Router>;
  let activatedRoute: any;

  const mockLocations: Location[] = [
    { code: 'BER-HBF', name: 'Berlin Hauptbahnhof', street: 'Europaplatz 1', city: 'Berlin', postalCode: '10557', fullAddress: 'Europaplatz 1, 10557 Berlin' },
    { code: 'MUC-FLG', name: 'Munich Airport', street: 'Nordallee 25', city: 'Munich', postalCode: '85356', fullAddress: 'Nordallee 25, 85356 Munich' }
  ];

  const mockVehicle: Vehicle = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    name: 'VW Golf',
    categoryCode: 'MITTEL',
    categoryName: 'Mittelklasse',
    locationCode: 'BER-HBF',
    city: 'Berlin',
    dailyRateNet: 50.00,
    dailyRateVat: 9.50,
    dailyRateGross: 59.50,
    currency: 'EUR',
    seats: 5,
    fuelType: 'Petrol',
    transmissionType: 'Manual',
    status: 'Available',
    licensePlate: 'B-AB 1234',
    manufacturer: 'Volkswagen',
    model: 'Golf 8',
    year: 2023,
    imageUrl: null
  };

  const mockReservationResponse: GuestReservationResponse = {
    reservationId: '987e6543-e89b-12d3-a456-426614174000',
    customerId: '111e2222-e89b-12d3-a456-426614174000',
    totalPriceNet: 250.00,
    totalPriceVat: 47.50,
    totalPriceGross: 297.50,
    currency: 'EUR'
  };

  beforeEach(async () => {
    const locationServiceSpy = jasmine.createSpyObj('LocationService', ['getAllLocations']);
    const vehicleServiceSpy = jasmine.createSpyObj('VehicleService', ['getVehicleById']);
    const reservationServiceSpy = jasmine.createSpyObj('ReservationService', ['createGuestReservation']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    activatedRoute = {
      queryParams: of({})
    };

    await TestBed.configureTestingModule({
      imports: [BookingComponent, ReactiveFormsModule],
      providers: [
        { provide: LocationService, useValue: locationServiceSpy },
        { provide: VehicleService, useValue: vehicleServiceSpy },
        { provide: ReservationService, useValue: reservationServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRoute }
      ]
    }).compileComponents();

    locationService = TestBed.inject(LocationService) as jasmine.SpyObj<LocationService>;
    vehicleService = TestBed.inject(VehicleService) as jasmine.SpyObj<VehicleService>;
    reservationService = TestBed.inject(ReservationService) as jasmine.SpyObj<ReservationService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    locationService.getAllLocations.and.returnValue(of(mockLocations));

    fixture = TestBed.createComponent(BookingComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with all required fields', () => {
    expect(component['bookingForm']).toBeDefined();
    expect(component['bookingForm'].get('vehicleId')).toBeDefined();
    expect(component['bookingForm'].get('categoryCode')).toBeDefined();
    expect(component['bookingForm'].get('firstName')).toBeDefined();
    expect(component['bookingForm'].get('email')).toBeDefined();
    expect(component['bookingForm'].get('street')).toBeDefined();
    expect(component['bookingForm'].get('licenseNumber')).toBeDefined();
  });

  it('should start at step 1', () => {
    expect(component['currentStep']()).toBe(1);
  });

  it('should load locations on init', () => {
    fixture.detectChanges();
    expect(locationService.getAllLocations).toHaveBeenCalled();
    expect(component['locations']().length).toBe(2);
  });

  it('should load vehicle when vehicleId is in query params', () => {
    vehicleService.getVehicleById.and.returnValue(of(mockVehicle));
    activatedRoute.queryParams = of({ vehicleId: '123e4567-e89b-12d3-a456-426614174000' });

    fixture.detectChanges();

    expect(vehicleService.getVehicleById).toHaveBeenCalledWith('123e4567-e89b-12d3-a456-426614174000');
  });

  it('should populate form fields from query params', () => {
    vehicleService.getVehicleById.and.returnValue(of(mockVehicle));
    activatedRoute.queryParams = of({
      vehicleId: '123e4567-e89b-12d3-a456-426614174000',
      pickupDate: '2024-01-15',
      returnDate: '2024-01-20',
      locationCode: 'BER-HBF'
    });

    fixture.detectChanges();

    expect(component['bookingForm'].get('vehicleId')?.value).toBe('123e4567-e89b-12d3-a456-426614174000');
    expect(component['bookingForm'].get('pickupDate')?.value).toBe('2024-01-15');
    expect(component['bookingForm'].get('returnDate')?.value).toBe('2024-01-20');
  });

  describe('Form Validation', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should require vehicleId', () => {
      const control = component['bookingForm'].get('vehicleId');
      control?.setValue('');
      expect(control?.valid).toBeFalse();
      control?.setValue('123e4567-e89b-12d3-a456-426614174000');
      expect(control?.valid).toBeTrue();
    });

    it('should validate email format', () => {
      const control = component['bookingForm'].get('email');
      control?.setValue('invalid-email');
      expect(control?.valid).toBeFalse();
      control?.setValue('test@example.com');
      expect(control?.valid).toBeTrue();
    });

    it('should validate phone number pattern', () => {
      const control = component['bookingForm'].get('phoneNumber');
      control?.setValue('abc');
      expect(control?.valid).toBeFalse();
      control?.setValue('+49 30 12345678');
      expect(control?.valid).toBeTrue();
    });

    it('should validate postal code pattern (5 digits)', () => {
      const control = component['bookingForm'].get('postalCode');
      control?.setValue('123');
      expect(control?.valid).toBeFalse();
      control?.setValue('12345');
      expect(control?.valid).toBeTrue();
    });
  });

  describe('Multi-step Navigation', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should move to next step when current step is valid', () => {
      component['bookingForm'].patchValue({
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF'
      });

      component['nextStep']();
      expect(component['currentStep']()).toBe(2);
    });

    it('should not move to next step when current step is invalid', () => {
      component['bookingForm'].patchValue({
        vehicleId: '',
        categoryCode: 'MITTEL'
      });

      component['nextStep']();
      expect(component['currentStep']()).toBe(1);
    });

    it('should move to previous step', () => {
      component['currentStep'].set(3);
      component['previousStep']();
      expect(component['currentStep']()).toBe(2);
    });
  });

  describe('Form Submission', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should submit valid reservation with nested structure', () => {
      reservationService.createGuestReservation.and.returnValue(of(mockReservationResponse));

      component['bookingForm'].patchValue({
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Max',
        lastName: 'Mustermann',
        email: 'max@example.com',
        phoneNumber: '+49 30 12345678',
        dateOfBirth: '1990-01-15',
        street: 'Musterstraße 123',
        city: 'Berlin',
        postalCode: '10115',
        country: 'Deutschland',
        licenseNumber: 'B123456789',
        licenseIssueCountry: 'Deutschland',
        licenseIssueDate: '2010-06-01',
        licenseExpiryDate: '2030-06-01'
      });

      component['onSubmit']();

      expect(reservationService.createGuestReservation).toHaveBeenCalled();
      const request = reservationService.createGuestReservation.calls.mostRecent().args[0];
      expect(request.reservation).toBeDefined();
      expect(request.customer).toBeDefined();
      expect(request.address).toBeDefined();
      expect(request.driversLicense).toBeDefined();
    });

    it('should navigate to confirmation page on success', () => {
      reservationService.createGuestReservation.and.returnValue(of(mockReservationResponse));

      component['bookingForm'].patchValue({
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Max',
        lastName: 'Mustermann',
        email: 'max@example.com',
        phoneNumber: '+49 30 12345678',
        dateOfBirth: '1990-01-15',
        street: 'Musterstraße 123',
        city: 'Berlin',
        postalCode: '10115',
        country: 'Deutschland',
        licenseNumber: 'B123456789',
        licenseIssueCountry: 'Deutschland',
        licenseIssueDate: '2010-06-01',
        licenseExpiryDate: '2030-06-01'
      });

      component['onSubmit']();

      expect(router.navigate).toHaveBeenCalledWith(['/confirmation'], {
        queryParams: {
          reservationId: mockReservationResponse.reservationId,
          customerId: mockReservationResponse.customerId
        }
      });
    });

    it('should handle submission error', () => {
      const errorResponse = { status: 400, error: { message: 'Validation error' } };
      reservationService.createGuestReservation.and.returnValue(throwError(() => errorResponse));

      component['bookingForm'].patchValue({
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Max',
        lastName: 'Mustermann',
        email: 'max@example.com',
        phoneNumber: '+49 30 12345678',
        dateOfBirth: '1990-01-15',
        street: 'Musterstraße 123',
        city: 'Berlin',
        postalCode: '10115',
        country: 'Deutschland',
        licenseNumber: 'B123456789',
        licenseIssueCountry: 'Deutschland',
        licenseIssueDate: '2010-06-01',
        licenseExpiryDate: '2030-06-01'
      });

      component['onSubmit']();

      expect(component['submitting']()).toBeFalse();
      expect(component['error']()).toBeTruthy();
    });
  });

  describe('Rental Days Calculation', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should calculate rental days correctly', () => {
      component['bookingForm'].patchValue({
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20'
      });

      expect(component['getRentalDays']()).toBe(5);
    });

    it('should return 0 when dates are missing', () => {
      component['bookingForm'].patchValue({
        pickupDate: '',
        returnDate: ''
      });

      expect(component['getRentalDays']()).toBe(0);
    });
  });
});
