import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReservationService } from '../../services/reservation.service';
import { Reservation } from '../../services/reservation.model';

/**
 * Reservations management page for call center
 * Displays booking management tools
 */
@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reservations.component.html',
  styleUrl: './reservations.component.css'
})
export class ReservationsComponent implements OnInit {
  private readonly reservationService = inject(ReservationService);

  protected readonly reservations = signal<Reservation[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedReservation = signal<Reservation | null>(null);
  protected readonly showDetails = signal(false);

  // Search filters
  protected readonly searchStatus = signal<string>('');
  protected readonly searchCustomerId = signal<string>('');

  // Stats
  protected readonly totalReservations = signal(0);
  protected readonly todayReservations = signal(0);
  protected readonly activeReservations = signal(0);
  protected readonly pendingReservations = signal(0);

  ngOnInit(): void {
    this.loadReservations();
  }

  /**
   * Load reservations with optional filters
   */
  protected loadReservations(): void {
    this.loading.set(true);
    this.error.set(null);

    const query = {
      status: this.searchStatus() || undefined,
      customerId: this.searchCustomerId() || undefined,
      pageSize: 50
    };

    this.reservationService.searchReservations(query).subscribe({
      next: (result) => {
        this.reservations.set(result.reservations);
        this.totalReservations.set(result.totalCount);
        this.calculateStats(result.reservations);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading reservations:', err);
        this.error.set('Fehler beim Laden der Reservierungen');
        this.loading.set(false);
      }
    });
  }

  /**
   * Calculate statistics from reservations
   */
  private calculateStats(reservations: Reservation[]): void {
    const today = new Date().toISOString().split('T')[0];

    this.todayReservations.set(
      reservations.filter(r => r.createdAt.startsWith(today)).length
    );

    this.activeReservations.set(
      reservations.filter(r => r.status === 'Confirmed' || r.status === 'Active').length
    );

    this.pendingReservations.set(
      reservations.filter(r => r.status === 'Pending').length
    );
  }

  /**
   * View reservation details
   */
  protected viewDetails(reservation: Reservation): void {
    this.selectedReservation.set(reservation);
    this.showDetails.set(true);
  }

  /**
   * Close details view
   */
  protected closeDetails(): void {
    this.showDetails.set(false);
    this.selectedReservation.set(null);
  }

  /**
   * Apply search filters
   */
  protected applyFilters(): void {
    this.loadReservations();
  }

  /**
   * Clear search filters
   */
  protected clearFilters(): void {
    this.searchStatus.set('');
    this.searchCustomerId.set('');
    this.loadReservations();
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
}
