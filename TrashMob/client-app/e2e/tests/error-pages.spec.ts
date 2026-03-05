import { test, expect } from '../fixtures/base.fixture';

test.describe('Error Pages', () => {
    test('should display 404 page for non-existent routes', async ({ page }) => {
        await page.goto('/this-page-does-not-exist');

        // Should display 404 heading
        await expect(page.getByText('404')).toBeVisible();
        await expect(page.getByText('Page Not Found')).toBeVisible();

        // Should show the attempted path
        await expect(page.getByText('/this-page-does-not-exist')).toBeVisible();
    });

    test('should have Back to Home link on 404 page', async ({ page }) => {
        await page.goto('/nonexistent-route');

        const homeLink = page.getByRole('link', { name: /back to home/i });
        await expect(homeLink).toBeVisible();

        await homeLink.click();
        await expect(page).toHaveURL('/');
    });

    test('should have Go Back button on 404 page', async ({ page }) => {
        // Navigate to home first, then to a non-existent page
        await page.goto('/');
        await page.goto('/nonexistent-route');

        await expect(page.getByRole('button', { name: /go back/i })).toBeVisible();
    });

    test('should have contact link on 404 page', async ({ page }) => {
        await page.goto('/nonexistent-route');

        const contactLink = page.getByRole('link', { name: /let us know/i });
        await expect(contactLink).toBeVisible();

        await contactLink.click();
        await expect(page).toHaveURL('/contactus');
    });
});
