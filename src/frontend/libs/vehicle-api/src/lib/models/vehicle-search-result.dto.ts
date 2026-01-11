/**
 * Search result with pagination metadata
 * Matches backend PagedResult<VehicleDto> structure
 */
import type { Vehicle } from "./vehicle.dto";

export type VehicleSearchResult = {
  readonly items: Vehicle[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage: boolean;
  readonly hasNextPage: boolean;
};
