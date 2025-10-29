import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VehicleSearchQuery, VehicleSearchResult } from '../models/vehicle.model';

/**
 * Service for accessing the Fleet API
 * Handles vehicle search and related operations
 */
@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5046/api/vehicles';

  /**
   * Search vehicles with optional filters
   * @param query Search query parameters
   * @returns Observable of search results with vehicles and pagination
   */
  searchVehicles(query?: VehicleSearchQuery): Observable<VehicleSearchResult> {
    let params = new HttpParams();

    if (query) {
      // Add query parameters only if they have values
      if (query.pickupDate) params = params.set('pickupDate', query.pickupDate);
      if (query.returnDate) params = params.set('returnDate', query.returnDate);
      if (query.locationCode) params = params.set('locationCode', query.locationCode);
      if (query.categoryCode) params = params.set('categoryCode', query.categoryCode);
      if (query.minSeats !== undefined) params = params.set('minSeats', query.minSeats.toString());
      if (query.fuelType) params = params.set('fuelType', query.fuelType);
      if (query.transmissionType) params = params.set('transmissionType', query.transmissionType);
      if (query.maxDailyRateGross !== undefined) params = params.set('maxDailyRateGross', query.maxDailyRateGross.toString());
      if (query.pageNumber !== undefined) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
    }

    return this.http.get<VehicleSearchResult>(this.apiUrl, { params });
  }
}
