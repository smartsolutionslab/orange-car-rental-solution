import type { Meta, StoryObj } from '@storybook/angular';
import { CheckboxComponent } from './checkbox.component';

const meta: Meta<CheckboxComponent> = {
  title: 'Forms/Checkbox',
  component: CheckboxComponent,
  parameters: {
    layout: 'centered',
    docs: {
      description: {
        component: `
A custom styled checkbox component with multiple sizes, error states,
and reactive forms integration.

## Features
- Custom checkmark icon
- Three sizes: sm, md, lg
- Error and hint text
- Required field indicator
- Full reactive forms support (ControlValueAccessor)

## Usage

\`\`\`html
<!-- Basic checkbox -->
<ocr-checkbox
  [label]="'Accept terms'"
  [(ngModel)]="accepted"
/>

<!-- With reactive forms -->
<ocr-checkbox
  [label]="'Subscribe to newsletter'"
  formControlName="newsletter"
/>

<!-- With validation -->
<ocr-checkbox
  [label]="'I agree'"
  [required]="true"
  [error]="termsError()"
  formControlName="terms"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ['autodocs'],
  argTypes: {
    size: {
      control: 'select',
      options: ['sm', 'md', 'lg'],
    },
  },
};

export default meta;
type Story = StoryObj<CheckboxComponent>;

/**
 * Default checkbox (unchecked)
 */
export const Default: Story = {
  args: {
    label: 'Accept terms and conditions',
    size: 'md',
  },
};

/**
 * Checked checkbox
 */
export const Checked: Story = {
  render: () => ({
    template: `
      <ocr-checkbox label="I agree to the terms" />
    `,
  }),
};

/**
 * Small size
 */
export const Small: Story = {
  args: {
    label: 'Small checkbox',
    size: 'sm',
  },
};

/**
 * Large size
 */
export const Large: Story = {
  args: {
    label: 'Large checkbox',
    size: 'lg',
  },
};

/**
 * Required field
 */
export const Required: Story = {
  args: {
    label: 'Required checkbox',
    required: true,
  },
};

/**
 * With error message
 */
export const WithError: Story = {
  args: {
    label: 'Accept terms and conditions',
    required: true,
    error: 'You must accept the terms to continue',
  },
};

/**
 * With hint text
 */
export const WithHint: Story = {
  args: {
    label: 'Subscribe to newsletter',
    hint: 'We will send you weekly updates',
  },
};

/**
 * Disabled unchecked
 */
export const DisabledUnchecked: Story = {
  args: {
    label: 'Disabled checkbox',
    disabled: true,
  },
};

/**
 * Disabled checked
 */
export const DisabledChecked: Story = {
  render: () => ({
    template: `
      <ocr-checkbox label="Disabled checked" [disabled]="true" />
    `,
  }),
};

/**
 * Without label (custom content)
 */
export const WithCustomContent: Story = {
  render: () => ({
    template: `
      <ocr-checkbox>
        <span>I agree to the <a href="#" style="color: #f97316;">Terms of Service</a> and <a href="#" style="color: #f97316;">Privacy Policy</a></span>
      </ocr-checkbox>
    `,
  }),
};

/**
 * All sizes comparison
 */
export const AllSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1rem;">
        <ocr-checkbox label="Small checkbox" size="sm" />
        <ocr-checkbox label="Medium checkbox (default)" size="md" />
        <ocr-checkbox label="Large checkbox" size="lg" />
      </div>
    `,
  }),
};

/**
 * Group of checkboxes
 */
export const CheckboxGroup: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 0.75rem;">
        <span style="font-size: 0.875rem; font-weight: 500; color: #374151;">Select your interests:</span>
        <ocr-checkbox label="Technology" />
        <ocr-checkbox label="Design" />
        <ocr-checkbox label="Business" />
        <ocr-checkbox label="Marketing" />
      </div>
    `,
  }),
};
