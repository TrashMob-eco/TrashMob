import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

test.describe('Team Detail Page', () => {
    test.describe.configure({ retries: 1 });

    test('should display team name and info', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/teams?pageSize=5`);
        const data = await response.json();
        const teams = data.items || data;
        test.skip(!Array.isArray(teams) || teams.length === 0, 'No teams available');

        await page.goto(`/teams/${teams[0].id}`);

        // Team name should be visible
        await expect(page.locator('h1, h2').first()).toBeVisible({ timeout: 15000 });
    });

    test('should show members section', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/teams?pageSize=5`);
        const data = await response.json();
        const teams = data.items || data;
        test.skip(!Array.isArray(teams) || teams.length === 0, 'No teams available');

        await page.goto(`/teams/${teams[0].id}`);

        await expect(page.getByText(/members/i).first()).toBeVisible({ timeout: 15000 });
    });

    test('should show share button', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/teams?pageSize=5`);
        const data = await response.json();
        const teams = data.items || data;
        test.skip(!Array.isArray(teams) || teams.length === 0, 'No teams available');

        await page.goto(`/teams/${teams[0].id}`);

        await expect(page.getByRole('button', { name: /share/i })).toBeVisible({ timeout: 15000 });
    });

    test('should show join or manage team action', async ({ authenticatedPage: page }) => {
        const response = await page.request.get(`${BASE_API}/v2/teams?pageSize=5`);
        const data = await response.json();
        const teams = data.items || data;
        test.skip(!Array.isArray(teams) || teams.length === 0, 'No teams available');

        await page.goto(`/teams/${teams[0].id}`);

        // Should show either Join, Request to Join, Manage, or Leave button
        const actionBtn = page.getByRole('button', { name: /join|manage|leave/i }).or(
            page.getByRole('link', { name: /manage/i }),
        );
        await expect(actionBtn.first()).toBeVisible({ timeout: 15000 });
    });
});
