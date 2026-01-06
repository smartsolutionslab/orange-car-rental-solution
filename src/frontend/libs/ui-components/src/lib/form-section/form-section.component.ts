import { Component, input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IconComponent, type IconName } from "../icon";

export type FormSectionVariant = "default" | "card" | "bordered" | "compact";

/**
 * Form Section Component
 *
 * Groups related form fields together with a header, optional description,
 * and optional icon. Useful for organizing complex forms into logical sections.
 *
 * @example
 * <ocr-form-section
 *   title="Kontaktdaten"
 *   description="Ihre Kontaktinformationen"
 *   icon="user"
 * >
 *   <ocr-input label="E-Mail" ... />
 *   <ocr-input label="Telefon" ... />
 * </ocr-form-section>
 */
@Component({
  selector: "ocr-form-section",
  standalone: true,
  imports: [CommonModule, IconComponent],
  template: `
    <section
      class="form-section"
      [class.form-section--card]="variant() === 'card'"
      [class.form-section--bordered]="variant() === 'bordered'"
      [class.form-section--compact]="variant() === 'compact'"
      [class.form-section--collapsible]="collapsible()"
      [class.form-section--collapsed]="isCollapsed"
    >
      @if (title() || description() || icon()) {
        <header
          class="form-section__header"
          [class.form-section__header--clickable]="collapsible()"
          (click)="toggleCollapse()"
          [attr.role]="collapsible() ? 'button' : null"
          [attr.aria-expanded]="collapsible() ? !isCollapsed : null"
        >
          <div class="form-section__header-content">
            @if (icon()) {
              <div class="form-section__icon">
                <lib-icon [name]="icon()!" variant="outline" size="md" />
              </div>
            }
            <div class="form-section__titles">
              @if (title()) {
                <h3 class="form-section__title">{{ title() }}</h3>
              }
              @if (description()) {
                <p class="form-section__description">{{ description() }}</p>
              }
            </div>
          </div>
          @if (collapsible()) {
            <div class="form-section__toggle">
              <lib-icon
                [name]="isCollapsed ? 'chevron-down' : 'chevron-up'"
                variant="outline"
                size="sm"
              />
            </div>
          }
        </header>
      }

      <div
        class="form-section__content"
        [class.form-section__content--hidden]="isCollapsed"
      >
        <ng-content></ng-content>
      </div>
    </section>
  `,
  styles: [
    `
      .form-section {
        display: flex;
        flex-direction: column;
      }

      /* Variant: Card */
      .form-section--card {
        background: white;
        border-radius: 0.5rem;
        box-shadow:
          0 1px 3px 0 rgba(0, 0, 0, 0.1),
          0 1px 2px -1px rgba(0, 0, 0, 0.1);
        padding: 1.5rem;
      }

      /* Variant: Bordered */
      .form-section--bordered {
        border: 1px solid #e5e7eb;
        border-radius: 0.5rem;
        padding: 1.5rem;
      }

      /* Variant: Compact */
      .form-section--compact .form-section__header {
        margin-bottom: 0.75rem;
      }

      .form-section--compact .form-section__content {
        gap: 0.75rem;
      }

      /* Header */
      .form-section__header {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        margin-bottom: 1.25rem;
      }

      .form-section__header--clickable {
        cursor: pointer;
        user-select: none;
        padding: 0.5rem;
        margin: -0.5rem;
        margin-bottom: 0.75rem;
        border-radius: 0.375rem;
        transition: background-color 0.15s ease;
      }

      .form-section__header--clickable:hover {
        background-color: #f9fafb;
      }

      .form-section__header-content {
        display: flex;
        align-items: flex-start;
        gap: 0.75rem;
      }

      .form-section__icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 2.5rem;
        height: 2.5rem;
        background-color: #fff7ed;
        color: #f97316;
        border-radius: 0.5rem;
        flex-shrink: 0;
      }

      .form-section__titles {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .form-section__title {
        margin: 0;
        font-size: 1rem;
        font-weight: 600;
        color: #111827;
        line-height: 1.5;
      }

      .form-section__description {
        margin: 0;
        font-size: 0.875rem;
        color: #6b7280;
        line-height: 1.5;
      }

      .form-section__toggle {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 1.5rem;
        height: 1.5rem;
        color: #6b7280;
        transition: transform 0.2s ease;
      }

      /* Content */
      .form-section__content {
        display: flex;
        flex-direction: column;
        gap: 1rem;
      }

      .form-section__content--hidden {
        display: none;
      }

      /* Collapsed state */
      .form-section--collapsed .form-section__header {
        margin-bottom: 0;
      }

      /* Section divider (when used with multiple sections) */
      .form-section + .form-section {
        margin-top: 1.5rem;
        padding-top: 1.5rem;
        border-top: 1px solid #e5e7eb;
      }

      /* Card and bordered variants don't need the divider */
      .form-section--card + .form-section--card,
      .form-section--bordered + .form-section--bordered {
        margin-top: 1rem;
        padding-top: 0;
        border-top: none;
      }
    `,
  ],
})
export class FormSectionComponent {
  /** Section title */
  readonly title = input<string>();

  /** Section description */
  readonly description = input<string>();

  /** Optional icon for the section */
  readonly icon = input<IconName>();

  /** Visual variant */
  readonly variant = input<FormSectionVariant>("default");

  /** Whether the section can be collapsed */
  readonly collapsible = input(false);

  /** Initial collapsed state (only used when collapsible is true) */
  readonly initiallyCollapsed = input(false);

  /** Internal collapsed state */
  isCollapsed = false;

  constructor() {
    // Set initial collapsed state after inputs are initialized
    setTimeout(() => {
      if (this.collapsible()) {
        this.isCollapsed = this.initiallyCollapsed();
      }
    });
  }

  toggleCollapse(): void {
    if (this.collapsible()) {
      this.isCollapsed = !this.isCollapsed;
    }
  }
}
