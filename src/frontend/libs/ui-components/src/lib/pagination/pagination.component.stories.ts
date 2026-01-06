import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { PaginationComponent } from "./pagination.component";

const meta: Meta<PaginationComponent> = {
  title: "Components/Navigation/Pagination",
  component: PaginationComponent,
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [PaginationComponent],
    }),
  ],
  argTypes: {
    currentPage: {
      control: { type: "number", min: 1 },
      description: "Current page number (1-indexed)",
    },
    totalPages: {
      control: { type: "number", min: 1 },
      description: "Total number of pages",
    },
    totalItems: {
      control: "number",
      description: "Total count of items (optional, for display)",
    },
    pageSize: {
      control: "number",
      description: "Items per page (optional, for display)",
    },
    previousLabel: {
      control: "text",
      description: "Label for previous button",
    },
    nextLabel: {
      control: "text",
      description: "Label for next button",
    },
    showItemRange: {
      control: "boolean",
      description: 'Show item range info (e.g., "Showing 1-10 of 100")',
    },
    itemLabel: {
      control: "text",
      description: "Item label for range display (singular)",
    },
    itemLabelPlural: {
      control: "text",
      description: "Item label for range display (plural)",
    },
  },
  parameters: {
    docs: {
      description: {
        component:
          "Reusable pagination component with ellipsis for large page ranges. Supports item range display and German localization.",
      },
    },
  },
};

export default meta;
type Story = StoryObj<PaginationComponent>;

export const Default: Story = {
  args: {
    currentPage: 5,
    totalPages: 10,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    showItemRange: false,
  },
};

export const FirstPage: Story = {
  args: {
    currentPage: 1,
    totalPages: 10,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    showItemRange: false,
  },
};

export const LastPage: Story = {
  args: {
    currentPage: 10,
    totalPages: 10,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    showItemRange: false,
  },
};

export const FewPages: Story = {
  args: {
    currentPage: 2,
    totalPages: 3,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    showItemRange: false,
  },
};

export const ManyPages: Story = {
  args: {
    currentPage: 25,
    totalPages: 50,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    showItemRange: false,
  },
  parameters: {
    docs: {
      description: {
        story:
          "With many pages, ellipsis are shown between the first page, current page region, and last page.",
      },
    },
  },
};

export const WithItemRange: Story = {
  args: {
    currentPage: 3,
    totalPages: 10,
    totalItems: 95,
    pageSize: 10,
    showItemRange: true,
    itemLabel: "Eintrag",
    itemLabelPlural: "Einträge",
    previousLabel: "Zurück",
    nextLabel: "Weiter",
  },
  parameters: {
    docs: {
      description: {
        story:
          'Shows item range information like "Showing 21-30 of 95 entries".',
      },
    },
  },
};

export const EnglishLabels: Story = {
  args: {
    currentPage: 5,
    totalPages: 20,
    totalItems: 195,
    pageSize: 10,
    showItemRange: true,
    itemLabel: "item",
    itemLabelPlural: "items",
    previousLabel: "Previous",
    nextLabel: "Next",
  },
  parameters: {
    docs: {
      description: {
        story: "Example with English labels instead of German.",
      },
    },
  },
};

export const SinglePage: Story = {
  args: {
    currentPage: 1,
    totalPages: 1,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    showItemRange: false,
  },
  parameters: {
    docs: {
      description: {
        story:
          "When there is only one page, both navigation buttons are disabled.",
      },
    },
  },
};

export const VehicleList: Story = {
  render: () => ({
    props: {
      currentPage: 2,
      totalPages: 8,
      totalItems: 75,
      pageSize: 10,
      onPageChange: (page: number) => console.log("Page changed to:", page),
    },
    template: `
      <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1rem;">
        <h3 style="margin: 0 0 1rem 0; font-size: 1rem; font-weight: 600;">Fahrzeuge</h3>
        <div style="display: flex; flex-direction: column; gap: 0.5rem; margin-bottom: 1rem;">
          <div style="padding: 0.75rem; background: #f9fafb; border-radius: 0.375rem;">BMW X5 - M-AB 1234</div>
          <div style="padding: 0.75rem; background: #f9fafb; border-radius: 0.375rem;">VW Golf - M-CD 5678</div>
          <div style="padding: 0.75rem; background: #f9fafb; border-radius: 0.375rem;">Mercedes A-Klasse - M-EF 9012</div>
        </div>
        <ui-pagination
          [currentPage]="currentPage"
          [totalPages]="totalPages"
          [totalItems]="totalItems"
          [pageSize]="pageSize"
          [showItemRange]="true"
          itemLabel="Fahrzeug"
          itemLabelPlural="Fahrzeuge"
          (pageChange)="onPageChange($event)"
        ></ui-pagination>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Example of pagination in a vehicle list context.",
      },
    },
  },
};
