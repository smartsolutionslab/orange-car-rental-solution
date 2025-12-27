/**
 * Mock reservation data for testing
 */
import type {
  Reservation,
  ReservationId,
  CustomerId,
  ReservationStatus,
} from '@orange-car-rental/reservation-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import type { Price, Currency } from '../types';
import { GERMAN_VAT_MULTIPLIER } from '../constants/business-rules.constants';
import { getFutureDate, getPastDate } from './test-helpers';
import { TEST_VEHICLE_IDS } from './mock-vehicles';

/**
 * Test reservation IDs
 */
export const TEST_RESERVATION_IDS = {
  CONFIRMED: '123e4567-e89b-12d3-a456-426614174000' as ReservationId,
  PENDING: '223e4567-e89b-12d3-a456-426614174001' as ReservationId,
  COMPLETED: '323e4567-e89b-12d3-a456-426614174002' as ReservationId,
  CANCELLED: '423e4567-e89b-12d3-a456-426614174003' as ReservationId,
  ACTIVE: '523e4567-e89b-12d3-a456-426614174004' as ReservationId,
} as const;

/**
 * Test customer IDs
 */
export const TEST_CUSTOMER_IDS = {
  HANS_MUELLER: '11111111-1111-1111-1111-111111111111' as CustomerId,
  ANNA_SCHMIDT: '22222222-2222-2222-2222-222222222222' as CustomerId,
  MAX_WEBER: '33333333-3333-3333-3333-333333333333' as CustomerId,
} as const;

/**
 * Test location codes (extended format with suffix)
 */
export const TEST_LOCATION_CODES = {
  BERLIN_HBF: 'BER-HBF' as LocationCode,
  MUNICH_AIRPORT: 'MUC-FLG' as LocationCode,
  FRANKFURT_CITY: 'FRA-CTY' as LocationCode,
  HAMBURG_CITY: 'HAM-CTY' as LocationCode,
} as const;

/**
 * Short location codes (for call-center-portal and integration tests)
 */
export const SHORT_LOCATION_CODES = {
  MUNICH: 'MUC' as LocationCode,
  BERLIN: 'BER' as LocationCode,
  FRANKFURT: 'FRA' as LocationCode,
  HAMBURG: 'HAM' as LocationCode,
} as const;

/**
 * Create a mock reservation with sensible defaults
 */
export function createMockReservation(overrides: Partial<Reservation> = {}): Reservation {
  const totalPriceNet = (overrides.totalPriceNet as number) ?? 250.0;
  const totalPriceVat = totalPriceNet * (GERMAN_VAT_MULTIPLIER - 1);
  const totalPriceGross = totalPriceNet * GERMAN_VAT_MULTIPLIER;

  return {
    id: TEST_RESERVATION_IDS.CONFIRMED,
    vehicleId: TEST_VEHICLE_IDS.VW_GOLF,
    customerId: TEST_CUSTOMER_IDS.HANS_MUELLER,
    pickupDate: getFutureDate(7),
    returnDate: getFutureDate(12),
    pickupLocationCode: TEST_LOCATION_CODES.BERLIN_HBF,
    dropoffLocationCode: TEST_LOCATION_CODES.BERLIN_HBF,
    totalPriceNet: totalPriceNet as Price,
    totalPriceVat: Math.round(totalPriceVat * 100) / 100 as Price,
    totalPriceGross: Math.round(totalPriceGross * 100) / 100 as Price,
    currency: 'EUR' as Currency,
    status: 'Confirmed' as ReservationStatus,
    createdAt: getPastDate(3),
    ...overrides,
  };
}

/**
 * Pre-built mock reservations for common test scenarios
 */
export const MOCK_RESERVATIONS = {
  /** Upcoming confirmed reservation */
  CONFIRMED: createMockReservation({
    id: TEST_RESERVATION_IDS.CONFIRMED,
    status: 'Confirmed' as ReservationStatus,
    pickupDate: getFutureDate(7),
    returnDate: getFutureDate(12),
    totalPriceNet: 250.0 as Price,
  }),

  /** Pending reservation awaiting confirmation */
  PENDING: createMockReservation({
    id: TEST_RESERVATION_IDS.PENDING,
    vehicleId: TEST_VEHICLE_IDS.BMW_3ER,
    customerId: TEST_CUSTOMER_IDS.ANNA_SCHMIDT,
    status: 'Pending' as ReservationStatus,
    pickupDate: getFutureDate(14),
    returnDate: getFutureDate(17),
    pickupLocationCode: TEST_LOCATION_CODES.MUNICH_AIRPORT,
    dropoffLocationCode: TEST_LOCATION_CODES.MUNICH_AIRPORT,
    totalPriceNet: 420.15 as Price,
  }),

  /** Past completed reservation */
  COMPLETED: createMockReservation({
    id: TEST_RESERVATION_IDS.COMPLETED,
    vehicleId: TEST_VEHICLE_IDS.AUDI_A4,
    customerId: TEST_CUSTOMER_IDS.MAX_WEBER,
    status: 'Completed' as ReservationStatus,
    pickupDate: getPastDate(30),
    returnDate: getPastDate(25),
    pickupLocationCode: TEST_LOCATION_CODES.FRANKFURT_CITY,
    dropoffLocationCode: TEST_LOCATION_CODES.FRANKFURT_CITY,
    totalPriceNet: 399.15 as Price,
  }),

  /** Cancelled reservation */
  CANCELLED: createMockReservation({
    id: TEST_RESERVATION_IDS.CANCELLED,
    status: 'Cancelled' as ReservationStatus,
    pickupDate: getFutureDate(3),
    returnDate: getFutureDate(5),
    totalPriceNet: 150.0 as Price,
  }),

  /** Currently active reservation */
  ACTIVE: createMockReservation({
    id: TEST_RESERVATION_IDS.ACTIVE,
    vehicleId: TEST_VEHICLE_IDS.OPEL_ASTRA,
    status: 'Active' as ReservationStatus,
    pickupDate: getPastDate(2),
    returnDate: getFutureDate(3),
    totalPriceNet: 225.0 as Price,
  }),
} as const;

/**
 * Get an array of mock reservations for list testing
 */
export function getMockReservationList(count: number = 3): Reservation[] {
  const allReservations = [
    MOCK_RESERVATIONS.CONFIRMED,
    MOCK_RESERVATIONS.PENDING,
    MOCK_RESERVATIONS.COMPLETED,
  ];
  return allReservations.slice(0, Math.min(count, allReservations.length));
}

/**
 * Create a mock reservation search response
 */
export function createMockReservationSearchResponse(
  reservations: Reservation[] = getMockReservationList(),
  totalCount?: number,
) {
  return {
    items: reservations,
    totalCount: totalCount ?? reservations.length,
    pageNumber: 1,
    pageSize: 100,
    totalPages: 1,
  };
}
