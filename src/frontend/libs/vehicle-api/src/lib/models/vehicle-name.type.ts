/**
 * Vehicle Name - branded type for type safety
 * Matches backend VehicleName value object
 */
export type VehicleName = string & { readonly __brand: 'VehicleName' };

const MAX_LENGTH = 100;

export function isVehicleName(value: string): value is VehicleName {
  return value.length > 0 && value.length <= MAX_LENGTH;
}

export function createVehicleName(value: string): VehicleName {
  const trimmed = value.trim();
  if (!isVehicleName(trimmed)) {
    throw new Error(`Invalid VehicleName: ${value}`);
  }
  return trimmed;
}

/**
 * Safely convert a string to VehicleName, returning undefined if invalid
 */
export function toVehicleName(value: string | null | undefined): VehicleName | undefined {
  if (!value) return undefined;
  const trimmed = value.trim();
  return isVehicleName(trimmed) ? trimmed : undefined;
}
