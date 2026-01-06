import type { Meta, StoryObj } from "@storybook/angular";
import { RadioGroupComponent, RadioOption } from "./radio-group.component";

const meta: Meta<RadioGroupComponent> = {
  title: "Forms/Radio Group",
  component: RadioGroupComponent,
  parameters: {
    layout: "centered",
    docs: {
      description: {
        component: `
A styled radio button group with options, descriptions, multiple sizes,
and reactive forms integration.

## Features
- Vertical and horizontal orientations
- Optional descriptions for each option
- Three sizes: sm, md, lg
- Error and hint text
- Required field indicator
- Full reactive forms support (ControlValueAccessor)

## Usage

\`\`\`typescript
// Define options
const options: RadioOption[] = [
  { value: 'small', label: 'Small' },
  { value: 'medium', label: 'Medium' },
  { value: 'large', label: 'Large' },
];
\`\`\`

\`\`\`html
<ocr-radio-group
  [label]="'Select size'"
  [options]="options"
  [(ngModel)]="selectedSize"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    size: {
      control: "select",
      options: ["sm", "md", "lg"],
    },
    orientation: {
      control: "select",
      options: ["vertical", "horizontal"],
    },
  },
};

export default meta;
type Story = StoryObj<RadioGroupComponent>;

const basicOptions: RadioOption[] = [
  { value: "option1", label: "Option 1" },
  { value: "option2", label: "Option 2" },
  { value: "option3", label: "Option 3" },
];

const sizeOptions: RadioOption[] = [
  { value: "sm", label: "Small" },
  { value: "md", label: "Medium" },
  { value: "lg", label: "Large" },
];

const planOptions: RadioOption[] = [
  {
    value: "free",
    label: "Free",
    description: "Basic features for personal use",
  },
  {
    value: "pro",
    label: "Pro",
    description: "Advanced features for professionals",
  },
  {
    value: "enterprise",
    label: "Enterprise",
    description: "Full features for large teams",
  },
];

const colorOptions: RadioOption[] = [
  { value: "red", label: "Red" },
  { value: "blue", label: "Blue" },
  { value: "green", label: "Green" },
  { value: "yellow", label: "Yellow", disabled: true },
];

/**
 * Default vertical radio group
 */
export const Default: Story = {
  args: {
    label: "Select an option",
    options: basicOptions,
    orientation: "vertical",
    size: "md",
  },
};

/**
 * Horizontal orientation
 */
export const Horizontal: Story = {
  args: {
    label: "Select size",
    options: sizeOptions,
    orientation: "horizontal",
  },
};

/**
 * With descriptions
 */
export const WithDescriptions: Story = {
  args: {
    label: "Select a plan",
    options: planOptions,
    orientation: "vertical",
  },
};

/**
 * Small size
 */
export const Small: Story = {
  args: {
    label: "Small radio group",
    options: basicOptions,
    size: "sm",
  },
};

/**
 * Large size
 */
export const Large: Story = {
  args: {
    label: "Large radio group",
    options: basicOptions,
    size: "lg",
  },
};

/**
 * Required field
 */
export const Required: Story = {
  args: {
    label: "Required selection",
    options: basicOptions,
    required: true,
  },
};

/**
 * With error message
 */
export const WithError: Story = {
  args: {
    label: "Select an option",
    options: basicOptions,
    required: true,
    error: "Please select an option to continue",
  },
};

/**
 * With hint text
 */
export const WithHint: Story = {
  args: {
    label: "Select preferred contact method",
    options: [
      { value: "email", label: "Email" },
      { value: "phone", label: "Phone" },
      { value: "sms", label: "SMS" },
    ],
    hint: "We will only contact you using this method",
  },
};

/**
 * With disabled option
 */
export const WithDisabledOption: Story = {
  args: {
    label: "Select a color",
    options: colorOptions,
  },
};

/**
 * Entirely disabled
 */
export const Disabled: Story = {
  args: {
    label: "Disabled group",
    options: basicOptions,
    disabled: true,
  },
};

/**
 * Without label
 */
export const WithoutLabel: Story = {
  args: {
    options: sizeOptions,
    orientation: "horizontal",
  },
};

/**
 * Payment method example
 */
export const PaymentMethod: Story = {
  args: {
    label: "Payment method",
    options: [
      {
        value: "card",
        label: "Credit Card",
        description: "Visa, Mastercard, American Express",
      },
      {
        value: "paypal",
        label: "PayPal",
        description: "Pay with your PayPal account",
      },
      {
        value: "bank",
        label: "Bank Transfer",
        description: "Direct bank transfer (1-3 days)",
      },
    ],
    required: true,
  },
};

/**
 * All sizes comparison
 */
export const AllSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 2rem;">
        <ocr-radio-group
          label="Small"
          [options]="options"
          size="sm"
        />
        <ocr-radio-group
          label="Medium (Default)"
          [options]="options"
          size="md"
        />
        <ocr-radio-group
          label="Large"
          [options]="options"
          size="lg"
        />
      </div>
    `,
    props: {
      options: basicOptions,
    },
  }),
};
