import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { AccordionComponent } from "./accordion.component";
import { AccordionItemComponent } from "./accordion-item.component";

const meta: Meta<AccordionComponent> = {
  title: "Components/Layout/Accordion",
  component: AccordionComponent,
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [AccordionComponent, AccordionItemComponent],
    }),
  ],
  argTypes: {
    multiple: {
      control: "boolean",
      description: "Allow multiple items to be expanded at once",
    },
    keepAlive: {
      control: "boolean",
      description: "Keep content in DOM after collapse",
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A collapsible content panel component with smooth animations.

## Features
- Single or multiple expansion modes
- Animated expand/collapse transitions
- Keyboard navigation (arrow keys, Home, End)
- Accessible ARIA attributes
- Lazy content loading
- Disabled item support

## Keyboard Navigation
- **Arrow Up/Down**: Move between accordion headers
- **Home**: Go to first header
- **End**: Go to last header
- **Enter/Space**: Toggle current item
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<AccordionComponent>;

export const Default: Story = {
  render: (args) => ({
    props: {
      ...args,
      onItemChange: (event: unknown) => console.log("Item changed:", event),
    },
    template: `
      <ocr-accordion
        [multiple]="multiple"
        [keepAlive]="keepAlive"
        (itemChange)="onItemChange($event)"
      >
        <ocr-accordion-item itemId="item1" title="Was ist Orange Car Rental?" [expanded]="true">
          <ng-template>
            <p style="margin: 0; color: #6b7280; line-height: 1.6;">
              Orange Car Rental ist ein moderner Autovermietungsservice, der Ihnen eine breite Palette
              von Fahrzeugen für jeden Bedarf bietet. Von Kompaktwagen bis hin zu Luxuslimousinen -
              wir haben das passende Fahrzeug für Sie.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="item2" title="Wie kann ich ein Fahrzeug buchen?">
          <ng-template>
            <p style="margin: 0; color: #6b7280; line-height: 1.6;">
              Sie können ein Fahrzeug ganz einfach über unsere Website buchen. Wählen Sie Ihren
              gewünschten Abhol- und Rückgabeort, Datum und Zeit aus. Anschließend können Sie aus
              unserer verfügbaren Flotte das passende Fahrzeug auswählen.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="item3" title="Welche Dokumente benötige ich?">
          <ng-template>
            <p style="margin: 0; color: #6b7280; line-height: 1.6;">
              Für die Fahrzeugübergabe benötigen Sie einen gültigen Führerschein (mindestens 1 Jahr alt),
              einen gültigen Personalausweis oder Reisepass sowie eine Kreditkarte für die Kaution.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="item4" title="Wie funktioniert die Stornierung?">
          <ng-template>
            <p style="margin: 0; color: #6b7280; line-height: 1.6;">
              Stornierungen sind bis 24 Stunden vor der geplanten Abholung kostenlos möglich.
              Bei späteren Stornierungen kann eine Gebühr anfallen. Die genauen Stornierungsbedingungen
              finden Sie in unseren AGB.
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  args: {
    multiple: false,
    keepAlive: true,
  },
};

export const MultipleOpen: Story = {
  render: () => ({
    template: `
      <ocr-accordion [multiple]="true">
        <ocr-accordion-item itemId="a" title="Kompaktwagen" [expanded]="true">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Ideal für Stadtfahrten und kurze Strecken. Sparsam im Verbrauch und einfach zu parken.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="b" title="SUV" [expanded]="true">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Perfekt für Familien und Outdoor-Abenteuer. Viel Platz und Komfort für lange Reisen.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="c" title="Luxuslimousine">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Für besondere Anlässe und Geschäftsreisen. Premium-Ausstattung und erstklassiger Komfort.
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "In multiple mode, several accordion items can be open simultaneously.",
      },
    },
  },
};

export const WithSubtitles: Story = {
  render: () => ({
    template: `
      <ocr-accordion>
        <ocr-accordion-item
          itemId="step1"
          title="Schritt 1: Fahrzeug wählen"
          subtitle="Wählen Sie aus unserer Flotte"
          [expanded]="true"
        >
          <ng-template>
            <div style="padding: 0.5rem 0;">
              <p style="margin: 0 0 1rem 0; color: #6b7280;">
                Durchsuchen Sie unsere verfügbaren Fahrzeuge und wählen Sie das passende aus.
              </p>
              <button style="padding: 0.5rem 1rem; background: #f97316; color: white; border: none; border-radius: 0.375rem; cursor: pointer;">
                Fahrzeuge ansehen
              </button>
            </div>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item
          itemId="step2"
          title="Schritt 2: Zeitraum festlegen"
          subtitle="Abhol- und Rückgabedatum"
        >
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Legen Sie Ihren Buchungszeitraum fest.
            </p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item
          itemId="step3"
          title="Schritt 3: Buchung bestätigen"
          subtitle="Abschließende Prüfung"
        >
          <ng-template>
            <p style="margin: 0; color: #6b7280;">
              Überprüfen Sie Ihre Buchungsdetails und bestätigen Sie.
            </p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Accordion items with subtitles for additional context.",
      },
    },
  },
};

export const WithDisabledItem: Story = {
  render: () => ({
    template: `
      <ocr-accordion>
        <ocr-accordion-item itemId="active1" title="Verfügbare Option" [expanded]="true">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">Diese Option ist verfügbar.</p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="disabled" title="Nicht verfügbar" subtitle="Kommt bald" [disabled]="true">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">Diese Option ist noch nicht verfügbar.</p>
          </ng-template>
        </ocr-accordion-item>
        <ocr-accordion-item itemId="active2" title="Weitere Option">
          <ng-template>
            <p style="margin: 0; color: #6b7280;">Eine weitere verfügbare Option.</p>
          </ng-template>
        </ocr-accordion-item>
      </ocr-accordion>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Example with a disabled accordion item that cannot be expanded.",
      },
    },
  },
};

export const FAQSection: Story = {
  render: () => ({
    template: `
      <div style="max-width: 600px;">
        <h2 style="margin: 0 0 1.5rem 0; font-size: 1.5rem; font-weight: 600;">Häufige Fragen</h2>
        <ocr-accordion>
          <ocr-accordion-item itemId="faq1" title="Wie alt muss ich sein, um ein Auto zu mieten?">
            <ng-template>
              <p style="margin: 0; color: #6b7280; line-height: 1.6;">
                Das Mindestalter für die Anmietung beträgt 21 Jahre. Für bestimmte Fahrzeugkategorien
                (z.B. Luxusfahrzeuge) kann ein höheres Mindestalter von 25 Jahren gelten.
              </p>
            </ng-template>
          </ocr-accordion-item>
          <ocr-accordion-item itemId="faq2" title="Ist eine Vollkaskoversicherung enthalten?">
            <ng-template>
              <p style="margin: 0; color: #6b7280; line-height: 1.6;">
                Ja, alle unsere Mietfahrzeuge sind vollkaskoversichert mit einer Selbstbeteiligung
                von 500€. Sie können optional eine Versicherung ohne Selbstbeteiligung hinzubuchen.
              </p>
            </ng-template>
          </ocr-accordion-item>
          <ocr-accordion-item itemId="faq3" title="Kann ich das Fahrzeug an einem anderen Ort zurückgeben?">
            <ng-template>
              <p style="margin: 0; color: #6b7280; line-height: 1.6;">
                Ja, Einwegmieten sind möglich. Je nach Strecke kann eine zusätzliche Gebühr anfallen.
                Die genauen Kosten werden Ihnen bei der Buchung angezeigt.
              </p>
            </ng-template>
          </ocr-accordion-item>
          <ocr-accordion-item itemId="faq4" title="Was passiert bei einem Unfall?">
            <ng-template>
              <p style="margin: 0; color: #6b7280; line-height: 1.6;">
                Im Falle eines Unfalls kontaktieren Sie bitte sofort unsere 24/7-Hotline.
                Dokumentieren Sie den Schaden mit Fotos und füllen Sie den Unfallbericht aus.
                Wir kümmern uns um alles Weitere.
              </p>
            </ng-template>
          </ocr-accordion-item>
        </ocr-accordion>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Example of accordion used as a FAQ section.",
      },
    },
  },
};

export const OrderDetails: Story = {
  render: () => ({
    template: `
      <div style="max-width: 500px; border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1rem;">
        <h3 style="margin: 0 0 1rem 0; font-size: 1.125rem; font-weight: 600;">Buchungsdetails</h3>
        <ocr-accordion [multiple]="true">
          <ocr-accordion-item itemId="vehicle" title="Fahrzeug" subtitle="BMW X5" [expanded]="true">
            <ng-template>
              <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 0.5rem; font-size: 0.875rem;">
                <span style="color: #6b7280;">Kategorie:</span>
                <span>SUV</span>
                <span style="color: #6b7280;">Getriebe:</span>
                <span>Automatik</span>
                <span style="color: #6b7280;">Sitze:</span>
                <span>5</span>
              </div>
            </ng-template>
          </ocr-accordion-item>
          <ocr-accordion-item itemId="dates" title="Zeitraum" subtitle="15.01. - 18.01.2026">
            <ng-template>
              <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 0.5rem; font-size: 0.875rem;">
                <span style="color: #6b7280;">Abholung:</span>
                <span>15.01.2026, 10:00</span>
                <span style="color: #6b7280;">Rückgabe:</span>
                <span>18.01.2026, 10:00</span>
                <span style="color: #6b7280;">Dauer:</span>
                <span>3 Tage</span>
              </div>
            </ng-template>
          </ocr-accordion-item>
          <ocr-accordion-item itemId="price" title="Preis" subtitle="€249,00">
            <ng-template>
              <div style="display: grid; grid-template-columns: 1fr auto; gap: 0.5rem; font-size: 0.875rem;">
                <span style="color: #6b7280;">Mietpreis (3 Tage):</span>
                <span>€210,00</span>
                <span style="color: #6b7280;">Versicherung:</span>
                <span>€39,00</span>
                <span style="font-weight: 600; padding-top: 0.5rem; border-top: 1px solid #e5e7eb;">Gesamt:</span>
                <span style="font-weight: 600; color: #f97316; padding-top: 0.5rem; border-top: 1px solid #e5e7eb;">€249,00</span>
              </div>
            </ng-template>
          </ocr-accordion-item>
        </ocr-accordion>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "Accordion used to display order details in a compact format.",
      },
    },
  },
};
