import { test, expect } from '../fixtures/auth.fixture';

test.describe('Profile Edit Flow', () => {
    test.describe.configure({ retries: 2 });
    test('should edit and save profile name fields', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');
        await page.waitForLoadState('networkidle').catch(() => {});

        const givenNameInput = page.locator('input[name="givenName"]');
        await expect(givenNameInput).toBeVisible({ timeout: 30000 });

        // Save original value
        const originalGivenName = await givenNameInput.inputValue();

        // Edit the field
        const testName = `E2ETest${Date.now() % 10000}`;
        await givenNameInput.fill(testName);

        // Click save
        await page.getByRole('button', { name: /save/i }).click();

        // Should show success toast
        await expect(page.getByText('Profile updated!', { exact: true })).toBeVisible({ timeout: 10000 });

        // Restore original value — reload same page and wait for form to repopulate
        await page.reload({ waitUntil: 'domcontentloaded' });
        await expect(givenNameInput).toBeVisible({ timeout: 15000 });
        // Wait for React Query to repopulate the form
        await page.waitForTimeout(2000);
        await givenNameInput.fill(originalGivenName);
        await page.getByRole('button', { name: /save/i }).click();
        await expect(page.getByText('Profile updated!', { exact: true })).toBeVisible({ timeout: 10000 });
    });

    test('should discard changes and navigate away', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        const givenNameInput = page.locator('input[name="givenName"]');
        await expect(givenNameInput).toBeVisible({ timeout: 15000 });

        // Edit the field
        await givenNameInput.fill('DiscardTest');

        // Click discard
        await page.getByRole('button', { name: /discard/i }).click();

        // Should navigate away from profile page
        await expect(page).not.toHaveURL(/myprofile/, { timeout: 5000 });
    });

    test('should show validation error for short username', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');

        const usernameInput = page.locator('input[name="userName"]');
        await expect(usernameInput).toBeVisible({ timeout: 15000 });

        // Save original
        const originalUsername = await usernameInput.inputValue();

        // Set short username
        await usernameInput.fill('ab');
        await page.getByRole('button', { name: /save/i }).click();

        // Should show validation error
        await expect(page.getByText(/at least 3/i)).toBeVisible({ timeout: 5000 });

        // Restore original
        await usernameInput.fill(originalUsername);
    });

    test('should persist profile data after page reload', async ({ authenticatedPage: page }) => {
        await page.goto('/myprofile');
        await page.waitForLoadState('networkidle').catch(() => {});

        const cityInput = page.locator('input[name="city"]');
        await expect(cityInput).toBeVisible({ timeout: 30000 });

        // Get current city value
        const currentCity = await cityInput.inputValue();

        // Reload and verify it persists
        await page.reload();
        await expect(cityInput).toBeVisible({ timeout: 15000 });
        const reloadedCity = await cityInput.inputValue();

        expect(reloadedCity).toBe(currentCity);
    });
});
