/**
 * Category Code - branded type for type safety
 * Matches backend VehicleCategory value object
 * Examples: KLEIN, KOMPAKT, MITTEL, OBER, SUV, KOMBI, TRANS, LUXUS
 */
export type CategoryCode = string & { readonly __brand: "CategoryCode" };

const CATEGORY_CODE_REGEX = /^[A-Z]{2,10}$/;

export function isCategoryCode(value: string): value is CategoryCode {
  return CATEGORY_CODE_REGEX.test(value);
}

export function createCategoryCode(value: string): CategoryCode {
  const normalized = value.toUpperCase().trim();
  if (!isCategoryCode(normalized)) {
    throw new Error(`Invalid CategoryCode format: ${value}`);
  }
  return normalized;
}

/**
 * Safely convert a string to CategoryCode, returning undefined if invalid
 */
export function toCategoryCode(
  value: string | null | undefined,
): CategoryCode | undefined {
  if (!value) return undefined;
  const normalized = value.toUpperCase().trim();
  return isCategoryCode(normalized) ? normalized : undefined;
}
