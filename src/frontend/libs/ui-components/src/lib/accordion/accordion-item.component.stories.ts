import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { AccordionComponent } from './accordion.component';
import { AccordionItemComponent } from './accordion-item.component';

const meta: Meta<AccordionItemComponent> = {
  title: 'Components/Layout/AccordionItem',
  component: AccordionItemComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [AccordionComponent, AccordionItemComponent],
    }),
  ],
  argTypes: {
    itemId: {
      control: 'text',
      description: 'Unique identifier for this item',
    },
    title: {
      control: 'text',
      description: 'Title/header text for the accordion item',
    },
    subtitle: {
      control: 'text',
      description: 'Optional subtitle or description',
    },
    disabled: {
      control: 'boolean',
      description: 'Whether this item is disabled',
    },
    expanded: {
      control: 'boolean',
      description: 'Whether this item is initially expanded',
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A single expandable item within an Accordion container.

## Features
- Required unique ID for tracking
- Title and optional subtitle
- Disabled state support
- Initial expanded state option
- Lazy content loading via ng-template

## Usage
AccordionItemComponent must be used inside an AccordionComponent:

\`\`\`html
<ocr-accordion>
  <ocr-accordion-item itemId="item1" title="My Title" subtitle="Optional subtitle">
    <ng-template>
      <p>Content here is lazily rendered</p>
    </ng-template>
  </ocr-accordion-item>
</ocr-accordion>
\`\`\`

## Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| itemId | string | required | Unique identifier |
| title | string | required | Header text |
| subtitle | string | undefined | Optional description |
| disabled | boolean | false | Prevents expansion |
| expanded | boolean | false | Initial state |
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<AccordionItemComponent>;

export const Default: Story = {
  render: (args) => ({
    props: args,
    template: `
      <ocr-accordion>
        <ocr-accordion-item
          [itemId]="itemId"
          [title]="title"
          [subtitle]="subtitle"
          [disabled]="disabled"
          [expanded]="expanded"
        >
          <ng-template>
            <p style="margin: 0; padding: 0.5rem 0; color: #6b7280; line-height: 1.6;">
              Dies ist der Inhalt des Accordion-Items. Der Inhalt wird erst gerendert,
              wenn das Item zum ersten Mal geöffnet wird (Lazy Loading).
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  args: {
    itemId: 'demo-item',
    title: 'Beispiel Accordion Item',
    subtitle: 'Optionaler Untertitel',
    disabled: false,
    expanded: true,
  },
};

export const WithoutSubtitle: Story = {
  render: () => ({
    template: `
      <ocr-accordion>
        <ocr-accordion-item itemId="simple" title="Einfaches Item" [expanded]="true">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Ein Accordion-Item ohne Untertitel.
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Accordion item without a subtitle - just the title is displayed.',
      },
    },
  },
};

export const Disabled: Story = {
  render: () => ({
    template: `
      <ocr-accordion>
        <ocr-accordion-item
          itemId="disabled-item"
          title="Deaktiviertes Item"
          subtitle="Kann nicht geöffnet werden"
          [disabled]="true"
        >
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Dieser Inhalt ist nicht erreichbar.
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'A disabled accordion item that cannot be expanded or collapsed.',
      },
    },
  },
};

export const InitiallyExpanded: Story = {
  render: () => ({
    template: `
      <ocr-accordion>
        <ocr-accordion-item
          itemId="expanded-item"
          title="Standardmäßig geöffnet"
          [expanded]="true"
        >
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Dieses Item ist beim Laden bereits geöffnet.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item
          itemId="collapsed-item"
          title="Standardmäßig geschlossen"
        >
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Dieses Item muss erst angeklickt werden.
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Use the expanded input to control initial expansion state.',
      },
    },
  },
};

export const RichContent: Story = {
  render: () => ({
    template: `
      <ocr-accordion>
        <ocr-accordion-item
          itemId="rich"
          title="Fahrzeugdetails"
          subtitle="BMW X5 - M-AB 1234"
          [expanded]="true"
        >
          <ng-template>
            <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem; padding: 0.5rem 0;">
              <div>
                <span style="font-size: 0.75rem; color: #9ca3af; text-transform: uppercase;">Marke</span>
                <p style="margin: 0.25rem 0 0 0; font-weight: 500;">BMW</p>
              </div>
              <div>
                <span style="font-size: 0.75rem; color: #9ca3af; text-transform: uppercase;">Modell</span>
                <p style="margin: 0.25rem 0 0 0; font-weight: 500;">X5</p>
              </div>
              <div>
                <span style="font-size: 0.75rem; color: #9ca3af; text-transform: uppercase;">Baujahr</span>
                <p style="margin: 0.25rem 0 0 0; font-weight: 500;">2024</p>
              </div>
              <div>
                <span style="font-size: 0.75rem; color: #9ca3af; text-transform: uppercase;">Tagespreis</span>
                <p style="margin: 0.25rem 0 0 0; font-weight: 600; color: #f97316;">€89,00</p>
              </div>
            </div>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Accordion items can contain rich, structured content.',
      },
    },
  },
};
