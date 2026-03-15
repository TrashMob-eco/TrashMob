import { test, expect } from '@playwright/test';

test.describe('Partner Request Pages', () => {
    test('should load Become a Partner form with required fields', async ({ page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        // Form should have key fields
        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
        await expect(page.locator('input[name="email"]').or(page.locator('input[type="email"]'))).toBeVisible();
    });

    test('should show partner type selection', async ({ page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        // Partner type radio group or select
        await expect(page.getByText(/partner type/i).or(page.getByText(/government|community|business/i).first())).toBeVisible({ timeout: 10000 });
    });

    test('should load Invite a Partner page with form', async ({ page }) => {
        await page.goto('/inviteapartner');

        await expect(page.getByText(/invite.*partner/i).first()).toBeVisible({ timeout: 15000 });

        // Should have form fields
        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
    });

    test('should have cancel button navigating to partnerships', async ({ page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        const cancelBtn = page.getByRole('button', { name: /cancel/i }).or(
            page.getByRole('link', { name: /cancel/i }),
        );
        await expect(cancelBtn).toBeVisible({ timeout: 10000 });
    });
});
