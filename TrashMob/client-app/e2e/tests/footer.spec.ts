import { test, expect } from '../fixtures/base.fixture';

test.describe('Footer', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/');
        // Scroll to footer
        await page.locator('footer').scrollIntoViewIfNeeded();
    });

    test('should display footer with copyright', async ({ page }) => {
        const footer = page.locator('footer');
        await expect(footer).toBeVisible();

        // Should show copyright text
        await expect(footer.getByText(/copyright/i)).toBeVisible();
        await expect(footer.getByText(/trashmob\.eco/i)).toBeVisible();
    });

    test('should display footer navigation links', async ({ page }) => {
        const footer = page.locator('footer');

        // Key navigation links should be present
        await expect(footer.getByRole('link', { name: /about us/i })).toBeVisible();
        await expect(footer.getByRole('link', { name: /faq/i })).toBeVisible();
        await expect(footer.getByRole('link', { name: /contact us/i })).toBeVisible();
        await expect(footer.getByRole('link', { name: /privacy policy/i })).toBeVisible();
        await expect(footer.getByRole('link', { name: /terms of service/i })).toBeVisible();
    });

    test('should display social media links', async ({ page }) => {
        const footer = page.locator('footer');

        // Social media links should have correct hrefs
        await expect(footer.locator('a[href*="instagram.com"]')).toBeVisible();
        await expect(footer.locator('a[href*="youtube.com"]')).toBeVisible();
        await expect(footer.locator('a[href*="facebook.com"]')).toBeVisible();
        await expect(footer.locator('a[href*="twitter.com"]')).toBeVisible();
    });

    test('should have working footer navigation links', async ({ page }) => {
        const footer = page.locator('footer');

        // Click About Us link in footer and verify navigation
        await footer.getByRole('link', { name: /about us/i }).click();
        await expect(page).toHaveURL('/aboutus');

        // Go back and click FAQ
        await page.goto('/');
        await page.locator('footer').scrollIntoViewIfNeeded();
        await page.locator('footer').getByRole('link', { name: /faq/i }).click();
        await expect(page).toHaveURL('/faq');
    });

    test('should display non-profit disclosure', async ({ page }) => {
        const footer = page.locator('footer');

        // Should mention 501(c)(3) status
        await expect(footer.getByText(/501\(c\)\(3\)/)).toBeVisible();
    });
});
