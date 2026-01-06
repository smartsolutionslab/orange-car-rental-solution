/**
 * Manufacturing Year - branded type for type safety
 * Matches backend ManufacturingYear value object
 * Valid range: 1990 to current year
 */
export type ManufacturingYear = number & {
  readonly __brand: "ManufacturingYear";
};

const MIN_YEAR = 1990;

export function isManufacturingYear(value: number): value is ManufacturingYear {
  const currentYear = new Date().getFullYear();
  return Number.isInteger(value) && value >= MIN_YEAR && value <= currentYear;
}

export function createManufacturingYear(value: number): ManufacturingYear {
  if (!isManufacturingYear(value)) {
    throw new Error(
      `Invalid ManufacturingYear: ${value}. Must be ${MIN_YEAR} or later and not in the future.`,
    );
  }
  return value;
}

/**
 * Safely convert a number to ManufacturingYear, returning undefined if invalid
 */
export function toManufacturingYear(
  value: number | null | undefined,
): ManufacturingYear | undefined {
  if (value === null || value === undefined) return undefined;
  return isManufacturingYear(value) ? value : undefined;
}
