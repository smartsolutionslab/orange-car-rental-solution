import { test, expect } from '@playwright/test';

test('debug: capture browser console output', async ({ page }) => {
  const logs: string[] = [];
  const errors: string[] = [];

  // Capture console messages including all arguments
  page.on('console', async msg => {
    const args = await Promise.all(msg.args().map(arg => arg.jsonValue().catch(() => arg.toString())));
    const text = `[${msg.type()}] ${args.join(' ')}`;
    logs.push(text);
    console.log(text);
  });

  // Capture page errors
  page.on('pageerror', err => {
    errors.push(err.message);
    console.log(`[PAGE ERROR] ${err.message}`);
  });

  // Navigate - try /login which has fewer component dependencies
  await page.goto('/login');

  // Wait for potential loading
  await page.waitForTimeout(5000);

  // Get page HTML
  const html = await page.content();
  console.log('\n=== PAGE HTML ===');
  console.log(html);
  console.log('\n=== END HTML ===\n');

  // Check if app rendered
  const appRoot = await page.locator('app-root').innerHTML();
  console.log('\n=== APP-ROOT CONTENT ===');
  console.log(appRoot);
  console.log('\n=== END APP-ROOT ===\n');

  // Output all logs
  console.log('\n=== ALL CONSOLE LOGS ===');
  logs.forEach(log => console.log(log));
  console.log('\n=== END LOGS ===');

  // Output errors
  console.log('\n=== ALL PAGE ERRORS ===');
  errors.forEach(err => console.log(err));
  console.log('\n=== END ERRORS ===');

  // This test always passes - it's for debugging
  expect(true).toBe(true);
});
