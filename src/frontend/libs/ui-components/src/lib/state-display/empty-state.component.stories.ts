import type { Meta, StoryObj } from '@storybook/angular';
import { EmptyStateComponent } from './empty-state.component';

const meta: Meta<EmptyStateComponent> = {
  title: 'Components/Feedback/EmptyState',
  component: EmptyStateComponent,
  tags: ['autodocs'],
  argTypes: {
    icon: {
      control: 'select',
      options: ['default', 'car', 'reservation', 'location', 'search', 'document'],
      description: 'Icon to display',
    },
    title: {
      control: 'text',
      description: 'Main title text',
    },
    description: {
      control: 'text',
      description: 'Optional description text',
    },
    showAction: {
      control: 'boolean',
      description: 'Whether to show the action button',
    },
    actionLabel: {
      control: 'text',
      description: 'Label for the action button',
    },
  },
  parameters: {
    docs: {
      description: {
        component: 'Displays a placeholder when no data is available. Supports different icons, optional description, and an action button.',
      },
    },
  },
};

export default meta;
type Story = StoryObj<EmptyStateComponent>;

export const Default: Story = {
  args: {
    icon: 'default',
    title: 'Keine Daten vorhanden',
    description: 'Es wurden keine Einträge gefunden.',
    showAction: false,
    actionLabel: '',
  },
};

export const NoVehicles: Story = {
  args: {
    icon: 'car',
    title: 'Keine Fahrzeuge gefunden',
    description: 'Es sind derzeit keine Fahrzeuge verfügbar, die Ihren Suchkriterien entsprechen.',
    showAction: true,
    actionLabel: 'Neue Suche starten',
  },
};

export const NoReservations: Story = {
  args: {
    icon: 'reservation',
    title: 'Keine Reservierungen',
    description: 'Sie haben noch keine Reservierungen. Starten Sie jetzt Ihre erste Buchung!',
    showAction: true,
    actionLabel: 'Fahrzeug buchen',
  },
};

export const NoLocations: Story = {
  args: {
    icon: 'location',
    title: 'Keine Standorte gefunden',
    description: 'In Ihrer Nähe wurden keine Mietstationen gefunden.',
    showAction: true,
    actionLabel: 'Standort ändern',
  },
};

export const SearchNoResults: Story = {
  args: {
    icon: 'search',
    title: 'Keine Ergebnisse',
    description: 'Ihre Suche ergab keine Treffer. Versuchen Sie andere Suchbegriffe.',
    showAction: false,
    actionLabel: '',
  },
};

export const NoDocuments: Story = {
  args: {
    icon: 'document',
    title: 'Keine Dokumente',
    description: 'Es sind keine Rechnungen oder Dokumente vorhanden.',
    showAction: false,
    actionLabel: '',
  },
};

export const WithoutDescription: Story = {
  args: {
    icon: 'car',
    title: 'Keine Fahrzeuge verfügbar',
    description: '',
    showAction: false,
    actionLabel: '',
  },
};

export const AllIcons: Story = {
  render: () => ({
    template: `
      <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem;">
        <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem;">
          <ui-empty-state icon="default" title="Default"></ui-empty-state>
        </div>
        <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem;">
          <ui-empty-state icon="car" title="Car"></ui-empty-state>
        </div>
        <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem;">
          <ui-empty-state icon="reservation" title="Reservation"></ui-empty-state>
        </div>
        <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem;">
          <ui-empty-state icon="location" title="Location"></ui-empty-state>
        </div>
        <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem;">
          <ui-empty-state icon="search" title="Search"></ui-empty-state>
        </div>
        <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem;">
          <ui-empty-state icon="document" title="Document"></ui-empty-state>
        </div>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'All available icons for the empty state component.',
      },
    },
  },
};
