import type { Signal } from "@angular/core";

/**
 * Configuration for a wizard step
 */
export interface WizardStepConfig {
  /** Unique identifier for the step */
  id: string;

  /** Display title for the step */
  title: string;

  /** Optional subtitle or description */
  subtitle?: string;

  /** Signal indicating if the step is valid (for form validation) */
  isValid?: Signal<boolean>;

  /** Whether this step can be skipped */
  canSkip?: boolean;

  /** Whether this step is disabled */
  disabled?: boolean;
}

/**
 * Status of a wizard step
 */
export type WizardStepStatus =
  | "pending"
  | "active"
  | "completed"
  | "error"
  | "skipped";

/**
 * Internal state for tracking step status
 */
export interface WizardStepState {
  status: WizardStepStatus;
  visited: boolean;
}

/**
 * Event emitted when the wizard completes
 */
export interface WizardCompleteEvent {
  /** Data from all steps (if collected) */
  data?: Record<string, unknown>;

  /** Array of step IDs in completion order */
  completedSteps: string[];

  /** Array of skipped step IDs */
  skippedSteps: string[];
}

/**
 * Event emitted when step changes
 */
export interface WizardStepChangeEvent {
  /** Previous step ID */
  previousStepId: string | null;

  /** Current step ID */
  currentStepId: string;

  /** Direction of navigation */
  direction: "forward" | "backward";

  /** Current step index (0-based) */
  currentIndex: number;
}
