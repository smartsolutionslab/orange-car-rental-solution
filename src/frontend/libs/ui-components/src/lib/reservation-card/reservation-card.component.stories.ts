import type { Meta, StoryObj } from "@storybook/angular";
import { ReservationCardComponent, type ReservationCardData } from "./reservation-card.component";

const meta: Meta<ReservationCardComponent> = {
  title: "Data Display/Reservation Card",
  component: ReservationCardComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A reusable reservation card component that displays reservation information with variant-specific styling and actions.

## Features
- Multiple variants: upcoming, pending, past, guest
- Status badge integration
- Formatted dates and prices (German locale)
- Optional location display
- Action buttons: Details, Print, Cancel

## Variants
- **upcoming**: Default style for confirmed reservations
- **pending**: Yellow border for pending confirmation
- **past**: Muted style for completed reservations
- **guest**: For guest lookups, shows locations

## Usage

\`\`\`html
<ui-reservation-card
  [reservation]="reservation"
  variant="upcoming"
  [canCancel]="true"
  (viewDetails)="onViewDetails($event)"
  (cancel)="onCancel($event)"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    variant: {
      control: "select",
      options: ["upcoming", "pending", "past", "guest"],
      description: "Visual variant of the card",
    },
    canCancel: {
      control: "boolean",
      description: "Whether cancellation is allowed",
    },
    showDetails: {
      control: "boolean",
      description: "Show details button",
    },
    showPrint: {
      control: "boolean",
      description: "Show print button",
    },
    showCancel: {
      control: "boolean",
      description: "Show cancel button",
    },
  },
};

export default meta;
type Story = StoryObj<ReservationCardComponent>;

const confirmedReservation: ReservationCardData = {
  id: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  status: "Confirmed",
  pickupDate: "2024-02-15",
  returnDate: "2024-02-18",
  pickupLocationCode: "MUC",
  dropoffLocationCode: "MUC",
  totalPriceGross: 249.99,
  currency: "EUR",
};

const pendingReservation: ReservationCardData = {
  id: "b2c3d4e5-f6a7-8901-bcde-f23456789012",
  status: "Pending",
  pickupDate: "2024-02-20",
  returnDate: "2024-02-22",
  pickupLocationCode: "FRA",
  dropoffLocationCode: "FRA",
  totalPriceGross: 189.50,
  currency: "EUR",
};

const completedReservation: ReservationCardData = {
  id: "c3d4e5f6-a7b8-9012-cdef-345678901234",
  status: "Completed",
  pickupDate: "2024-01-10",
  returnDate: "2024-01-12",
  pickupLocationCode: "BER",
  dropoffLocationCode: "BER",
  totalPriceGross: 159.00,
  currency: "EUR",
};

const cancelledReservation: ReservationCardData = {
  id: "d4e5f6a7-b8c9-0123-defa-456789012345",
  status: "Cancelled",
  pickupDate: "2024-01-05",
  returnDate: "2024-01-07",
  pickupLocationCode: "HAM",
  dropoffLocationCode: "HAM",
  totalPriceGross: 129.00,
  currency: "EUR",
};

/**
 * Upcoming confirmed reservation
 */
export const Upcoming: Story = {
  args: {
    reservation: confirmedReservation,
    variant: "upcoming",
    canCancel: true,
    showDetails: true,
    showPrint: false,
    showCancel: true,
  },
};

/**
 * Pending reservation awaiting confirmation
 */
export const Pending: Story = {
  args: {
    reservation: pendingReservation,
    variant: "pending",
    canCancel: true,
    showDetails: true,
    showCancel: true,
  },
};

/**
 * Completed past reservation
 */
export const Past: Story = {
  args: {
    reservation: completedReservation,
    variant: "past",
    canCancel: false,
    showDetails: true,
    showPrint: true,
    showCancel: false,
  },
};

/**
 * Cancelled reservation
 */
export const Cancelled: Story = {
  args: {
    reservation: cancelledReservation,
    variant: "past",
    canCancel: false,
    showDetails: true,
    showCancel: false,
  },
};

/**
 * Guest lookup variant with locations shown
 */
export const GuestLookup: Story = {
  args: {
    reservation: confirmedReservation,
    variant: "guest",
    canCancel: true,
    showDetails: true,
    showPrint: true,
    showCancel: true,
  },
};

/**
 * Card without action buttons
 */
export const NoActions: Story = {
  args: {
    reservation: confirmedReservation,
    variant: "upcoming",
    showDetails: false,
    showPrint: false,
    showCancel: false,
  },
};

/**
 * Multiple cards in a grid
 */
export const CardGrid: Story = {
  render: () => ({
    props: {
      confirmed: confirmedReservation,
      pending: pendingReservation,
      completed: completedReservation,
      cancelled: cancelledReservation,
    },
    template: `
      <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1rem;">
        <ui-reservation-card
          [reservation]="confirmed"
          variant="upcoming"
          [canCancel]="true"
        ></ui-reservation-card>

        <ui-reservation-card
          [reservation]="pending"
          variant="pending"
          [canCancel]="true"
        ></ui-reservation-card>

        <ui-reservation-card
          [reservation]="completed"
          variant="past"
          [showPrint]="true"
          [showCancel]="false"
        ></ui-reservation-card>

        <ui-reservation-card
          [reservation]="cancelled"
          variant="past"
          [showCancel]="false"
        ></ui-reservation-card>
      </div>
    `,
  }),
};
