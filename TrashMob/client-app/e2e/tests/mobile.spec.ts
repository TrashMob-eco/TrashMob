import { test, expect, devices } from '@playwright/test';

// Mobile-specific tests using iPhone 13 viewport settings.
// Destructure out defaultBrowserType so the test uses the project's browser (chromium in CI)
// instead of webkit, which may not be installed.
const { defaultBrowserType: _, ...mobileDevice } = devices['iPhone 13'];
test.use(mobileDevice);

test.describe('Mobile Responsiveness', () => {
    test('should display hamburger menu on mobile', async ({ page }) => {
        await page.goto('/');

        // Hamburger menu button should be visible on mobile
        const menuButton = page.getByRole('button', { name: /toggle menu/i });
        await expect(menuButton).toBeVisible();
    });

    test('should toggle navigation menu on hamburger click', async ({ page }) => {
        await page.goto('/');

        const menuButton = page.getByRole('button', { name: /toggle menu/i });
        await menuButton.click();

        // Navigation items should become visible after clicking hamburger
        await expect(page.getByRole('button', { name: /explore/i })).toBeVisible();
        await expect(page.getByRole('button', { name: /about/i })).toBeVisible();
    });

    test('should display sign in button on mobile', async ({ page }) => {
        await page.goto('/');

        // Open mobile menu first
        const menuButton = page.getByRole('button', { name: /toggle menu/i });
        await menuButton.click();

        await expect(page.getByRole('button', { name: /sign in/i })).toBeVisible();
    });

    test('should render home page sections on mobile', async ({ page }) => {
        await page.goto('/');

        // Key sections should still be visible
        await expect(page.locator('header')).toBeVisible();
        await expect(page.getByRole('heading', { name: /what is a trashmob/i })).toBeVisible();
        await expect(page.getByRole('heading', { name: /events near/i })).toBeVisible();
    });

    test('should load static pages on mobile', async ({ page }) => {
        // Test a few key pages render correctly on mobile
        await page.goto('/aboutus');
        await expect(page.locator('h1')).toBeVisible();

        await page.goto('/faq');
        await expect(page.locator('h1').first()).toBeVisible();

        await page.goto('/contactus');
        await expect(page.locator('h1')).toBeVisible();
    });

    test('should display footer on mobile', async ({ page }) => {
        await page.goto('/');
        await page.locator('footer').scrollIntoViewIfNeeded();
        await expect(page.locator('footer')).toBeVisible();
        await expect(page.locator('footer').getByText(/copyright/i)).toBeVisible();
    });
});
