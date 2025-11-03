import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GuestReservationRequest, GuestReservationResponse, Reservation } from './reservation.model';
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
  getReservation(id: string): Observable<Reservation> {
    return this.http.get<Reservation>(`${this.apiUrl}/${id}`);
  }
}
