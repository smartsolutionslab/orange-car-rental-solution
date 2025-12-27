/**
 * Vehicle distribution by status at a location
 */
export interface VehicleDistribution {
  readonly available: number;
  readonly rented: number;
  readonly maintenance: number;
  readonly outOfService: number;
  readonly reserved: number;
}
