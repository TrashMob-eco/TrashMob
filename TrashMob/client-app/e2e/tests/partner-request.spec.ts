import { test, expect } from '../fixtures/auth.fixture';

test.describe('Partner Request Pages', () => {
    test.describe.configure({ retries: 1 });

    test('should load Become a Partner form with required fields', async ({ authenticatedPage: page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
    });

    test('should show partner type selection', async ({ authenticatedPage: page }) => {
        await page.goto('/becomeapartner');

        await expect(page.getByText(/apply.*partner|become.*partner/i).first()).toBeVisible({ timeout: 15000 });

        await expect(page.getByText(/partner type/i).or(page.getByText(/government|community|business/i).first())).toBeVisible({ timeout: 10000 });
    });

    test('should load Invite a Partner page with form', async ({ authenticatedPage: page }) => {
        await page.goto('/inviteapartner');

        await expect(page.getByText(/invite.*partner/i).first()).toBeVisible({ timeout: 15000 });

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
    });
});
