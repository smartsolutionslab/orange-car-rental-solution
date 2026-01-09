import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectVehicleStatusComponent } from "./select-vehicle-status.component";

const meta: Meta<SelectVehicleStatusComponent> = {
  title: "Forms/Select Vehicle Status",
  component: SelectVehicleStatusComponent,
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
A select component for filtering vehicles by status.

## Features
- Predefined vehicle status options (Available, Rented, Maintenance, etc.)
- German labels
- Reactive forms support
- Custom placeholder
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-vehicle-status [(ngModel)]="status" placeholder="Alle Status" />

<!-- Reactive forms -->
<ui-select-vehicle-status formControlName="vehicleStatus" />
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
type Story = StoryObj<SelectVehicleStatusComponent>;

/**
 * Default vehicle status select
 */
export const Default: Story = {
  args: {
    placeholder: "Alle Status",
    cssClass: "form-input",
  },
};

/**
 * With custom placeholder
 */
export const CustomPlaceholder: Story = {
  args: {
    placeholder: "Fahrzeugstatus wählen...",
    cssClass: "form-input",
  },
};

/**
 * Pre-selected status
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      status: "Available",
    },
    template: `
      <ui-select-vehicle-status [(ngModel)]="status" placeholder="Alle Status"></ui-select-vehicle-status>
      <p style="margin-top: 0.5rem; font-size: 0.875rem; color: #6b7280;">
        Ausgewählt: {{ status || 'Alle' }}
      </p>
    `,
  }),
};

/**
 * In filter form context
 */
export const InFilterForm: Story = {
  render: () => ({
    props: {
      status: "",
    },
    template: `
      <div style="max-width: 250px;">
        <label for="vehicleStatus" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Fahrzeugstatus
        </label>
        <ui-select-vehicle-status
          id="vehicleStatus"
          [(ngModel)]="status"
          placeholder="Alle Status"
          cssClass="form-input"
        ></ui-select-vehicle-status>
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
 * Fleet management filter group
 */
export const FleetManagementFilters: Story = {
  render: () => ({
    props: {
      status: "",
    },
    template: `
      <div style="display: flex; gap: 1rem; align-items: flex-end;">
        <div>
          <label style="display: block; margin-bottom: 0.25rem; font-size: 0.75rem; font-weight: 500; color: #6b7280; text-transform: uppercase;">
            Status
          </label>
          <ui-select-vehicle-status
            [(ngModel)]="status"
            placeholder="Alle"
            cssClass="form-input"
          ></ui-select-vehicle-status>
        </div>
        <button style="padding: 0.5rem 1rem; background: #f97316; color: white; border: none; border-radius: 0.375rem; font-size: 0.875rem; cursor: pointer;">
          Filtern
        </button>
      </div>
    `,
    styles: [
      `
        :host ::ng-deep .form-input {
          padding: 0.5rem 0.75rem;
          border: 1px solid #d1d5db;
          border-radius: 0.375rem;
          font-size: 0.875rem;
          min-width: 180px;
        }
      `,
    ],
  }),
};
