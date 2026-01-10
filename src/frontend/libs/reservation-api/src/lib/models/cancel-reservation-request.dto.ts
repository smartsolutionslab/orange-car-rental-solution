/**
 * Cancel reservation request
 * Property name matches backend: CancellationReason (camelCase in JSON)
 */
import type { CancellationReason } from "./cancellation-reason.type";

export type CancelReservationRequest = {
  readonly cancellationReason?: CancellationReason;
};
