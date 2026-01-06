import {
  Component,
  input,
  signal,
  TemplateRef,
  contentChild,
  computed,
} from "@angular/core";

/**
 * Tab Component
 *
 * Defines a single tab within a TabsComponent.
 * The tab content is lazily rendered when the tab becomes active.
 *
 * @example
 * <ocr-tab tabId="overview" label="Overview">
 *   <ng-template>
 *     <p>Overview content here</p>
 *   </ng-template>
 * </ocr-tab>
 */
@Component({
  selector: "ocr-tab",
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
export class TabComponent {
  /**
   * Unique identifier for this tab
   */
  readonly tabId = input.required<string>();

  /**
   * Display label for the tab button
   */
  readonly label = input.required<string>();

  /**
   * Optional icon name to display before the label
   */
  readonly icon = input<string | undefined>(undefined);

  /**
   * Whether this tab is disabled
   */
  readonly disabled = input(false);

  /**
   * Optional badge/count to display on the tab
   */
  readonly badge = input<string | number | undefined>(undefined);

  /**
   * Internal: Whether this tab is currently active
   */
  readonly isActive = signal(false);

  /**
   * Internal: Whether this tab has been visited (for lazy loading)
   */
  readonly hasVisited = signal(false);

  /**
   * Content template reference for lazy loading
   */
  readonly contentTemplate = contentChild<TemplateRef<unknown>>(TemplateRef);

  /**
   * Whether the tab has content template
   */
  readonly hasContent = computed(() => !!this.contentTemplate());
}
