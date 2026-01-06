import {
  Component,
  input,
  signal,
  TemplateRef,
  contentChild,
  computed,
} from "@angular/core";

/**
 * Accordion Item Component
 *
 * An expandable item within an Accordion.
 * Use ng-template for lazy content rendering.
 *
 * @example
 * <ocr-accordion-item itemId="faq1" title="What is your return policy?">
 *   <ng-template>
 *     <p>Our return policy details...</p>
 *   </ng-template>
 * </ocr-accordion-item>
 */
@Component({
  selector: "ocr-accordion-item",
  standalone: true,
  template: `<ng-content></ng-content>`,
  styles: [
    `
      :host {
        display: contents;
      }
    `,
  ],
})
export class AccordionItemComponent {
  /**
   * Unique identifier for this item
   */
  readonly itemId = input.required<string>();

  /**
   * Title/header text for the accordion item
   */
  readonly title = input.required<string>();

  /**
   * Optional subtitle or description
   */
  readonly subtitle = input<string | undefined>(undefined);

  /**
   * Whether this item is disabled
   */
  readonly disabled = input(false);

  /**
   * Whether this item is initially expanded
   */
  readonly expanded = input(false);

  /**
   * Internal: Whether this item is currently expanded
   */
  readonly isExpanded = signal(false);

  /**
   * Internal: Whether this item has been opened (for lazy loading)
   */
  readonly hasOpened = signal(false);

  /**
   * Content template reference for lazy loading
   */
  readonly contentTemplate = contentChild<TemplateRef<unknown>>(TemplateRef);

  /**
   * Whether the item has content template
   */
  readonly hasContent = computed(() => !!this.contentTemplate());
}
