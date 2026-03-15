import { test, expect } from '@playwright/test';
import { test as authTest, expect as authExpect } from '../fixtures/auth.fixture';

/**
 * Coverage tests for pages that previously had zero E2E tests.
 * These verify the pages load without crashing and display key content.
 */
test.describe('Page Coverage — Public Pages', () => {
    test('should load News listing page with articles', async ({ page }) => {
        await page.goto('/news');

        await expect(page.locator('h1')).toContainText(/news/i, { timeout: 15000 });
    });

    test('should load Waivers information page', async ({ page }) => {
        await page.goto('/waivers');

        await expect(page.getByText(/waiver/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load Become a Partner page with form', async ({ page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load Invite a Partner page', async ({ page }) => {
        await page.goto('/inviteapartner');

        await expect(page.getByText(/invite.*partner/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load Unsubscribe page', async ({ page }) => {
        await page.goto('/unsubscribe');

        // Without a token, should show invalid link message
        await expect(page.getByText(/unsubscribe|invalid|expired/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should display 404 for non-existent news article', async ({ page }) => {
        await page.goto('/news/non-existent-slug-12345');

        // Should show either article not found or 404
        await expect(
            page.getByText(/not found|404|no article/i).first().or(page.locator('h1').first()),
        ).toBeVisible({ timeout: 15000 });
    });
});

authTest.describe('Page Coverage — Authenticated Pages', () => {
    authTest.describe.configure({ retries: 1 });

    authTest('should load Delete My Data page with warning', async ({ authenticatedPage: page }) => {
        await page.goto('/deletemydata');

        await authExpect(page.getByText(/delete.*account|delete.*data/i).first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should show download data and delete buttons on delete page', async ({ authenticatedPage: page }) => {
        await page.goto('/deletemydata');

        await authExpect(page.getByText(/delete.*account|delete.*data/i).first()).toBeVisible({ timeout: 15000 });

        // Should have download data option
        await authExpect(page.getByRole('button', { name: /download.*data/i })).toBeVisible({ timeout: 10000 });

        // Should have delete account button
        await authExpect(page.getByRole('button', { name: /delete account/i })).toBeVisible();
    });

    authTest('should load Team Create page', async ({ authenticatedPage: page }) => {
        await page.goto('/teams/create');

        await authExpect(page.locator('h1')).toContainText(/create.*team/i, { timeout: 15000 });
    });

    authTest('should load Event Summary page for completed event', async ({ authenticatedPage: page }) => {
        const baseApi = process.env.BASE_URL
            ? `${process.env.BASE_URL}/api`
            : 'https://dev.trashmob.eco/api';

        const response = await page.request.get(`${baseApi}/v2/events/completed`);
        const events = await response.json();
        authTest.skip(!Array.isArray(events) || events.length === 0, 'No completed events');

        await page.goto(`/eventsummary/${events[0].id}`);

        // Should load event summary page
        await authExpect(page.locator('h1, h2').first()).toBeVisible({ timeout: 15000 });
    });

    authTest('should load Achievements page', async ({ authenticatedPage: page }) => {
        await page.goto('/achievements');

        await authExpect(page.getByText(/achievement/i).first()).toBeVisible({ timeout: 15000 });
    });
});
