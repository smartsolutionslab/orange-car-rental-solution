import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import type {
  Reservation,
  ReservationId,
  ReservationSearchFilters,
  ReservationSearchResponse,
  GuestReservationRequest,
  GuestReservationResponse,
  CancelReservationRequest
} from '../models';
import { API_CONFIG } from '@orange-car-rental/shared';

/**
 * Base service for accessing the Reservation API
 * Contains common reservation operations shared across all applications
 *
 * Applications must provide API_CONFIG token with their ConfigService:
 * @example
 * providers: [{ provide: API_CONFIG, useExisting: ConfigService }]
 */
@Injectable({
  providedIn: 'root'
})
export class BaseReservationService {
  protected readonly http = inject(HttpClient);
  protected readonly config = inject(API_CONFIG);

  protected get reservationsUrl(): string {
    return `${this.config.apiUrl}/api/reservations`;
  }

  /**
   * Get reservation by ID
   * @param id Reservation ID
   * @returns Observable of reservation details
   */
  getReservation(id: ReservationId): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.reservationsUrl}/${id}`);
  }

  /**
   * Search reservations with filters
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

    return this.http.get<ReservationSearchResponse>(`${this.reservationsUrl}/search`, { params });
  }

  /**
   * Create a guest reservation (customer registration + reservation creation in one call)
   * @param request Guest reservation request with customer and booking details
   * @returns Observable of reservation response with IDs and pricing
   */
  createGuestReservation(request: GuestReservationRequest): Observable<GuestReservationResponse> {
    return this.http.post<GuestReservationResponse>(`${this.reservationsUrl}/guest`, request);
  }

  /**
   * Cancel a reservation with a reason
   * @param reservationId Reservation ID to cancel
   * @param reason Cancellation reason
   * @returns Observable of void (success/failure)
   */
  cancelReservation(reservationId: ReservationId, reason: string): Observable<void> {
    const request: CancelReservationRequest = { reason };
    return this.http.put<void>(`${this.reservationsUrl}/${reservationId}/cancel`, request);
  }
}
