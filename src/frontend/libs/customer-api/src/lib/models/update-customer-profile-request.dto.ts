/**
 * Customer profile update request
 * Matches backend UpdateCustomerProfileRequest nested structure
 */
import type { FirstName, LastName, PhoneNumber } from "@orange-car-rental/shared";
import type { AddressDetails } from "@orange-car-rental/reservation-api";

/**
 * Profile data for update (matches backend CustomerProfileDto)
 */
export type CustomerProfileUpdateData = {
  readonly firstName: FirstName;
  readonly lastName: LastName;
  readonly phoneNumber: PhoneNumber;
};

/**
 * Address data for update (matches backend AddressUpdateDto)
 */
export type AddressUpdateData = {
  readonly street: string;
  readonly city: string;
  readonly postalCode: string;
  readonly country?: string;
};

/**
 * Request structure matching backend UpdateCustomerProfileRequest
 */
export type UpdateCustomerProfileRequest = {
  readonly profile: CustomerProfileUpdateData;
  readonly address: AddressUpdateData;
};
