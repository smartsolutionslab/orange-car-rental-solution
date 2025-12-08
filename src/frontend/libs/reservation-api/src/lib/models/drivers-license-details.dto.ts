/**
 * Driver's license details
 */
import type { ISODateString, CountryCode } from '@orange-car-rental/shared';
import type { LicenseNumber } from './license-number.type';

export type DriversLicenseDetails = {
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
};
