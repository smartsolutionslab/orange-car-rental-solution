import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import type {
  GuestReservationRequest,
  GuestReservationResponse,
  Reservation,
  ReservationId,
  ReservationSearchFilters,
  ReservationSearchResponse,
  CancelReservationRequest
} from '@orange-car-rental/reservation-api';
import type { EmailAddress } from '@orange-car-rental/shared';
import { ConfigService } from './config.service';

/**
 * Service for accessing the Reservations API
 * Handles creating and managing reservations
 */
@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private readonly http = inject(HttpClient);
  private readonly configService = inject(ConfigService);

  private get apiUrl(): string {
    return `${this.configService.apiUrl}/api/reservations`;
  }

  /**
   * Create a guest reservation (customer registration + reservation creation in one call)
   * @param request Guest reservation request with customer and booking details
   * @returns Observable of reservation response with IDs and pricing
   */
  createGuestReservation(request: GuestReservationRequest): Observable<GuestReservationResponse> {
    return this.http.post<GuestReservationResponse>(`${this.apiUrl}/guest`, request);
  }

  /**
   * Get reservation by ID
   * @param id Reservation ID
   * @returns Observable of reservation details
   */
  getReservation(id: ReservationId): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.apiUrl}/${id}`);
  }

  /**
   * Search reservations with filters (requires authentication for customer-specific searches)
   * @param filters Search filters including customerId, status, dates, pagination
   * @returns Observable of paginated reservation results
   */
  searchReservations(filters: ReservationSearchFilters): Observable<ReservationSearchResponse> {
    let params = new HttpParams();

    if (filters.customerId) params = params.set('customerId', filters.customerId);
    if (filters.status) params = params.set('status', filters.status);
    if (filters.pickupDateFrom) params = params.set('pickupDateFrom', filters.pickupDateFrom);
    if (filters.pickupDateTo) params = params.set('pickupDateTo', filters.pickupDateTo);
    if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
    if (filters.sortOrder) params = params.set('sortOrder', filters.sortOrder);
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());

    return this.http.get<ReservationSearchResponse>(`${this.apiUrl}/search`, { params });
  }

  /**
   * Cancel a reservation with a reason
   * @param reservationId Reservation ID to cancel
   * @param reason Cancellation reason
   * @returns Observable of void (success/failure)
   */
  cancelReservation(reservationId: ReservationId, reason: string): Observable<void> {
    const request: CancelReservationRequest = { reason };
    return this.http.put<void>(`${this.apiUrl}/${reservationId}/cancel`, request);
  }

  /**
   * Guest lookup - find reservation by ID and email (no authentication required)
   * @param reservationId Reservation ID
   * @param email Email address used during booking
   * @returns Observable of reservation details
   */
  lookupGuestReservation(reservationId: ReservationId, email: EmailAddress): Observable<Reservation> {
    const params = new HttpParams()
      .set('reservationId', reservationId)
      .set('email', email);

    return this.http.get<Reservation>(`${this.apiUrl}/lookup`, { params });
  }
}
