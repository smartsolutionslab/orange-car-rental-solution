import type { Meta, StoryObj } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { moduleMetadata } from "@storybook/angular";
import { SelectFuelTypeComponent } from "./select-fuel-type.component";

const meta: Meta<SelectFuelTypeComponent> = {
  title: "Forms/Select Fuel Type",
  component: SelectFuelTypeComponent,
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
A select component for fuel types with predefined options.
Implements ControlValueAccessor for use with Angular forms.

## Features
- Predefined fuel types (Benzin, Diesel, Elektro, Hybrid)
- German labels
- Reactive forms support
- Custom placeholder
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-fuel-type [(ngModel)]="fuelType" placeholder="Kraftstoff wählen" />

<!-- Reactive forms -->
<ui-select-fuel-type formControlName="fuelType" />
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
    cssClass: {
      control: "text",
      description: "CSS class for the select element",
    },
  },
};

export default meta;
type Story = StoryObj<SelectFuelTypeComponent>;

/**
 * Default select with all fuel types
 */
export const Default: Story = {
  args: {
    placeholder: "Alle Kraftstoffe",
    cssClass: "form-input",
  },
};

/**
 * Select with custom placeholder
 */
export const CustomPlaceholder: Story = {
  args: {
    placeholder: "Kraftstoffart wählen...",
    cssClass: "form-input",
  },
};

/**
 * Select with pre-selected value
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      selectedFuelType: "Electric",
    },
    template: `
      <ui-select-fuel-type [(ngModel)]="selectedFuelType" placeholder="Alle Kraftstoffe"></ui-select-fuel-type>
      <p style="margin-top: 0.5rem; font-size: 0.875rem; color: #6b7280;">Ausgewählt: {{ selectedFuelType || 'Keine' }}</p>
    `,
  }),
};

/**
 * In a form field context
 */
export const InFormField: Story = {
  render: () => ({
    props: {
      fuelType: "",
    },
    template: `
      <div style="max-width: 300px;">
        <label for="fuelType" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Kraftstoffart
        </label>
        <ui-select-fuel-type
          id="fuelType"
          [(ngModel)]="fuelType"
          placeholder="Bitte wählen..."
          cssClass="form-input"
        ></ui-select-fuel-type>
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
        :host ::ng-deep .form-input:focus {
          outline: none;
          border-color: #f97316;
          box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
        }
      `,
    ],
  }),
};
