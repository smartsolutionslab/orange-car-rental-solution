/**
 * Customer details (flat structure for call-center)
 */
import type {
  CustomerId,
  EmailAddress,
  PhoneNumber,
  ISODateString,
  PostalCode,
  CountryCode,
  LicenseNumber,
  CityName,
} from '@orange-car-rental/data-access';

export type Customer = {
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
};
