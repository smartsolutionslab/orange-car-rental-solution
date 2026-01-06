import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { ValidationMessagesComponent } from "./validation-messages.component";
import { InputComponent } from "../input";

const meta: Meta<ValidationMessagesComponent> = {
  title: "Forms/ValidationMessages",
  component: ValidationMessagesComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A component that displays validation error messages for form controls.
Automatically detects errors and shows appropriate German messages.

## Features
- Automatic error detection
- German default messages
- Custom message support
- Show first or all errors
- Optional error icon
- Animation on appear

## Usage

\`\`\`html
<!-- Basic usage -->
<ocr-input formControlName="email" />
<ocr-validation-messages [control]="form.get('email')" />

<!-- Show all errors -->
<ocr-validation-messages
  [control]="form.get('password')"
  [showFirst]="false"
/>

<!-- Custom messages -->
<ocr-validation-messages
  [control]="form.get('age')"
  [messages]="[{ key: 'min', message: 'Must be at least 18' }]"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [ReactiveFormsModule, InputComponent],
    }),
  ],
};

export default meta;
type Story = StoryObj<ValidationMessagesComponent>;

// Create form controls for demos
const createControl = (validators: any[] = []) => {
  const ctrl = new FormControl("", validators);
  ctrl.markAsTouched();
  return ctrl;
};

/**
 * Required field error
 */
export const RequiredError: Story = {
  render: () => {
    const control = createControl([Validators.required]);

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Email" [error]="' '" />
          <ocr-validation-messages [control]="control" />
        </div>
      `,
    };
  },
};

/**
 * Email validation error
 */
export const EmailError: Story = {
  render: () => {
    const control = new FormControl("invalid-email", [Validators.email]);
    control.markAsTouched();

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Email" [error]="' '" />
          <ocr-validation-messages [control]="control" />
        </div>
      `,
    };
  },
};

/**
 * Minimum length error
 */
export const MinLengthError: Story = {
  render: () => {
    const control = new FormControl("abc", [Validators.minLength(8)]);
    control.markAsTouched();

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Password" type="password" [error]="' '" />
          <ocr-validation-messages [control]="control" />
        </div>
      `,
    };
  },
};

/**
 * Multiple errors (show first only)
 */
export const MultipleErrorsShowFirst: Story = {
  render: () => {
    const control = new FormControl("", [
      Validators.required,
      Validators.email,
      Validators.minLength(5),
    ]);
    control.markAsTouched();

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Email" [error]="' '" />
          <ocr-validation-messages [control]="control" [showFirst]="true" />
          <p style="font-size: 12px; color: #666; margin-top: 8px;">
            (Only showing first error)
          </p>
        </div>
      `,
    };
  },
};

/**
 * Multiple errors (show all)
 */
export const MultipleErrorsShowAll: Story = {
  render: () => {
    const control = new FormControl("ab", [
      Validators.required,
      Validators.email,
      Validators.minLength(5),
    ]);
    control.markAsTouched();

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Email" [error]="' '" />
          <ocr-validation-messages [control]="control" [showFirst]="false" />
          <p style="font-size: 12px; color: #666; margin-top: 8px;">
            (Showing all errors)
          </p>
        </div>
      `,
    };
  },
};

/**
 * Without icon
 */
export const WithoutIcon: Story = {
  render: () => {
    const control = createControl([Validators.required]);

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Name" [error]="' '" />
          <ocr-validation-messages [control]="control" [showIcon]="false" />
        </div>
      `,
    };
  },
};

/**
 * Custom validation messages
 */
export const CustomMessages: Story = {
  render: () => {
    const control = new FormControl("12", [
      Validators.required,
      Validators.min(18),
    ]);
    control.markAsTouched();

    const customMessages = [
      { key: "min", message: "You must be at least 18 years old to register" },
    ];

    return {
      props: { control, customMessages },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Age" type="number" [error]="' '" />
          <ocr-validation-messages
            [control]="control"
            [messages]="customMessages"
            [showFirst]="false"
          />
        </div>
      `,
    };
  },
};

/**
 * No errors (valid state)
 */
export const NoErrors: Story = {
  render: () => {
    const control = new FormControl("valid@email.com", [
      Validators.required,
      Validators.email,
    ]);
    control.markAsTouched();

    return {
      props: { control },
      template: `
        <div style="width: 320px;">
          <ocr-input label="Email" />
          <ocr-validation-messages [control]="control" />
          <p style="font-size: 12px; color: #22c55e; margin-top: 8px;">
            ✓ No errors to display
          </p>
        </div>
      `,
    };
  },
};

/**
 * Integration with form
 */
export const FormIntegration: Story = {
  render: () => {
    const form = new FormGroup({
      email: new FormControl("", [Validators.required, Validators.email]),
      password: new FormControl("", [
        Validators.required,
        Validators.minLength(8),
      ]),
    });

    // Mark as touched for demo
    form.get("email")?.markAsTouched();
    form.get("password")?.setValue("123");
    form.get("password")?.markAsTouched();

    return {
      props: { form },
      template: `
        <form [formGroup]="form" style="width: 320px; display: flex; flex-direction: column; gap: 1rem;">
          <div>
            <ocr-input
              label="Email"
              formControlName="email"
              [required]="true"
              [error]="form.get('email')?.invalid && form.get('email')?.touched ? ' ' : null"
            />
            <ocr-validation-messages [control]="form.get('email')" />
          </div>

          <div>
            <ocr-input
              label="Password"
              type="password"
              formControlName="password"
              [required]="true"
              [error]="form.get('password')?.invalid && form.get('password')?.touched ? ' ' : null"
            />
            <ocr-validation-messages [control]="form.get('password')" />
          </div>
        </form>
      `,
    };
  },
};
