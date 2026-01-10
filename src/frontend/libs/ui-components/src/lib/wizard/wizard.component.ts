import {
  Component,
  input,
  output,
  signal,
  computed,
  contentChildren,
  effect,
  HostListener,
} from "@angular/core";
import type { AfterContentInit } from "@angular/core";
import { CommonModule, NgTemplateOutlet } from "@angular/common";
import { WizardStepComponent } from "./wizard-step.component";
import type {
  WizardStepConfig,
  WizardStepState,
  WizardCompleteEvent,
  WizardStepChangeEvent,
} from "./wizard.types";

/**
 * Wizard Component
 *
 * A multi-step wizard container with step navigation, validation support,
 * and keyboard accessibility.
 *
 * @example
 * <ocr-wizard [steps]="wizardSteps" (stepChange)="onStepChange($event)" (complete)="onComplete($event)">
 *   <ocr-wizard-step stepId="account">
 *     <ng-template>
 *       <account-form></account-form>
 *     </ng-template>
 *   </ocr-wizard-step>
 *   <ocr-wizard-step stepId="personal">
 *     <ng-template>
 *       <personal-form></personal-form>
 *     </ng-template>
 *   </ocr-wizard-step>
 * </ocr-wizard>
 */
@Component({
  selector: "ocr-wizard",
  standalone: true,
  imports: [CommonModule, NgTemplateOutlet],
  template: `
    <div class="wizard" [class.wizard--vertical]="orientation() === 'vertical'">
      <!-- Step indicator header -->
      <nav class="wizard__nav" role="tablist" [attr.aria-label]="ariaLabel()">
        @for (step of steps(); track step.id; let i = $index) {
          <button
            class="wizard__step-indicator"
            [class.wizard__step-indicator--active]="i === currentIndex()"
            [class.wizard__step-indicator--completed]="
              stepStates()[step.id]?.status === 'completed'
            "
            [class.wizard__step-indicator--error]="
              stepStates()[step.id]?.status === 'error'
            "
            [class.wizard__step-indicator--skipped]="
              stepStates()[step.id]?.status === 'skipped'
            "
            [class.wizard__step-indicator--disabled]="
              step.disabled || (!linear() && !canNavigateToStep(i))
            "
            [disabled]="step.disabled || (linear() && !canNavigateToStep(i))"
            role="tab"
            [attr.aria-selected]="i === currentIndex()"
            [attr.aria-controls]="'wizard-panel-' + step.id"
            [id]="'wizard-tab-' + step.id"
            (click)="goToStep(i)"
          >
            <span class="wizard__step-number">
              @if (stepStates()[step.id]?.status === "completed") {
                <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                  <path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z" />
                </svg>
              } @else if (stepStates()[step.id]?.status === "error") {
                <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                  <path
                    d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"
                  />
                </svg>
              } @else {
                {{ i + 1 }}
              }
            </span>
            <span class="wizard__step-label">
              <span class="wizard__step-title">{{ step.title }}</span>
              @if (step.subtitle) {
                <span class="wizard__step-subtitle">{{ step.subtitle }}</span>
              }
            </span>
          </button>
          @if (i < steps().length - 1) {
            <div
              class="wizard__connector"
              [class.wizard__connector--completed]="
                stepStates()[step.id]?.status === 'completed'
              "
            ></div>
          }
        }
      </nav>

      <!-- Step content panels -->
      <div class="wizard__content">
        @for (step of steps(); track step.id; let i = $index) {
          <div
            class="wizard__panel"
            [class.wizard__panel--active]="i === currentIndex()"
            role="tabpanel"
            [id]="'wizard-panel-' + step.id"
            [attr.aria-labelledby]="'wizard-tab-' + step.id"
            [attr.hidden]="i !== currentIndex() ? true : null"
          >
            @if (i === currentIndex() || stepStates()[step.id]?.visited) {
              @if (getStepComponent(step.id); as stepComponent) {
                @if (stepComponent.contentTemplate()) {
                  <ng-container
                    *ngTemplateOutlet="stepComponent.contentTemplate()!"
                  ></ng-container>
                }
              }
            }
          </div>
        }
      </div>

      <!-- Navigation footer -->
      @if (showNavigation()) {
        <footer class="wizard__footer">
          <button
            type="button"
            class="wizard__btn wizard__btn--secondary"
            [disabled]="currentIndex() === 0"
            (click)="previousStep()"
          >
            {{ previousLabel() }}
          </button>

          <div class="wizard__footer-spacer"></div>

          @if (currentStep()?.canSkip) {
            <button
              type="button"
              class="wizard__btn wizard__btn--text"
              (click)="skipStep()"
            >
              {{ skipLabel() }}
            </button>
          }

          @if (isLastStep()) {
            <button
              type="button"
              class="wizard__btn wizard__btn--primary"
              [disabled]="!canComplete()"
              (click)="completeWizard()"
            >
              {{ completeLabel() }}
            </button>
          } @else {
            <button
              type="button"
              class="wizard__btn wizard__btn--primary"
              [disabled]="!canProceed()"
              (click)="nextStep()"
            >
              {{ nextLabel() }}
            </button>
          }
        </footer>
      }
    </div>
  `,
  styles: [
    `
      .wizard {
        display: flex;
        flex-direction: column;
        gap: 1.5rem;
      }

      .wizard--vertical {
        flex-direction: row;
      }

      .wizard--vertical .wizard__nav {
        flex-direction: column;
        width: 16rem;
        flex-shrink: 0;
      }

      .wizard--vertical .wizard__connector {
        width: 2px;
        height: 1.5rem;
        margin: 0 auto;
      }

      .wizard__nav {
        display: flex;
        align-items: flex-start;
        gap: 0;
      }

      .wizard__step-indicator {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.5rem 0.75rem;
        background: none;
        border: none;
        cursor: pointer;
        transition: all 0.2s ease;
        border-radius: 0.5rem;
      }

      .wizard__step-indicator:hover:not(:disabled) {
        background-color: #f3f4f6;
      }

      .wizard__step-indicator:disabled {
        cursor: not-allowed;
        opacity: 0.5;
      }

      .wizard__step-number {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 2rem;
        height: 2rem;
        border-radius: 50%;
        background-color: #e5e7eb;
        color: #6b7280;
        font-size: 0.875rem;
        font-weight: 600;
        flex-shrink: 0;
        transition: all 0.2s ease;
      }

      .wizard__step-number svg {
        width: 1rem;
        height: 1rem;
      }

      .wizard__step-indicator--active .wizard__step-number {
        background-color: #f97316;
        color: white;
      }

      .wizard__step-indicator--completed .wizard__step-number {
        background-color: #10b981;
        color: white;
      }

      .wizard__step-indicator--error .wizard__step-number {
        background-color: #ef4444;
        color: white;
      }

      .wizard__step-indicator--skipped .wizard__step-number {
        background-color: #9ca3af;
        color: white;
      }

      .wizard__step-label {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
        text-align: left;
      }

      .wizard__step-title {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .wizard__step-indicator--active .wizard__step-title {
        color: #111827;
        font-weight: 600;
      }

      .wizard__step-subtitle {
        font-size: 0.75rem;
        color: #6b7280;
      }

      .wizard__connector {
        flex: 1;
        height: 2px;
        background-color: #e5e7eb;
        margin: 1rem 0.5rem;
        min-width: 1rem;
        transition: background-color 0.2s ease;
      }

      .wizard__connector--completed {
        background-color: #10b981;
      }

      .wizard__content {
        flex: 1;
        min-height: 0;
      }

      .wizard__panel {
        display: none;
      }

      .wizard__panel--active {
        display: block;
      }

      .wizard__footer {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding-top: 1rem;
        border-top: 1px solid #e5e7eb;
      }

      .wizard__footer-spacer {
        flex: 1;
      }

      .wizard__btn {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        padding: 0.5rem 1rem;
        font-size: 0.875rem;
        font-weight: 500;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .wizard__btn:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      .wizard__btn--primary {
        background-color: #f97316;
        color: white;
        border: none;
      }

      .wizard__btn--primary:hover:not(:disabled) {
        background-color: #ea580c;
      }

      .wizard__btn--secondary {
        background-color: white;
        color: #374151;
        border: 1px solid #d1d5db;
      }

      .wizard__btn--secondary:hover:not(:disabled) {
        background-color: #f9fafb;
      }

      .wizard__btn--text {
        background: none;
        color: #6b7280;
        border: none;
      }

      .wizard__btn--text:hover:not(:disabled) {
        color: #374151;
        background-color: #f3f4f6;
      }
    `,
  ],
})
export class WizardComponent implements AfterContentInit {
  /**
   * Configuration for wizard steps
   */
  readonly steps = input.required<WizardStepConfig[]>();

  /**
   * Whether navigation is linear (must complete steps in order)
   */
  readonly linear = input(true);

  /**
   * Layout orientation
   */
  readonly orientation = input<"horizontal" | "vertical">("horizontal");

  /**
   * Whether to show navigation buttons
   */
  readonly showNavigation = input(true);

  /**
   * Label for previous button
   */
  readonly previousLabel = input("Zurück");

  /**
   * Label for next button
   */
  readonly nextLabel = input("Weiter");

  /**
   * Label for skip button
   */
  readonly skipLabel = input("Überspringen");

  /**
   * Label for complete button
   */
  readonly completeLabel = input("Abschließen");

  /**
   * Accessible label for the wizard navigation
   */
  readonly ariaLabel = input("Wizard navigation");

  /**
   * Emitted when step changes
   */
  readonly stepChange = output<WizardStepChangeEvent>();

  /**
   * Emitted when wizard completes
   */
  readonly complete = output<WizardCompleteEvent>();

  /**
   * Step content components from content projection
   */
  readonly stepComponents = contentChildren(WizardStepComponent);

  /**
   * Current step index (0-based)
   */
  readonly currentIndex = signal(0);

  /**
   * Internal state for each step
   */
  readonly stepStates = signal<Record<string, WizardStepState>>({});

  /**
   * Current step configuration
   */
  readonly currentStep = computed(() => this.steps()[this.currentIndex()]);

  /**
   * Whether on the last step
   */
  readonly isLastStep = computed(
    () => this.currentIndex() === this.steps().length - 1,
  );

  /**
   * Whether current step allows proceeding
   */
  readonly canProceed = computed(() => {
    const current = this.currentStep();
    if (!current) return false;
    if (current.isValid) return current.isValid();
    return true;
  });

  /**
   * Whether wizard can be completed
   */
  readonly canComplete = computed(() => {
    // Check if all required steps are completed
    const states = this.stepStates();
    return (
      this.steps().every((step) => {
        if (step.canSkip) return true;
        const state = states[step.id];
        return (
          state?.status === "completed" || step.id === this.currentStep()?.id
        );
      }) && this.canProceed()
    );
  });

  constructor() {
    // Initialize step states when steps change
    effect(
      () => {
        const steps = this.steps();
        const currentStates = this.stepStates();
        const newStates: Record<string, WizardStepState> = {};

        steps.forEach((step, index) => {
          newStates[step.id] = currentStates[step.id] || {
            status: index === 0 ? "active" : "pending",
            visited: index === 0,
          };
        });

        this.stepStates.set(newStates);
      },
      { allowSignalWrites: true },
    );
  }

  ngAfterContentInit(): void {
    // Sync step component active states
    this.updateStepComponentStates();
  }

  /**
   * Navigate to a specific step by index
   */
  goToStep(index: number): void {
    if (index < 0 || index >= this.steps().length) return;
    if (!this.canNavigateToStep(index)) return;

    const previousIndex = this.currentIndex();
    const previousStepId = this.steps()[previousIndex]?.id ?? null;

    // Update previous step status
    if (previousStepId) {
      const prevState = this.stepStates()[previousStepId];
      if (prevState?.status === "active" && this.canProceed()) {
        this.updateStepState(previousStepId, { status: "completed" });
      }
    }

    // Update current index
    this.currentIndex.set(index);

    const currentStepId = this.steps()[index].id;

    // Update new step status
    this.updateStepState(currentStepId, { status: "active", visited: true });

    // Update step component states
    this.updateStepComponentStates();

    // Emit step change event
    this.stepChange.emit({
      previousStepId,
      currentStepId,
      direction: index > previousIndex ? "forward" : "backward",
      currentIndex: index,
    });
  }

  /**
   * Go to the next step
   */
  nextStep(): void {
    if (this.canProceed()) this.goToStep(this.currentIndex() + 1);
  }

  /**
   * Go to the previous step
   */
  previousStep(): void {
    this.goToStep(this.currentIndex() - 1);
  }

  /**
   * Skip the current step (if allowed)
   */
  skipStep(): void {
    const current = this.currentStep();
    if (current?.canSkip) {
      this.updateStepState(current.id, { status: "skipped" });
      this.goToStep(this.currentIndex() + 1);
    }
  }

  /**
   * Complete the wizard
   */
  completeWizard(): void {
    if (!this.canComplete()) return;

    // Mark current step as completed
    const current = this.currentStep();
    if (current) this.updateStepState(current.id, { status: "completed" });

    // Gather completion data
    const states = this.stepStates();
    const completedSteps = this.steps()
      .filter((s) => states[s.id]?.status === "completed")
      .map((s) => s.id);
    const skippedSteps = this.steps()
      .filter((s) => states[s.id]?.status === "skipped")
      .map((s) => s.id);

    this.complete.emit({
      completedSteps,
      skippedSteps,
    });
  }

  /**
   * Check if navigation to a step is allowed
   */
  canNavigateToStep(index: number): boolean {
    if (this.steps()[index]?.disabled) return false;

    if (!this.linear()) return true;

    // In linear mode, can only navigate to visited steps or next step
    const targetStep = this.steps()[index];
    if (!targetStep) return false;

    const state = this.stepStates()[targetStep.id];
    return state?.visited || index === this.currentIndex() + 1;
  }

  /**
   * Get step component by ID
   */
  getStepComponent(stepId: string): WizardStepComponent | undefined {
    return this.stepComponents().find((c) => c.stepId() === stepId);
  }

  /**
   * Handle keyboard navigation
   */
  @HostListener("keydown", ["$event"])
  onKeydown(event: KeyboardEvent): void {
    if (event.key === "ArrowLeft" || event.key === "ArrowUp") {
      event.preventDefault();
      this.previousStep();
    } else if (event.key === "ArrowRight" || event.key === "ArrowDown") {
      event.preventDefault();
      if (this.canProceed()) this.nextStep();
    }
  }

  /**
   * Mark a step as having an error
   */
  setStepError(stepId: string, hasError: boolean): void {
    const state = this.stepStates()[stepId];
    if (state) this.updateStepState(stepId, { status: hasError ? "error" : "active" });
  }

  /**
   * Reset the wizard to the first step
   */
  reset(): void {
    const newStates: Record<string, WizardStepState> = {};
    this.steps().forEach((step, index) => {
      newStates[step.id] = {
        status: index === 0 ? "active" : "pending",
        visited: index === 0,
      };
    });
    this.stepStates.set(newStates);
    this.currentIndex.set(0);
    this.updateStepComponentStates();
  }

  private updateStepState(
    stepId: string,
    updates: Partial<WizardStepState>,
  ): void {
    const states = this.stepStates();
    this.stepStates.set({
      ...states,
      [stepId]: { ...states[stepId], ...updates },
    });
  }

  private updateStepComponentStates(): void {
    const currentId = this.currentStep()?.id;
    const states = this.stepStates();

    this.stepComponents().forEach((component) => {
      component.isActive.set(component.stepId() === currentId);
      component.hasVisited.set(states[component.stepId()]?.visited ?? false);
    });
  }
}
