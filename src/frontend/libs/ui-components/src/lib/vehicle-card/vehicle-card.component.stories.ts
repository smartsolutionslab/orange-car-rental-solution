import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { VehicleCardComponent } from './vehicle-card.component';
import type { Vehicle } from '@orange-car-rental/vehicle-api';

// Mock vehicle data - use type assertion for branded types
const mockVehicle = {
  id: 'v-001',
  name: 'BMW 3 Series',
  manufacturer: 'BMW',
  model: '320i',
  year: 2024,
  categoryCode: 'SEDAN',
  categoryName: 'Sedan',
  fuelType: 'Petrol',
  transmissionType: 'Automatic',
  seats: 5,
  dailyRateNet: 75.0,
  dailyRateVat: 14.25,
  dailyRateGross: 89.25,
  currency: 'EUR',
  status: 'Available',
  locationCode: 'MUC',
  city: 'München',
  imageUrl: 'https://images.unsplash.com/photo-1555215695-3004980ad54e?w=400',
} as unknown as Vehicle;

const mockVehicleNoImage = {
  ...mockVehicle,
  id: 'v-002',
  name: 'Volkswagen Golf',
  manufacturer: 'Volkswagen',
  model: 'Golf 8',
  categoryCode: 'COMPACT',
  categoryName: 'Compact',
  dailyRateGross: 59.9,
  imageUrl: null,
} as unknown as Vehicle;

const mockExpensiveVehicle = {
  ...mockVehicle,
  id: 'v-003',
  name: 'Mercedes E-Class',
  manufacturer: 'Mercedes',
  model: 'E 300',
  categoryCode: 'LUXURY',
  categoryName: 'Luxury',
  dailyRateGross: 149.0,
} as unknown as Vehicle;

const meta: Meta<VehicleCardComponent> = {
  title: 'Components/VehicleCard',
  component: VehicleCardComponent,
  decorators: [
    moduleMetadata({
      imports: [TranslateModule.forRoot()],
      providers: [
        {
          provide: TranslateService,
          useValue: {
            instant: (key: string, params?: object) => {
              const translations: Record<string, string> = {
                'vehicles.card.seats': `${(params as { count: number })?.count || 0} Sitze`,
                'vehicles.card.perDay': '/ Tag',
                'vehicles.card.inclVat': 'inkl. MwSt.',
                'common.actions.bookNow': 'Jetzt buchen',
                'similarVehicles.cheaper': `${(params as { amount: string })?.amount || '0'} € günstiger`,
                'similarVehicles.moreExpensive': `${(params as { amount: string })?.amount || '0'} € teurer`,
                'similarVehicles.samePrice': 'Gleicher Preis',
                'vehicles.fuelType.petrol': 'Benzin',
                'vehicles.fuelType.diesel': 'Diesel',
                'vehicles.fuelType.electric': 'Elektro',
                'vehicles.fuelType.hybrid': 'Hybrid',
                'vehicles.transmission.automatic': 'Automatik',
                'vehicles.transmission.manual': 'Schaltgetriebe',
              };
              return translations[key] || key;
            },
          },
        },
      ],
    }),
  ],
  tags: ['autodocs'],
  argTypes: {
    variant: {
      control: { type: 'select' },
      options: ['browse', 'similar', 'compact'],
    },
    showLocation: { control: 'boolean' },
    showPriceComparison: { control: 'boolean' },
    actionLabel: { control: 'text' },
    similarityReason: { control: 'text' },
  },
};

export default meta;
type Story = StoryObj<VehicleCardComponent>;

/**
 * Default browse variant - used in vehicle list pages
 */
export const Browse: Story = {
  args: {
    vehicle: mockVehicle,
    variant: 'browse',
    showLocation: true,
    showPriceComparison: false,
    actionLabel: null,
    similarityReason: null,
  },
};

/**
 * Browse variant without image - shows placeholder
 */
export const BrowseNoImage: Story = {
  args: {
    vehicle: mockVehicleNoImage,
    variant: 'browse',
    showLocation: true,
  },
};

/**
 * Similar variant - used for vehicle alternatives
 */
export const Similar: Story = {
  args: {
    vehicle: mockVehicleNoImage,
    variant: 'similar',
    showPriceComparison: true,
    comparisonVehicle: mockVehicle,
    similarityReason: 'Gleiche Kategorie • Günstiger • Automatik',
    actionLabel: 'Stattdessen buchen',
    showLocation: false,
  },
};

/**
 * Similar variant - more expensive option
 */
export const SimilarMoreExpensive: Story = {
  args: {
    vehicle: mockExpensiveVehicle,
    variant: 'similar',
    showPriceComparison: true,
    comparisonVehicle: mockVehicle,
    similarityReason: 'Premium Alternative • Mehr Komfort',
    actionLabel: 'Stattdessen buchen',
    showLocation: false,
  },
};

/**
 * Compact variant - smaller card for grids
 */
export const Compact: Story = {
  args: {
    vehicle: mockVehicle,
    variant: 'compact',
    showLocation: true,
    showPriceComparison: false,
  },
};

/**
 * Custom action label
 */
export const CustomAction: Story = {
  args: {
    vehicle: mockVehicle,
    variant: 'browse',
    actionLabel: 'Details ansehen',
    showLocation: true,
  },
};

/**
 * Without location display
 */
export const NoLocation: Story = {
  args: {
    vehicle: mockVehicle,
    variant: 'browse',
    showLocation: false,
  },
};
