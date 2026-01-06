import { HttpParams } from "@angular/common/http";

/**
 * Utility class for building HttpParams from objects.
 * Automatically filters out null, undefined, and empty string values.
 *
 * @example
 * ```typescript
 * import { HttpParamsBuilder } from '@orange-car-rental/shared';
 *
 * const params = HttpParamsBuilder.fromObject({
 *   pickupDate: '2025-01-01',
 *   returnDate: '2025-01-05',
 *   categoryCode: null,        // Will be excluded
 *   pageNumber: 1,             // Will be converted to string
 *   status: undefined          // Will be excluded
 * });
 *
 * // Result: ?pickupDate=2025-01-01&returnDate=2025-01-05&pageNumber=1
 * ```
 */
export const HttpParamsBuilder = {
  /**
   * Build HttpParams from an object, excluding null/undefined/empty values
   * @param params Object with query parameters (any object type accepted)
   * @returns HttpParams instance
   */
  fromObject<T extends object>(params: T | undefined | null): HttpParams {
    if (!params) {
      return new HttpParams();
    }

    return Object.entries(params).reduce((httpParams, [key, value]) => {
      if (value === null || value === undefined || value === "") {
        return httpParams;
      }

      const stringValue = typeof value === "string" ? value : String(value);
      return httpParams.set(key, stringValue);
    }, new HttpParams());
  },

  /**
   * Build HttpParams from an object, keeping only specified keys
   * @param params Object with query parameters
   * @param keys Keys to include (if they have values)
   * @returns HttpParams instance
   */
  fromObjectKeys<T extends object>(
    params: T | undefined | null,
    keys: (keyof T)[],
  ): HttpParams {
    if (!params) {
      return new HttpParams();
    }

    return keys.reduce((httpParams, key) => {
      const value = params[key];
      if (value === null || value === undefined || value === "") {
        return httpParams;
      }

      const stringValue = typeof value === "string" ? value : String(value);
      return httpParams.set(String(key), stringValue);
    }, new HttpParams());
  },

  /**
   * Merge an existing HttpParams with additional params from an object
   * @param existing Existing HttpParams
   * @param params Object with additional query parameters
   * @returns New HttpParams instance with merged values
   */
  merge<T extends object>(
    existing: HttpParams,
    params: T | undefined | null,
  ): HttpParams {
    if (!params) {
      return existing;
    }

    return Object.entries(params).reduce((httpParams, [key, value]) => {
      if (value === null || value === undefined || value === "") {
        return httpParams;
      }

      const stringValue = typeof value === "string" ? value : String(value);
      return httpParams.set(key, stringValue);
    }, existing);
  },
};
