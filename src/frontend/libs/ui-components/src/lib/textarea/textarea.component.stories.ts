import type { Meta, StoryObj } from "@storybook/angular";
import { TextareaComponent } from "./textarea.component";

const meta: Meta<TextareaComponent> = {
  title: "Forms/Textarea",
  component: TextareaComponent,
  parameters: {
    layout: "centered",
    docs: {
      description: {
        component: `
A styled textarea component with character count, multiple sizes,
error states, and reactive forms integration.

## Features
- Character count display
- Maximum length enforcement
- Three sizes: sm, md, lg
- Configurable resize behavior
- Error and hint text
- Full reactive forms support (ControlValueAccessor)

## Usage

\`\`\`html
<!-- Basic textarea -->
<ocr-textarea
  [label]="'Description'"
  [placeholder]="'Enter a description...'"
  [(ngModel)]="description"
/>

<!-- With character limit -->
<ocr-textarea
  [label]="'Bio'"
  [maxLength]="200"
  [showCharCount]="true"
  formControlName="bio"
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
    resize: {
      control: "select",
      options: ["none", "vertical", "horizontal", "both"],
    },
    rows: {
      control: { type: "number", min: 1, max: 20 },
    },
  },
  decorators: [
    (story) => ({
      ...story,
      styles: ["div { width: 400px; }"],
    }),
  ],
};

export default meta;
type Story = StoryObj<TextareaComponent>;

/**
 * Default textarea
 */
export const Default: Story = {
  args: {
    label: "Description",
    placeholder: "Enter a description...",
    rows: 4,
    size: "md",
    resize: "vertical",
  },
};

/**
 * With character count
 */
export const WithCharacterCount: Story = {
  args: {
    label: "Bio",
    placeholder: "Tell us about yourself...",
    maxLength: 200,
    showCharCount: true,
    rows: 4,
  },
};

/**
 * At character limit
 */
export const AtCharacterLimit: Story = {
  render: () => ({
    template: `
      <ocr-textarea
        label="Limited Text"
        placeholder="Type here..."
        [maxLength]="50"
        [showCharCount]="true"
        [rows]="3"
      />
    `,
  }),
};

/**
 * Small size
 */
export const Small: Story = {
  args: {
    label: "Small Textarea",
    placeholder: "Small placeholder...",
    size: "sm",
    rows: 3,
  },
};

/**
 * Large size
 */
export const Large: Story = {
  args: {
    label: "Large Textarea",
    placeholder: "Large placeholder...",
    size: "lg",
    rows: 5,
  },
};

/**
 * Required field
 */
export const Required: Story = {
  args: {
    label: "Required Field",
    placeholder: "This field is required",
    required: true,
    rows: 4,
  },
};

/**
 * With error message
 */
export const WithError: Story = {
  args: {
    label: "Comments",
    placeholder: "Enter your comments...",
    error: "Please enter at least 10 characters",
    rows: 4,
  },
};

/**
 * With hint text
 */
export const WithHint: Story = {
  args: {
    label: "Notes",
    placeholder: "Add any notes...",
    hint: "Optional: Add any additional information",
    rows: 4,
  },
};

/**
 * No resize
 */
export const NoResize: Story = {
  args: {
    label: "Fixed Size",
    placeholder: "This textarea cannot be resized",
    resize: "none",
    rows: 4,
  },
};

/**
 * Both directions resize
 */
export const BothResize: Story = {
  args: {
    label: "Free Resize",
    placeholder: "Resize in any direction",
    resize: "both",
    rows: 4,
  },
};

/**
 * Disabled state
 */
export const Disabled: Story = {
  args: {
    label: "Disabled Textarea",
    placeholder: "This textarea is disabled",
    disabled: true,
    rows: 4,
  },
};

/**
 * Read-only state
 */
export const ReadOnly: Story = {
  args: {
    label: "Read Only",
    placeholder: "This textarea is read-only",
    readonly: true,
    rows: 4,
  },
};

/**
 * Many rows
 */
export const ManyRows: Story = {
  args: {
    label: "Long Form Content",
    placeholder: "Enter detailed content...",
    rows: 10,
    maxLength: 1000,
    showCharCount: true,
  },
};
