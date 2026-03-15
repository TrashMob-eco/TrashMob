import { test, expect } from '@playwright/test';

test.describe('Communities', () => {
    test('should display communities page', async ({ page }) => {
        await page.goto('/communities');

        await expect(page.locator('h1').first()).toContainText(/communities/i, { timeout: 15000 });
    });

    test('should display browse communities heading', async ({ page }) => {
        await page.goto('/communities');

        await expect(page.getByRole('heading', { name: /browse communities/i })).toBeVisible({ timeout: 15000 });
    });

    test('should load community detail when clicking view', async ({ page }) => {
        await page.goto('/communities');

        // Wait for the list to load
        await expect(page.locator('h1').first()).toContainText(/communities/i, { timeout: 15000 });

        // Try to click the first community view link (if any exist)
        const viewButton = page.getByRole('link', { name: /view/i }).first();
        if (await viewButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await viewButton.click();

            // Community detail should show a heading and back link
            await expect(page.locator('h1').first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByText(/back to communities/i)).toBeVisible();
        }
    });
});
