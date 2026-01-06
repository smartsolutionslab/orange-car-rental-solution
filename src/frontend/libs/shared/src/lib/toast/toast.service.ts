import { Injectable, signal, computed } from "@angular/core";

/**
 * Toast notification type
 */
export type ToastType = "success" | "error" | "info" | "warning";

/**
 * Toast notification interface
 */
export interface Toast {
  id: string;
  type: ToastType;
  message: string;
  duration: number;
}

/**
 * Options for showing a toast notification
 */
export interface ToastOptions {
  duration?: number;
  type?: ToastType;
}

const DEFAULT_DURATION = 5000;

/**
 * Toast Service
 *
 * Provides centralized toast notification management.
 * Use this service to show user feedback messages instead of native alert().
 *
 * @example
 * // In a component
 * private readonly toast = inject(ToastService);
 *
 * // Show success message
 * this.toast.success('Reservierung erfolgreich storniert!');
 *
 * // Show error message
 * this.toast.error('Fehler beim Speichern. Bitte versuchen Sie es erneut.');
 *
 * // Show with custom duration (10 seconds)
 * this.toast.success('Nachricht gesendet', { duration: 10000 });
 */
@Injectable({
  providedIn: "root",
})
export class ToastService {
  private readonly _toasts = signal<Toast[]>([]);

  /**
   * Read-only signal of active toasts
   */
  readonly toasts = computed(() => this._toasts());

  /**
   * Show a success toast notification
   */
  success(message: string, options?: Omit<ToastOptions, "type">): void {
    this.show(message, { ...options, type: "success" });
  }

  /**
   * Show an error toast notification
   */
  error(message: string, options?: Omit<ToastOptions, "type">): void {
    this.show(message, { ...options, type: "error" });
  }

  /**
   * Show an info toast notification
   */
  info(message: string, options?: Omit<ToastOptions, "type">): void {
    this.show(message, { ...options, type: "info" });
  }

  /**
   * Show a warning toast notification
   */
  warning(message: string, options?: Omit<ToastOptions, "type">): void {
    this.show(message, { ...options, type: "warning" });
  }

  /**
   * Show a toast notification
   */
  show(message: string, options: ToastOptions = {}): void {
    const toast: Toast = {
      id: this.generateId(),
      type: options.type ?? "info",
      message,
      duration: options.duration ?? DEFAULT_DURATION,
    };

    this._toasts.update((toasts) => [...toasts, toast]);

    // Auto-remove after duration
    if (toast.duration > 0) {
      setTimeout(() => {
        this.remove(toast.id);
      }, toast.duration);
    }
  }

  /**
   * Remove a toast by ID
   */
  remove(id: string): void {
    this._toasts.update((toasts) => toasts.filter((t) => t.id !== id));
  }

  /**
   * Clear all toasts
   */
  clear(): void {
    this._toasts.set([]);
  }

  private generateId(): string {
    return `toast-${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
  }
}
