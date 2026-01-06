import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { Component } from "@angular/core";
import {
  FormGroupDirective,
  FormRowDirective,
  FullWidthDirective,
} from "./form-group.directive";
import { InputComponent } from "../input";
import { TextareaComponent } from "../textarea";
import { CheckboxComponent } from "../checkbox";

// Helper component for stories
@Component({
  selector: "story-form-group-demo",
  standalone: true,
  imports: [
    FormGroupDirective,
    FormRowDirective,
    FullWidthDirective,
    InputComponent,
    TextareaComponent,
    CheckboxComponent,
  ],
  template: `<ng-content />`,
})
class FormGroupDemoComponent {}

const meta: Meta = {
  title: "Forms/FormGroup",
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
Directives for organizing form layouts with consistent spacing and alignment.

## Directives

### ocrFormGroup
Main container directive that applies layout and spacing to form fields.

### ocrFormRow
Creates a horizontal row within a vertical form group.

### ocrFullWidth
Makes a field span the full width in a grid layout.

## Usage

\`\`\`html
<!-- Vertical layout (default) -->
<div ocrFormGroup>
  <ocr-input label="First Name" />
  <ocr-input label="Last Name" />
</div>

<!-- Horizontal grid layout -->
<div ocrFormGroup layout="horizontal" [columns]="2">
  <ocr-input label="First Name" />
  <ocr-input label="Last Name" />
  <ocr-textarea label="Bio" ocrFullWidth />
</div>

<!-- Inline layout -->
<div ocrFormGroup layout="inline" spacing="sm">
  <ocr-input placeholder="Search..." />
  <button>Search</button>
</div>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [
        FormGroupDemoComponent,
        FormGroupDirective,
        FormRowDirective,
        FullWidthDirective,
        InputComponent,
        TextareaComponent,
        CheckboxComponent,
      ],
    }),
  ],
};

export default meta;
type Story = StoryObj;

/**
 * Vertical layout (default)
 */
export const VerticalLayout: Story = {
  render: () => ({
    template: `
      <div ocrFormGroup style="width: 400px;">
        <ocr-input label="First Name" placeholder="Enter first name" />
        <ocr-input label="Last Name" placeholder="Enter last name" />
        <ocr-input label="Email" type="email" placeholder="Enter email" />
      </div>
    `,
  }),
};

/**
 * Horizontal grid layout
 */
export const HorizontalLayout: Story = {
  render: () => ({
    template: `
      <div ocrFormGroup layout="horizontal" [columns]="2" style="width: 600px;">
        <ocr-input label="First Name" placeholder="Enter first name" />
        <ocr-input label="Last Name" placeholder="Enter last name" />
        <ocr-input label="Email" type="email" placeholder="Enter email" />
        <ocr-input label="Phone" type="tel" placeholder="Enter phone" />
      </div>
    `,
  }),
};

/**
 * Three-column layout
 */
export const ThreeColumnLayout: Story = {
  render: () => ({
    template: `
      <div ocrFormGroup layout="horizontal" [columns]="3" style="width: 800px;">
        <ocr-input label="City" placeholder="Enter city" />
        <ocr-input label="State" placeholder="Enter state" />
        <ocr-input label="Postal Code" placeholder="Enter postal code" />
      </div>
    `,
  }),
};

/**
 * With full-width field
 */
export const WithFullWidthField: Story = {
  render: () => ({
    template: `
      <div ocrFormGroup layout="horizontal" [columns]="2" style="width: 600px;">
        <ocr-input label="First Name" placeholder="Enter first name" />
        <ocr-input label="Last Name" placeholder="Enter last name" />
        <ocr-textarea label="Bio" placeholder="Tell us about yourself" [rows]="3" ocrFullWidth />
        <ocr-checkbox label="Subscribe to newsletter" ocrFullWidth />
      </div>
    `,
  }),
};

/**
 * Inline layout
 */
export const InlineLayout: Story = {
  render: () => ({
    template: `
      <div ocrFormGroup layout="inline" spacing="sm" alignItems="end" style="width: 500px;">
        <ocr-input placeholder="Search vehicles..." leadingIcon="search" style="flex: 1;" />
        <button style="
          padding: 0.625rem 1rem;
          background: #f97316;
          color: white;
          border: none;
          border-radius: 0.375rem;
          font-weight: 500;
          cursor: pointer;
        ">Search</button>
      </div>
    `,
  }),
};

/**
 * Using FormRow directive
 */
export const WithFormRow: Story = {
  render: () => ({
    template: `
      <div ocrFormGroup style="width: 600px;">
        <div ocrFormRow [columns]="2">
          <ocr-input label="First Name" placeholder="Enter first name" />
          <ocr-input label="Last Name" placeholder="Enter last name" />
        </div>
        <ocr-input label="Email" type="email" placeholder="Enter email" />
        <div ocrFormRow [columns]="3">
          <ocr-input label="City" placeholder="City" />
          <ocr-input label="State" placeholder="State" />
          <ocr-input label="Postal Code" placeholder="Code" />
        </div>
      </div>
    `,
  }),
};

/**
 * Different spacing options
 */
export const SpacingOptions: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 2rem;">
        <div>
          <h4 style="margin-bottom: 0.5rem; font-size: 14px; color: #666;">spacing="sm"</h4>
          <div ocrFormGroup spacing="sm" style="width: 200px; padding: 1rem; border: 1px dashed #ccc;">
            <ocr-input placeholder="Field 1" />
            <ocr-input placeholder="Field 2" />
            <ocr-input placeholder="Field 3" />
          </div>
        </div>

        <div>
          <h4 style="margin-bottom: 0.5rem; font-size: 14px; color: #666;">spacing="md" (default)</h4>
          <div ocrFormGroup spacing="md" style="width: 200px; padding: 1rem; border: 1px dashed #ccc;">
            <ocr-input placeholder="Field 1" />
            <ocr-input placeholder="Field 2" />
            <ocr-input placeholder="Field 3" />
          </div>
        </div>

        <div>
          <h4 style="margin-bottom: 0.5rem; font-size: 14px; color: #666;">spacing="lg"</h4>
          <div ocrFormGroup spacing="lg" style="width: 200px; padding: 1rem; border: 1px dashed #ccc;">
            <ocr-input placeholder="Field 1" />
            <ocr-input placeholder="Field 2" />
            <ocr-input placeholder="Field 3" />
          </div>
        </div>
      </div>
    `,
  }),
};

/**
 * Complete form example
 */
export const CompleteFormExample: Story = {
  render: () => ({
    template: `
      <div style="width: 600px; padding: 1.5rem; background: white; border-radius: 0.5rem; box-shadow: 0 1px 3px rgba(0,0,0,0.1);">
        <h2 style="margin: 0 0 1.5rem 0; font-size: 1.25rem; font-weight: 600;">Contact Information</h2>

        <div ocrFormGroup>
          <div ocrFormRow [columns]="2">
            <ocr-input label="First Name" placeholder="Max" [required]="true" />
            <ocr-input label="Last Name" placeholder="Mustermann" [required]="true" />
          </div>

          <ocr-input
            label="Email"
            type="email"
            placeholder="max@example.com"
            leadingIcon="mail"
            [required]="true"
          />

          <ocr-input
            label="Phone"
            type="tel"
            placeholder="+49 123 456789"
            leadingIcon="phone"
          />

          <div ocrFormRow [columns]="3">
            <ocr-input label="City" placeholder="Berlin" />
            <ocr-input label="State" placeholder="Berlin" />
            <ocr-input label="Postal Code" placeholder="10115" />
          </div>

          <ocr-textarea
            label="Message"
            placeholder="How can we help you?"
            [rows]="4"
          />

          <ocr-checkbox label="I agree to the terms and conditions" />

          <div style="display: flex; gap: 0.75rem; justify-content: flex-end; margin-top: 0.5rem;">
            <button style="
              padding: 0.625rem 1rem;
              background: white;
              color: #374151;
              border: 1px solid #d1d5db;
              border-radius: 0.375rem;
              font-weight: 500;
              cursor: pointer;
            ">Cancel</button>
            <button style="
              padding: 0.625rem 1rem;
              background: #f97316;
              color: white;
              border: none;
              border-radius: 0.375rem;
              font-weight: 500;
              cursor: pointer;
            ">Submit</button>
          </div>
        </div>
      </div>
    `,
  }),
};
