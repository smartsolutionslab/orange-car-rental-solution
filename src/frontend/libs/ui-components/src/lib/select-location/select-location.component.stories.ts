import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectLocationComponent } from "./select-location.component";

const meta: Meta<SelectLocationComponent> = {
  title: "Forms/Select Location",
  component: SelectLocationComponent,
  decorators: [
    moduleMetadata({
      imports: [FormsModule],
    }),
  ],
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A select component for rental locations that auto-loads options from the API.

## Features
- Auto-loads locations from LocationService
- Loading and error states
- Optional city name display
- Reactive forms support
- Custom placeholder and CSS class

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-location [(ngModel)]="locationCode" placeholder="Standort wählen" />

<!-- Reactive forms -->
<ui-select-location formControlName="locationCode" />

<!-- With city display -->
<ui-select-location [(ngModel)]="location" [showCity]="true" />
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    placeholder: {
      control: "text",
      description: "Placeholder text for empty selection",
    },
    showCity: {
      control: "boolean",
      description: "Show city name alongside location name",
    },
    cssClass: {
      control: "text",
      description: "CSS class for the select element",
    },
  },
};

export default meta;
type Story = StoryObj<SelectLocationComponent>;

/**
 * Default location select
 */
export const Default: Story = {
  args: {
    placeholder: "Alle Standorte",
    cssClass: "form-input",
  },
};

/**
 * Location select with city names
 */
export const WithCityNames: Story = {
  args: {
    placeholder: "Standort wählen",
    showCity: true,
    cssClass: "form-input",
  },
};

/**
 * Custom placeholder text
 */
export const CustomPlaceholder: Story = {
  args: {
    placeholder: "Bitte Abholstation wählen...",
    cssClass: "form-input",
  },
};

/**
 * In a form context with label
 */
export const InFormField: Story = {
  render: () => ({
    props: {
      location: "",
    },
    template: `
      <div style="max-width: 300px;">
        <label for="location" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Abholstation *
        </label>
        <ui-select-location
          id="location"
          [(ngModel)]="location"
          placeholder="Standort wählen..."
          cssClass="form-input"
        ></ui-select-location>
        <p style="margin-top: 0.5rem; font-size: 0.75rem; color: #6b7280;">
          Ausgewählt: {{ location || 'Keine' }}
        </p>
      </div>
    `,
    styles: [
      `
        :host ::ng-deep .form-input {
          width: 100%;
          padding: 0.5rem 0.75rem;
          border: 1px solid #d1d5db;
          border-radius: 0.375rem;
          font-size: 0.875rem;
        }
      `,
    ],
  }),
};

/**
 * Pickup and dropoff location pair
 */
export const PickupDropoffPair: Story = {
  render: () => ({
    props: {
      pickup: "",
      dropoff: "",
    },
    template: `
      <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; max-width: 600px;">
        <div>
          <label style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
            Abholstation
          </label>
          <ui-select-location
            [(ngModel)]="pickup"
            placeholder="Abholstation wählen"
            [showCity]="true"
            cssClass="form-input"
          ></ui-select-location>
        </div>
        <div>
          <label style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
            Rückgabestation
          </label>
          <ui-select-location
            [(ngModel)]="dropoff"
            placeholder="Rückgabestation wählen"
            [showCity]="true"
            cssClass="form-input"
          ></ui-select-location>
        </div>
      </div>
    `,
    styles: [
      `
        :host ::ng-deep .form-input {
          width: 100%;
          padding: 0.5rem 0.75rem;
          border: 1px solid #d1d5db;
          border-radius: 0.375rem;
          font-size: 0.875rem;
        }
      `,
    ],
  }),
};
