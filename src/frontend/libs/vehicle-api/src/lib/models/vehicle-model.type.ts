/**
 * Vehicle Model - branded type for type safety
 * Matches backend VehicleModel value object
 */
export type VehicleModel = string & { readonly __brand: 'VehicleModel' };

const MIN_LENGTH = 1;
const MAX_LENGTH = 100;

export function isVehicleModel(value: string): value is VehicleModel {
  return value.length >= MIN_LENGTH && value.length <= MAX_LENGTH;
}

export function createVehicleModel(value: string): VehicleModel {
  const trimmed = value.trim();
  if (!isVehicleModel(trimmed)) {
    throw new Error(`Invalid VehicleModel format: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to VehicleModel, returning undefined if invalid
 */
export function toVehicleModel(value: string | null | undefined): VehicleModel | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isVehicleModel(trimmed) ? trimmed : undefined;
}
