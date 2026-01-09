import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { ReactiveFormsModule } from "@angular/forms";
import { VehicleSearchComponent } from "./vehicle-search.component";

const meta: Meta<VehicleSearchComponent> = {
  title: "Forms/Vehicle Search",
  component: VehicleSearchComponent,
  decorators: [
    moduleMetadata({
      imports: [ReactiveFormsModule],
    }),
  ],
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A comprehensive vehicle search form component with date, location, and filter options.

## Features
- Pickup/return date and time selection
- Location selection (auto-loads from API)
- Category, fuel type, transmission filters
- Minimum seats filter
- Form validation
- Reset functionality

## Usage

\`\`\`html
<ocr-vehicle-search (search)="onSearch($event)" />
\`\`\`

## Events
- \`search\`: Emits VehicleSearchQuery when form is submitted
        `,
      },
    },
  },
  tags: ["autodocs"],
};

export default meta;
type Story = StoryObj<VehicleSearchComponent>;

/**
 * Default vehicle search form with all options
 */
export const Default: Story = {
  render: () => ({
    template: `
      <ocr-vehicle-search (search)="onSearch($event)"></ocr-vehicle-search>
      <div style="margin-top: 1rem; padding: 1rem; background: #f3f4f6; border-radius: 0.5rem;">
        <p style="margin: 0; font-size: 0.875rem; color: #6b7280;">
          Search query will appear here when form is submitted
        </p>
      </div>
    `,
    props: {
      onSearch: (query: unknown) => console.log("Search query:", query),
    },
  }),
};

/**
 * Vehicle search with action logging
 */
export const WithLogging: Story = {
  render: () => ({
    props: {
      searchQuery: null as unknown,
      onSearch: function (this: { searchQuery: unknown }, query: unknown) {
        this.searchQuery = query;
      },
    },
    template: `
      <ocr-vehicle-search (search)="onSearch($event)"></ocr-vehicle-search>
      @if (searchQuery) {
        <div style="margin-top: 1rem; padding: 1rem; background: #dcfce7; border: 1px solid #86efac; border-radius: 0.5rem;">
          <p style="margin: 0 0 0.5rem; font-weight: 600; color: #166534;">Search Query:</p>
          <pre style="margin: 0; font-size: 0.75rem; color: #166534; white-space: pre-wrap;">{{ searchQuery | json }}</pre>
        </div>
      }
    `,
  }),
};
