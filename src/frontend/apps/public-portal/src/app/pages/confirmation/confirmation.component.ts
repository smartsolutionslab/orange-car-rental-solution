import { Component, signal, inject } from '@angular/core';
import type { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import type { Reservation } from '@orange-car-rental/reservation-api';
import { logError } from '@orange-car-rental/util';
import {
  StatusBadgeComponent,
  LoadingStateComponent,
  ErrorStateComponent,
  calculateRentalDays,
  formatDateLongDE,
  IconComponent,
} from '@orange-car-rental/ui-components';
import { ReservationService } from '../../services/reservation.service';

/**
 * Confirmation page component
 * Displays reservation confirmation details after successful booking
 */
@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [
    CommonModule,
    StatusBadgeComponent,
    LoadingStateComponent,
    ErrorStateComponent,
    IconComponent,
  ],
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
        logError('ConfirmationComponent', 'Error loading reservation', err);
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
   * Calculate number of rental days using shared utility
   */
  protected getRentalDays(): number {
    const reservation = this.reservation();
    if (!reservation) return 0;
    return calculateRentalDays(reservation.pickupDate, reservation.returnDate);
  }

  /**
   * Format date for display using shared utility
   */
  protected formatDate = formatDateLongDE;
}
