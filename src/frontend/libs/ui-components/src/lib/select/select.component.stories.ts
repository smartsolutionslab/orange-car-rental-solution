import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormsModule } from "@angular/forms";
import { SelectComponent, type SelectOption } from "./select.component";

const meta: Meta<SelectComponent<string>> = {
  title: "Forms/Select",
  component: SelectComponent,
  decorators: [
    moduleMetadata({
      imports: [FormsModule],
    }),
  ],
  tags: ["autodocs"],
  argTypes: {
    size: {
      control: { type: "select" },
      options: ["sm", "md", "lg"],
    },
    label: { control: "text" },
    placeholder: { control: "text" },
    hint: { control: "text" },
    error: { control: "text" },
    required: { control: "boolean" },
    disabled: { control: "boolean" },
    leadingIcon: { control: "text" },
  },
};

export default meta;
type Story = StoryObj<SelectComponent<string>>;

const defaultOptions: SelectOption<string>[] = [
  { value: "option1", label: "Option 1" },
  { value: "option2", label: "Option 2" },
  { value: "option3", label: "Option 3" },
];

const sortOptions: SelectOption<string>[] = [
  { value: "date_asc", label: "Date (Oldest first)" },
  { value: "date_desc", label: "Date (Newest first)" },
  { value: "name_asc", label: "Name (A-Z)" },
  { value: "name_desc", label: "Name (Z-A)" },
  { value: "price_asc", label: "Price (Low to High)" },
  { value: "price_desc", label: "Price (High to Low)" },
];

const cancellationReasons: SelectOption<string>[] = [
  { value: "change_plans", label: "Change of plans" },
  { value: "alternative", label: "Found alternative transportation" },
  { value: "trip_cancelled", label: "Trip cancelled" },
  { value: "booking_error", label: "Booking error" },
  { value: "other", label: "Other" },
];

export const Default: Story = {
  args: {
    label: "Select Option",
    placeholder: "Choose an option",
    options: defaultOptions,
    size: "md",
    required: false,
    disabled: false,
  },
};

export const WithLabel: Story = {
  args: {
    label: "Sort By",
    placeholder: "Select sort order",
    options: sortOptions,
    size: "md",
  },
};

export const Required: Story = {
  args: {
    label: "Cancellation Reason",
    placeholder: "Select a reason",
    options: cancellationReasons,
    required: true,
  },
};

export const WithHint: Story = {
  args: {
    label: "Group By",
    placeholder: "Select grouping",
    options: [
      { value: "none", label: "No grouping" },
      { value: "status", label: "By Status" },
      { value: "date", label: "By Date" },
      { value: "location", label: "By Location" },
    ],
    hint: "Group reservations by category",
  },
};

export const WithError: Story = {
  args: {
    label: "Cancellation Reason",
    placeholder: "Select a reason",
    options: cancellationReasons,
    required: true,
    error: "Please select a cancellation reason",
  },
};

export const WithLeadingIcon: Story = {
  args: {
    label: "Sort Order",
    placeholder: "Select sort order",
    options: sortOptions,
    leadingIcon: "arrow-up-down",
  },
};

export const Disabled: Story = {
  args: {
    label: "Disabled Select",
    placeholder: "Cannot select",
    options: defaultOptions,
    disabled: true,
  },
};

export const Small: Story = {
  args: {
    label: "Small Select",
    placeholder: "Choose option",
    options: defaultOptions,
    size: "sm",
  },
};

export const Large: Story = {
  args: {
    label: "Large Select",
    placeholder: "Choose option",
    options: defaultOptions,
    size: "lg",
  },
};

export const WithDisabledOptions: Story = {
  args: {
    label: "Select with Disabled Options",
    placeholder: "Choose option",
    options: [
      { value: "available1", label: "Available Option 1" },
      { value: "unavailable1", label: "Unavailable Option", disabled: true },
      { value: "available2", label: "Available Option 2" },
      { value: "unavailable2", label: "Also Unavailable", disabled: true },
    ],
  },
};
