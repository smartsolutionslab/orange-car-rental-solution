/**
 * German Currency Formatter
 * Formats amounts according to German locale (1.234,56 €)
 */

export class CurrencyFormatter {
  /**
   * Format amount as German currency
   * @param amount - The amount to format
   * @param currency - Currency code (default: EUR)
   * @returns Formatted currency string (e.g., "1.234,56 €")
   */
  static formatGerman(amount: number, currency: string = 'EUR'): string {
    const formatter = new Intl.NumberFormat('de-DE', {
      style: 'currency',
      currency: currency,
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    });
    return formatter.format(amount);
  }

  /**
   * Format amount with VAT information
   * @param netAmount - Net amount
   * @param vatRate - VAT rate (default: 0.19 for 19%)
   * @returns Object with formatted net, VAT, and gross amounts
   */
  static formatWithVat(
    netAmount: number,
    vatRate: number = 0.19
  ): {
    net: string;
    vat: string;
    gross: string;
    netValue: number;
    vatValue: number;
    grossValue: number;
  } {
    const vatAmount = netAmount * vatRate;
    const grossAmount = netAmount + vatAmount;

    return {
      net: this.formatGerman(netAmount),
      vat: this.formatGerman(vatAmount),
      gross: this.formatGerman(grossAmount),
      netValue: netAmount,
      vatValue: vatAmount,
      grossValue: grossAmount,
    };
  }

  /**
   * Parse German formatted currency string to number
   * @param value - German formatted currency string (e.g., "1.234,56 €")
   * @returns Parsed number
   */
  static parseGerman(value: string): number {
    // Remove currency symbols and spaces
    let cleaned = value.replace(/[€$£\s]/g, '');
    // Replace German decimal separator (,) with dot
    cleaned = cleaned.replace(',', '.');
    // Remove thousands separators
    cleaned = cleaned.replace(/\./g, '');
    return parseFloat(cleaned);
  }
}
