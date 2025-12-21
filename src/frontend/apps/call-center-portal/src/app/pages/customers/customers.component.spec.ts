import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { TranslateModule } from '@ngx-translate/core';
import { CustomersComponent } from './customers.component';
import { CustomerService } from '../../services/customer.service';
import { ReservationService } from '../../services/reservation.service';
import { of, throwError } from 'rxjs';
import { createCustomerId } from '@orange-car-rental/reservation-api';
import type {
  CustomerId,
  ReservationId,
  ReservationStatus,
  LicenseNumber,
} from '@orange-car-rental/reservation-api';
import type { VehicleId } from '@orange-car-rental/vehicle-api';
import type { LocationCode, CityName, StreetAddress } from '@orange-car-rental/location-api';
import type { Customer, Reservation } from '../../types';
import { API_CONFIG } from '@orange-car-rental/shared';
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
import { UI_TIMING } from '../../constants/app.constants';

describe('CustomersComponent', () => {
  let component: CustomersComponent;
  let fixture: ComponentFixture<CustomersComponent>;
  let mockCustomerService: jasmine.SpyObj<CustomerService>;
  let mockReservationService: jasmine.SpyObj<ReservationService>;

  const mockCustomers: Customer[] = [
    {
      id: createCustomerId('11111111-1111-1111-1111-111111111111'),
      firstName: 'Hans' as FirstName,
      lastName: 'Müller' as LastName,
      email: 'hans.mueller@example.de' as EmailAddress,
      phoneNumber: '+49 89 12345678' as PhoneNumber,
      dateOfBirth: '1985-06-15' as ISODateString,
      street: 'Teststraße 1' as StreetAddress,
      city: 'München' as CityName,
      postalCode: '80331' as PostalCode,
      country: 'Germany' as CountryCode,
      licenseNumber: 'DE123456789' as LicenseNumber,
      licenseIssueCountry: 'Germany' as CountryCode,
      licenseIssueDate: '2010-01-01' as ISODateString,
      licenseExpiryDate: '2030-01-01' as ISODateString,
      createdAt: '2025-01-01' as ISODateString,
    },
    {
      id: createCustomerId('22222222-2222-2222-2222-222222222222'),
      firstName: 'Anna' as FirstName,
      lastName: 'Schmidt' as LastName,
      email: 'anna.schmidt@example.de' as EmailAddress,
      phoneNumber: '+49 30 87654321' as PhoneNumber,
      dateOfBirth: '1990-03-20' as ISODateString,
      street: 'Hauptstraße 42' as StreetAddress,
      city: 'Berlin' as CityName,
      postalCode: '10115' as PostalCode,
      country: 'Germany' as CountryCode,
      licenseNumber: 'DE987654321' as LicenseNumber,
      licenseIssueCountry: 'Germany' as CountryCode,
      licenseIssueDate: '2015-06-01' as ISODateString,
      licenseExpiryDate: '2035-06-01' as ISODateString,
      createdAt: '2025-02-15' as ISODateString,
    },
  ];

  const mockReservations: Reservation[] = [
    {
      reservationId: '123e4567-e89b-12d3-a456-426614174000' as ReservationId,
      vehicleId: 'veh-001' as VehicleId,
      customerId: '11111111-1111-1111-1111-111111111111' as CustomerId,
      pickupDate: '2025-12-01' as ISODateString,
      returnDate: '2025-12-05' as ISODateString,
      pickupLocationCode: 'MUC' as LocationCode,
      dropoffLocationCode: 'MUC' as LocationCode,
      rentalDays: 4,
      totalPriceNet: 336.13 as Price,
      totalPriceVat: 63.87 as Price,
      totalPriceGross: 400.0 as Price,
      currency: 'EUR' as Currency,
      status: 'Confirmed' as ReservationStatus,
      createdAt: '2025-11-20' as ISODateString,
    },
  ];

  beforeEach(async () => {
    const customerServiceSpy = jasmine.createSpyObj('CustomerService', [
      'searchCustomers',
      'updateCustomer',
    ]);
    const reservationServiceSpy = jasmine.createSpyObj('ReservationService', [
      'searchReservations',
    ]);

    await TestBed.configureTestingModule({
      imports: [CustomersComponent, TranslateModule.forRoot()],
      providers: [
        { provide: CustomerService, useValue: customerServiceSpy },
        { provide: ReservationService, useValue: reservationServiceSpy },
        { provide: API_CONFIG, useValue: { apiUrl: 'http://localhost:5000' } },
      ],
    }).compileComponents();

    mockCustomerService = TestBed.inject(CustomerService) as jasmine.SpyObj<CustomerService>;
    mockReservationService = TestBed.inject(
      ReservationService,
    ) as jasmine.SpyObj<ReservationService>;

    fixture = TestBed.createComponent(CustomersComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Search Functionality', () => {
    beforeEach(() => {
      mockCustomerService.searchCustomers.and.returnValue(
        of({
          customers: mockCustomers,
          totalCount: 2,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );
    });

    it('should search customers by email', () => {
      component['searchEmail'].set('hans.mueller@example.de');
      component['searchCustomers']();

      expect(mockCustomerService.searchCustomers).toHaveBeenCalledWith(
        jasmine.objectContaining({
          email: 'hans.mueller@example.de',
        }),
      );
      expect(component['customers']().length).toBe(2);
      expect(component['loading']()).toBe(false);
    });

    it('should search customers by phone', () => {
      component['searchPhone'].set('+49 89 12345678');
      component['searchCustomers']();

      expect(mockCustomerService.searchCustomers).toHaveBeenCalledWith(
        jasmine.objectContaining({
          phoneNumber: '+49 89 12345678',
        }),
      );
    });

    it('should search customers by last name', () => {
      component['searchLastName'].set('Müller');
      component['searchCustomers']();

      expect(mockCustomerService.searchCustomers).toHaveBeenCalledWith(
        jasmine.objectContaining({
          lastName: 'Müller',
        }),
      );
    });

    it('should search with multiple criteria', () => {
      component['searchEmail'].set('hans@example.de');
      component['searchPhone'].set('+49 89 12345678');
      component['searchLastName'].set('Müller');
      component['searchCustomers']();

      expect(mockCustomerService.searchCustomers).toHaveBeenCalledWith(
        jasmine.objectContaining({
          email: 'hans@example.de',
          phoneNumber: '+49 89 12345678',
          lastName: 'Müller',
        }),
      );
    });

    it('should require at least one search criterion', () => {
      component['searchEmail'].set('');
      component['searchPhone'].set('');
      component['searchLastName'].set('');
      component['searchCustomers']();

      expect(mockCustomerService.searchCustomers).not.toHaveBeenCalled();
      expect(component['error']()).toBe('customers.search.minCriteria');
    });

    it('should trim search input', () => {
      component['searchEmail'].set('  hans@example.de  ');
      component['searchCustomers']();

      expect(mockCustomerService.searchCustomers).toHaveBeenCalledWith(
        jasmine.objectContaining({
          email: 'hans@example.de',
        }),
      );
    });

    it('should handle search error', () => {
      mockCustomerService.searchCustomers.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      component['searchEmail'].set('test@example.de');
      component['searchCustomers']();

      expect(component['error']()).toBe('customers.error');
      expect(component['loading']()).toBe(false);
    });

    it('should set searchPerformed flag after search', () => {
      component['searchEmail'].set('test@example.de');
      component['searchCustomers']();

      expect(component['searchPerformed']()).toBe(true);
    });

    it('should update totalCustomers after search', () => {
      component['searchEmail'].set('test@example.de');
      component['searchCustomers']();

      expect(component['totalCustomers']()).toBe(2);
    });
  });

  describe('Clear Search', () => {
    it('should clear all search filters', () => {
      component['searchEmail'].set('test@example.de');
      component['searchPhone'].set('+49 89 12345678');
      component['searchLastName'].set('Müller');
      component['customers'].set(mockCustomers);
      component['totalCustomers'].set(2);
      component['searchPerformed'].set(true);
      component['error'].set('Some error');

      component['clearSearch']();

      expect(component['searchEmail']()).toBe('');
      expect(component['searchPhone']()).toBe('');
      expect(component['searchLastName']()).toBe('');
      expect(component['customers']().length).toBe(0);
      expect(component['totalCustomers']()).toBe(0);
      expect(component['searchPerformed']()).toBe(false);
      expect(component['error']()).toBeNull();
    });
  });

  describe('View Customer Details', () => {
    beforeEach(() => {
      mockReservationService.searchReservations.and.returnValue(
        of({
          reservations: mockReservations,
          totalCount: 1,
          pageNumber: 1,
          pageSize: 10,
          totalPages: 1,
        }),
      );
    });

    it('should open details modal for customer', () => {
      const customer = mockCustomers[0];
      component['viewDetails'](customer);

      expect(component['selectedCustomer']()).toEqual(customer);
      expect(component['showDetails']()).toBe(true);
    });

    it('should load customer reservations when viewing details', () => {
      const customer = mockCustomers[0];
      component['viewDetails'](customer);

      expect(mockReservationService.searchReservations).toHaveBeenCalledWith(
        jasmine.objectContaining({
          customerId: customer.id,
        }),
      );
      expect(component['customerReservations']().length).toBe(1);
    });

    it('should handle reservation loading error', () => {
      mockReservationService.searchReservations.and.returnValue(
        throwError(() => new Error('Network error')),
      );

      const customer = mockCustomers[0];
      component['viewDetails'](customer);

      expect(component['loadingReservations']()).toBe(false);
    });
  });

  describe('Close Details', () => {
    it('should close details modal and clear state', () => {
      component['selectedCustomer'].set(mockCustomers[0]);
      component['showDetails'].set(true);
      component['customerReservations'].set(mockReservations);
      component['editMode'].set(true);

      component['closeDetails']();

      expect(component['showDetails']()).toBe(false);
      expect(component['selectedCustomer']()).toBeNull();
      expect(component['customerReservations']().length).toBe(0);
      expect(component['editMode']()).toBe(false);
    });
  });

  describe('Edit Mode', () => {
    beforeEach(() => {
      component['selectedCustomer'].set(mockCustomers[0]);
    });

    it('should enter edit mode with customer data', () => {
      component['enterEditMode']();

      expect(component['editMode']()).toBe(true);
      expect(component['editForm']().firstName).toBe('Hans');
      expect(component['editForm']().lastName).toBe('Müller');
      expect(component['editForm']().email).toBe('hans.mueller@example.de');
      expect(component['editForm']().phoneNumber).toBe('+49 89 12345678');
    });

    it('should populate all form fields from customer', () => {
      component['enterEditMode']();

      const form = component['editForm']();
      expect(form.street).toBe('Teststraße 1');
      expect(form.city).toBe('München');
      expect(form.postalCode).toBe('80331');
      expect(form.country).toBe('Germany');
      expect(form.licenseNumber).toBe('DE123456789');
      expect(form.licenseIssueCountry).toBe('Germany');
    });

    it('should cancel edit mode', () => {
      component['editMode'].set(true);
      component['cancelEdit']();

      expect(component['editMode']()).toBe(false);
    });

    it('should update edit form field', () => {
      component['enterEditMode']();
      component['updateEditForm']('firstName', 'NewName');

      expect(component['editForm']().firstName).toBe('NewName');
    });

    it('should handle null values in customer data', () => {
      const customerWithNulls = {
        ...mockCustomers[0],
        street: null as unknown as StreetAddress,
        licenseNumber: null as unknown as LicenseNumber,
      };
      component['selectedCustomer'].set(customerWithNulls);
      component['enterEditMode']();

      expect(component['editForm']().street).toBe('');
      expect(component['editForm']().licenseNumber).toBe('');
    });
  });

  describe('Save Customer', () => {
    beforeEach(() => {
      component['selectedCustomer'].set(mockCustomers[0]);
      component['enterEditMode']();
      mockCustomerService.searchCustomers.and.returnValue(
        of({
          customers: mockCustomers,
          totalCount: 2,
          pageNumber: 1,
          pageSize: 25,
          totalPages: 1,
        }),
      );
    });

    it('should save customer successfully', fakeAsync(() => {
      const updatedCustomer = { ...mockCustomers[0], firstName: 'Updated' as FirstName };
      mockCustomerService.updateCustomer.and.returnValue(of(updatedCustomer));

      component['saveCustomer']();

      expect(mockCustomerService.updateCustomer).toHaveBeenCalledWith(
        mockCustomers[0].id,
        jasmine.any(Object),
      );
      expect(component['selectedCustomer']()).toEqual(updatedCustomer);
      expect(component['editMode']()).toBe(false);
      expect(component['saving']()).toBe(false);
      expect(component['successMessage']()).toBe('customers.edit.success');

      tick(UI_TIMING.SUCCESS_MESSAGE_SHORT);
      expect(component['successMessage']()).toBeNull();
    }));

    it('should handle save error', () => {
      mockCustomerService.updateCustomer.and.returnValue(
        throwError(() => new Error('Save failed')),
      );

      component['saveCustomer']();

      expect(component['error']()).toBe('customers.edit.error');
      expect(component['saving']()).toBe(false);
    });

    it('should not save if no customer selected', () => {
      component['selectedCustomer'].set(null);
      component['saveCustomer']();

      expect(mockCustomerService.updateCustomer).not.toHaveBeenCalled();
    });

    it('should refresh customer list after save', () => {
      const updatedCustomer = { ...mockCustomers[0], firstName: 'Updated' };
      mockCustomerService.updateCustomer.and.returnValue(of(updatedCustomer));
      component['searchEmail'].set('test@example.de');

      component['saveCustomer']();

      expect(mockCustomerService.searchCustomers).toHaveBeenCalled();
    });
  });

  describe('Helper Methods', () => {
    it('should format date correctly', () => {
      const dateString = '2025-12-01';
      const formatted = component['formatDate'](dateString);

      expect(formatted).toMatch(/\d{2}\.\d{2}\.\d{4}/);
    });

    it('should calculate age correctly', () => {
      const birthDate = '1985-06-15';
      const age = component['calculateAge'](birthDate);

      expect(age).toBeGreaterThanOrEqual(39);
      expect(age).toBeLessThanOrEqual(40);
    });

    it('should return correct status class for Confirmed', () => {
      expect(component['getStatusClass']('Confirmed')).toBe('status-success');
    });

    it('should return correct status class for Pending', () => {
      expect(component['getStatusClass']('Pending')).toBe('status-warning');
    });

    it('should return correct status class for Cancelled', () => {
      expect(component['getStatusClass']('Cancelled')).toBe('status-error');
    });
  });
});
