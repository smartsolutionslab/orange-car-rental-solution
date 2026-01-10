/**
 * Update customer request - flat structure for form values
 * The customer service maps this to the nested backend request format
 */
import type {
  EmailAddress,
  PhoneNumber,
  ISODateString,
  PostalCode,
  CountryCode,
  FirstName,
  LastName,
} from '@orange-car-rental/shared';
import type { LicenseNumber } from '@orange-car-rental/reservation-api';
import type { CityName, StreetAddress } from '@orange-car-rental/location-api';

export interface UpdateCustomerRequest {
  readonly firstName: FirstName;
  readonly lastName: LastName;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
}
