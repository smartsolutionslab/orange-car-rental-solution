/**
 * Transmission type enum - use TransmissionType.Manual instead of 'Manual'
 */
export const TransmissionType = {
  Manual: 'Manual',
  Automatic: 'Automatic',
} as const;

export type TransmissionType = (typeof TransmissionType)[keyof typeof TransmissionType];
