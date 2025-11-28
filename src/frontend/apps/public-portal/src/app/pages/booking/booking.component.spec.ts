import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { of, throwError, Observable } from 'rxjs';
import { BookingComponent } from './booking.component';
import { LocationService } from '../../services/location.service';
import { VehicleService } from '../../services/vehicle.service';
import { ReservationService } from '../../services/reservation.service';
import { AuthService } from '../../services/auth.service';
import { CustomerService } from '../../services/customer.service';
import { Location } from '../../services/location.model';
import { Vehicle } from '../../services/vehicle.model';
import { GuestReservationResponse } from '../../services/reservation.model';
import { CustomerProfile } from '../../services/customer.model';

describe('BookingComponent', () => {
  let component: BookingComponent;
  let fixture: ComponentFixture<BookingComponent>;
  let locationService: jasmine.SpyObj<LocationService>;
  let vehicleService: jasmine.SpyObj<VehicleService>;
  let reservationService: jasmine.SpyObj<ReservationService>;
  let authService: jasmine.SpyObj<AuthService>;
  let customerService: jasmine.SpyObj<CustomerService>;
  let router: jasmine.SpyObj<Router>;
  let activatedRoute: { queryParams: Observable<Record<string, string>> };

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

  const mockCustomerProfile: CustomerProfile = {
    id: '111e2222-e89b-12d3-a456-426614174000',
    firstName: 'Max',
    lastName: 'Mustermann',
    email: 'max.mustermann@example.com',
    phoneNumber: '+49 123 456789',
    dateOfBirth: '1990-01-15',
    address: {
      street: 'Musterstraße 123',
      city: 'Berlin',
      postalCode: '10115',
      country: 'Deutschland'
    },
    driversLicense: {
      licenseNumber: 'B12345678',
      licenseIssueCountry: 'Deutschland',
      licenseIssueDate: '2015-03-01',
      licenseExpiryDate: '2035-03-01'
    }
  };

  beforeEach(async () => {
    const locationServiceSpy = jasmine.createSpyObj('LocationService', ['getAllLocations']);
    const vehicleServiceSpy = jasmine.createSpyObj('VehicleService', ['getVehicleById']);
    const reservationServiceSpy = jasmine.createSpyObj('ReservationService', ['createGuestReservation']);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated']);
    const customerServiceSpy = jasmine.createSpyObj('CustomerService', ['getMyProfile', 'updateMyProfile']);
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
        { provide: AuthService, useValue: authServiceSpy },
        { provide: CustomerService, useValue: customerServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRoute }
      ]
    }).compileComponents();

    locationService = TestBed.inject(LocationService) as jasmine.SpyObj<LocationService>;
    vehicleService = TestBed.inject(VehicleService) as jasmine.SpyObj<VehicleService>;
    reservationService = TestBed.inject(ReservationService) as jasmine.SpyObj<ReservationService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    customerService = TestBed.inject(CustomerService) as jasmine.SpyObj<CustomerService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    locationService.getAllLocations.and.returnValue(of(mockLocations));
    authService.isAuthenticated.and.returnValue(false); // Default to not authenticated

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

  describe('Profile Pre-fill for Authenticated Users (US-5)', () => {
    beforeEach(() => {
      // Reset auth service to authenticated for these tests
      authService.isAuthenticated.and.returnValue(true);
      customerService.getMyProfile.and.returnValue(of(mockCustomerProfile));
    });

    it('should check authentication status on init', () => {
      fixture.detectChanges();
      expect(authService.isAuthenticated).toHaveBeenCalled();
      expect(component['isAuthenticated']()).toBeTruthy();
    });

    it('should load customer profile when authenticated', () => {
      fixture.detectChanges();
      expect(customerService.getMyProfile).toHaveBeenCalled();
      expect(component['customerProfile']()).toEqual(mockCustomerProfile);
    });

    it('should not load profile when not authenticated', () => {
      authService.isAuthenticated.and.returnValue(false);
      fixture.detectChanges();
      expect(customerService.getMyProfile).not.toHaveBeenCalled();
    });

    it('should pre-fill customer information fields', () => {
      fixture.detectChanges();

      expect(component['bookingForm'].get('firstName')?.value).toBe('Max');
      expect(component['bookingForm'].get('lastName')?.value).toBe('Mustermann');
      expect(component['bookingForm'].get('email')?.value).toBe('max.mustermann@example.com');
      expect(component['bookingForm'].get('phoneNumber')?.value).toBe('+49 123 456789');
      expect(component['bookingForm'].get('dateOfBirth')?.value).toBe('1990-01-15');
    });

    it('should pre-fill address fields', () => {
      fixture.detectChanges();

      expect(component['bookingForm'].get('street')?.value).toBe('Musterstraße 123');
      expect(component['bookingForm'].get('city')?.value).toBe('Berlin');
      expect(component['bookingForm'].get('postalCode')?.value).toBe('10115');
      expect(component['bookingForm'].get('country')?.value).toBe('Deutschland');
    });

    it('should pre-fill driver\'s license fields when available', () => {
      fixture.detectChanges();

      expect(component['bookingForm'].get('licenseNumber')?.value).toBe('B12345678');
      expect(component['bookingForm'].get('licenseIssueCountry')?.value).toBe('Deutschland');
      expect(component['bookingForm'].get('licenseIssueDate')?.value).toBe('2015-03-01');
      expect(component['bookingForm'].get('licenseExpiryDate')?.value).toBe('2035-03-01');
    });

    it('should not pre-fill license fields when license is not available', () => {
      const profileWithoutLicense = { ...mockCustomerProfile, driversLicense: undefined };
      customerService.getMyProfile.and.returnValue(of(profileWithoutLicense));

      fixture.detectChanges();

      expect(component['bookingForm'].get('licenseNumber')?.value).toBe('');
    });

    it('should allow user to edit pre-filled fields', () => {
      fixture.detectChanges();

      component['bookingForm'].patchValue({
        firstName: 'Updated',
        email: 'new@example.com'
      });

      expect(component['bookingForm'].get('firstName')?.value).toBe('Updated');
      expect(component['bookingForm'].get('email')?.value).toBe('new@example.com');
    });

    it('should handle profile loading errors gracefully', () => {
      customerService.getMyProfile.and.returnValue(throwError(() => new Error('Profile not found')));

      fixture.detectChanges();

      // Should not crash and form should remain editable
      expect(component['customerProfile']()).toBeNull();
      expect(component['bookingForm'].get('firstName')?.value).toBe('');
    });

    it('should set loading state while fetching profile', () => {
      fixture.detectChanges();
      // Profile loading should be false after observable completes
      expect(component['profileLoading']()).toBeFalsy();
    });
  });

  describe('Update My Profile Checkbox (US-5)', () => {
    beforeEach(() => {
      authService.isAuthenticated.and.returnValue(true);
      customerService.getMyProfile.and.returnValue(of(mockCustomerProfile));
      customerService.updateMyProfile.and.returnValue(of(mockCustomerProfile));
      reservationService.createGuestReservation.and.returnValue(of(mockReservationResponse));
      router.navigate.and.returnValue(Promise.resolve(true));
      fixture.detectChanges();
    });

    it('should have updateMyProfile checkbox in form', () => {
      expect(component['bookingForm'].get('updateMyProfile')).toBeDefined();
    });

    it('should default updateMyProfile to false', () => {
      expect(component['bookingForm'].get('updateMyProfile')?.value).toBeFalsy();
    });

    it('should update profile after booking when checkbox is checked', () => {
      // Set checkbox to true
      component['bookingForm'].patchValue({
        updateMyProfile: true,
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Updated',
        lastName: 'Name',
        email: 'updated@example.com',
        phoneNumber: '+49 999 999999',
        dateOfBirth: '1985-05-20',
        street: 'New Street 456',
        city: 'Munich',
        postalCode: '80331',
        country: 'Deutschland',
        licenseNumber: 'N87654321',
        licenseIssueCountry: 'Deutschland',
        licenseIssueDate: '2020-01-01',
        licenseExpiryDate: '2040-01-01'
      });

      component['onSubmit']();

      expect(customerService.updateMyProfile).toHaveBeenCalledWith(jasmine.objectContaining({
        firstName: 'Updated',
        lastName: 'Name',
        email: 'updated@example.com'
      }));
    });

    it('should not update profile when checkbox is unchecked', () => {
      component['bookingForm'].patchValue({
        updateMyProfile: false,
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Test',
        lastName: 'User'
      });

      component['onSubmit']();

      expect(customerService.updateMyProfile).not.toHaveBeenCalled();
    });

    it('should not update profile for guest users', () => {
      authService.isAuthenticated.and.returnValue(false);
      component['isAuthenticated'].set(false);

      component['bookingForm'].patchValue({
        updateMyProfile: true
      });

      component['onSubmit']();

      expect(customerService.updateMyProfile).not.toHaveBeenCalled();
    });

    it('should still complete booking even if profile update fails', () => {
      customerService.updateMyProfile.and.returnValue(throwError(() => new Error('Update failed')));

      component['bookingForm'].patchValue({
        updateMyProfile: true,
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Test',
        lastName: 'User',
        email: 'test@example.com',
        phoneNumber: '+49 123 456789',
        dateOfBirth: '1990-01-01',
        street: 'Test St',
        city: 'Berlin',
        postalCode: '10115',
        country: 'Deutschland',
        licenseNumber: 'TEST123',
        licenseIssueCountry: 'Deutschland',
        licenseIssueDate: '2020-01-01',
        licenseExpiryDate: '2040-01-01'
      });

      component['onSubmit']();

      // Should still navigate to confirmation even if profile update fails
      expect(router.navigate).toHaveBeenCalledWith(['/confirmation'], jasmine.any(Object));
    });

    it('should update customerProfile signal after successful profile update', fakeAsync(() => {
      const updatedProfile = { ...mockCustomerProfile, firstName: 'Updated' };
      customerService.updateMyProfile.and.returnValue(of(updatedProfile));

      component['bookingForm'].patchValue({
        updateMyProfile: true,
        vehicleId: '123e4567-e89b-12d3-a456-426614174000',
        categoryCode: 'MITTEL',
        pickupDate: '2024-01-15',
        returnDate: '2024-01-20',
        pickupLocationCode: 'BER-HBF',
        dropoffLocationCode: 'BER-HBF',
        firstName: 'Updated'
      });

      component['onSubmit']();
      tick(100);

      // After booking completes, profile should be updated
      expect(component['customerProfile']()?.firstName).toBe('Updated');
    }));
  });
});
