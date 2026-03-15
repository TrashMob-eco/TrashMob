import { test, expect } from '../fixtures/auth.fixture';

test.describe('Location Preference', () => {
    test('should display location preference page with heading', async ({ authenticatedPage: page }) => {
        await page.goto('/locationpreference');

        await expect(page.locator('h1')).toContainText(/set your location/i, { timeout: 20000 });
    });

    test('should show location preferences card', async ({ authenticatedPage: page }) => {
        await page.goto('/locationpreference');

        await expect(page.getByText(/location preferences/i)).toBeVisible({ timeout: 20000 });
    });

    test('should have Save and Discard buttons', async ({ authenticatedPage: page }) => {
        await page.goto('/locationpreference');

        await expect(page.getByRole('button', { name: /save/i })).toBeVisible({ timeout: 20000 });
        await expect(page.getByRole('button', { name: /discard/i })).toBeVisible();
    });

    test('should have address fields', async ({ authenticatedPage: page }) => {
        await page.goto('/locationpreference');

        await expect(page.locator('input[name="city"]')).toBeVisible({ timeout: 20000 });
        await expect(page.locator('input[name="region"]')).toBeVisible();
        await expect(page.locator('input[name="postalCode"]')).toBeVisible();
    });
});
