import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectTransmissionComponent } from "./select-transmission.component";

const meta: Meta<SelectTransmissionComponent> = {
  title: "Forms/Select Transmission",
  component: SelectTransmissionComponent,
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
A select component for transmission types with predefined options.

## Features
- Predefined transmission types (Automatik, Schaltgetriebe)
- German labels
- Reactive forms support
- Custom placeholder
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-transmission [(ngModel)]="transmission" placeholder="Alle Getriebe" />

<!-- Reactive forms -->
<ui-select-transmission formControlName="transmissionType" />
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
type Story = StoryObj<SelectTransmissionComponent>;

/**
 * Default transmission select
 */
export const Default: Story = {
  args: {
    placeholder: "Alle Getriebe",
    cssClass: "form-input",
  },
};

/**
 * With custom placeholder
 */
export const CustomPlaceholder: Story = {
  args: {
    placeholder: "Getriebe wählen...",
    cssClass: "form-input",
  },
};

/**
 * Pre-selected value
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      transmission: "Automatic",
    },
    template: `
      <ui-select-transmission [(ngModel)]="transmission" placeholder="Alle Getriebe"></ui-select-transmission>
      <p style="margin-top: 0.5rem; font-size: 0.875rem; color: #6b7280;">
        Ausgewählt: {{ transmission || 'Keine' }}
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
      transmission: "",
    },
    template: `
      <div style="max-width: 200px;">
        <label for="transmission" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Getriebeart
        </label>
        <ui-select-transmission
          id="transmission"
          [(ngModel)]="transmission"
          placeholder="Alle Getriebe"
          cssClass="form-input"
        ></ui-select-transmission>
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
