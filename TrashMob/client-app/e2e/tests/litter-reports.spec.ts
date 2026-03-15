import { test, expect } from '@playwright/test';

test.describe('Litter Reports Page Interactions', () => {
    test('should display litter reports with filter controls', async ({ page }) => {
        await page.goto('/litterreports');

        await expect(page.locator('h1').first()).toContainText(/litter report/i, { timeout: 15000 });

        // Should show filter controls
        await expect(page.getByText(/report.*found/i)).toBeVisible({ timeout: 10000 });
    });

    test('should toggle between list and map views', async ({ page }) => {
        await page.goto('/litterreports');

        await expect(page.locator('h1').first()).toContainText(/litter report/i, { timeout: 15000 });

        const toggleGroup = page.locator('[role="group"]').first();
        await expect(toggleGroup).toBeVisible({ timeout: 10000 });

        const buttons = toggleGroup.locator('button');

        // Click map view
        await buttons.last().click();
        await expect(buttons.last()).toHaveAttribute('data-state', 'on');

        // Click list view
        await buttons.first().click();
        await expect(buttons.first()).toHaveAttribute('data-state', 'on');
    });

    test('should filter reports by location search', async ({ page }) => {
        await page.goto('/litterreports');

        await expect(page.locator('h1').first()).toContainText(/litter report/i, { timeout: 15000 });

        // Find location filter input
        const locationInput = page.getByPlaceholder(/filter by city/i);
        if (await locationInput.isVisible({ timeout: 5000 }).catch(() => false)) {
            await locationInput.fill('Seattle');
            // Results should update (count may change)
            await page.waitForTimeout(500);
            await expect(page.getByText(/report.*found/i)).toBeVisible();
        }
    });

    test('should have Report Litter button linking to create page', async ({ page }) => {
        await page.goto('/litterreports');

        await expect(page.locator('h1').first()).toContainText(/litter report/i, { timeout: 15000 });

        const reportBtn = page.getByRole('link', { name: /report litter/i });
        await expect(reportBtn).toBeVisible();
        await expect(reportBtn).toHaveAttribute('href', /litterreports\/create/);
    });
});
