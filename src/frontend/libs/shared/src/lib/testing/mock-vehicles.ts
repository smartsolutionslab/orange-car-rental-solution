/**
 * Mock vehicle data for testing
 */
import type {
  Vehicle,
  VehicleId,
  VehicleName,
  CategoryCode,
  CategoryName,
  SeatingCapacity,
  DailyRate,
  FuelType,
  TransmissionType,
  VehicleStatus,
  LicensePlate,
  Manufacturer,
  VehicleModel,
  ManufacturingYear,
  ImageUrl,
} from "@orange-car-rental/vehicle-api";
import type { LocationCode, CityName } from "@orange-car-rental/location-api";
import type { Currency } from "../types/common/currency.type";
import { GERMAN_VAT_MULTIPLIER } from "../constants/business-rules.constants";

/**
 * Test vehicle IDs
 */
export const TEST_VEHICLE_IDS = {
  VW_GOLF: "123e4567-e89b-12d3-a456-426614174000" as VehicleId,
  BMW_3ER: "223e4567-e89b-12d3-a456-426614174001" as VehicleId,
  AUDI_A4: "323e4567-e89b-12d3-a456-426614174002" as VehicleId,
  OPEL_ASTRA: "423e4567-e89b-12d3-a456-426614174003" as VehicleId,
  FORD_FOCUS: "523e4567-e89b-12d3-a456-426614174004" as VehicleId,
  MERCEDES_E: "623e4567-e89b-12d3-a456-426614174005" as VehicleId,
} as const;

/**
 * Create a mock vehicle with sensible defaults
 */
export function createMockVehicle(overrides: Partial<Vehicle> = {}): Vehicle {
  const dailyRateNet = (overrides.dailyRateNet as number) ?? 50.0;
  const dailyRateVat = dailyRateNet * (GERMAN_VAT_MULTIPLIER - 1);
  const dailyRateGross = dailyRateNet * GERMAN_VAT_MULTIPLIER;

  return {
    id: TEST_VEHICLE_IDS.VW_GOLF,
    name: "VW Golf" as VehicleName,
    categoryCode: "MITTEL" as CategoryCode,
    categoryName: "Mittelklasse" as CategoryName,
    locationCode: "BER-HBF" as LocationCode,
    city: "Berlin" as CityName,
    dailyRateNet: dailyRateNet as DailyRate,
    dailyRateVat: (Math.round(dailyRateVat * 100) / 100) as DailyRate,
    dailyRateGross: (Math.round(dailyRateGross * 100) / 100) as DailyRate,
    currency: "EUR" as Currency,
    seats: 5 as SeatingCapacity,
    fuelType: "Petrol" as FuelType,
    transmissionType: "Manual" as TransmissionType,
    status: "Available" as VehicleStatus,
    licensePlate: "B-AB 1234" as LicensePlate,
    manufacturer: "Volkswagen" as Manufacturer,
    model: "Golf 8" as VehicleModel,
    year: 2023 as ManufacturingYear,
    imageUrl: null as ImageUrl | null,
    ...overrides,
  };
}

/**
 * Pre-built mock vehicles for common test scenarios
 */
export const MOCK_VEHICLES = {
  /** VW Golf - Standard mid-range vehicle */
  VW_GOLF: createMockVehicle({
    id: TEST_VEHICLE_IDS.VW_GOLF,
    name: "VW Golf" as VehicleName,
    categoryCode: "MITTEL" as CategoryCode,
    categoryName: "Mittelklasse" as CategoryName,
    dailyRateNet: 50.0 as DailyRate,
    manufacturer: "Volkswagen" as Manufacturer,
    model: "Golf 8" as VehicleModel,
    licensePlate: "B-AB 1234" as LicensePlate,
  }),

  /** BMW 3er - Premium vehicle */
  BMW_3ER: createMockVehicle({
    id: TEST_VEHICLE_IDS.BMW_3ER,
    name: "BMW 3er" as VehicleName,
    categoryCode: "PREMIUM" as CategoryCode,
    categoryName: "Premium" as CategoryName,
    dailyRateNet: 84.03 as DailyRate,
    manufacturer: "BMW" as Manufacturer,
    model: "320i" as VehicleModel,
    licensePlate: "M-BM 5678" as LicensePlate,
    locationCode: "MUC-FLG" as LocationCode,
    city: "München" as CityName,
    transmissionType: "Automatic" as TransmissionType,
  }),

  /** Audi A4 - Premium vehicle */
  AUDI_A4: createMockVehicle({
    id: TEST_VEHICLE_IDS.AUDI_A4,
    name: "Audi A4" as VehicleName,
    categoryCode: "PREMIUM" as CategoryCode,
    categoryName: "Premium" as CategoryName,
    dailyRateNet: 79.83 as DailyRate,
    manufacturer: "Audi" as Manufacturer,
    model: "A4 Avant" as VehicleModel,
    licensePlate: "M-AU 9012" as LicensePlate,
    locationCode: "MUC-FLG" as LocationCode,
    city: "München" as CityName,
    fuelType: "Diesel" as FuelType,
    transmissionType: "Automatic" as TransmissionType,
  }),

  /** Opel Astra - Economy option */
  OPEL_ASTRA: createMockVehicle({
    id: TEST_VEHICLE_IDS.OPEL_ASTRA,
    name: "Opel Astra" as VehicleName,
    categoryCode: "MITTEL" as CategoryCode,
    categoryName: "Mittelklasse" as CategoryName,
    dailyRateNet: 45.0 as DailyRate,
    manufacturer: "Opel" as Manufacturer,
    model: "Astra" as VehicleModel,
    licensePlate: "B-CD 5678" as LicensePlate,
  }),

  /** Ford Focus - Diesel option */
  FORD_FOCUS: createMockVehicle({
    id: TEST_VEHICLE_IDS.FORD_FOCUS,
    name: "Ford Focus" as VehicleName,
    categoryCode: "KOMPAKT" as CategoryCode,
    categoryName: "Kompaktklasse" as CategoryName,
    dailyRateNet: 40.0 as DailyRate,
    manufacturer: "Ford" as Manufacturer,
    model: "Focus" as VehicleModel,
    licensePlate: "B-EF 9012" as LicensePlate,
    fuelType: "Diesel" as FuelType,
    year: 2022 as ManufacturingYear,
  }),

  /** Rented vehicle for status testing */
  RENTED_VEHICLE: createMockVehicle({
    id: TEST_VEHICLE_IDS.MERCEDES_E,
    name: "Mercedes E-Klasse" as VehicleName,
    categoryCode: "LUXURY" as CategoryCode,
    categoryName: "Luxusklasse" as CategoryName,
    dailyRateNet: 120.0 as DailyRate,
    manufacturer: "Mercedes-Benz" as Manufacturer,
    model: "E 300" as VehicleModel,
    licensePlate: "M-MB 3456" as LicensePlate,
    status: "Rented" as VehicleStatus,
    locationCode: "MUC-FLG" as LocationCode,
    city: "München" as CityName,
    transmissionType: "Automatic" as TransmissionType,
  }),
} as const;

/**
 * Get an array of mock vehicles for list testing
 */
export function getMockVehicleList(count: number = 3): Vehicle[] {
  const allVehicles = Object.values(MOCK_VEHICLES);
  return allVehicles.slice(0, Math.min(count, allVehicles.length));
}
