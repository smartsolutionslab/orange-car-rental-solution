/**
 * Guest lookup request
 */
import type { EmailAddress } from "@orange-car-rental/shared";
import type { ReservationId } from "./reservation-id.type";

export type GuestLookupRequest = {
  readonly reservationId: ReservationId;
  readonly email: EmailAddress;
};
