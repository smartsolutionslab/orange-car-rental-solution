/**
 * Reservation status labels (German)
 */
import type { ReservationStatus } from "./reservation-status.enum";
import { ReservationStatus as RS } from "./reservation-status.enum";

export const ReservationStatusLabel: Record<ReservationStatus, string> = {
  [RS.Pending]: "Ausstehend",
  [RS.Confirmed]: "Bestätigt",
  [RS.Active]: "Aktiv",
  [RS.Completed]: "Abgeschlossen",
  [RS.Cancelled]: "Storniert",
};
