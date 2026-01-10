import type { Meta, StoryObj } from "@storybook/angular";
import { ButtonComponent } from "./button.component";

const meta: Meta<ButtonComponent> = {
  title: "Components/Button",
  component: ButtonComponent,
  tags: ["autodocs"],
  argTypes: {
    variant: {
      control: "select",
      options: ["primary", "secondary", "danger", "success", "ghost"],
      description: "Visual style variant",
    },
    size: {
      control: "select",
      options: ["sm", "md", "lg"],
      description: "Button size",
    },
    type: {
      control: "select",
      options: ["button", "submit", "reset"],
      description: "HTML button type attribute",
    },
    disabled: {
      control: "boolean",
      description: "Whether button is disabled",
    },
    loading: {
      control: "boolean",
      description: "Whether button shows loading spinner",
    },
    block: {
      control: "boolean",
      description: "Whether button takes full width",
    },
    icon: {
      control: "text",
      description: "Icon name to display",
    },
    iconPosition: {
      control: "select",
      options: ["left", "right"],
      description: "Position of the icon",
    },
    iconOnly: {
      control: "boolean",
      description: "Whether to show only the icon",
    },
  },
};

export default meta;
type Story = StoryObj<ButtonComponent>;

export const Primary: Story = {
  args: {
    variant: "primary",
    size: "md",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [size]="size">Primary Button</ocr-button>`,
  }),
};

export const Secondary: Story = {
  args: {
    variant: "secondary",
    size: "md",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [size]="size">Secondary Button</ocr-button>`,
  }),
};

export const Danger: Story = {
  args: {
    variant: "danger",
    size: "md",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [size]="size">Delete</ocr-button>`,
  }),
};

export const Success: Story = {
  args: {
    variant: "success",
    size: "md",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [size]="size">Confirm</ocr-button>`,
  }),
};

export const Ghost: Story = {
  args: {
    variant: "ghost",
    size: "md",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [size]="size">Ghost Button</ocr-button>`,
  }),
};

export const Sizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; align-items: center; gap: 1rem;">
        <ocr-button size="sm">Small</ocr-button>
        <ocr-button size="md">Medium</ocr-button>
        <ocr-button size="lg">Large</ocr-button>
      </div>
    `,
  }),
};

export const WithIcon: Story = {
  args: {
    icon: "plus",
    variant: "primary",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [icon]="icon">Add Item</ocr-button>`,
  }),
};

export const IconRight: Story = {
  args: {
    icon: "arrow-right",
    iconPosition: "right",
    variant: "primary",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [icon]="icon" [iconPosition]="iconPosition">Next</ocr-button>`,
  }),
};

export const IconOnly: Story = {
  args: {
    icon: "x-mark",
    iconOnly: true,
    variant: "ghost",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [icon]="icon" [iconOnly]="iconOnly">Close</ocr-button>`,
  }),
};

export const Loading: Story = {
  args: {
    loading: true,
    variant: "primary",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [loading]="loading">Saving...</ocr-button>`,
  }),
};

export const Disabled: Story = {
  args: {
    disabled: true,
    variant: "primary",
  },
  render: (args) => ({
    props: args,
    template: `<ocr-button [variant]="variant" [disabled]="disabled">Disabled</ocr-button>`,
  }),
};

export const Block: Story = {
  args: {
    block: true,
    variant: "primary",
  },
  render: (args) => ({
    props: args,
    template: `
      <div style="width: 300px;">
        <ocr-button [variant]="variant" [block]="block">Full Width Button</ocr-button>
      </div>
    `,
  }),
};

export const AllVariants: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1rem;">
        <div style="display: flex; gap: 0.5rem; align-items: center;">
          <ocr-button variant="primary">Primary</ocr-button>
          <ocr-button variant="secondary">Secondary</ocr-button>
          <ocr-button variant="danger">Danger</ocr-button>
          <ocr-button variant="success">Success</ocr-button>
          <ocr-button variant="ghost">Ghost</ocr-button>
        </div>
        <div style="display: flex; gap: 0.5rem; align-items: center;">
          <ocr-button variant="primary" [loading]="true">Loading</ocr-button>
          <ocr-button variant="primary" [disabled]="true">Disabled</ocr-button>
          <ocr-button variant="primary" icon="plus">With Icon</ocr-button>
          <ocr-button variant="ghost" icon="x-mark" [iconOnly]="true">Close</ocr-button>
        </div>
      </div>
    `,
  }),
};
