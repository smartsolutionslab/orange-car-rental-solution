import {
  VehicleStatus,
  VehicleStatusLabel,
} from '@orange-car-rental/vehicle-api';
import {
  ReservationStatus,
  ReservationStatusLabel,
} from '@orange-car-rental/reservation-api';

/**
 * Get CSS class for vehicle status badge
 */
export function getVehicleStatusClass(status: VehicleStatus | string): string {
  switch (status) {
    case VehicleStatus.Available:
      return 'status-success';
    case VehicleStatus.Rented:
      return 'status-info';
    case VehicleStatus.Maintenance:
      return 'status-warning';
    case VehicleStatus.OutOfService:
      return 'status-error';
    default:
      return '';
  }
}

/**
 * Get display label for vehicle status
 */
export function getVehicleStatusLabel(status: VehicleStatus | string): string {
  return VehicleStatusLabel[status as VehicleStatus] ?? status;
}

/**
 * Get CSS class for reservation status badge
 */
export function getReservationStatusClass(status: ReservationStatus | string): string {
  switch (status) {
    case ReservationStatus.Confirmed:
      return 'status-success';
    case ReservationStatus.Active:
      return 'status-info';
    case ReservationStatus.Pending:
      return 'status-warning';
    case ReservationStatus.Completed:
      return 'status-completed';
    case ReservationStatus.Cancelled:
      return 'status-error';
    default:
      return '';
  }
}

/**
 * Get display label for reservation status
 */
export function getReservationStatusLabel(status: ReservationStatus | string): string {
  return ReservationStatusLabel[status as ReservationStatus] ?? status;
}

/**
 * Format date to German locale (dd.MM.yyyy)
 */
export function formatDateDE(dateString: string): string {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('de-DE', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  });
}

/**
 * Format date to German locale with weekday (Montag, 1. Januar 2025)
 */
export function formatDateLongDE(dateString: string): string {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('de-DE', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
}

/**
 * Format price to German currency format
 */
export function formatPriceDE(price: number, currency = 'EUR'): string {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency,
  }).format(price);
}

/**
 * Calculate the number of rental days between two dates
 * @param pickupDate - Start date (ISO string or Date)
 * @param returnDate - End date (ISO string or Date)
 * @returns Number of days between the two dates
 */
export function calculateRentalDays(
  pickupDate: string | Date | null | undefined,
  returnDate: string | Date | null | undefined
): number {
  if (!pickupDate || !returnDate) {
    return 0;
  }

  const pickup = typeof pickupDate === 'string' ? new Date(pickupDate) : pickupDate;
  const returnD = typeof returnDate === 'string' ? new Date(returnDate) : returnDate;
  const diffTime = Math.abs(returnD.getTime() - pickup.getTime());
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

  return diffDays;
}
