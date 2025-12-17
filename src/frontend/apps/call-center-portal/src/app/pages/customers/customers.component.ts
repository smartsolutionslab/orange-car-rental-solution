import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
import type { Customer, UpdateCustomerRequest, Reservation } from '../../types';

/**
 * Customers management page for call center
 * Search and view customer information
 */
@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, ModalComponent, LoadingStateComponent, EmptyStateComponent, ErrorStateComponent, StatusBadgeComponent, SuccessAlertComponent, IconComponent],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})
export class CustomersComponent {
  private readonly customerService = inject(CustomerService);
  private readonly reservationService = inject(ReservationService);

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

  // Edit form data
  protected readonly editForm = signal<UpdateCustomerRequest>({
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
    licenseExpiryDate: ''
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
      this.error.set('Bitte geben Sie mindestens ein Suchkriterium ein');
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.searchPerformed.set(true);

    const query = {
      ...(email && { email }),
      ...(phone && { phoneNumber: phone }),
      ...(lastName && { lastName }),
      pageSize: DEFAULT_PAGE_SIZE.CUSTOMERS
    };

    this.customerService.searchCustomers(query).subscribe({
      next: (result) => {
        this.customers.set(result.customers);
        this.totalCustomers.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        logError('CustomersComponent', 'Error searching customers', err);
        this.error.set('Fehler bei der Kundensuche');
        this.loading.set(false);
      }
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

    this.reservationService.searchReservations({ customerId: createCustomerId(customerId), pageSize: DEFAULT_PAGE_SIZE.CUSTOMER_RESERVATIONS }).subscribe({
      next: (result) => {
        this.customerReservations.set(result.reservations);
        this.loadingReservations.set(false);
      },
      error: (err) => {
        logError('CustomersComponent', 'Error loading customer reservations', err);
        this.loadingReservations.set(false);
      }
    });
  }

  // Helpers - using shared utilities
  protected formatDate = formatDateDE;
  protected getStatusClass = getReservationStatusClass;
  protected calculateAge = calculateAge;

  /**
   * Update edit form field
   */
  protected updateEditForm(field: keyof UpdateCustomerRequest, value: string): void {
    this.editForm.update(form => ({
      ...form,
      [field]: value
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
        licenseExpiryDate: customer.licenseExpiryDate ?? ''
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

    this.customerService.updateCustomer(customer.id, this.editForm()).subscribe({
      next: (updatedCustomer) => {
        this.selectedCustomer.set(updatedCustomer);
        this.editMode.set(false);
        this.saving.set(false);
        this.successMessage.set('Kundendaten erfolgreich aktualisiert');
        setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_SHORT);
        // Refresh the customer list
        this.searchCustomers();
      },
      error: (err) => {
        logError('CustomersComponent', 'Error updating customer', err);
        this.error.set('Fehler beim Speichern der Kundendaten');
        this.saving.set(false);
      }
    });
  }
}
