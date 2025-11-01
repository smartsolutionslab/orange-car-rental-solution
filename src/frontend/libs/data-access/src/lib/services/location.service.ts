import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Location } from '../models/location.model';
import { ConfigService } from './config.service';

/**
 * Service for accessing the Location API
 * Handles location retrieval operations
 */
@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private readonly http = inject(HttpClient);
  private readonly configService = inject(ConfigService);

  private get apiUrl(): string {
    return `${this.configService.apiUrl}/api/locations`;
  }

  /**
   * Get all available rental locations
   * @returns Observable of all locations
   */
  getAllLocations(): Observable<Location[]> {
    return this.http.get<Location[]>(this.apiUrl);
  }

  /**
   * Get a specific location by code
   * @param code Location code (e.g., "BER-HBF")
   * @returns Observable of location or null if not found
   */
  getLocationByCode(code: string): Observable<Location> {
    return this.http.get<Location>(`${this.apiUrl}/${code}`);
  }
}
