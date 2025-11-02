/**
 * Create reservation request matching backend CreateReservationCommand
 */
export interface CreateReservationRequest {
  vehicleId: string;
  customerId: string;
  categoryCode: string;
  pickupDate: string; // ISO 8601 format
  returnDate: string; // ISO 8601 format
  pickupLocationCode: string;
  dropoffLocationCode: string;
  totalPriceNet?: number; // Optional - will be calculated if not provided
}

/**
 * Create reservation result matching backend CreateReservationResult
 */
export interface CreateReservationResult {
  reservationId: string;
  status: string;
  totalPriceNet: number;
  totalPriceVat: number;
  totalPriceGross: number;
}

/**
 * Reservation data model
 */
export interface Reservation {
  id: string;
  vehicleId: string;
  customerId: string;
  pickupDate: string;
  returnDate: string;
  pickupLocationCode: string;
  dropoffLocationCode: string;
  totalPriceNet: number;
  totalPriceVat: number;
  totalPriceGross: number;
  status: string;
  createdAt: string;
}
