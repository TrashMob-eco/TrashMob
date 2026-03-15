import { test, expect } from '../fixtures/auth.fixture';

test.describe('Partner Request Pages', () => {
    test.describe.configure({ retries: 1 });

    test('should load Become a Partner form with required fields', async ({ authenticatedPage: page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
    });

    test('should show email field on partner form', async ({ authenticatedPage: page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        // Email field should be present
        await expect(page.locator('input[name="email"]').or(page.locator('input[type="email"]'))).toBeVisible({ timeout: 10000 });
    });

    test('should load Invite a Partner page with form', async ({ authenticatedPage: page }) => {
        await page.goto('/inviteapartner');

        await expect(page.getByText(/invite.*partner/i).first()).toBeVisible({ timeout: 15000 });

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
    });
});
