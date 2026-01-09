import type { Meta, StoryObj } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { moduleMetadata } from "@storybook/angular";
import { SelectCategoryComponent } from "./select-category.component";

const meta: Meta<SelectCategoryComponent> = {
  title: "Forms/Select Category",
  component: SelectCategoryComponent,
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
A select component for vehicle categories with predefined options.
Implements ControlValueAccessor for use with Angular forms.

## Features
- Predefined vehicle categories (Economy, Compact, SUV, etc.)
- German labels
- Reactive forms support
- Custom placeholder
- CSS class customization

## Usage

\`\`\`html
<!-- Template-driven -->
<ui-select-category [(ngModel)]="category" placeholder="Kategorie wählen" />

<!-- Reactive forms -->
<ui-select-category formControlName="category" />
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
type Story = StoryObj<SelectCategoryComponent>;

/**
 * Default select with all categories
 */
export const Default: Story = {
  args: {
    placeholder: "Alle Kategorien",
    cssClass: "form-input",
  },
};

/**
 * Select with custom placeholder
 */
export const CustomPlaceholder: Story = {
  args: {
    placeholder: "Kategorie wählen...",
    cssClass: "form-input",
  },
};

/**
 * Select with pre-selected value
 */
export const PreSelected: Story = {
  render: () => ({
    props: {
      selectedCategory: "SUV",
    },
    template: `
      <ui-select-category [(ngModel)]="selectedCategory" placeholder="Alle Kategorien"></ui-select-category>
      <p style="margin-top: 0.5rem; font-size: 0.875rem; color: #6b7280;">Ausgewählt: {{ selectedCategory || 'Keine' }}</p>
    `,
  }),
};

/**
 * In a form field context
 */
export const InFormField: Story = {
  render: () => ({
    props: {
      category: "",
    },
    template: `
      <div style="max-width: 300px;">
        <label for="category" style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
          Fahrzeugkategorie
        </label>
        <ui-select-category
          id="category"
          [(ngModel)]="category"
          placeholder="Bitte wählen..."
          cssClass="form-input"
        ></ui-select-category>
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

/**
 * Multiple selects for filtering
 */
export const FilterGroup: Story = {
  render: () => ({
    props: {
      category: "",
    },
    template: `
      <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
        <div style="min-width: 200px;">
          <label style="display: block; margin-bottom: 0.25rem; font-size: 0.75rem; font-weight: 500; color: #6b7280; text-transform: uppercase;">
            Kategorie
          </label>
          <ui-select-category
            [(ngModel)]="category"
            placeholder="Alle Kategorien"
            cssClass="form-input"
          ></ui-select-category>
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
          background: white;
        }
      `,
    ],
  }),
};
