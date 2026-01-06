/**
 * Customer details
 */
import type {
  ISODateString,
  EmailAddress,
  PhoneNumber,
  FirstName,
  LastName,
} from "@orange-car-rental/shared";

export type CustomerDetails = {
  readonly firstName: FirstName;
  readonly lastName: LastName;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
};
