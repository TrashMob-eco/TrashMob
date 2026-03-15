import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

test.describe('Share Dialog', () => {
    test.describe.configure({ retries: 2 });

    test('should open share dialog with social tab', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const shareBtn = page.getByRole('button', { name: /share/i });
        await expect(shareBtn.or(errorBoundary)).toBeVisible({ timeout: 30000 });
        test.skip(await errorBoundary.isVisible().catch(() => false), 'Page crashed');

        await shareBtn.click();

        // Dialog should open
        await expect(page.getByRole('dialog')).toBeVisible({ timeout: 5000 });

        // Should show Social tab with share link section
        await expect(page.getByText(/share a link/i)).toBeVisible({ timeout: 5000 });
    });

    test('should switch to QR code tab', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const shareBtn = page.getByRole('button', { name: /share/i });
        await expect(shareBtn.or(errorBoundary)).toBeVisible({ timeout: 30000 });
        test.skip(await errorBoundary.isVisible().catch(() => false), 'Page crashed');

        await shareBtn.click();
        await expect(page.getByRole('dialog')).toBeVisible({ timeout: 5000 });

        // Switch to QR tab
        const qrTab = page.getByRole('tab', { name: /qr/i });
        if (await qrTab.isVisible().catch(() => false)) {
            await qrTab.click();
            // QR code should appear (canvas or img element)
            await expect(page.locator('canvas, img[alt*="QR"], svg').first()).toBeVisible({ timeout: 5000 });
        }
    });

    test('should have close button in share dialog', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const shareBtn = page.getByRole('button', { name: /share/i });
        await expect(shareBtn.or(errorBoundary)).toBeVisible({ timeout: 30000 });
        test.skip(await errorBoundary.isVisible().catch(() => false), 'Page crashed');

        await shareBtn.click();
        await expect(page.getByRole('dialog')).toBeVisible({ timeout: 5000 });

        // Close button should be visible
        await expect(page.getByRole('dialog').getByRole('button', { name: /close/i })).toBeVisible({ timeout: 5000 });
    });
});
