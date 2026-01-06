/**
 * Search result with pagination metadata
 */
import type { Vehicle } from "./vehicle.dto";

export type VehicleSearchResult = {
  readonly vehicles: Vehicle[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
};
