import type { Meta, StoryObj } from '@storybook/angular';
import { StatusBadgeComponent } from './status-badge.component';

const meta: Meta<StatusBadgeComponent> = {
  title: 'Components/StatusBadge',
  component: StatusBadgeComponent,
  tags: ['autodocs'],
  argTypes: {
    type: {
      control: 'radio',
      options: ['vehicle', 'reservation'],
      description: 'The type of status badge to display',
    },
    status: {
      control: 'select',
      options: [
        'Available',
        'Rented',
        'Maintenance',
        'OutOfService',
        'Confirmed',
        'Active',
        'Pending',
        'Completed',
        'Cancelled',
      ],
      description: 'The status value to display',
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
## Status Badge Component

Displays a styled badge for vehicle or reservation status.

### Usage

\`\`\`html
<ui-status-badge type="vehicle" status="Available"></ui-status-badge>
<ui-status-badge type="reservation" status="Confirmed"></ui-status-badge>
\`\`\`

### Status Types

**Vehicle statuses:** Available, Rented, Maintenance, OutOfService

**Reservation statuses:** Confirmed, Active, Pending, Completed, Cancelled
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<StatusBadgeComponent>;

// Vehicle Status Stories
export const VehicleAvailable: Story = {
  args: {
    type: 'vehicle',
    status: 'Available',
  },
};

export const VehicleRented: Story = {
  args: {
    type: 'vehicle',
    status: 'Rented',
  },
};

export const VehicleMaintenance: Story = {
  args: {
    type: 'vehicle',
    status: 'Maintenance',
  },
};

export const VehicleOutOfService: Story = {
  args: {
    type: 'vehicle',
    status: 'OutOfService',
  },
};

// Reservation Status Stories
export const ReservationConfirmed: Story = {
  args: {
    type: 'reservation',
    status: 'Confirmed',
  },
};

export const ReservationActive: Story = {
  args: {
    type: 'reservation',
    status: 'Active',
  },
};

export const ReservationPending: Story = {
  args: {
    type: 'reservation',
    status: 'Pending',
  },
};

export const ReservationCompleted: Story = {
  args: {
    type: 'reservation',
    status: 'Completed',
  },
};

export const ReservationCancelled: Story = {
  args: {
    type: 'reservation',
    status: 'Cancelled',
  },
};

// All Vehicle Statuses
export const AllVehicleStatuses: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 8px; flex-wrap: wrap;">
        <ui-status-badge type="vehicle" status="Available"></ui-status-badge>
        <ui-status-badge type="vehicle" status="Rented"></ui-status-badge>
        <ui-status-badge type="vehicle" status="Maintenance"></ui-status-badge>
        <ui-status-badge type="vehicle" status="OutOfService"></ui-status-badge>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'All available vehicle statuses displayed together.',
      },
    },
  },
};

// All Reservation Statuses
export const AllReservationStatuses: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 8px; flex-wrap: wrap;">
        <ui-status-badge type="reservation" status="Confirmed"></ui-status-badge>
        <ui-status-badge type="reservation" status="Active"></ui-status-badge>
        <ui-status-badge type="reservation" status="Pending"></ui-status-badge>
        <ui-status-badge type="reservation" status="Completed"></ui-status-badge>
        <ui-status-badge type="reservation" status="Cancelled"></ui-status-badge>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'All available reservation statuses displayed together.',
      },
    },
  },
};
