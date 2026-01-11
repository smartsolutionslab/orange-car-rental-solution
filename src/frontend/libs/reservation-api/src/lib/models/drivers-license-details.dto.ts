/**
 * Driver's license details for requests (e.g., creating reservations)
 * Matches backend DriversLicenseDetailsDto structure
 */
import type { ISODateString, CountryCode } from "@orange-car-rental/shared";
import type { LicenseNumber } from "./license-number.type";

export type DriversLicenseDetails = {
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
};

/**
 * Driver's license information from customer profile response
 * Matches backend DriversLicenseDto structure with computed fields
 */
export type DriversLicenseInfo = {
  readonly licenseNumber: LicenseNumber;
  readonly issueCountry: CountryCode;
  readonly issueDate: ISODateString;
  readonly expiryDate: ISODateString;
  readonly isValid: boolean;
  readonly isEuLicense: boolean;
  readonly daysUntilExpiry: number;
};
