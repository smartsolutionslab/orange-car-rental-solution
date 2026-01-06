import type { Meta, StoryObj } from '@storybook/angular';
import { InputComponent } from './input.component';

const meta: Meta<InputComponent> = {
  title: 'Forms/Input',
  component: InputComponent,
  parameters: {
    layout: 'centered',
    docs: {
      description: {
        component: `
A styled input component with icon support, multiple sizes, error states,
and reactive forms integration.

## Features
- Leading and trailing icon support
- Three sizes: sm, md, lg
- Error and hint text
- Password visibility toggle
- Required field indicator
- Full reactive forms support (ControlValueAccessor)

## Usage

\`\`\`html
<!-- Basic input -->
<ocr-input
  [label]="'Email'"
  [placeholder]="'Enter your email'"
  [(ngModel)]="email"
/>

<!-- With icons -->
<ocr-input
  [label]="'Search'"
  [leadingIcon]="'search'"
  [placeholder]="'Search...'"
/>

<!-- With error -->
<ocr-input
  [label]="'Email'"
  [error]="emailError()"
  formControlName="email"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ['autodocs'],
  argTypes: {
    type: {
      control: 'select',
      options: ['text', 'email', 'password', 'number', 'tel', 'url', 'search'],
    },
    size: {
      control: 'select',
      options: ['sm', 'md', 'lg'],
    },
  },
  decorators: [
    (story) => ({
      ...story,
      styles: ['div { width: 320px; }'],
    }),
  ],
};

export default meta;
type Story = StoryObj<InputComponent>;

/**
 * Default text input
 */
export const Default: Story = {
  args: {
    label: 'Email',
    placeholder: 'Enter your email',
    type: 'text',
    size: 'md',
  },
};

/**
 * Input with leading icon
 */
export const WithLeadingIcon: Story = {
  args: {
    label: 'Email',
    placeholder: 'Enter your email',
    leadingIcon: 'mail',
    type: 'email',
  },
};

/**
 * Input with trailing icon
 */
export const WithTrailingIcon: Story = {
  args: {
    label: 'Search',
    placeholder: 'Search...',
    trailingIcon: 'search',
    type: 'search',
  },
};

/**
 * Input with both icons
 */
export const WithBothIcons: Story = {
  args: {
    label: 'Website',
    placeholder: 'https://example.com',
    leadingIcon: 'globe',
    trailingIcon: 'external-link',
    type: 'url',
  },
};

/**
 * Password input with visibility toggle
 */
export const Password: Story = {
  args: {
    label: 'Password',
    placeholder: 'Enter your password',
    type: 'password',
    leadingIcon: 'lock',
    showPasswordToggle: true,
  },
};

/**
 * Small size input
 */
export const Small: Story = {
  args: {
    label: 'Small Input',
    placeholder: 'Small placeholder',
    size: 'sm',
    leadingIcon: 'user',
  },
};

/**
 * Large size input
 */
export const Large: Story = {
  args: {
    label: 'Large Input',
    placeholder: 'Large placeholder',
    size: 'lg',
    leadingIcon: 'user',
  },
};

/**
 * Required field
 */
export const Required: Story = {
  args: {
    label: 'Required Field',
    placeholder: 'This field is required',
    required: true,
  },
};

/**
 * With error message
 */
export const WithError: Story = {
  args: {
    label: 'Email',
    placeholder: 'Enter your email',
    leadingIcon: 'mail',
    error: 'Please enter a valid email address',
  },
};

/**
 * With hint text
 */
export const WithHint: Story = {
  args: {
    label: 'Username',
    placeholder: 'Choose a username',
    leadingIcon: 'user',
    hint: 'Must be between 3-20 characters',
  },
};

/**
 * Disabled state
 */
export const Disabled: Story = {
  args: {
    label: 'Disabled Input',
    placeholder: 'This input is disabled',
    disabled: true,
    leadingIcon: 'lock',
  },
};

/**
 * Read-only state
 */
export const ReadOnly: Story = {
  args: {
    label: 'Read Only',
    placeholder: 'This input is read-only',
    readonly: true,
  },
};

/**
 * Phone number input
 */
export const PhoneNumber: Story = {
  args: {
    label: 'Phone Number',
    placeholder: '+49 123 456789',
    type: 'tel',
    leadingIcon: 'phone',
    autocomplete: 'tel',
  },
};

/**
 * Number input
 */
export const NumberInput: Story = {
  args: {
    label: 'Quantity',
    placeholder: '0',
    type: 'number',
    leadingIcon: 'hash',
  },
};

/**
 * All sizes comparison
 */
export const AllSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1rem; width: 320px;">
        <ocr-input label="Small" placeholder="Small input" size="sm" leadingIcon="user" />
        <ocr-input label="Medium (Default)" placeholder="Medium input" size="md" leadingIcon="user" />
        <ocr-input label="Large" placeholder="Large input" size="lg" leadingIcon="user" />
      </div>
    `,
  }),
};
