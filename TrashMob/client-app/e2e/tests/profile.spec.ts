import { test, expect } from '../fixtures/auth.fixture';

test.describe('My Profile', () => {
    test('should display profile form with user data', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        // Username field should be populated
        const usernameInput = page.locator('input[name="userName"]');
        await expect(usernameInput).toBeVisible({ timeout: 15000 });
        await expect(usernameInput).not.toBeEmpty();
    });

    test('should display email as read-only text', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        // Email is displayed as text, not an editable input
        await expect(page.getByText(/email is managed by your identity provider/i)).toBeVisible({ timeout: 15000 });
    });

    test('should have Save and Discard buttons', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        await expect(page.getByRole('button', { name: /save/i })).toBeVisible({ timeout: 15000 });
        await expect(page.getByRole('button', { name: /discard/i })).toBeVisible();
    });

    test('should display Data & Privacy section', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        await expect(page.getByText(/data.*privacy/i)).toBeVisible({ timeout: 15000 });
        await expect(page.getByRole('button', { name: /download my data/i })).toBeVisible();
    });

    test('should show profile photo section', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        await expect(page.getByText(/change photo/i)).toBeVisible({ timeout: 15000 });
    });

    test('should display first name and last name fields', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        await expect(page.locator('input[name="givenName"]')).toBeVisible({ timeout: 15000 });
        await expect(page.locator('input[name="surname"]')).toBeVisible();
    });
});
