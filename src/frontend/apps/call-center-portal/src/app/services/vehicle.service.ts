import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import type {
  Vehicle,
  VehicleId,
  DailyRate,
  VehicleSearchQuery,
  VehicleSearchResult,
  AddVehicleRequest,
  AddVehicleResult
} from '@orange-car-rental/vehicle-api';
import { VehicleStatus } from '@orange-car-rental/vehicle-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import { HttpParamsBuilder } from '@orange-car-rental/shared';
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
    const params = HttpParamsBuilder.fromObject(query);
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
