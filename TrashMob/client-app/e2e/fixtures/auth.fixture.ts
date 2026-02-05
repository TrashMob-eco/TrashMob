import { test as base, Page, BrowserContext } from '@playwright/test';
import path from 'path';

/**
 * Test user credentials - configure these in Azure B2C dev tenant.
 * Passwords should be stored in environment variables for CI.
 */
export const TEST_USERS = {
    standard: {
        email: process.env.E2E_USER_EMAIL || 'e2e-user@trashmob.eco',
        password: process.env.E2E_USER_PASSWORD || '',
    },
    admin: {
        email: process.env.E2E_ADMIN_EMAIL || 'e2e-admin@trashmob.eco',
        password: process.env.E2E_ADMIN_PASSWORD || '',
    },
    partner: {
        email: process.env.E2E_PARTNER_EMAIL || 'e2e-partner@trashmob.eco',
        password: process.env.E2E_PARTNER_PASSWORD || '',
    },
};

// Path to store auth state for reuse across tests
const AUTH_STATE_DIR = path.join(__dirname, '..', '.auth');
export const USER_AUTH_FILE = path.join(AUTH_STATE_DIR, 'user.json');
export const ADMIN_AUTH_FILE = path.join(AUTH_STATE_DIR, 'admin.json');

/**
 * Extended test fixture with authenticated page support.
 *
 * Usage:
 * ```typescript
 * import { test, expect } from '../fixtures/auth.fixture';
 *
 * test('authenticated test', async ({ authenticatedPage }) => {
 *   await authenticatedPage.goto('/mydashboard');
 *   // User is already logged in
 * });
 * ```
 */
export const test = base.extend<{
    authenticatedPage: Page;
    authenticatedContext: BrowserContext;
    adminPage: Page;
}>({
    // Standard authenticated user page
    authenticatedPage: async ({ browser }, use) => {
        const context = await browser.newContext({
            storageState: USER_AUTH_FILE,
        });
        const page = await context.newPage();
        await use(page);
        await context.close();
    },

    // Authenticated context for multiple pages
    authenticatedContext: async ({ browser }, use) => {
        const context = await browser.newContext({
            storageState: USER_AUTH_FILE,
        });
        await use(context);
        await context.close();
    },

    // Admin user page
    adminPage: async ({ browser }, use) => {
        const context = await browser.newContext({
            storageState: ADMIN_AUTH_FILE,
        });
        const page = await context.newPage();
        await use(page);
        await context.close();
    },
});

export { expect } from '@playwright/test';

/**
 * Helper to perform login via Azure B2C.
 * Call this in global setup to generate auth state files.
 *
 * @param page - Playwright page
 * @param email - User email
 * @param password - User password
 */
export async function loginWithB2C(page: Page, email: string, password: string): Promise<void> {
    // Navigate to a page that triggers login
    await page.goto('/');

    // Click sign in button
    await page.getByRole('button', { name: /sign in/i }).click();

    // Wait for B2C redirect and fill credentials
    // Note: The exact selectors depend on your B2C configuration
    await page.waitForURL(/.*b2clogin\.com.*|.*ciamlogin\.com.*/);

    // Fill email
    await page.fill('input[type="email"], input[name="signInName"], #email', email);

    // Click next or continue if multi-step
    const nextButton = page.locator('button:has-text("Next"), button:has-text("Continue")');
    if (await nextButton.isVisible()) {
        await nextButton.click();
    }

    // Fill password
    await page.fill('input[type="password"], #password', password);

    // Submit
    await page.click('button[type="submit"], #next');

    // Wait for redirect back to app
    await page.waitForURL(/.*localhost.*|.*trashmob\.eco.*/);

    // Wait for user to be loaded
    await page.waitForSelector('[data-testid="user-menu"], [aria-label="User menu"]', {
        timeout: 30000,
    });
}
