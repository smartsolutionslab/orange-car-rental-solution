/**
 * Accessibility Testing Helper
 *
 * Provides utilities for accessibility testing using axe-core.
 * Integrates with Playwright for E2E accessibility checks.
 */

import { Page } from '@playwright/test';
import { expect } from '@playwright/test';

export interface AxeResults {
  violations: AxeViolation[];
  passes: AxePass[];
  incomplete: AxeIncomplete[];
}

export interface AxeViolation {
  id: string;
  impact: 'minor' | 'moderate' | 'serious' | 'critical';
  description: string;
  help: string;
  helpUrl: string;
  nodes: AxeNode[];
}

export interface AxePass {
  id: string;
  description: string;
  help: string;
}

export interface AxeIncomplete {
  id: string;
  impact: string;
  description: string;
  help: string;
  nodes: AxeNode[];
}

export interface AxeNode {
  html: string;
  target: string[];
  failureSummary?: string;
}

/**
 * Helper class for accessibility testing
 */
export class AccessibilityHelper {
  constructor(private page: Page) {}

  /**
   * Inject axe-core into the page
   */
  async injectAxe(): Promise<void> {
    await this.page.addScriptTag({
      url: 'https://cdnjs.cloudflare.com/ajax/libs/axe-core/4.8.3/axe.min.js'
    });
  }

  /**
   * Run axe accessibility checks on the current page
   */
  async runAxe(options?: {
    include?: string[];
    exclude?: string[];
    rules?: string[];
  }): Promise<AxeResults> {
    await this.injectAxe();

    const results = await this.page.evaluate((opts) => {
      // @ts-ignore - axe is injected via script tag
      return axe.run(document, {
        runOnly: opts?.rules ? { type: 'rule', values: opts.rules } : undefined,
        ...(opts?.include || opts?.exclude
          ? {
              include: opts.include,
              exclude: opts.exclude
            }
          : {})
      });
    }, options);

    return results as AxeResults;
  }

  /**
   * Assert that there are no accessibility violations
   */
  async assertNoViolations(options?: {
    include?: string[];
    exclude?: string[];
    rules?: string[];
    allowedViolations?: string[];
  }): Promise<void> {
    const results = await this.runAxe(options);

    const violations = options?.allowedViolations
      ? results.violations.filter(v => !options.allowedViolations!.includes(v.id))
      : results.violations;

    if (violations.length > 0) {
      const violationReport = this.formatViolations(violations);
      throw new Error(`Accessibility violations found:\n${violationReport}`);
    }
  }

  /**
   * Assert specific accessibility rules
   */
  async assertRules(rules: string[]): Promise<void> {
    await this.assertNoViolations({ rules });
  }

  /**
   * Check for WCAG 2.1 Level A compliance
   */
  async assertWCAG21A(): Promise<void> {
    await this.assertNoViolations({
      rules: [
        'color-contrast',
        'image-alt',
        'label',
        'link-name',
        'button-name',
        'document-title',
        'html-has-lang',
        'valid-lang'
      ]
    });
  }

  /**
   * Check for WCAG 2.1 Level AA compliance
   */
  async assertWCAG21AA(): Promise<void> {
    await this.assertNoViolations({
      rules: [
        'color-contrast',
        'color-contrast-enhanced',
        'image-alt',
        'label',
        'link-name',
        'button-name',
        'document-title',
        'html-has-lang',
        'valid-lang',
        'landmark-one-main',
        'region'
      ]
    });
  }

  /**
   * Get a summary of accessibility results
   */
  async getAccessibilitySummary(): Promise<{
    violations: number;
    passes: number;
    incomplete: number;
  }> {
    const results = await this.runAxe();

    return {
      violations: results.violations.length,
      passes: results.passes.length,
      incomplete: results.incomplete.length
    };
  }

  /**
   * Format violations for error reporting
   */
  private formatViolations(violations: AxeViolation[]): string {
    return violations
      .map((violation, index) => {
        const nodes = violation.nodes
          .map(node => `    - ${node.html}\n      Target: ${node.target.join(', ')}`)
          .join('\n');

        return `
${index + 1}. ${violation.id} (${violation.impact})
   Description: ${violation.description}
   Help: ${violation.help}
   More info: ${violation.helpUrl}
   Affected elements:
${nodes}`;
      })
      .join('\n');
  }

  /**
   * Take a screenshot with violation markers (for debugging)
   */
  async screenshotWithViolations(filename: string): Promise<void> {
    const results = await this.runAxe();

    if (results.violations.length > 0) {
      // Highlight violations on the page
      await this.page.evaluate((violations) => {
        violations.forEach((violation) => {
          violation.nodes.forEach((node) => {
            node.target.forEach((selector) => {
              const elements = document.querySelectorAll(selector);
              elements.forEach((el) => {
                if (el instanceof HTMLElement) {
                  el.style.outline = '3px solid red';
                  el.style.outlineOffset = '2px';
                }
              });
            });
          });
        });
      }, results.violations);

      // Take screenshot
      await this.page.screenshot({ path: filename, fullPage: true });
    }
  }

  /**
   * Generate an accessibility report
   */
  async generateReport(filename: string): Promise<void> {
    const results = await this.runAxe();
    const summary = await this.getAccessibilitySummary();

    const report = `
# Accessibility Test Report
Generated: ${new Date().toISOString()}
URL: ${this.page.url()}

## Summary
- Violations: ${summary.violations}
- Passes: ${summary.passes}
- Incomplete: ${summary.incomplete}

## Violations
${results.violations.length > 0 ? this.formatViolations(results.violations) : 'No violations found!'}

## Passed Rules
${results.passes.map(pass => `- ${pass.id}: ${pass.description}`).join('\n')}

## Incomplete Checks
${results.incomplete.map(inc => `- ${inc.id}: ${inc.description} (Impact: ${inc.impact})`).join('\n')}
`;

    // Write report to file
    const fs = require('fs');
    fs.writeFileSync(filename, report);
  }
}

/**
 * Quick helper function to check accessibility in a Playwright test
 */
export async function checkAccessibility(page: Page): Promise<void> {
  const helper = new AccessibilityHelper(page);
  await helper.assertNoViolations();
}

/**
 * Quick helper to check WCAG 2.1 AA compliance
 */
export async function checkWCAG21AA(page: Page): Promise<void> {
  const helper = new AccessibilityHelper(page);
  await helper.assertWCAG21AA();
}
