import { test, expect } from '@playwright/test';
import { test as authTest, expect as authExpect } from '../fixtures/auth.fixture';

/**
 * Coverage tests for pages that previously had zero E2E tests.
 */
test.describe('Page Coverage — Public Pages', () => {
    test('should load News listing page', async ({ page }) => {
        await page.goto('/news');

        await expect(page.locator('h1')).toContainText(/news/i, { timeout: 15000 });
    });

    test('should load Unsubscribe page (no token shows invalid)', async ({ page }) => {
        await page.goto('/unsubscribe');

        await expect(page.getByText(/unsubscribe|invalid|expired/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should handle non-existent news article', async ({ page }) => {
        await page.goto('/news/non-existent-slug-12345');

        // Should show either not found or the news page
        await expect(page.locator('h1').first()).toBeVisible({ timeout: 15000 });
    });
});

authTest.describe('Page Coverage — Authenticated Pages', () => {
    authTest.describe.configure({ retries: 1 });

    authTest('should load Waivers page', async ({ authenticatedPage: page }) => {
        await page.goto('/waivers');

        await authExpect(page.getByText(/waiver/i).first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should load Become a Partner page with form', async ({ authenticatedPage: page }) => {
        await page.goto('/becomeapartner');

        await authExpect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should load Invite a Partner page', async ({ authenticatedPage: page }) => {
        await page.goto('/inviteapartner');

        await authExpect(page.getByText(/invite.*partner/i).first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should load Delete My Data page with warning', async ({ authenticatedPage: page }) => {
        await page.goto('/deletemydata');

        await authExpect(page.getByText(/delete.*account|delete.*data/i).first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should show download data and delete buttons', async ({ authenticatedPage: page }) => {
        await page.goto('/deletemydata');

        await authExpect(page.getByText(/delete.*account|delete.*data/i).first()).toBeVisible({ timeout: 15000 });
        await authExpect(page.getByRole('button', { name: /download.*data/i })).toBeVisible({ timeout: 10000 });
        await authExpect(page.getByRole('button', { name: /delete account/i })).toBeVisible();
    });

    authTest('should load Event Summary for completed event', async ({ authenticatedPage: page }) => {
        const baseApi = process.env.BASE_URL
            ? `${process.env.BASE_URL}/api`
            : 'https://dev.trashmob.eco/api';

        const response = await page.request.get(`${baseApi}/v2/events/completed`);
        const events = await response.json();
        authTest.skip(!Array.isArray(events) || events.length === 0, 'No completed events');

        await page.goto(`/eventsummary/${events[0].id}`);
        await authExpect(page.locator('h1, h2').first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should load Achievements page', async ({ authenticatedPage: page }) => {
        await page.goto('/achievements');

        await authExpect(page.getByText(/achievement/i).first()).toBeVisible({ timeout: 15000 });
    });
});
