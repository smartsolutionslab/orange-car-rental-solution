import { Component, inject, signal } from '@angular/core';
import type { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import type { Reservation, ReservationSearchFilters } from '@orange-car-rental/data-access';
import { ReservationStatus, createCustomerId } from '@orange-car-rental/data-access';
import { logError } from '@orange-car-rental/util';
import { ReservationService } from '../../services/reservation.service';
import { AuthService } from '../../services/auth.service';
import { DEFAULT_PAGE_SIZE } from '../../constants/app.constants';
import type { GroupedReservations } from '../../types';

@Component({
  selector: 'app-booking-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-history.component.html',
  styleUrls: ['./booking-history.component.css']
})
export class BookingHistoryComponent implements OnInit {
  private readonly reservationService = inject(ReservationService);
  private readonly authService = inject(AuthService);

  // State signals
  isAuthenticated = signal(false);
  isLoading = signal(false);
  error = signal<string | null>(null);
  groupedReservations = signal<GroupedReservations>({
    upcoming: [],
    past: [],
    pending: []
  });

  // Guest lookup form
  guestLookupForm = {
    reservationId: '',
    email: ''
  };
  guestReservation = signal<Reservation | null>(null);
  guestLookupError = signal<string | null>(null);

  // Cancellation modal
  showCancelModal = signal(false);
  selectedReservation = signal<Reservation | null>(null);
  cancellationReason = '';
  cancellationReasons = [
    'Change of plans',
    'Found alternative transportation',
    'Trip cancelled',
    'Booking error',
    'Other'
  ];

  // Detail modal
  showDetailModal = signal(false);
  detailReservation = signal<Reservation | null>(null);

  ngOnInit() {
    return this.checkAuthAndLoadReservations();
  }

  private async checkAuthAndLoadReservations() {
    const authenticated = this.authService.isAuthenticated();
    this.isAuthenticated.set(authenticated);

    if (authenticated) {
      await this.loadCustomerReservations();
    }
  }

  async loadCustomerReservations() {
    this.isLoading.set(true);
    this.error.set(null);

    const userProfile = await this.authService.getUserProfile();
    if (!userProfile?.id) {
      this.error.set('Unable to retrieve user information');
      this.isLoading.set(false);
      return;
    }

    const filters: ReservationSearchFilters = {
      customerId: createCustomerId(userProfile.id),
      sortBy: 'PickupDate',
      sortOrder: 'desc',
      pageSize: DEFAULT_PAGE_SIZE.BOOKING_HISTORY
    };

    try {
      const response = await firstValueFrom(this.reservationService.searchReservations(filters));
      this.groupReservations(response.items);
      this.isLoading.set(false);
    } catch (err) {
      logError('BookingHistoryComponent', 'Error loading reservations', err);
      this.error.set('Failed to load your booking history. Please try again later.');
      this.isLoading.set(false);
    }
  }

  private groupReservations(reservations: Reservation[]) {
    const now = new Date();
    const grouped: GroupedReservations = {
      upcoming: [],
      past: [],
      pending: []
    };

    reservations.forEach(reservation => {
      const pickupDate = new Date(reservation.pickupDate);

      if (reservation.status === 'Pending') {
        grouped.pending.push(reservation);
      } else if (reservation.status === 'Confirmed' || reservation.status === 'Active') {
        if (pickupDate >= now) {
          grouped.upcoming.push(reservation);
        } else {
          grouped.past.push(reservation);
        }
      } else {
        grouped.past.push(reservation);
      }
    });

    this.groupedReservations.set(grouped);
  }

  // Guest lookup
  onGuestLookup() {
    const reservationId = this.guestLookupForm.reservationId?.trim();
    const email = this.guestLookupForm.email?.trim();

    if (!reservationId || !email) {
      this.guestLookupError.set('Please enter both Reservation ID and Email');
      return;
    }

    this.isLoading.set(true);
    this.guestLookupError.set(null);
    this.guestReservation.set(null);

    this.reservationService.lookupGuestReservation(
      reservationId,
      email
    ).subscribe({
      next: (reservation) => {
        this.guestReservation.set(reservation);
        this.isLoading.set(false);
      },
      error: (err) => {
        logError('BookingHistoryComponent', 'Guest lookup error', err);
        this.guestLookupError.set('Reservation not found. Please check your Reservation ID and Email.');
        this.isLoading.set(false);
      }
    });
  }

  // View details
  viewDetails(reservation: Reservation) {
    this.detailReservation.set(reservation);
    this.showDetailModal.set(true);
  }

  closeDetailModal() {
    this.showDetailModal.set(false);
    this.detailReservation.set(null);
  }

  // Cancellation
  canCancel(reservation: Reservation): boolean {
    if (reservation.status !== 'Confirmed' && reservation.status !== 'Pending') {
      return false;
    }

    // Free cancellation up to 48 hours before pickup
    const pickupDate = new Date(reservation.pickupDate);
    const now = new Date();
    const hoursUntilPickup = (pickupDate.getTime() - now.getTime()) / (1000 * 60 * 60);

    return hoursUntilPickup >= 48;
  }

  openCancelModal(reservation: Reservation) {
    this.selectedReservation.set(reservation);
    this.cancellationReason = '';
    this.showCancelModal.set(true);
  }

  closeCancelModal() {
    this.showCancelModal.set(false);
    this.selectedReservation.set(null);
    this.cancellationReason = '';
  }

  confirmCancellation() {
    const reservation = this.selectedReservation();
    if (!reservation || !this.cancellationReason) {
      return;
    }

    this.isLoading.set(true);

    this.reservationService.cancelReservation(reservation.id, this.cancellationReason).subscribe({
      next: () => {
        this.closeCancelModal();
        this.isLoading.set(false);

        // Reload reservations
        if (this.isAuthenticated()) {
          this.loadCustomerReservations();
        } else {
          // For guest, just update the status locally
          const updated: Reservation = { ...reservation, status: 'Cancelled' as ReservationStatus };
          this.guestReservation.set(updated);
        }

        alert('Reservation cancelled successfully!');
      },
      error: (err) => {
        logError('BookingHistoryComponent', 'Cancellation error', err);
        this.isLoading.set(false);
        alert('Failed to cancel reservation. Please try again or contact support.');
      }
    });
  }

  // Helpers
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('de-DE', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('de-DE', {
      style: 'currency',
      currency: 'EUR'
    }).format(price);
  }

  getStatusClass(status: string): string {
    const statusMap: Record<string, string> = {
      'Pending': 'status-pending',
      'Confirmed': 'status-confirmed',
      'Active': 'status-active',
      'Completed': 'status-completed',
      'Cancelled': 'status-cancelled'
    };
    return statusMap[status] || '';
  }

  getStatusLabel(status: string): string {
    const labelMap: Record<string, string> = {
      'Pending': 'Ausstehend',
      'Confirmed': 'Bestätigt',
      'Active': 'Aktiv',
      'Completed': 'Abgeschlossen',
      'Cancelled': 'Storniert'
    };
    return labelMap[status] || status;
  }

  printReservation() {
    // TODO: Implement print functionality
    window.print();
  }
}
