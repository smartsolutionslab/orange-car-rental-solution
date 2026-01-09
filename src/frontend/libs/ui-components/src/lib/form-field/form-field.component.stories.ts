import type { Meta, StoryObj } from "@storybook/angular";
import { FormFieldComponent } from "./form-field.component";

const meta: Meta<FormFieldComponent> = {
  title: "Forms/Form Field",
  component: FormFieldComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A reusable form field wrapper component that provides consistent styling for label and input groups.

## Features
- Label with optional required indicator
- Error state with error message
- Hint text support
- Consistent styling for wrapped inputs

## Usage

\`\`\`html
<!-- Basic usage -->
<ui-form-field label="E-Mail" inputId="email" [required]="true">
  <input type="email" id="email" />
</ui-form-field>

<!-- With error -->
<ui-form-field
  label="Passwort"
  inputId="password"
  [hasError]="true"
  errorMessage="Passwort ist erforderlich"
>
  <input type="password" id="password" />
</ui-form-field>

<!-- With hint -->
<ui-form-field label="Telefon" inputId="phone" hint="Format: +49 xxx xxxxxxx">
  <input type="tel" id="phone" />
</ui-form-field>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    label: {
      control: "text",
      description: "Label text for the field",
    },
    inputId: {
      control: "text",
      description: "ID to link label with input",
    },
    required: {
      control: "boolean",
      description: "Show required indicator",
    },
    hasError: {
      control: "boolean",
      description: "Apply error styling",
    },
    errorMessage: {
      control: "text",
      description: "Error message to display",
    },
    hint: {
      control: "text",
      description: "Hint text below input",
    },
  },
};

export default meta;
type Story = StoryObj<FormFieldComponent>;

/**
 * Basic form field with text input
 */
export const Default: Story = {
  args: {
    label: "E-Mail-Adresse",
    inputId: "email",
    required: false,
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field [label]="label" [inputId]="inputId" [required]="required">
        <input type="email" [id]="inputId" placeholder="max@example.com" />
      </ui-form-field>
    `,
  }),
};

/**
 * Required form field
 */
export const Required: Story = {
  args: {
    label: "Vorname",
    inputId: "firstName",
    required: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field [label]="label" [inputId]="inputId" [required]="required">
        <input type="text" [id]="inputId" placeholder="Max" />
      </ui-form-field>
    `,
  }),
};

/**
 * Form field with error state
 */
export const WithError: Story = {
  args: {
    label: "Passwort",
    inputId: "password",
    required: true,
    hasError: true,
    errorMessage: "Passwort muss mindestens 8 Zeichen lang sein",
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field
        [label]="label"
        [inputId]="inputId"
        [required]="required"
        [hasError]="hasError"
        [errorMessage]="errorMessage"
      >
        <input type="password" [id]="inputId" value="123" />
      </ui-form-field>
    `,
  }),
};

/**
 * Form field with hint text
 */
export const WithHint: Story = {
  args: {
    label: "Telefonnummer",
    inputId: "phone",
    hint: "Format: +49 xxx xxxxxxx",
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field [label]="label" [inputId]="inputId" [hint]="hint">
        <input type="tel" [id]="inputId" placeholder="+49 151 12345678" />
      </ui-form-field>
    `,
  }),
};

/**
 * Form field with select element
 */
export const WithSelect: Story = {
  args: {
    label: "Land",
    inputId: "country",
    required: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field [label]="label" [inputId]="inputId" [required]="required">
        <select [id]="inputId">
          <option value="">Bitte wählen...</option>
          <option value="DE">Deutschland</option>
          <option value="AT">Österreich</option>
          <option value="CH">Schweiz</option>
        </select>
      </ui-form-field>
    `,
  }),
};

/**
 * Form field with textarea
 */
export const WithTextarea: Story = {
  args: {
    label: "Bemerkungen",
    inputId: "notes",
    hint: "Optional: Zusätzliche Informationen zur Buchung",
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field [label]="label" [inputId]="inputId" [hint]="hint">
        <textarea [id]="inputId" rows="3" placeholder="Ihre Nachricht..."></textarea>
      </ui-form-field>
    `,
  }),
};

/**
 * Complete form example
 */
export const CompleteForm: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px; display: flex; flex-direction: column; gap: 1rem;">
        <ui-form-field label="Vorname" inputId="firstName" [required]="true">
          <input type="text" id="firstName" value="Max" />
        </ui-form-field>

        <ui-form-field label="Nachname" inputId="lastName" [required]="true">
          <input type="text" id="lastName" value="Mustermann" />
        </ui-form-field>

        <ui-form-field label="E-Mail" inputId="email" [required]="true" [hasError]="true" errorMessage="Ungültige E-Mail-Adresse">
          <input type="email" id="email" value="invalid-email" />
        </ui-form-field>

        <ui-form-field label="Telefon" inputId="phone" hint="Optional">
          <input type="tel" id="phone" placeholder="+49 xxx xxxxxxx" />
        </ui-form-field>

        <ui-form-field label="Land" inputId="country" [required]="true">
          <select id="country">
            <option value="DE" selected>Deutschland</option>
            <option value="AT">Österreich</option>
            <option value="CH">Schweiz</option>
          </select>
        </ui-form-field>
      </div>
    `,
  }),
};

/**
 * Disabled form field
 */
export const Disabled: Story = {
  args: {
    label: "Buchungsnummer",
    inputId: "bookingId",
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-form-field [label]="label" [inputId]="inputId">
        <input type="text" [id]="inputId" value="RES-2024-001234" disabled />
      </ui-form-field>
    `,
  }),
};
