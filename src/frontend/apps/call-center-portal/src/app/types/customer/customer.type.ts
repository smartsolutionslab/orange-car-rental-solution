/**
 * Customer details (flat structure for call-center)
 */
import type { CustomerId } from '@orange-car-rental/customer-api';
import type {
  EmailAddress,
  PhoneNumber,
  ISODateString,
  PostalCode,
  CountryCode,
} from '@orange-car-rental/shared';
import type { LicenseNumber } from '@orange-car-rental/reservation-api';
import type { CityName } from '@orange-car-rental/location-api';

export interface Customer {
  readonly id: CustomerId;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
  readonly street: string;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
  readonly createdAt: ISODateString;
  readonly updatedAt?: ISODateString;
}
