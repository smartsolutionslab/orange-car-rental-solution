import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectReservationStatusComponent } from "./select-reservation-status.component";

const meta: Meta<SelectReservationStatusComponent> = {
  title: "Forms/Select Reservation Status",
  component: SelectReservationStatusComponent,
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
A select component for filtering reservations by status.

## Features
- Predefined reservation status options (Pending, Confirmed, Cancelled, Completed)
- German labels
- Reactive forms support
- Custom placeholder
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-reservation-status [(ngModel)]="status" placeholder="Alle Status" />

<!-- Reactive forms -->
<ui-select-reservation-status formControlName="status" />
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
type Story = StoryObj<SelectReservationStatusComponent>;

/**
 * Default reservation status select
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
    placeholder: "Status filtern...",
    cssClass: "form-input",
  },
};

/**
 * Pre-selected status
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      status: "Confirmed",
    },
    template: `
      <ui-select-reservation-status [(ngModel)]="status" placeholder="Alle Status"></ui-select-reservation-status>
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
        <label for="status" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Buchungsstatus
        </label>
        <ui-select-reservation-status
          id="status"
          [(ngModel)]="status"
          placeholder="Alle Status"
          cssClass="form-input"
        ></ui-select-reservation-status>
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
