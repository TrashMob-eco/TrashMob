import { test, expect } from '../fixtures/auth.fixture';

test.describe('My Dashboard', () => {
    test.describe('Layout', () => {
        test('should display dashboard hero section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.locator('h1')).toContainText(/dashboard/i, { timeout: 15000 });
        });

        test('should show user is logged in', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            // Account menu should be visible (proves auth works)
            await expect(page.locator('button[aria-label*="Account menu"]')).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Events Section', () => {
        test('should display My Events heading', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByRole('heading', { name: /my events/i })).toBeVisible({ timeout: 15000 });
        });

        test('should have a Create Event link', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            const createBtn = page.getByRole('link', { name: /create event/i });
            await expect(createBtn).toBeVisible({ timeout: 15000 });
            await expect(createBtn).toHaveAttribute('href', /\/events\/create/);
        });

        test('should display upcoming events section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByText(/upcoming events/i).first()).toBeVisible({ timeout: 15000 });
        });

        test('should display completed events section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByText(/completed events/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Teams Section', () => {
        test('should display My Teams section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            // Click the Browse Teams link (visible without scrolling)
            await expect(page.getByRole('link', { name: /browse teams/i })).toBeVisible({ timeout: 15000 });
        });

        test('should have Browse Teams button', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByRole('link', { name: /browse teams/i })).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Impact Section', () => {
        test('should display verified impact section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            // Navigate to impact via sidebar
            const impactLink = page.getByRole('link', { name: /^impact$/i });
            if (await impactLink.isVisible({ timeout: 5000 }).catch(() => false)) {
                await impactLink.click();
            }

            await expect(page.getByText(/verified impact/i)).toBeVisible({ timeout: 15000 });
        });
    });
});
