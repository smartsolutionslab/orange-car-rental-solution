/**
 * Customer ID - serialized GUID from backend
 * Format: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx (36 chars)
 */
export type CustomerId = string & { readonly __brand: "CustomerId" };

const GUID_REGEX =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export function isCustomerId(value: string): value is CustomerId {
  return GUID_REGEX.test(value);
}

export function createCustomerId(value: string): CustomerId {
  if (!isCustomerId(value)) {
    throw new Error(`Invalid CustomerId format: ${value}`);
  }
  return value;
}
