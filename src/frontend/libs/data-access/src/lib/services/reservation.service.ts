import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateReservationRequest, CreateReservationResult, Reservation } from '../models/reservation.model';
import { ConfigService } from './config.service';

/**
 * Service for accessing the Reservations API
 * Handles reservation creation and management
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
   * Create a new reservation
   * Price will be automatically calculated if not provided
   * @param request Reservation request
   * @returns Observable of created reservation result
   */
  createReservation(request: CreateReservationRequest): Observable<CreateReservationResult> {
    return this.http.post<CreateReservationResult>(this.apiUrl, request);
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
