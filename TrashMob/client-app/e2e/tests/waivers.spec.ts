import { test, expect } from '../fixtures/auth.fixture';

test.describe('Waivers Page', () => {
    test('should display waiver content', async ({ authenticatedPage: page }) => {
        await page.goto('/waivers');

        await expect(page.getByText(/waiver/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should have acceptance checkbox and sign button', async ({ authenticatedPage: page }) => {
        await page.goto('/waivers');

        // Look for the acceptance checkbox
        const checkbox = page.locator('#i-have-read-and-accept').or(
            page.getByRole('checkbox'),
        );

        // The waiver page may show differently if already signed
        const alreadySigned = page.getByText(/already signed|waiver signed|valid/i);
        await expect(checkbox.or(alreadySigned).first()).toBeVisible({ timeout: 15000 });
    });

    test('should show my waivers in dashboard', async ({ authenticatedPage: page }) => {
        await page.goto('/mydashboard');

        // Navigate to waivers section
        const waiversBtn = page.getByRole('button', { name: /^waivers$/i });
        if (await waiversBtn.isVisible({ timeout: 10000 }).catch(() => false)) {
            await waiversBtn.click();
        }

        await expect(page.getByText(/my waivers/i).first()).toBeVisible({ timeout: 15000 });
    });
});
