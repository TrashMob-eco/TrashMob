import { test, expect } from '../fixtures/auth.fixture';

test.describe('Team Creation Page', () => {
    test.describe.configure({ retries: 1 });

    test('should display team creation form', async ({ authenticatedPage: page }) => {
        await page.goto('/teams/create');

        await expect(page.locator('h1')).toContainText(/create.*team/i, { timeout: 15000 });

        // Form fields should be visible
        await expect(page.locator('input[name="name"]')).toBeVisible();
    });

    test('should have name and description fields', async ({ authenticatedPage: page }) => {
        await page.goto('/teams/create');

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 15000 });
        await expect(page.locator('textarea[name="description"]')).toBeVisible();
    });

    test('should have public and approval toggle labels', async ({ authenticatedPage: page }) => {
        await page.goto('/teams/create');

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 15000 });

        await expect(page.getByText('Public Team', { exact: true })).toBeVisible({ timeout: 10000 });
        await expect(page.getByText('Require Approval', { exact: true })).toBeVisible({ timeout: 5000 });
    });

    test('should have Create Team and Cancel buttons', async ({ authenticatedPage: page }) => {
        await page.goto('/teams/create');

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 15000 });

        await expect(page.getByRole('button', { name: /create team/i })).toBeVisible();
        await expect(page.getByRole('button', { name: /cancel/i }).or(page.getByRole('link', { name: /cancel/i }))).toBeVisible();
    });

    test('should validate required team name', async ({ authenticatedPage: page }) => {
        await page.goto('/teams/create');

        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 15000 });

        // Try to submit without filling name
        await page.getByRole('button', { name: /create team/i }).click();

        // Should show validation error or stay on page
        await expect(page).toHaveURL(/teams\/create/);
    });
});
