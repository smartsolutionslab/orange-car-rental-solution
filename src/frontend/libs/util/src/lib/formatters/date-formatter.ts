/**
 * German Date Formatter
 * Formats dates according to German locale (DD.MM.YYYY)
 */

export class DateFormatter {
  /**
   * Format date as German short date (DD.MM.YYYY)
   * @param date - Date to format
   * @returns Formatted date string
   */
  static formatGermanShort(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    const formatter = new Intl.DateTimeFormat('de-DE', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
    });
    return formatter.format(d);
  }

  /**
   * Format date as German long date (e.g., "28. Oktober 2025")
   * @param date - Date to format
   * @returns Formatted date string
   */
  static formatGermanLong(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    const formatter = new Intl.DateTimeFormat('de-DE', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
    return formatter.format(d);
  }

  /**
   * Format date and time for German locale
   * @param date - Date to format
   * @returns Formatted date-time string
   */
  static formatGermanDateTime(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    const formatter = new Intl.DateTimeFormat('de-DE', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    });
    return formatter.format(d);
  }

  /**
   * Parse German date string (DD.MM.YYYY) to Date object
   * @param dateString - German formatted date string
   * @returns Date object
   */
  static parseGermanDate(dateString: string): Date {
    const parts = dateString.split('.');
    if (parts.length !== 3) {
      throw new Error('Invalid German date format. Expected DD.MM.YYYY');
    }
    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1; // Months are 0-indexed
    const year = parseInt(parts[2], 10);
    return new Date(year, month, day);
  }

  /**
   * Get day name in German
   * @param date - Date
   * @returns German day name (e.g., "Montag")
   */
  static getGermanDayName(date: Date | string): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    const formatter = new Intl.DateTimeFormat('de-DE', { weekday: 'long' });
    return formatter.format(d);
  }

  /**
   * Calculate rental duration in days
   * @param startDate - Rental start date
   * @param endDate - Rental end date
   * @returns Number of days
   */
  static calculateRentalDays(
    startDate: Date | string,
    endDate: Date | string
  ): number {
    const start =
      typeof startDate === 'string' ? new Date(startDate) : startDate;
    const end = typeof endDate === 'string' ? new Date(endDate) : endDate;
    const diffTime = Math.abs(end.getTime() - start.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
  }
}
