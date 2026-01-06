/**
 * Mock location data for testing
 */
import type {
  LocationCode,
  LocationName,
  CityName,
  StreetAddress,
  TimeString,
} from "@orange-car-rental/location-api";
import type { PostalCode, CountryCode } from "../types";
import { TEST_LOCATION_CODES } from "./mock-reservations";

/**
 * Location structure for testing
 */
export interface MockLocation {
  code: LocationCode;
  name: LocationName;
  city: CityName;
  address: StreetAddress;
  postalCode: PostalCode;
  country: CountryCode;
  phone: string;
  email: string;
  openingTime: TimeString;
  closingTime: TimeString;
  isActive: boolean;
  vehicleCount?: number;
}

/**
 * Create a mock location with sensible defaults
 */
export function createMockLocation(
  overrides: Partial<MockLocation> = {},
): MockLocation {
  return {
    code: TEST_LOCATION_CODES.BERLIN_HBF,
    name: "Berlin Hauptbahnhof" as LocationName,
    city: "Berlin" as CityName,
    address: "Europaplatz 1" as StreetAddress,
    postalCode: "10557" as PostalCode,
    country: "DE" as CountryCode,
    phone: "+49 30 1234567",
    email: "berlin-hbf@orange-car-rental.de",
    openingTime: "06:00" as TimeString,
    closingTime: "22:00" as TimeString,
    isActive: true,
    vehicleCount: 25,
    ...overrides,
  };
}

/**
 * Pre-built mock locations for common test scenarios
 */
export const MOCK_LOCATIONS = {
  /** Berlin Hauptbahnhof */
  BERLIN_HBF: createMockLocation({
    code: TEST_LOCATION_CODES.BERLIN_HBF,
    name: "Berlin Hauptbahnhof" as LocationName,
    city: "Berlin" as CityName,
    address: "Europaplatz 1" as StreetAddress,
    postalCode: "10557" as PostalCode,
    vehicleCount: 25,
  }),

  /** Munich Airport */
  MUNICH_AIRPORT: createMockLocation({
    code: TEST_LOCATION_CODES.MUNICH_AIRPORT,
    name: "München Flughafen" as LocationName,
    city: "München" as CityName,
    address: "Nordallee 25" as StreetAddress,
    postalCode: "85356" as PostalCode,
    phone: "+49 89 9876543",
    email: "muenchen-flughafen@orange-car-rental.de",
    openingTime: "05:00" as TimeString,
    closingTime: "23:00" as TimeString,
    vehicleCount: 45,
  }),

  /** Frankfurt City */
  FRANKFURT_CITY: createMockLocation({
    code: TEST_LOCATION_CODES.FRANKFURT_CITY,
    name: "Frankfurt City" as LocationName,
    city: "Frankfurt" as CityName,
    address: "Kaiserstraße 50" as StreetAddress,
    postalCode: "60329" as PostalCode,
    phone: "+49 69 5551234",
    email: "frankfurt-city@orange-car-rental.de",
    vehicleCount: 30,
  }),

  /** Hamburg City */
  HAMBURG_CITY: createMockLocation({
    code: TEST_LOCATION_CODES.HAMBURG_CITY,
    name: "Hamburg City" as LocationName,
    city: "Hamburg" as CityName,
    address: "Mönckebergstraße 21" as StreetAddress,
    postalCode: "20095" as PostalCode,
    phone: "+49 40 4445678",
    email: "hamburg-city@orange-car-rental.de",
    vehicleCount: 20,
  }),

  /** Inactive location for testing */
  INACTIVE: createMockLocation({
    code: "DUS-CLO" as LocationCode,
    name: "Düsseldorf Closed" as LocationName,
    city: "Düsseldorf" as CityName,
    address: "Königsallee 1" as StreetAddress,
    postalCode: "40212" as PostalCode,
    isActive: false,
    vehicleCount: 0,
  }),
} as const;

/**
 * Get an array of mock locations for list testing
 */
export function getMockLocationList(count: number = 3): MockLocation[] {
  const allLocations = [
    MOCK_LOCATIONS.BERLIN_HBF,
    MOCK_LOCATIONS.MUNICH_AIRPORT,
    MOCK_LOCATIONS.FRANKFURT_CITY,
    MOCK_LOCATIONS.HAMBURG_CITY,
  ];
  return allLocations.slice(0, Math.min(count, allLocations.length));
}

/**
 * Get location codes as an array
 */
export function getMockLocationCodes(): LocationCode[] {
  return Object.values(TEST_LOCATION_CODES);
}
