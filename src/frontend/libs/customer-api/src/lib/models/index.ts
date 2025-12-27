// Types
export * from './customer-display-name.type';

// Re-export CustomerId from reservation-api
export type { CustomerId } from '@orange-car-rental/reservation-api';

// DTOs
export * from './customer-profile.dto';
export * from './update-customer-profile-request.dto';
export * from './partial-customer-update.dto';

// Utilities
export * from './get-customer-display-name.util';
