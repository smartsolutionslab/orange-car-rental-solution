import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import type { Location, LocationCode } from '../models';
import { API_CONFIG } from '@orange-car-rental/shared';

/**
 * Shared service for accessing the Location API
 * Handles location retrieval operations across all applications
 *
 * Applications must provide API_CONFIG token with their ConfigService:
 * @example
 * providers: [{ provide: API_CONFIG, useExisting: ConfigService }]
 */
@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(API_CONFIG);

  private get locationsUrl(): string {
    return `${this.config.apiUrl}/api/locations`;
  }

  /**
   * Get all available rental locations
   * @returns Observable of all locations
   */
  getAllLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(this.locationsUrl);
  }

  /**
   * Get a specific location by code
   * @param code Location code (e.g., "BER-HBF")
   * @returns Observable of location or null if not found
   */
  getLocationByCode(code: LocationCode): Observable<Location> {
    return this.http.get<Location>(`${this.locationsUrl}/${code}`);
  }
}
