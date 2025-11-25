import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Vehicle, VehicleSearchQuery, VehicleSearchResult } from './vehicle.model';
import { ConfigService } from './config.service';

/**
 * Service for accessing the Fleet API
 * Handles vehicle search and related operations
 */
@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private readonly http = inject(HttpClient);
  private readonly configService = inject(ConfigService);

  private get apiUrl(): string {
    return `${this.configService.apiUrl}/api/vehicles`;
  }

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

  /**
   * Get a specific vehicle by ID
   * @param id Vehicle ID
   * @returns Observable of vehicle details
   */
  getVehicleById(id: string): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get similar vehicles based on current vehicle and availability
   * @param currentVehicle The current vehicle to find similar alternatives for
   * @param pickupDate Pickup date for availability check
   * @param returnDate Return date for availability check
   * @param maxResults Maximum number of similar vehicles to return (default: 4)
   * @returns Observable of similar vehicles array
   */
  getSimilarVehicles(
    currentVehicle: Vehicle,
    pickupDate?: string,
    returnDate?: string,
    maxResults = 4
  ): Observable<Vehicle[]> {
    // Search for vehicles with similar criteria
    const query: VehicleSearchQuery = {
      locationCode: currentVehicle.locationCode,
      pickupDate,
      returnDate,
      pageSize: 20 // Get more results to filter from
    };

    return this.searchVehicles(query).pipe(
      map(result => {
        // Filter out the current vehicle and find similar ones
        const candidates = result.vehicles.filter(v => v.id !== currentVehicle.id);

        // Category hierarchy for similarity matching
        const categories = ['KLEIN', 'KOMPAKT', 'MITTEL', 'OBER', 'SUV', 'KOMBI', 'TRANS', 'LUXUS'];
        const currentCategoryIndex = categories.indexOf(currentVehicle.categoryCode);

        // Score and sort vehicles by similarity
        const scored = candidates.map(vehicle => {
          let score = 0;

          // Category similarity (same = 100, +/- 1 = 50, +/- 2 = 25)
          const categoryIndex = categories.indexOf(vehicle.categoryCode);
          const categoryDiff = Math.abs(categoryIndex - currentCategoryIndex);
          if (categoryDiff === 0) score += 100;
          else if (categoryDiff === 1) score += 50;
          else if (categoryDiff === 2) score += 25;

          // Price similarity (within 20% = 50 points, within 40% = 25 points)
          const priceDiff = Math.abs(vehicle.dailyRateGross - currentVehicle.dailyRateGross);
          const pricePercent = (priceDiff / currentVehicle.dailyRateGross) * 100;
          if (pricePercent <= 20) score += 50;
          else if (pricePercent <= 40) score += 25;

          // Same fuel type (+30 points)
          if (vehicle.fuelType === currentVehicle.fuelType) score += 30;

          // Same transmission (+20 points)
          if (vehicle.transmissionType === currentVehicle.transmissionType) score += 20;

          // Similar seat count (+10 points)
          if (Math.abs(vehicle.seats - currentVehicle.seats) <= 1) score += 10;

          return { vehicle, score };
        });

        // Sort by score (highest first) and return top results
        return scored
          .sort((a, b) => b.score - a.score)
          .slice(0, maxResults)
          .map(item => item.vehicle);
      })
    );
  }
}
