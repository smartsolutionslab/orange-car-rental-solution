import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Reservation,
  ReservationId,
  CreateReservationRequest,
  CreateReservationResponse,
  GuestReservationRequest,
  GuestReservationResponse,
  ReservationSearchQuery,
  ReservationSearchResult
} from './reservation.model';
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
   * Create a reservation for a registered customer
   * @param request Reservation request
   * @returns Observable of reservation response
   */
  createReservation(request: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.http.post<CreateReservationResponse>(this.apiUrl, request);
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
   * Search reservations with optional filters
   * @param query Search query parameters
   * @returns Observable of search results with reservations and pagination
   */
  searchReservations(query?: ReservationSearchQuery): Observable<ReservationSearchResult> {
    let params = new HttpParams();

    if (query) {
      if (query.customerId) params = params.set('customerId', query.customerId);
      if (query.vehicleId) params = params.set('vehicleId', query.vehicleId);
      if (query.status) params = params.set('status', query.status);
      if (query.pickupDateFrom) params = params.set('pickupDateFrom', query.pickupDateFrom);
      if (query.pickupDateTo) params = params.set('pickupDateTo', query.pickupDateTo);
      if (query.pageNumber !== undefined) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
    }

    return this.http.get<ReservationSearchResult>(`${this.apiUrl}/search`, { params });
  }

  /**
   * Cancel a reservation
   * @param id Reservation ID
   * @param reason Cancellation reason
   * @returns Observable of void
   */
  cancelReservation(id: ReservationId, reason: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/cancel`, { cancellationReason: reason });
  }

  /**
   * Confirm a reservation
   * @param id Reservation ID
   * @returns Observable of void
   */
  confirmReservation(id: ReservationId): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/confirm`, {});
  }
}
