import {
  Component,
  input,
  signal,
  TemplateRef,
  contentChild,
  computed,
} from "@angular/core";

/**
 * Wizard Step Component
 *
 * Wrapper for individual step content within a Wizard.
 * Use ng-template for lazy content rendering.
 *
 * @example
 * <ocr-wizard-step stepId="personal" title="Personal Info">
 *   <ng-template>
 *     <form>...</form>
 *   </ng-template>
 * </ocr-wizard-step>
 */
@Component({
  selector: "ocr-wizard-step",
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
export class WizardStepComponent {
  /**
   * Unique identifier for this step.
   * Must match the id in the step configuration.
   */
  readonly stepId = input.required<string>();

  /**
   * Optional: Override the step title from config
   */
  readonly title = input<string | undefined>(undefined);

  /**
   * Optional: Override the step subtitle from config
   */
  readonly subtitle = input<string | undefined>(undefined);

  /**
   * Internal: Whether this step is currently active
   */
  readonly isActive = signal(false);

  /**
   * Internal: Whether this step has been visited
   */
  readonly hasVisited = signal(false);

  /**
   * Content template reference for lazy loading
   */
  readonly contentTemplate = contentChild<TemplateRef<unknown>>(TemplateRef);

  /**
   * Whether the step has content template
   */
  readonly hasContent = computed(() => !!this.contentTemplate());
}
