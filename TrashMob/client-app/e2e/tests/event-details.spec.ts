import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

test.describe('Event Details', () => {
    // Event details page has intermittent crashes due to backend 500s on attendee routes
    test.describe.configure({ retries: 2 });
    test('should display event name and details', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);
        await expect(page.locator('h1, h2, h3').first()).toBeVisible({ timeout: 30000 });
    });

    test('should show register or unregister button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);
        await expect(page.getByRole('button', { name: /register|unregister/i })).toBeVisible({ timeout: 30000 });
    });

    test('should show share button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);
        await expect(page.getByRole('button', { name: /share/i })).toBeVisible({ timeout: 30000 });
    });
});
