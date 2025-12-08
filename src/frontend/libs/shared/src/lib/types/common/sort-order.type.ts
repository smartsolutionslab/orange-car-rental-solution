/**
 * Sort order enum - use SortOrder.Asc instead of 'asc'
 */
export const SortOrder = {
  Asc: 'asc',
  Desc: 'desc',
} as const;

export type SortOrder = (typeof SortOrder)[keyof typeof SortOrder];
