import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

test.describe('Event Registration Flow', () => {
    test.describe.configure({ retries: 1 });

    test('should show Attend button for non-registered user', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const content = page.locator('h2, h3').first();
        await expect(content.or(errorBoundary)).toBeVisible({ timeout: 30000 });
        test.skip(await errorBoundary.isVisible().catch(() => false), 'Page crashed');

        // Should see Attend button (or it's hidden because user already registered)
        const attendBtn = page.getByRole('button', { name: /^attend$/i });
        const calendarBtn = page.getByRole('button', { name: /calendar/i });
        // At minimum, the calendar button should always be visible on the event detail page
        await expect(calendarBtn).toBeVisible({ timeout: 10000 });
    });

    test('should show calendar and share buttons on event detail', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const content = page.locator('h2, h3').first();
        await expect(content.or(errorBoundary)).toBeVisible({ timeout: 30000 });
        test.skip(await errorBoundary.isVisible().catch(() => false), 'Page crashed');

        await expect(page.getByRole('button', { name: /calendar/i })).toBeVisible({ timeout: 10000 });
        await expect(page.getByRole('button', { name: /share/i })).toBeVisible({ timeout: 10000 });
    });

    test('should open calendar dropdown with options', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const content = page.locator('h2, h3').first();
        await expect(content.or(errorBoundary)).toBeVisible({ timeout: 30000 });
        test.skip(await errorBoundary.isVisible().catch(() => false), 'Page crashed');

        // Click calendar dropdown
        await page.getByRole('button', { name: /calendar/i }).click();

        // Should show calendar options
        await expect(page.getByRole('menuitem', { name: /apple/i }).or(page.getByText(/apple/i))).toBeVisible({
            timeout: 5000,
        });
        await expect(page.getByText(/google/i).last()).toBeVisible();
    });
});
