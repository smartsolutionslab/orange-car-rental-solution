/**
 * Reservation data model
 */
import type { Currency, ISODateString, Price } from "@orange-car-rental/shared";
import type { VehicleId } from "@orange-car-rental/vehicle-api";
import type { LocationCode } from "@orange-car-rental/location-api";
import type { ReservationId } from "./reservation-id.type";
import type { CustomerId } from "./customer-id.type";
import type { ReservationStatus } from "./reservation-status.enum";

export type Reservation = {
  readonly id: ReservationId;
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly rentalDays: number;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
  readonly status: ReservationStatus;
  readonly cancellationReason: string | null;
  readonly createdAt: ISODateString;
  readonly confirmedAt: ISODateString | null;
  readonly cancelledAt: ISODateString | null;
  readonly completedAt: ISODateString | null;
};
