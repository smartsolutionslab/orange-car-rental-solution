/**
 * Cancel reservation request
 */
import type { CancellationReason } from './cancellation-reason.type';

export type CancelReservationRequest = {
  readonly reason: CancellationReason;
};
