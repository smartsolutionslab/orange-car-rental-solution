import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectPageSizeComponent } from "./select-page-size.component";

const meta: Meta<SelectPageSizeComponent> = {
  title: "Forms/Select Page Size",
  component: SelectPageSizeComponent,
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
A select component for page size options in pagination.

## Features
- Predefined page size options (10, 25, 50, 100)
- Reactive forms support
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-page-size [(ngModel)]="pageSize" />

<!-- With change handler -->
<ui-select-page-size
  [(ngModel)]="pageSize"
  (ngModelChange)="onPageSizeChange($event)"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    cssClass: {
      control: "text",
      description: "CSS class for the select element",
    },
  },
};

export default meta;
type Story = StoryObj<SelectPageSizeComponent>;

/**
 * Default page size select
 */
export const Default: Story = {
  args: {
    cssClass: "form-input",
  },
};

/**
 * Pre-selected value
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      pageSize: 25,
    },
    template: `
      <ui-select-page-size [(ngModel)]="pageSize"></ui-select-page-size>
      <p style="margin-top: 0.5rem; font-size: 0.875rem; color: #6b7280;">
        Einträge pro Seite: {{ pageSize }}
      </p>
    `,
  }),
};

/**
 * In pagination context
 */
export const InPaginationContext: Story = {
  render: () => ({
    props: {
      pageSize: 10,
      currentPage: 1,
      totalItems: 156,
      Math: Math,
    },
    template: `
      <div style="display: flex; align-items: center; justify-content: space-between; padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
        <div style="display: flex; align-items: center; gap: 0.5rem;">
          <label for="pageSize" style="font-size: 0.875rem; color: #6b7280;">
            Einträge pro Seite:
          </label>
          <ui-select-page-size
            id="pageSize"
            [(ngModel)]="pageSize"
            cssClass="form-input-sm"
          ></ui-select-page-size>
        </div>
        <div style="font-size: 0.875rem; color: #6b7280;">
          Zeige {{ (currentPage - 1) * pageSize + 1 }} bis {{ Math.min(currentPage * pageSize, totalItems) }} von {{ totalItems }} Einträgen
        </div>
      </div>
    `,
    styles: [
      `
        :host ::ng-deep .form-input-sm {
          padding: 0.25rem 0.5rem;
          border: 1px solid #d1d5db;
          border-radius: 0.375rem;
          font-size: 0.875rem;
        }
      `,
    ],
  }),
};

/**
 * Compact style for table headers
 */
export const Compact: Story = {
  render: () => ({
    props: {
      pageSize: 10,
    },
    template: `
      <div style="display: inline-flex; align-items: center; gap: 0.5rem; font-size: 0.75rem; color: #6b7280;">
        <span>Anzeigen:</span>
        <ui-select-page-size
          [(ngModel)]="pageSize"
          cssClass="compact-select"
        ></ui-select-page-size>
      </div>
    `,
    styles: [
      `
        :host ::ng-deep .compact-select {
          padding: 0.125rem 0.375rem;
          border: 1px solid #d1d5db;
          border-radius: 0.25rem;
          font-size: 0.75rem;
        }
      `,
    ],
  }),
};
