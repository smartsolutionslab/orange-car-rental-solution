import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { createCustomerId } from '@orange-car-rental/reservation-api';
import { logError } from '@orange-car-rental/util';
import {
  formatDateDE,
  getReservationStatusClass,
  calculateAge,
  ModalComponent,
  LoadingStateComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  StatusBadgeComponent,
  SuccessAlertComponent,
  IconComponent,
} from '@orange-car-rental/ui-components';
import { CustomerService } from '../../services/customer.service';
import { ReservationService } from '../../services/reservation.service';
import { DEFAULT_PAGE_SIZE, UI_TIMING } from '../../constants/app.constants';
import type { Customer, UpdateCustomerRequest, CustomerFormValues, Reservation } from '../../types';
import type {
  EmailAddress,
  PhoneNumber,
  ISODateString,
  PostalCode,
  CountryCode,
} from '@orange-car-rental/shared';
import type { CityName } from '@orange-car-rental/location-api';
import type { LicenseNumber } from '@orange-car-rental/reservation-api';

/**
 * Customers management page for call center
 * Search and view customer information
 */
@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    ModalComponent,
    LoadingStateComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    StatusBadgeComponent,
    SuccessAlertComponent,
    IconComponent,
  ],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css',
})
export class CustomersComponent {
  private readonly customerService = inject(CustomerService);
  private readonly reservationService = inject(ReservationService);
  private readonly translate = inject(TranslateService);

  protected readonly customers = signal<Customer[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedCustomer = signal<Customer | null>(null);
  protected readonly customerReservations = signal<Reservation[]>([]);
  protected readonly showDetails = signal(false);
  protected readonly loadingReservations = signal(false);
  protected readonly editMode = signal(false);
  protected readonly saving = signal(false);
  protected readonly successMessage = signal<string | null>(null);

  // Edit form data - uses plain strings for form binding
  protected readonly editForm = signal<CustomerFormValues>({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    dateOfBirth: '',
    street: '',
    city: '',
    postalCode: '',
    country: '',
    licenseNumber: '',
    licenseIssueCountry: '',
    licenseIssueDate: '',
    licenseExpiryDate: '',
  });

  // Search filters
  protected readonly searchEmail = signal<string>('');
  protected readonly searchPhone = signal<string>('');
  protected readonly searchLastName = signal<string>('');

  // Stats
  protected readonly totalCustomers = signal(0);
  protected readonly searchPerformed = signal(false);

  /**
   * Search customers with filters
   */
  protected searchCustomers(): void {
    const email = this.searchEmail().trim();
    const phone = this.searchPhone().trim();
    const lastName = this.searchLastName().trim();

    if (!email && !phone && !lastName) {
      this.error.set(this.translate.instant('customers.search.minCriteria'));
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.searchPerformed.set(true);

    const query = {
      ...(email && { email: email as EmailAddress }),
      ...(phone && { phoneNumber: phone as PhoneNumber }),
      ...(lastName && { lastName }),
      pageSize: DEFAULT_PAGE_SIZE.CUSTOMERS,
    };

    this.customerService.searchCustomers(query).subscribe({
      next: (result) => {
        this.customers.set(result.customers);
        this.totalCustomers.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        logError('CustomersComponent', 'Error searching customers', err);
        this.error.set(this.translate.instant('customers.error'));
        this.loading.set(false);
      },
    });
  }

  /**
   * Clear search filters
   */
  protected clearSearch(): void {
    this.searchEmail.set('');
    this.searchPhone.set('');
    this.searchLastName.set('');
    this.customers.set([]);
    this.totalCustomers.set(0);
    this.searchPerformed.set(false);
    this.error.set(null);
  }

  /**
   * View customer details
   */
  protected viewDetails(customer: Customer): void {
    this.selectedCustomer.set(customer);
    this.showDetails.set(true);
    this.loadCustomerReservations(customer.id);
  }

  /**
   * Close details view
   */
  protected closeDetails(): void {
    this.showDetails.set(false);
    this.selectedCustomer.set(null);
    this.customerReservations.set([]);
    this.editMode.set(false);
  }

  /**
   * Load customer reservations
   */
  private loadCustomerReservations(customerId: string): void {
    this.loadingReservations.set(true);

    this.reservationService
      .searchReservations({
        customerId: createCustomerId(customerId),
        pageSize: DEFAULT_PAGE_SIZE.CUSTOMER_RESERVATIONS,
      })
      .subscribe({
        next: (result) => {
          this.customerReservations.set(result.reservations);
          this.loadingReservations.set(false);
        },
        error: (err) => {
          logError('CustomersComponent', 'Error loading customer reservations', err);
          this.loadingReservations.set(false);
        },
      });
  }

  // Helpers - using shared utilities
  protected formatDate = formatDateDE;
  protected getStatusClass = getReservationStatusClass;
  protected calculateAge = calculateAge;

  /**
   * Update edit form field
   */
  protected updateEditForm(field: keyof CustomerFormValues, value: string): void {
    this.editForm.update((form) => ({
      ...form,
      [field]: value,
    }));
  }

  /**
   * Enter edit mode
   */
  protected enterEditMode(): void {
    const customer = this.selectedCustomer();
    if (customer) {
      this.editForm.set({
        firstName: customer.firstName ?? '',
        lastName: customer.lastName ?? '',
        email: customer.email ?? '',
        phoneNumber: customer.phoneNumber ?? '',
        dateOfBirth: customer.dateOfBirth ?? '',
        street: customer.street ?? '',
        city: customer.city ?? '',
        postalCode: customer.postalCode ?? '',
        country: customer.country ?? '',
        licenseNumber: customer.licenseNumber ?? '',
        licenseIssueCountry: customer.licenseIssueCountry ?? '',
        licenseIssueDate: customer.licenseIssueDate ?? '',
        licenseExpiryDate: customer.licenseExpiryDate ?? '',
      });
      this.editMode.set(true);
    }
  }

  /**
   * Cancel edit mode
   */
  protected cancelEdit(): void {
    this.editMode.set(false);
  }

  /**
   * Save customer changes
   */
  protected saveCustomer(): void {
    const customer = this.selectedCustomer();
    if (!customer) return;

    this.saving.set(true);
    this.error.set(null);

    // Convert form values to UpdateCustomerRequest with type assertions
    const formValues = this.editForm();
    const request: UpdateCustomerRequest = {
      firstName: formValues.firstName,
      lastName: formValues.lastName,
      email: formValues.email as EmailAddress,
      phoneNumber: formValues.phoneNumber as PhoneNumber,
      dateOfBirth: formValues.dateOfBirth as ISODateString,
      street: formValues.street,
      city: formValues.city as CityName,
      postalCode: formValues.postalCode as PostalCode,
      country: formValues.country as CountryCode,
      licenseNumber: formValues.licenseNumber as LicenseNumber,
      licenseIssueCountry: formValues.licenseIssueCountry as CountryCode,
      licenseIssueDate: formValues.licenseIssueDate as ISODateString,
      licenseExpiryDate: formValues.licenseExpiryDate as ISODateString,
    };

    this.customerService.updateCustomer(customer.id, request).subscribe({
      next: (updatedCustomer) => {
        this.selectedCustomer.set(updatedCustomer);
        this.editMode.set(false);
        this.saving.set(false);
        this.successMessage.set(this.translate.instant('customers.edit.success'));
        setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_SHORT);
        // Refresh the customer list
        this.searchCustomers();
      },
      error: (err) => {
        logError('CustomersComponent', 'Error updating customer', err);
        this.error.set(this.translate.instant('customers.edit.error'));
        this.saving.set(false);
      },
    });
  }
}
