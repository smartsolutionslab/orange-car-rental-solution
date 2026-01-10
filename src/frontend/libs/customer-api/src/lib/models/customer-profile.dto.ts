/**
 * Complete customer profile
 * Matches backend CustomerDto structure
 */
import type {
  ISODateString,
  EmailAddress,
  PhoneNumber,
  FirstName,
  LastName,
} from "@orange-car-rental/shared";
import type {
  CustomerId,
  AddressDetails,
  DriversLicenseDetails,
} from "@orange-car-rental/reservation-api";

export type CustomerProfile = {
  readonly id: CustomerId;
  readonly firstName: FirstName;
  readonly lastName: LastName;
  readonly fullName?: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly phoneNumberFormatted?: string;
  readonly dateOfBirth: ISODateString;
  readonly age?: number;
  readonly address?: AddressDetails;
  readonly driversLicense?: DriversLicenseDetails;
  readonly status?: string;
  readonly canMakeReservation?: boolean;
  readonly registeredAtUtc?: ISODateString;
  readonly updatedAtUtc?: ISODateString;
};
