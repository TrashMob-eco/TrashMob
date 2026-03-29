import { test, expect } from '../fixtures/auth.fixture';

test.describe('Leaderboards', () => {
    test('should display leaderboard page with tabs', async ({ authenticatedPage: page }) => {
        await page.goto('/leaderboards');

        await expect(page.getByRole('heading', { name: /leaderboards/i })).toBeVisible({ timeout: 15000 });

        // Should have Volunteers and Teams tabs
        await expect(page.getByRole('tab', { name: /volunteers/i })).toBeVisible();
        await expect(page.getByRole('tab', { name: /teams/i })).toBeVisible();
    });

    test('should display filter dropdowns', async ({ authenticatedPage: page }) => {
        await page.goto('/leaderboards');

        // Wait for page load
        await expect(page.getByRole('heading', { name: /leaderboards/i })).toBeVisible({ timeout: 15000 });

        // Should have leaderboard type and time range selectors
        await expect(page.locator('[role="combobox"]').first()).toBeVisible();
    });

    test('should switch between volunteers and teams tabs', async ({ authenticatedPage: page }) => {
        await page.goto('/leaderboards');

        await expect(page.getByRole('tab', { name: /volunteers/i })).toBeVisible({ timeout: 15000 });

        // Click teams tab
        await page.getByRole('tab', { name: /teams/i }).click();

        // Teams tab should be selected
        await expect(page.getByRole('tab', { name: /teams/i })).toHaveAttribute('data-state', 'active');
    });

    test('should display ranking info section', async ({ authenticatedPage: page }) => {
        await page.goto('/leaderboards');

        // Wait for page to load
        await expect(page.getByRole('heading', { name: /leaderboards/i })).toBeVisible({ timeout: 15000 });

        // "How Rankings Work" is always rendered in the sidebar regardless of data load state
        await expect(page.getByText('How Rankings Work')).toBeVisible({ timeout: 10000 });
    });
});
