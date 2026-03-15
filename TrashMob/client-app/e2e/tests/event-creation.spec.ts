import { test, expect } from '../fixtures/auth.fixture';

test.describe('Event Creation Flow', () => {
    test.describe.configure({ retries: 1 });

    test('should display step 1 (Pick Location) with search', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        // Step 1 should show location picking UI
        await expect(page.getByText(/pick location/i).or(page.getByText(/search for address/i))).toBeVisible({ timeout: 15000 });
    });

    test('should show all three step indicators', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        await expect(page.getByText(/step 1/i)).toBeVisible({ timeout: 15000 });
        await expect(page.getByText(/step 2/i)).toBeVisible();
        await expect(page.getByText(/step 3/i)).toBeVisible();
    });

    test('should show Next button on step 1', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        await expect(page.getByText(/step 1/i)).toBeVisible({ timeout: 15000 });

        // Next button should exist (may be disabled until location selected)
        await expect(page.getByRole('button', { name: /next/i })).toBeVisible({ timeout: 10000 });
    });

    test('should display manage event layout', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        // Should have the manage event heading
        await expect(page.locator('h1')).toBeVisible({ timeout: 15000 });
    });
});
