import type { Meta, StoryObj } from "@storybook/angular";
import { StatCardComponent } from "./stat-card.component";

const meta: Meta<StatCardComponent> = {
  title: "Components/DataDisplay/StatCard",
  component: StatCardComponent,
  tags: ["autodocs"],
  argTypes: {
    label: {
      control: "text",
      description: "The title/label for the statistic",
    },
    value: {
      control: "text",
      description: "The value to display (number or string)",
    },
    subtitle: {
      control: "text",
      description: "Optional subtitle or additional info",
    },
    variant: {
      control: "select",
      options: ["default", "success", "warning", "info", "error"],
      description: "Visual variant for the card icon",
    },
    loading: {
      control: "boolean",
      description: "Whether to show loading state",
    },
  },
  parameters: {
    docs: {
      description: {
        component:
          "A statistic card for dashboards. Displays a value with label, optional subtitle, and icon. Supports multiple color variants.",
      },
    },
  },
};

export default meta;
type Story = StoryObj<StatCardComponent>;

export const Default: Story = {
  args: {
    label: "Gesamte Fahrzeuge",
    value: 42,
    subtitle: "+5 seit letztem Monat",
    variant: "default",
    loading: false,
  },
};

export const Success: Story = {
  args: {
    label: "Verfügbare Fahrzeuge",
    value: 28,
    subtitle: "Bereit zur Vermietung",
    variant: "success",
    loading: false,
  },
};

export const Warning: Story = {
  args: {
    label: "In Wartung",
    value: 8,
    subtitle: "3 planmäßig, 5 ungeplant",
    variant: "warning",
    loading: false,
  },
};

export const Info: Story = {
  args: {
    label: "Aktive Reservierungen",
    value: 156,
    subtitle: "Diese Woche",
    variant: "info",
    loading: false,
  },
};

export const Error: Story = {
  args: {
    label: "Überfällige Rückgaben",
    value: 3,
    subtitle: "Sofortige Aktion erforderlich",
    variant: "error",
    loading: false,
  },
};

export const Loading: Story = {
  args: {
    label: "Lädt...",
    value: "-",
    subtitle: "",
    variant: "default",
    loading: true,
  },
};

export const WithStringValue: Story = {
  args: {
    label: "Durchschnittliche Bewertung",
    value: "4.8 / 5.0",
    subtitle: "Basierend auf 234 Bewertungen",
    variant: "success",
    loading: false,
  },
};

export const WithoutSubtitle: Story = {
  args: {
    label: "Standorte",
    value: 12,
    variant: "default",
    loading: false,
  },
};

export const AllVariants: Story = {
  render: () => ({
    template: `
      <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem;">
        <ui-stat-card label="Default" [value]="42" subtitle="Standard variant" variant="default"></ui-stat-card>
        <ui-stat-card label="Success" [value]="28" subtitle="Positive indicator" variant="success"></ui-stat-card>
        <ui-stat-card label="Warning" [value]="8" subtitle="Needs attention" variant="warning"></ui-stat-card>
        <ui-stat-card label="Info" [value]="156" subtitle="Informational" variant="info"></ui-stat-card>
        <ui-stat-card label="Error" [value]="3" subtitle="Critical issue" variant="error"></ui-stat-card>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "All available color variants for the stat card component.",
      },
    },
  },
};

export const DashboardExample: Story = {
  render: () => ({
    template: `
      <div style="padding: 1.5rem; background: #f9fafb; border-radius: 0.5rem;">
        <h2 style="margin: 0 0 1.5rem 0; font-size: 1.25rem; font-weight: 600;">Dashboard Übersicht</h2>
        <div style="display: grid; grid-template-columns: repeat(4, 1fr); gap: 1rem;">
          <ui-stat-card label="Gesamte Fahrzeuge" [value]="42" subtitle="+5 diesen Monat" variant="default"></ui-stat-card>
          <ui-stat-card label="Verfügbar" [value]="28" subtitle="Bereit zur Vermietung" variant="success"></ui-stat-card>
          <ui-stat-card label="Vermietet" [value]="11" subtitle="Aktuell unterwegs" variant="info"></ui-stat-card>
          <ui-stat-card label="In Wartung" [value]="3" subtitle="Planmäßige Inspektion" variant="warning"></ui-stat-card>
        </div>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Example of stat cards arranged in a dashboard layout.",
      },
    },
  },
};
