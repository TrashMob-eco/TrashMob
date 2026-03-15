import { test, expect } from '../fixtures/auth.fixture';

test.describe('Waivers', () => {
    test('should show my waivers section in dashboard', async ({ authenticatedPage: page }) => {
        await page.goto('/mydashboard');

        const waiversBtn = page.getByRole('button', { name: /^waivers$/i });
        if (await waiversBtn.isVisible({ timeout: 10000 }).catch(() => false)) {
            await waiversBtn.click();
        }

        await expect(page.getByText(/my waivers/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should display signed waiver status', async ({ authenticatedPage: page }) => {
        await page.goto('/mydashboard');

        const waiversBtn = page.getByRole('button', { name: /^waivers$/i });
        if (await waiversBtn.isVisible({ timeout: 10000 }).catch(() => false)) {
            await waiversBtn.click();
        }

        // Should show waiver table or "no waivers" message
        const waiverTable = page.locator('table').last();
        const noWaivers = page.getByText(/no waivers|sign required/i);
        await expect(waiverTable.or(noWaivers)).toBeVisible({ timeout: 15000 });
    });
});
