/**
 * Shared testing utilities and mock data
 *
 * Usage:
 * ```typescript
 * import {
 *   MOCK_VEHICLES,
 *   MOCK_RESERVATIONS,
 *   MOCK_CUSTOMERS,
 *   TEST_EMAILS,
 *   TEST_PASSWORDS,
 *   createMockVehicle,
 *   getFutureDate,
 * } from '@orange-car-rental/shared/testing';
 * ```
 */

// Test helpers
export * from "./test-helpers";

// Mock data factories and fixtures
export * from "./mock-vehicles";
export * from "./mock-reservations";
export * from "./mock-customers";
export * from "./mock-locations";
export * from "./mock-auth";
