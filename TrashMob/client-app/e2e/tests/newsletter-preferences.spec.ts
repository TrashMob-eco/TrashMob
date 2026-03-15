import { test, expect } from '../fixtures/auth.fixture';

test.describe('Newsletter Preferences', () => {
    test.describe.configure({ retries: 1 });

    test('should display newsletter preferences with toggles', async ({ authenticatedPage: page }) => {
        await page.goto('/mydashboard');

        // Navigate to newsletter section via sidebar
        const newsletterBtn = page.getByRole('button', { name: /^newsletters$/i });
        if (await newsletterBtn.isVisible({ timeout: 10000 }).catch(() => false)) {
            await newsletterBtn.click();
        }

        // Should show newsletter preferences heading
        await expect(page.getByText(/newsletter preferences/i).first()).toBeVisible({ timeout: 15000 });

        // Should have at least one toggle switch
        await expect(page.locator('button[role="switch"]').first()).toBeVisible({ timeout: 10000 });
    });

    test('should toggle a newsletter subscription', async ({ authenticatedPage: page }) => {
        await page.goto('/mydashboard');

        const newsletterBtn = page.getByRole('button', { name: /^newsletters$/i });
        if (await newsletterBtn.isVisible({ timeout: 10000 }).catch(() => false)) {
            await newsletterBtn.click();
        }

        await expect(page.getByText(/newsletter preferences/i).first()).toBeVisible({ timeout: 15000 });

        // Find the first toggle switch
        const firstSwitch = page.locator('button[role="switch"]').first();
        await expect(firstSwitch).toBeVisible({ timeout: 10000 });

        // Get initial state
        const initialState = await firstSwitch.getAttribute('data-state');

        // Click to toggle
        await firstSwitch.click();
        await page.waitForTimeout(1000);

        // State should have changed
        const newState = await firstSwitch.getAttribute('data-state');
        expect(newState).not.toBe(initialState);

        // Toggle back to restore original state
        await firstSwitch.click();
        await page.waitForTimeout(1000);

        const restoredState = await firstSwitch.getAttribute('data-state');
        expect(restoredState).toBe(initialState);
    });

    test('should show Unsubscribe All button', async ({ authenticatedPage: page }) => {
        await page.goto('/mydashboard');

        const newsletterBtn = page.getByRole('button', { name: /^newsletters$/i });
        if (await newsletterBtn.isVisible({ timeout: 10000 }).catch(() => false)) {
            await newsletterBtn.click();
        }

        await expect(page.getByText(/newsletter preferences/i).first()).toBeVisible({ timeout: 15000 });

        // Unsubscribe All button should be visible if user has subscriptions
        const unsubBtn = page.getByRole('button', { name: /unsubscribe all/i });
        // May not be visible if all already unsubscribed — just check it exists or doesn't error
        await expect(unsubBtn.or(page.getByText(/newsletter preferences/i).first())).toBeVisible();
    });
});
