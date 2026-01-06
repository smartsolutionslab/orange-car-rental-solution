/**
 * Vehicle ID - branded type for type safety
 * Format: GUID (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
 * Matches backend VehicleIdentifier value object
 */
export type VehicleId = string & { readonly __brand: "VehicleId" };

const GUID_REGEX =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export function isVehicleId(value: string): value is VehicleId {
  return GUID_REGEX.test(value);
}

export function createVehicleId(value: string): VehicleId {
  if (!isVehicleId(value)) {
    throw new Error(`Invalid VehicleId format: ${value}`);
  }
  return value;
}

/**
 * Safely convert a string to VehicleId, returning undefined if invalid
 */
export function toVehicleId(
  value: string | null | undefined,
): VehicleId | undefined {
  if (!value) return undefined;
  return isVehicleId(value) ? value : undefined;
}
