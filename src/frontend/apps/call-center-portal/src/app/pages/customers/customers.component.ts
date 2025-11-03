import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { ReservationService } from '../../services/reservation.service';
import { Customer } from '../../services/customer.model';
import { Reservation } from '../../services/reservation.model';

/**
 * Customers management page for call center
 * Search and view customer information
 */
@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
      email: email || undefined,
      phoneNumber: phone || undefined,
      lastName: lastName || undefined,
      pageSize: 50
    };

    this.customerService.searchCustomers(query).subscribe({
      next: (result) => {
        this.customers.set(result.customers);
        this.totalCustomers.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error searching customers:', err);
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
  }

  /**
   * Load customer reservations
   */
  private loadCustomerReservations(customerId: string): void {
    this.loadingReservations.set(true);

    this.reservationService.searchReservations({ customerId, pageSize: 100 }).subscribe({
      next: (result) => {
        this.customerReservations.set(result.reservations);
        this.loadingReservations.set(false);
      },
      error: (err) => {
        console.error('Error loading customer reservations:', err);
        this.loadingReservations.set(false);
      }
    });
  }

  /**
   * Format date for display
   */
  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('de-DE', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit'
    });
  }

  /**
   * Get status badge class
   */
  protected getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'confirmed':
      case 'active':
        return 'status-success';
      case 'pending':
        return 'status-warning';
      case 'cancelled':
        return 'status-error';
      case 'completed':
        return 'status-info';
      default:
        return '';
    }
  }

  /**
   * Calculate customer age
   */
  protected calculateAge(dateOfBirth: string): number {
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }

    return age;
  }
}
