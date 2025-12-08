import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import type { Vehicle, VehicleId, VehicleSearchQuery, VehicleSearchResult } from '../models';
import { API_CONFIG } from '@orange-car-rental/shared';

/**
 * Base service for accessing the Vehicle API
 * Contains common vehicle operations shared across all applications
 *
 * Applications must provide API_CONFIG token with their ConfigService:
 * @example
 * providers: [{ provide: API_CONFIG, useExisting: ConfigService }]
 */
@Injectable({
  providedIn: 'root'
})
export class BaseVehicleService {
  protected readonly http = inject(HttpClient);
  protected readonly config = inject(API_CONFIG);

  protected get vehiclesUrl(): string {
    return `${this.config.apiUrl}/api/vehicles`;
  }

  /**
   * Search vehicles with optional filters
   * @param query Search query parameters
   * @returns Observable of search results with vehicles and pagination
   */
  searchVehicles(query?: VehicleSearchQuery): Observable<VehicleSearchResult> {
    let params = new HttpParams();

    if (query) {
      if (query.pickupDate) params = params.set('pickupDate', query.pickupDate);
      if (query.returnDate) params = params.set('returnDate', query.returnDate);
      if (query.locationCode) params = params.set('locationCode', query.locationCode);
      if (query.categoryCode) params = params.set('categoryCode', query.categoryCode);
      if (query.minSeats !== undefined) params = params.set('minSeats', query.minSeats.toString());
      if (query.fuelType) params = params.set('fuelType', query.fuelType);
      if (query.transmissionType) params = params.set('transmissionType', query.transmissionType);
      if (query.maxDailyRateGross !== undefined) params = params.set('maxDailyRateGross', query.maxDailyRateGross.toString());
      if (query.status) params = params.set('status', query.status);
      if (query.pageNumber !== undefined) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
    }

    return this.http.get<VehicleSearchResult>(this.vehiclesUrl, { params });
  }

  /**
   * Get a specific vehicle by ID
   * @param id Vehicle ID
   * @returns Observable of vehicle details
   */
  getVehicleById(id: VehicleId): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.vehiclesUrl}/${id}`);
  }
}
