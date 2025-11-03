import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { ReservationService } from '../../services/reservation.service';
import { Reservation } from '../../services/reservation.model';

/**
 * Confirmation page component
 * Displays reservation confirmation details after successful booking
 */
@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation.component.html',
  styleUrl: './confirmation.component.css'
})
export class ConfirmationComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly reservationService = inject(ReservationService);

  protected readonly reservation = signal<Reservation | null>(null);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly reservationId = signal<string | null>(null);
  protected readonly customerId = signal<string | null>(null);

  ngOnInit(): void {
    // Get reservation ID from query params
    this.route.queryParams.subscribe(params => {
      const reservationId = params['reservationId'];
      const customerId = params['customerId'];

      if (!reservationId) {
        this.error.set('Keine Reservierungs-ID gefunden');
        this.loading.set(false);
        return;
      }

      this.reservationId.set(reservationId);
      this.customerId.set(customerId);
      this.loadReservation(reservationId);
    });
  }

  /**
   * Load reservation details from API
   */
  private loadReservation(reservationId: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.reservationService.getReservation(reservationId).subscribe({
      next: (reservation) => {
        this.reservation.set(reservation);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading reservation:', err);
        this.error.set('Fehler beim Laden der Reservierung');
        this.loading.set(false);
      }
    });
  }

  /**
   * Navigate back to home page
   */
  protected goToHome(): void {
    this.router.navigate(['/']);
  }

  /**
   * Print confirmation
   */
  protected printConfirmation(): void {
    window.print();
  }

  /**
   * Calculate number of rental days
   */
  protected getRentalDays(): number {
    const reservation = this.reservation();
    if (!reservation) return 0;

    const pickup = new Date(reservation.pickupDate);
    const returnDate = new Date(reservation.returnDate);
    const diffTime = Math.abs(returnDate.getTime() - pickup.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    return diffDays;
  }

  /**
   * Format date for display
   */
  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('de-DE', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
