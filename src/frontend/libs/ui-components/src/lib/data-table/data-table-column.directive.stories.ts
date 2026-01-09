import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { DataTableComponent } from './data-table.component';
import { DataTableColumnDirective } from './data-table-column.directive';

interface Vehicle {
  id: number;
  licensePlate: string;
  make: string;
  model: string;
  status: string;
  dailyRate: number;
}

const sampleVehicles: Vehicle[] = [
  { id: 1, licensePlate: 'M-AB 1234', make: 'BMW', model: 'X5', status: 'Available', dailyRate: 89 },
  { id: 2, licensePlate: 'M-CD 5678', make: 'VW', model: 'Golf', status: 'Rented', dailyRate: 45 },
  { id: 3, licensePlate: 'M-EF 9012', make: 'Mercedes', model: 'A-Klasse', status: 'Available', dailyRate: 55 },
  { id: 4, licensePlate: 'M-GH 3456', make: 'Audi', model: 'A4', status: 'Maintenance', dailyRate: 65 },
];

const meta: Meta<DataTableColumnDirective<Vehicle>> = {
  title: 'Components/DataDisplay/DataTableColumn',
  component: DataTableColumnDirective,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [DataTableComponent, DataTableColumnDirective],
    }),
  ],
  parameters: {
    docs: {
      description: {
        component: `
A structural directive for defining data table columns with custom templates.

## Features
- Define column key and header text
- Sortable/filterable configuration
- Custom cell templates
- Custom header templates
- Column width and alignment control
- Mobile visibility options

## Usage
Use the directive with \`*ocrDataTableColumn\` structural syntax:

\`\`\`html
<ocr-data-table [data]="vehicles">
  <ng-container *ocrDataTableColumn="'status'; header: 'Status'; sortable: true">
    <ng-template #cell let-item let-value="value">
      <span [class]="getStatusClass(value)">{{ value }}</span>
    </ng-template>
  </ng-container>
</ocr-data-table>
\`\`\`

## Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| ocrDataTableColumn | string | required | Column key (property name) |
| header | string | required | Column header text |
| sortable | boolean | false | Enable sorting |
| filterable | boolean | false | Enable filtering |
| width | string | undefined | CSS width value |
| minWidth | string | undefined | CSS min-width |
| align | 'left'/'center'/'right' | 'left' | Text alignment |
| hideOnMobile | boolean | false | Hide on small screens |

## Templates
- \`#cell\` - Custom cell template with access to item and value
- \`#header\` - Custom header template
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<DataTableColumnDirective<Vehicle>>;

export const BasicColumns: Story = {
  render: () => ({
    props: { data: sampleVehicles },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Kennzeichen'">
          <ng-template #cell let-value="value">
            {{ value }}
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Marke'">
          <ng-template #cell let-value="value">
            {{ value }}
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'model'; header: 'Modell'">
          <ng-template #cell let-value="value">
            {{ value }}
          </ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Basic column definitions using the directive syntax.',
      },
    },
  },
};

export const SortableColumns: Story = {
  render: () => ({
    props: { data: sampleVehicles },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Kennzeichen'; sortable: true">
          <ng-template #cell let-value="value">
            <code style="background: #f3f4f6; padding: 0.125rem 0.375rem; border-radius: 0.25rem;">
              {{ value }}
            </code>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Marke'; sortable: true">
          <ng-template #cell let-value="value">
            <strong>{{ value }}</strong>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'dailyRate'; header: 'Tagespreis'; sortable: true; align: 'right'">
          <ng-template #cell let-value="value">
            €{{ value }}/Tag
          </ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Columns with sortable: true can be sorted by clicking the header.',
      },
    },
  },
};

export const CustomCellTemplates: Story = {
  render: () => ({
    props: { data: sampleVehicles },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Kennzeichen'">
          <ng-template #cell let-item let-value="value">
            <div style="display: flex; align-items: center; gap: 0.5rem;">
              <span style="width: 8px; height: 8px; border-radius: 50%; background: #10b981;"></span>
              <code style="background: #f3f4f6; padding: 0.125rem 0.5rem; border-radius: 0.25rem; font-size: 0.875rem;">
                {{ value }}
              </code>
            </div>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Fahrzeug'">
          <ng-template #cell let-item>
            <div>
              <div style="font-weight: 600;">{{ item.make }}</div>
              <div style="font-size: 0.75rem; color: #6b7280;">{{ item.model }}</div>
            </div>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'status'; header: 'Status'">
          <ng-template #cell let-item>
            <span [style.background]="item.status === 'Available' ? '#dcfce7' : item.status === 'Rented' ? '#fef3c7' : '#f3f4f6'"
                  [style.color]="item.status === 'Available' ? '#166534' : item.status === 'Rented' ? '#92400e' : '#374151'"
                  style="padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.75rem; font-weight: 500;">
              {{ item.status }}
            </span>
          </ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'dailyRate'; header: 'Preis'; align: 'right'">
          <ng-template #cell let-item>
            <span style="font-weight: 600; color: #f97316;">€{{ item.dailyRate }}</span>
            <span style="font-size: 0.75rem; color: #6b7280;">/Tag</span>
          </ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Custom cell templates allow full control over cell rendering. Access both `value` and `item` in templates.',
      },
    },
  },
};

export const ColumnAlignment: Story = {
  render: () => ({
    props: { data: sampleVehicles },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Links (Standard)'; align: 'left'">
          <ng-template #cell let-value="value">{{ value }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Zentriert'; align: 'center'">
          <ng-template #cell let-value="value">{{ value }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'dailyRate'; header: 'Rechts'; align: 'right'">
          <ng-template #cell let-value="value">€{{ value }}</ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Use align input to control text alignment: left (default), center, or right.',
      },
    },
  },
};

export const ColumnWidths: Story = {
  render: () => ({
    props: { data: sampleVehicles },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Kennzeichen'; width: '150px'">
          <ng-template #cell let-value="value">{{ value }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Marke'; minWidth: '100px'">
          <ng-template #cell let-value="value">{{ value }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'model'; header: 'Modell'">
          <ng-template #cell let-value="value">{{ value }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'dailyRate'; header: 'Preis'; width: '100px'; align: 'right'">
          <ng-template #cell let-value="value">€{{ value }}</ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Use width and minWidth to control column sizing.',
      },
    },
  },
};

export const ActionColumn: Story = {
  render: () => ({
    props: {
      data: sampleVehicles,
      onEdit: (item: Vehicle) => console.log('Edit:', item),
      onDelete: (item: Vehicle) => console.log('Delete:', item),
    },
    template: `
      <ocr-data-table [data]="data">
        <ng-container *ocrDataTableColumn="'licensePlate'; header: 'Kennzeichen'; sortable: true">
          <ng-template #cell let-value="value">{{ value }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'make'; header: 'Marke'">
          <ng-template #cell let-item>{{ item.make }} {{ item.model }}</ng-template>
        </ng-container>
        <ng-container *ocrDataTableColumn="'id'; header: 'Aktionen'; align: 'right'">
          <ng-template #cell let-item>
            <div style="display: flex; gap: 0.5rem; justify-content: flex-end;">
              <button
                (click)="onEdit(item)"
                style="padding: 0.25rem 0.5rem; background: #3b82f6; color: white; border: none; border-radius: 0.25rem; cursor: pointer; font-size: 0.75rem;"
              >
                Bearbeiten
              </button>
              <button
                (click)="onDelete(item)"
                style="padding: 0.25rem 0.5rem; background: #ef4444; color: white; border: none; border-radius: 0.25rem; cursor: pointer; font-size: 0.75rem;"
              >
                Löschen
              </button>
            </div>
          </ng-template>
        </ng-container>
      </ocr-data-table>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Create action columns with buttons by using a non-data column key.',
      },
    },
  },
};
