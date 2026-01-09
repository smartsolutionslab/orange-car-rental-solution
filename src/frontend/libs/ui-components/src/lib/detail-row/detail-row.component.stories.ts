import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { DetailRowComponent } from "./detail-row.component";
import { StatusBadgeComponent } from "../status-badge/status-badge.component";

const meta: Meta<DetailRowComponent> = {
  title: "Data Display/Detail Row",
  component: DetailRowComponent,
  decorators: [
    moduleMetadata({
      imports: [StatusBadgeComponent],
    }),
  ],
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A reusable detail row component for displaying label-value pairs.
Commonly used in cards, modals, and detail views.

## Features
- Simple label-value display
- Support for custom content via ng-content
- Highlight mode for emphasis
- Price styling variant
- Strong text variant

## Usage

\`\`\`html
<!-- Simple usage -->
<ui-detail-row label="Name" [value]="customer.name"></ui-detail-row>

<!-- With custom content -->
<ui-detail-row label="Status">
  <ui-status-badge [status]="item.status"></ui-status-badge>
</ui-detail-row>

<!-- Highlighted row -->
<ui-detail-row label="Total" [value]="'€124.99'" [highlight]="true" [strong]="true"></ui-detail-row>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    label: {
      control: "text",
      description: "Label text displayed on the left",
    },
    value: {
      control: "text",
      description: "Value text displayed on the right",
    },
    highlight: {
      control: "boolean",
      description: "Apply highlight background",
    },
    isPrice: {
      control: "boolean",
      description: "Apply price styling",
    },
    strong: {
      control: "boolean",
      description: "Make value text bold",
    },
  },
};

export default meta;
type Story = StoryObj<DetailRowComponent>;

/**
 * Basic detail row with label and value
 */
export const Default: Story = {
  args: {
    label: "Kunde",
    value: "Max Mustermann",
  },
};

/**
 * Detail row with highlighted background
 */
export const Highlighted: Story = {
  args: {
    label: "Gesamtpreis",
    value: "€249.99",
    highlight: true,
    strong: true,
  },
};

/**
 * Detail row styled as price
 */
export const Price: Story = {
  args: {
    label: "Tagespreis",
    value: "€49.99",
    isPrice: true,
  },
};

/**
 * Detail row with strong value
 */
export const Strong: Story = {
  args: {
    label: "Fahrzeug",
    value: "BMW 3er",
    strong: true,
  },
};

/**
 * Multiple detail rows in a container
 */
export const MultipleRows: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px; background: white; padding: 1rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
        <ui-detail-row label="Reservierungsnummer" value="RES-2024-001234" [strong]="true"></ui-detail-row>
        <ui-detail-row label="Kunde" value="Max Mustermann"></ui-detail-row>
        <ui-detail-row label="E-Mail" value="max@example.com"></ui-detail-row>
        <ui-detail-row label="Telefon" value="+49 151 12345678"></ui-detail-row>
        <ui-detail-row label="Abholdatum" value="15.01.2024"></ui-detail-row>
        <ui-detail-row label="Rückgabedatum" value="18.01.2024"></ui-detail-row>
        <ui-detail-row label="Gesamtpreis" value="€149.97" [highlight]="true" [strong]="true"></ui-detail-row>
      </div>
    `,
  }),
};

/**
 * Detail row with custom content (status badge)
 */
export const WithCustomContent: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px; background: white; padding: 1rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
        <ui-detail-row label="Reservierungsnummer" value="RES-2024-001234"></ui-detail-row>
        <ui-detail-row label="Status">
          <ui-status-badge status="Confirmed" variant="reservation"></ui-status-badge>
        </ui-detail-row>
        <ui-detail-row label="Fahrzeugstatus">
          <ui-status-badge status="Available" variant="vehicle"></ui-status-badge>
        </ui-detail-row>
      </div>
    `,
  }),
};

/**
 * Invoice-style detail rows
 */
export const InvoiceStyle: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px; background: white; padding: 1rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
        <h3 style="margin: 0 0 1rem; font-size: 1rem; color: #374151;">Kostenübersicht</h3>
        <ui-detail-row label="Mietpreis (3 Tage)" value="€119.97" [isPrice]="true"></ui-detail-row>
        <ui-detail-row label="Vollkaskoversicherung" value="€29.97" [isPrice]="true"></ui-detail-row>
        <ui-detail-row label="Zusatzfahrer" value="€15.00" [isPrice]="true"></ui-detail-row>
        <ui-detail-row label="Zwischensumme" value="€164.94"></ui-detail-row>
        <ui-detail-row label="MwSt. (19%)" value="€31.34"></ui-detail-row>
        <ui-detail-row label="Gesamtbetrag" value="€196.28" [highlight]="true" [strong]="true"></ui-detail-row>
      </div>
    `,
  }),
};
