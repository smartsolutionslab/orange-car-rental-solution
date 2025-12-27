/**
 * Centralized error logging utility
 * Provides consistent error logging across the application
 */

export type ErrorContext = string;
export type ErrorDetails = unknown;

/**
 * Log levels for different severity of errors
 */
export type LogLevel = 'error' | 'warn' | 'info';

/**
 * Structured error log entry
 */
interface ErrorLogEntry {
  readonly context: ErrorContext;
  readonly message: string;
  readonly details?: ErrorDetails;
  readonly timestamp: string;
}

/**
 * Create a structured error log entry
 */
function createLogEntry(context: ErrorContext, message: string, details?: ErrorDetails): ErrorLogEntry {
  return {
    context,
    message,
    details,
    timestamp: new Date().toISOString()
  };
}

/**
 * Log an error with context
 * @param context - The component or service where the error occurred
 * @param message - Human-readable error message
 * @param error - The original error object
 */
export function logError(context: ErrorContext, message: string, error?: ErrorDetails): void {
  const entry = createLogEntry(context, message, error);
  console.error(`[${entry.context}] ${entry.message}`, entry.details ?? '');
}

/**
 * Log a warning with context
 * @param context - The component or service where the warning occurred
 * @param message - Human-readable warning message
 * @param details - Additional details
 */
export function logWarning(context: ErrorContext, message: string, details?: ErrorDetails): void {
  const entry = createLogEntry(context, message, details);
  console.warn(`[${entry.context}] ${entry.message}`, entry.details ?? '');
}

/**
 * Log info with context
 * @param context - The component or service where the log occurred
 * @param message - Human-readable message
 * @param details - Additional details
 */
export function logInfo(context: ErrorContext, message: string, details?: ErrorDetails): void {
  const entry = createLogEntry(context, message, details);
  console.info(`[${entry.context}] ${entry.message}`, entry.details ?? '');
}

/**
 * Extract user-friendly error message from HTTP errors
 * @param error - The error object (usually HttpErrorResponse)
 * @param defaultMessage - Default message if extraction fails
 */
export function getErrorMessage(error: unknown, defaultMessage: string): string {
  if (error && typeof error === 'object') {
    const httpError = error as { error?: { message?: string }; message?: string; status?: number };

    // Try to extract message from nested error object (API response)
    if (httpError.error?.message) {
      return httpError.error.message;
    }

    // Try direct message property
    if (httpError.message) {
      return httpError.message;
    }

    // Return status-based message for HTTP errors
    if (httpError.status) {
      return getHttpStatusMessage(httpError.status, defaultMessage);
    }
  }

  return defaultMessage;
}

/**
 * Get user-friendly message for HTTP status codes (German localization)
 */
function getHttpStatusMessage(status: number, defaultMessage: string): string {
  const statusMessages: Record<number, string> = {
    400: 'Ungültige Anfrage',
    401: 'Nicht autorisiert - bitte erneut anmelden',
    403: 'Zugriff verweigert',
    404: 'Ressource nicht gefunden',
    409: 'Konflikt - Daten wurden möglicherweise geändert',
    422: 'Validierungsfehler',
    500: 'Serverfehler - bitte später erneut versuchen',
    502: 'Server nicht erreichbar',
    503: 'Service temporär nicht verfügbar'
  };

  return statusMessages[status] ?? defaultMessage;
}

/**
 * Type guard to check if an error is an HTTP error
 */
export function isHttpError(error: unknown): error is { status: number; message?: string; error?: unknown } {
  return typeof error === 'object' && error !== null && 'status' in error;
}
