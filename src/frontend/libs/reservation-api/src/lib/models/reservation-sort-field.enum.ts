/**
 * Reservation sort field enum - use ReservationSortField.PickupDate instead of 'PickupDate'
 */
export const ReservationSortField = {
  PickupDate: "PickupDate",
  Price: "Price",
  Status: "Status",
  CreatedDate: "CreatedDate",
} as const;

export type ReservationSortField =
  (typeof ReservationSortField)[keyof typeof ReservationSortField];
