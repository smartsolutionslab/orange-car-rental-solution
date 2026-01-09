import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectSeatsComponent } from "./select-seats.component";

const meta: Meta<SelectSeatsComponent> = {
  title: "Forms/Select Seats",
  component: SelectSeatsComponent,
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
A select component for minimum seat requirements with predefined options.

## Features
- Predefined seat options (2+, 4+, 5+, 7+)
- German labels
- Reactive forms support
- Custom placeholder
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-seats [(ngModel)]="minSeats" placeholder="Beliebig" />

<!-- Reactive forms -->
<ui-select-seats formControlName="minSeats" />
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
type Story = StoryObj<SelectSeatsComponent>;

/**
 * Default seats select
 */
export const Default: Story = {
  args: {
    placeholder: "Alle",
    cssClass: "form-input",
  },
};

/**
 * With custom placeholder
 */
export const CustomPlaceholder: Story = {
  args: {
    placeholder: "Mindestanzahl Sitze",
    cssClass: "form-input",
  },
};

/**
 * Pre-selected value
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      minSeats: 4,
    },
    template: `
      <ui-select-seats [(ngModel)]="minSeats" placeholder="Alle"></ui-select-seats>
      <p style="margin-top: 0.5rem; font-size: 0.875rem; color: #6b7280;">
        Ausgewählt: {{ minSeats ? minSeats + '+ Sitze' : 'Beliebig' }}
      </p>
    `,
  }),
};

/**
 * In a filter form context
 */
export const InFilterForm: Story = {
  render: () => ({
    props: {
      minSeats: null,
    },
    template: `
      <div style="max-width: 200px;">
        <label for="seats" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Mindestanzahl Sitze
        </label>
        <ui-select-seats
          id="seats"
          [(ngModel)]="minSeats"
          placeholder="Beliebig"
          cssClass="form-input"
        ></ui-select-seats>
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
