/**
 * Transmission type labels (German)
 */
import type { TransmissionType } from './transmission-type.enum';
import { TransmissionType as TT } from './transmission-type.enum';

export const TransmissionTypeLabel: Record<TransmissionType, string> = {
  [TT.Manual]: 'Schaltgetriebe',
  [TT.Automatic]: 'Automatik',
};
