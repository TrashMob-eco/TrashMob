import { test, expect } from '../fixtures/auth.fixture';

test.describe('Authenticated User', () => {
    test('should show account menu when logged in', async ({ authenticatedPage }) => {
        await authenticatedPage.goto('/');
        await authenticatedPage.waitForLoadState('networkidle');

        // Wait for MSAL to restore session and load user — may take a few seconds
        const accountMenu = authenticatedPage.locator('button[aria-label*="Account menu"]');
        await expect(accountMenu).toBeVisible({ timeout: 30000 });
    });

    test('should load My Dashboard', async ({ authenticatedPage }) => {
        await authenticatedPage.goto('/mydashboard');

        // Dashboard should have user-specific content
        await expect(authenticatedPage.locator('button[aria-label*="Account menu"]')).toBeVisible({ timeout: 15000 });
    });

    test('should load My Profile page', async ({ authenticatedPage }) => {
        await authenticatedPage.goto('/myprofile');

        // Profile page should show the username input
        await expect(
            authenticatedPage.locator('input[name="userName"]'),
        ).toBeVisible({ timeout: 15000 });
    });

    test('should navigate to event creation page', async ({ authenticatedPage }) => {
        await authenticatedPage.goto('/events/create');

        // The page has h1 "Manage Event"
        await expect(authenticatedPage.locator('h1')).toContainText(/manage event/i, { timeout: 15000 });
    });

    test('should load Teams page', async ({ authenticatedPage }) => {
        await authenticatedPage.goto('/teams');

        await expect(
            authenticatedPage.getByRole('heading', { name: 'Teams', exact: true }),
        ).toBeVisible({ timeout: 15000 });
    });

    test('should load Litter Reports page', async ({ authenticatedPage }) => {
        await authenticatedPage.goto('/litterreports');

        await expect(
            authenticatedPage.getByRole('heading', { name: /litter report/i }).first(),
        ).toBeVisible({ timeout: 15000 });
    });
});
