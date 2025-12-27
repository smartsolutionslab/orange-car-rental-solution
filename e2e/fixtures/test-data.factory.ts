/**
 * Test Data Factory
 *
 * Provides reusable test data builders for E2E and integration tests.
 * Follows the Factory Pattern for consistent test data generation.
 */

import { Reservation, ReservationStatus, VehicleCategory } from '../../src/frontend/apps/public-portal/src/app/services/reservation.model';

export interface ReservationData {
  id?: string;
  customerId?: string;
  customerName?: string;
  email?: string;
  phoneNumber?: string;
  vehicleCategory?: VehicleCategory;
  pickupLocation?: string;
  returnLocation?: string;
  pickupDate?: string;
  returnDate?: string;
  status?: ReservationStatus;
  totalPrice?: number;
  bookingDate?: string;
}

/**
 * Factory class for creating test reservations
 */
export class ReservationFactory {
  private static idCounter = 1000;

  /**
   * Creates a basic reservation with default values
   */
  static createBasic(overrides?: Partial<ReservationData>): Reservation {
    const id = overrides?.id || `RES-${this.idCounter++}`;
    const now = new Date();
    const tomorrow = new Date(now);
    tomorrow.setDate(tomorrow.getDate() + 1);
    const nextWeek = new Date(now);
    nextWeek.setDate(nextWeek.getDate() + 7);

    return {
      id,
      customerId: overrides?.customerId || 'CUST-001',
      customerName: overrides?.customerName || 'Test Customer',
      email: overrides?.email || 'test@example.com',
      phoneNumber: overrides?.phoneNumber || '+49 123 456789',
      vehicleCategory: overrides?.vehicleCategory || 'Kompakt',
      vehicleModel: this.getVehicleModel(overrides?.vehicleCategory || 'Kompakt'),
      pickupLocation: overrides?.pickupLocation || 'Berlin Hauptbahnhof',
      returnLocation: overrides?.returnLocation || 'Berlin Hauptbahnhof',
      pickupDate: overrides?.pickupDate || tomorrow.toISOString(),
      returnDate: overrides?.returnDate || nextWeek.toISOString(),
      status: overrides?.status || 'Confirmed',
      totalPrice: overrides?.totalPrice || 350.00,
      bookingDate: overrides?.bookingDate || now.toISOString(),
      ...overrides
    };
  }

  /**
   * Creates an upcoming reservation
   */
  static createUpcoming(overrides?: Partial<ReservationData>): Reservation {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);

    return this.createBasic({
      status: 'Confirmed',
      pickupDate: tomorrow.toISOString(),
      returnDate: nextWeek.toISOString(),
      ...overrides
    });
  }

  /**
   * Creates a pending reservation
   */
  static createPending(overrides?: Partial<ReservationData>): Reservation {
    return this.createBasic({
      status: 'Pending',
      ...overrides
    });
  }

  /**
   * Creates a cancelled reservation
   */
  static createCancelled(overrides?: Partial<ReservationData>): Reservation {
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);

    return this.createBasic({
      status: 'Cancelled',
      bookingDate: yesterday.toISOString(),
      ...overrides
    });
  }

  /**
   * Creates a completed reservation
   */
  static createCompleted(overrides?: Partial<ReservationData>): Reservation {
    const lastMonth = new Date();
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    const lastWeek = new Date();
    lastWeek.setDate(lastWeek.getDate() - 7);

    return this.createBasic({
      status: 'Completed',
      pickupDate: lastMonth.toISOString(),
      returnDate: lastWeek.toISOString(),
      ...overrides
    });
  }

  /**
   * Creates an active (in-progress) reservation
   */
  static createActive(overrides?: Partial<ReservationData>): Reservation {
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    return this.createBasic({
      status: 'Active',
      pickupDate: yesterday.toISOString(),
      returnDate: tomorrow.toISOString(),
      ...overrides
    });
  }

  /**
   * Creates multiple reservations
   */
  static createMany(count: number, template?: Partial<ReservationData>): Reservation[] {
    return Array.from({ length: count }, (_, index) =>
      this.createBasic({
        ...template,
        id: `RES-${this.idCounter + index}`,
        customerName: `${template?.customerName || 'Test Customer'} ${index + 1}`
      })
    );
  }

  /**
   * Creates a reservation for each status
   */
  static createOneOfEachStatus(): Reservation[] {
    return [
      this.createPending({ id: 'RES-PENDING' }),
      this.createUpcoming({ id: 'RES-CONFIRMED' }),
      this.createActive({ id: 'RES-ACTIVE' }),
      this.createCompleted({ id: 'RES-COMPLETED' }),
      this.createCancelled({ id: 'RES-CANCELLED' })
    ];
  }

  /**
   * Gets a vehicle model for a given category
   */
  private static getVehicleModel(category: VehicleCategory): string {
    const models: Record<VehicleCategory, string[]> = {
      'Mini': ['Fiat 500', 'Smart ForTwo', 'VW Up!'],
      'Kompakt': ['VW Golf', 'Ford Focus', 'Opel Astra'],
      'Mittelklasse': ['VW Passat', 'BMW 3er', 'Mercedes C-Klasse'],
      'SUV': ['VW Tiguan', 'BMW X3', 'Audi Q5'],
      'Luxus': ['BMW 7er', 'Mercedes S-Klasse', 'Audi A8'],
      'Kombi': ['VW Passat Variant', 'BMW 3er Touring', 'Audi A4 Avant'],
      'Transporter': ['VW Transporter', 'Mercedes Sprinter', 'Ford Transit']
    };

    const categoryModels = models[category] || models['Kompakt'];
    return categoryModels[Math.floor(Math.random() * categoryModels.length)];
  }

  /**
   * Reset the ID counter (useful for tests)
   */
  static resetCounter(): void {
    this.idCounter = 1000;
  }
}

/**
 * Factory for creating customer data
 */
export class CustomerFactory {
  static createBasic(overrides?: Partial<any>) {
    return {
      id: overrides?.id || 'CUST-001',
      name: overrides?.name || 'Max Mustermann',
      email: overrides?.email || 'max.mustermann@example.com',
      phoneNumber: overrides?.phoneNumber || '+49 123 456789',
      dateOfBirth: overrides?.dateOfBirth || '1990-01-01',
      driversLicenseNumber: overrides?.driversLicenseNumber || 'D1234567890',
      address: overrides?.address || {
        street: 'Musterstraße 123',
        city: 'Berlin',
        postalCode: '10115',
        country: 'Deutschland'
      },
      ...overrides
    };
  }

  static createAgent(overrides?: Partial<any>) {
    return this.createBasic({
      id: overrides?.id || 'AGENT-001',
      name: overrides?.name || 'Agent Smith',
      email: overrides?.email || 'agent@orangecarrental.com',
      role: 'CallCenterAgent',
      ...overrides
    });
  }
}

/**
 * Factory for creating location data
 */
export class LocationFactory {
  private static locations = [
    'Berlin Hauptbahnhof',
    'Berlin Tegel Airport',
    'München Flughafen',
    'Frankfurt Hauptbahnhof',
    'Hamburg Zentrum',
    'Köln Messe',
    'Stuttgart Hauptbahnhof',
    'Düsseldorf Flughafen'
  ];

  static getRandom(): string {
    return this.locations[Math.floor(Math.random() * this.locations.length)];
  }

  static getAll(): string[] {
    return [...this.locations];
  }
}

/**
 * Utility functions for date manipulation in tests
 */
export class DateFactory {
  static today(): Date {
    return new Date();
  }

  static tomorrow(): Date {
    const date = new Date();
    date.setDate(date.getDate() + 1);
    return date;
  }

  static yesterday(): Date {
    const date = new Date();
    date.setDate(date.getDate() - 1);
    return date;
  }

  static daysFromNow(days: number): Date {
    const date = new Date();
    date.setDate(date.getDate() + days);
    return date;
  }

  static daysAgo(days: number): Date {
    const date = new Date();
    date.setDate(date.getDate() - days);
    return date;
  }

  static toISOString(date: Date): string {
    return date.toISOString();
  }
}
