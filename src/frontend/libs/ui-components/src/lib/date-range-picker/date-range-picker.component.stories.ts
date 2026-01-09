import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { DateRangePickerComponent, type DateRange } from "./date-range-picker.component";

const meta: Meta<DateRangePickerComponent> = {
  title: "Forms/Date Range Picker",
  component: DateRangePickerComponent,
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
A date range picker component for selecting start and end dates.

## Features
- Start and end date inputs
- Optional labels
- Minimum date constraints
- Minimum days apart validation
- Reactive forms support
- Custom styling

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-date-range-picker
  [(ngModel)]="dateRange"
  startDateLabel="Check-in"
  endDateLabel="Check-out"
/>

<!-- Reactive forms -->
<ui-date-range-picker formControlName="dateRange" />

<!-- With minimum days apart -->
<ui-date-range-picker
  [(ngModel)]="range"
  [minDaysApart]="1"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    startDateLabel: {
      control: "text",
      description: "Label for start date input",
    },
    endDateLabel: {
      control: "text",
      description: "Label for end date input",
    },
    showLabels: {
      control: "boolean",
      description: "Show/hide labels",
    },
    required: {
      control: "boolean",
      description: "Mark fields as required",
    },
    minDaysApart: {
      control: "number",
      description: "Minimum days between start and end",
    },
  },
};

export default meta;
type Story = StoryObj<DateRangePickerComponent>;

/**
 * Default date range picker with labels
 */
export const Default: Story = {
  args: {
    startDateLabel: "Startdatum",
    endDateLabel: "Enddatum",
    showLabels: true,
  },
};

/**
 * Without labels
 */
export const WithoutLabels: Story = {
  args: {
    showLabels: false,
  },
};

/**
 * Rental period (pickup/return)
 */
export const RentalPeriod: Story = {
  args: {
    startDateLabel: "Abholdatum",
    endDateLabel: "Rückgabedatum",
    showLabels: true,
    minDaysApart: 1,
  },
};

/**
 * With pre-selected dates
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      dateRange: {
        startDate: new Date().toISOString().split("T")[0],
        endDate: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString().split("T")[0],
      } as DateRange,
    },
    template: `
      <ui-date-range-picker
        [(ngModel)]="dateRange"
        startDateLabel="Von"
        endDateLabel="Bis"
      ></ui-date-range-picker>
      <p style="margin-top: 1rem; font-size: 0.875rem; color: #6b7280;">
        Ausgewählt: {{ dateRange.startDate }} bis {{ dateRange.endDate }}
      </p>
    `,
  }),
};

/**
 * Required fields
 */
export const Required: Story = {
  args: {
    startDateLabel: "Abholdatum",
    endDateLabel: "Rückgabedatum",
    showLabels: true,
    required: true,
  },
};

/**
 * In a booking form context
 */
export const InBookingForm: Story = {
  render: () => ({
    props: {
      dateRange: { startDate: "", endDate: "" } as DateRange,
      getDays: (range: DateRange) => {
        if (!range.startDate || !range.endDate) return 0;
        const start = new Date(range.startDate);
        const end = new Date(range.endDate);
        return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
      },
    },
    template: `
      <div style="max-width: 500px; padding: 1.5rem; background: white; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
        <h3 style="margin: 0 0 1rem; font-size: 1.125rem; font-weight: 600; color: #111827;">
          Mietdauer auswählen
        </h3>
        <ui-date-range-picker
          [(ngModel)]="dateRange"
          startDateLabel="Abholdatum"
          endDateLabel="Rückgabedatum"
          [minDaysApart]="1"
          [required]="true"
          inputClass="form-input date-input"
        ></ui-date-range-picker>
        @if (dateRange.startDate && dateRange.endDate) {
          <div style="margin-top: 1rem; padding: 0.75rem; background: #f0fdf4; border-radius: 0.375rem;">
            <p style="margin: 0; font-size: 0.875rem; color: #166534;">
              Mietdauer: {{ getDays(dateRange) }} Tag(e)
            </p>
          </div>
        }
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
