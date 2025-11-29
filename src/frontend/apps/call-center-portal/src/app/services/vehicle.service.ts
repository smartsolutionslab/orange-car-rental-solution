import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import type {
  Vehicle,
  VehicleId,
  LocationCode,
  DailyRate,
  VehicleSearchQuery,
  VehicleSearchResult,
  AddVehicleRequest,
  AddVehicleResult
} from '@orange-car-rental/data-access';
import { VehicleStatus } from '@orange-car-rental/data-access';
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
      if (query.status) params = params.set('status', query.status);
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
  getVehicleById(id: VehicleId): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.apiUrl}/${id}`);
  }

  /**
   * Add a new vehicle to the fleet
   * @param request Add vehicle request with vehicle details
   * @returns Observable of the created vehicle result
   */
  addVehicle(request: AddVehicleRequest): Observable<AddVehicleResult> {
    return this.http.post<AddVehicleResult>(this.apiUrl, request);
  }

  /**
   * Update vehicle status
   * @param vehicleId Vehicle ID
   * @param status New status
   * @returns Observable of void
   */
  updateVehicleStatus(vehicleId: VehicleId, status: VehicleStatus): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${vehicleId}/status?status=${status}`, {});
  }

  /**
   * Update vehicle location
   * @param vehicleId Vehicle ID
   * @param locationCode New location code
   * @returns Observable of void
   */
  updateVehicleLocation(vehicleId: VehicleId, locationCode: LocationCode): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${vehicleId}/location?locationCode=${locationCode}`, {});
  }

  /**
   * Update vehicle daily rate
   * @param vehicleId Vehicle ID
   * @param dailyRateNet New daily rate (net amount in EUR, VAT will be calculated)
   * @returns Observable of void
   */
  updateVehicleDailyRate(vehicleId: VehicleId, dailyRateNet: DailyRate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${vehicleId}/daily-rate?dailyRateNet=${dailyRateNet}`, {});
  }
}
