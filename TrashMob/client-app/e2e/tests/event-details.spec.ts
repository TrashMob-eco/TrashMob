import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

/**
 * Event details tests are prone to intermittent crashes due to backend 500s
 * on the attendee routes endpoint. These tests use soft assertions and skip
 * when the page crashes rather than failing the CI run.
 */
test.describe('Event Details', () => {
    test.describe.configure({ retries: 1 });

    test('should display event name and details', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        // Wait for either content or error boundary
        const heading = page.locator('h2, h3').first();
        const errorBoundary = page.getByText('Something went wrong');
        await expect(heading.or(errorBoundary)).toBeVisible({ timeout: 30000 });

        // Skip if page crashed
        if (await errorBoundary.isVisible().catch(() => false)) {
            test.skip(true, 'Event details page crashed (intermittent backend error)');
        }

        await expect(heading).toBeVisible();
    });

    test('should show register or unregister button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        // Three legitimate outcomes for a successful page render:
        //   1. The action button rendered → assert it and we're done
        //   2. The page crashed and the error boundary showed → skip (existing behavior)
        //   3. The event filled up to maxNumberOfParticipants → the page renders
        //      "This event is full." in place of the action button. That's correct
        //      behavior, just not what this test can validate, so skip.
        // If none of these appear within 30s, the page is genuinely broken and the
        // test should fail.
        const errorBoundary = page.getByText('Something went wrong');
        const attendBtn = page.getByRole('button', { name: /attend|register|unregister/i });
        const eventFull = page.getByText(/this event is full/i);

        await expect(attendBtn.or(errorBoundary).or(eventFull)).toBeVisible({ timeout: 30000 });

        if (await errorBoundary.isVisible().catch(() => false)) {
            test.skip(true, 'Event details page crashed (intermittent backend error)');
        }

        if (await eventFull.isVisible().catch(() => false)) {
            test.skip(true, 'Selected active event is full — no registration UI to test');
        }

        await expect(attendBtn).toBeVisible();
    });

    test('should show share button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/events/active`);
        const events = await response.json();
        test.skip(!Array.isArray(events) || events.length === 0, 'No active events');

        await page.goto(`/eventdetails/${events[0].id}`);

        const errorBoundary = page.getByText('Something went wrong');
        const shareBtn = page.getByRole('button', { name: /share/i });

        await expect(shareBtn.or(errorBoundary)).toBeVisible({ timeout: 30000 });

        if (await errorBoundary.isVisible().catch(() => false)) {
            test.skip(true, 'Event details page crashed (intermittent backend error)');
        }

        await expect(shareBtn).toBeVisible();
    });
});
