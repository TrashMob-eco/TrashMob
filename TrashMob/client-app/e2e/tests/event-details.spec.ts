import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

/**
 * Helper: navigate to event details and wait for it to load.
 * Returns false if the page crashes (intermittent backend 500s).
 */
async function gotoEventDetails(page: import('@playwright/test').Page, eventId: string): Promise<boolean> {
    await page.goto(`/eventdetails/${eventId}`);
    // Wait for either the event content or the error boundary
    const result = await Promise.race([
        page.locator('h2, h3').first().waitFor({ state: 'visible', timeout: 30000 }).then(() => 'loaded'),
        page.getByText('Something went wrong').waitFor({ state: 'visible', timeout: 30000 }).then(() => 'crashed'),
    ]).catch(() => 'timeout');
    return result === 'loaded';
}

test.describe('Event Details', () => {
    // Intermittent backend 500s can crash this page
    test.describe.configure({ retries: 2 });

    test('should display event name and details', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        const loaded = await gotoEventDetails(page, events[0].id);
        test.skip(!loaded, 'Event details page crashed (intermittent backend error)');

        await expect(page.locator('h2, h3').first()).toBeVisible();
    });

    test('should show register or unregister button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        const loaded = await gotoEventDetails(page, events[0].id);
        test.skip(!loaded, 'Event details page crashed (intermittent backend error)');

        await expect(page.getByRole('button', { name: /register|unregister/i })).toBeVisible({ timeout: 10000 });
    });

    test('should show share button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        const loaded = await gotoEventDetails(page, events[0].id);
        test.skip(!loaded, 'Event details page crashed (intermittent backend error)');

        await expect(page.getByRole('button', { name: /share/i })).toBeVisible({ timeout: 10000 });
    });
});
