import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { DataTableComponent } from "./data-table.component";
import { DataTableColumnDirective } from "./data-table-column.directive";
import type { DataTableColumn } from "./data-table.types";

interface Vehicle {
  id: number;
  licensePlate: string;
  make: string;
  model: string;
  year: number;
  status: string;
  dailyRate: number;
}

const sampleVehicles: Vehicle[] = [
  {
    id: 1,
    licensePlate: "M-AB 1234",
    make: "BMW",
    model: "X5",
    year: 2024,
    status: "Available",
    dailyRate: 89,
  },
  {
    id: 2,
    licensePlate: "M-CD 5678",
    make: "VW",
    model: "Golf",
    year: 2023,
    status: "Rented",
    dailyRate: 45,
  },
  {
    id: 3,
    licensePlate: "M-EF 9012",
    make: "Mercedes",
    model: "A-Klasse",
    year: 2024,
    status: "Available",
    dailyRate: 55,
  },
  {
    id: 4,
    licensePlate: "M-GH 3456",
    make: "Audi",
    model: "A4",
    year: 2023,
    status: "Maintenance",
    dailyRate: 65,
  },
  {
    id: 5,
    licensePlate: "M-IJ 7890",
    make: "BMW",
    model: "3er",
    year: 2024,
    status: "Available",
    dailyRate: 75,
  },
  {
    id: 6,
    licensePlate: "M-KL 2345",
    make: "VW",
    model: "Passat",
    year: 2023,
    status: "Rented",
    dailyRate: 55,
  },
  {
    id: 7,
    licensePlate: "M-MN 6789",
    make: "Mercedes",
    model: "C-Klasse",
    year: 2024,
    status: "Available",
    dailyRate: 70,
  },
  {
    id: 8,
    licensePlate: "M-OP 0123",
    make: "Audi",
    model: "Q5",
    year: 2023,
    status: "Available",
    dailyRate: 85,
  },
  {
    id: 9,
    licensePlate: "M-QR 4567",
    make: "BMW",
    model: "X3",
    year: 2024,
    status: "Rented",
    dailyRate: 80,
  },
  {
    id: 10,
    licensePlate: "M-ST 8901",
    make: "VW",
    model: "Tiguan",
    year: 2023,
    status: "Available",
    dailyRate: 60,
  },
  {
    id: 11,
    licensePlate: "M-UV 2345",
    make: "Mercedes",
    model: "E-Klasse",
    year: 2024,
    status: "Maintenance",
    dailyRate: 95,
  },
  {
    id: 12,
    licensePlate: "M-WX 6789",
    make: "Audi",
    model: "A6",
    year: 2023,
    status: "Available",
    dailyRate: 80,
  },
];

const vehicleColumns: DataTableColumn<Vehicle>[] = [
  { key: "licensePlate", header: "Kennzeichen", sortable: true },
  { key: "make", header: "Marke", sortable: true },
  { key: "model", header: "Modell", sortable: true },
  { key: "year", header: "Baujahr", sortable: true, align: "center" },
  { key: "status", header: "Status", sortable: true },
  { key: "dailyRate", header: "Tagespreis", sortable: true, align: "right" },
];

const meta: Meta<DataTableComponent<Vehicle>> = {
  title: "Components/DataDisplay/DataTable",
  component: DataTableComponent,
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [DataTableComponent, DataTableColumnDirective],
    }),
  ],
  argTypes: {
    loading: {
      control: "boolean",
      description: "Whether the table is loading",
    },
    selectable: {
      control: "boolean",
      description: "Whether rows are selectable",
    },
    selectionMode: {
      control: "select",
      options: ["single", "multiple"],
      description: "Selection mode",
    },
    rowClickable: {
      control: "boolean",
      description: "Whether rows are clickable",
    },
    filterable: {
      control: "boolean",
      description: "Whether the table is filterable",
    },
    paginated: {
      control: "boolean",
      description: "Whether to show pagination",
    },
    pageSize: {
      control: "number",
      description: "Items per page",
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A feature-rich data table component with sorting, filtering, pagination, and row selection.

## Features
- Column sorting (click headers)
- Global text filtering
- Client-side pagination
- Row selection (single/multiple)
- Custom cell templates
- Loading and empty states
- Responsive card view on mobile

## Usage with columns input
\`\`\`typescript
const columns: DataTableColumn<Vehicle>[] = [
  { key: 'licensePlate', header: 'Kennzeichen', sortable: true },
  { key: 'make', header: 'Marke', sortable: true },
];
\`\`\`

\`\`\`html
<ocr-data-table
  [data]="vehicles"
  [columns]="columns"
  [paginated]="true"
  [pageSize]="10"
>
</ocr-data-table>
\`\`\`
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<DataTableComponent<Vehicle>>;

export const Default: Story = {
  render: (args) => ({
    props: {
      ...args,
      data: sampleVehicles,
      columns: vehicleColumns,
      onSortChange: (event: unknown) => console.log("Sort changed:", event),
      onRowClick: (event: unknown) => console.log("Row clicked:", event),
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [loading]="loading"
        [selectable]="selectable"
        [selectionMode]="selectionMode"
        [rowClickable]="rowClickable"
        [filterable]="filterable"
        [paginated]="paginated"
        [pageSize]="pageSize"
        (sortChange)="onSortChange($event)"
        (rowClick)="onRowClick($event)"
      >
      </ocr-data-table>
    `,
  }),
  args: {
    loading: false,
    selectable: false,
    selectionMode: "multiple",
    rowClickable: false,
    filterable: false,
    paginated: false,
    pageSize: 10,
  },
};

export const WithSorting: Story = {
  render: () => ({
    props: {
      data: sampleVehicles,
      columns: vehicleColumns,
    },
    template: `
      <ocr-data-table [data]="data" [columns]="columns">
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Click on column headers to sort. Click again to reverse, third click clears sorting.",
      },
    },
  },
};

export const WithFiltering: Story = {
  render: () => ({
    props: {
      data: sampleVehicles,
      columns: vehicleColumns,
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [filterable]="true"
        filterPlaceholder="Fahrzeug suchen..."
      >
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Type in the search box to filter across all columns.",
      },
    },
  },
};

export const WithPagination: Story = {
  render: () => ({
    props: {
      data: sampleVehicles,
      columns: vehicleColumns,
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [paginated]="true"
        [pageSize]="5"
        itemLabel="Fahrzeug"
        itemLabelPlural="Fahrzeuge"
      >
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Table with client-side pagination showing 5 items per page.",
      },
    },
  },
};

export const WithSelection: Story = {
  render: () => ({
    props: {
      data: sampleVehicles.slice(0, 5),
      columns: vehicleColumns,
      onSelectionChange: (event: unknown) => console.log("Selection:", event),
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [selectable]="true"
        selectionMode="multiple"
        (selectionChange)="onSelectionChange($event)"
      >
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Table with row selection. Check individual rows or use the header checkbox to select all.",
      },
    },
  },
};

export const SingleSelection: Story = {
  render: () => ({
    props: {
      data: sampleVehicles.slice(0, 5),
      columns: vehicleColumns,
      onSelectionChange: (event: unknown) => console.log("Selection:", event),
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [selectable]="true"
        selectionMode="single"
        (selectionChange)="onSelectionChange($event)"
      >
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Single selection mode - only one row can be selected at a time.",
      },
    },
  },
};

export const ClickableRows: Story = {
  render: () => ({
    props: {
      data: sampleVehicles.slice(0, 5),
      columns: vehicleColumns,
      onRowClick: (event: unknown) => console.log("Row clicked:", event),
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [rowClickable]="true"
        (rowClick)="onRowClick($event)"
      >
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Rows emit click events when clicked. Useful for navigation or detail views.",
      },
    },
  },
};

export const Loading: Story = {
  render: () => ({
    props: {
      data: [],
      columns: vehicleColumns,
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [loading]="true"
        loadingMessage="Lade Fahrzeuge..."
      >
      </ocr-data-table>
    `,
  }),
};

export const Empty: Story = {
  render: () => ({
    props: {
      data: [],
      columns: vehicleColumns,
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        emptyIcon="car"
        emptyTitle="Keine Fahrzeuge gefunden"
        emptyDescription="Es sind keine Fahrzeuge vorhanden, die Ihren Kriterien entsprechen."
      >
      </ocr-data-table>
    `,
  }),
};

export const FullFeatured: Story = {
  render: () => ({
    props: {
      data: sampleVehicles,
      columns: vehicleColumns,
      onSortChange: (event: unknown) => console.log("Sort:", event),
      onSelectionChange: (event: unknown) => console.log("Selection:", event),
      onRowClick: (event: unknown) => console.log("Click:", event),
    },
    template: `
      <ocr-data-table
        [data]="data"
        [columns]="columns"
        [selectable]="true"
        [rowClickable]="true"
        [filterable]="true"
        [paginated]="true"
        [pageSize]="5"
        filterPlaceholder="Fahrzeug suchen..."
        itemLabel="Fahrzeug"
        itemLabelPlural="Fahrzeuge"
        (sortChange)="onSortChange($event)"
        (selectionChange)="onSelectionChange($event)"
        (rowClick)="onRowClick($event)"
      >
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Full-featured table with sorting, filtering, pagination, and selection.",
      },
    },
  },
};

export const WithCustomCells: Story = {
  render: () => ({
    props: {
      data: sampleVehicles.slice(0, 5),
    },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Kennzeichen'; sortable: true">
          <ng-template #cell let-item let-value="value">
            <code style="background: #f3f4f6; padding: 0.125rem 0.375rem; border-radius: 0.25rem;">
              {{ value }}
            </code>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Marke'; sortable: true">
          <ng-template #cell let-item>
            <strong>{{ item.make }}</strong>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'model'; header: 'Modell'">
          <ng-template #cell let-item>
            {{ item.model }}
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'status'; header: 'Status'; sortable: true">
          <ng-template #cell let-item>
            <span [style.color]="item.status === 'Available' ? '#10b981' : item.status === 'Rented' ? '#f97316' : '#6b7280'">
              {{ item.status }}
            </span>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'dailyRate'; header: 'Tagespreis'; sortable: true; align: 'right'">
          <ng-template #cell let-item>
            <span style="font-weight: 600;">€{{ item.dailyRate }}/Tag</span>
          </ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Using directive-based column definitions with custom cell templates.",
      },
    },
  },
};
