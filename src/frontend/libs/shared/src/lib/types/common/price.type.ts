/**
 * Price - branded type for monetary values
 * Matches backend Money value object (net amount in EUR)
 * Values are in EUR with 2 decimal precision
 */
export type Price = number & { readonly __brand: "Price" };

export function isPrice(value: number): value is Price {
  return Number.isFinite(value) && value >= 0;
}

export function createPrice(value: number): Price {
  if (!isPrice(value)) {
    throw new Error(`Invalid Price value: ${value}`);
  }
  // Round to 2 decimal places
  return (Math.round(value * 100) / 100) as Price;
}

/**
 * Safely convert a number to Price, returning undefined if invalid
 */
export function toPrice(value: number | null | undefined): Price | undefined {
  if (value === null || value === undefined) return undefined;
  return isPrice(value)
    ? ((Math.round(value * 100) / 100) as Price)
    : undefined;
}

/**
 * Format price for display (e.g., "123,45 €")
 */
export function formatPrice(price: Price, locale: string = "de-DE"): string {
  return new Intl.NumberFormat(locale, {
    style: "currency",
    currency: "EUR",
  }).format(price);
}
