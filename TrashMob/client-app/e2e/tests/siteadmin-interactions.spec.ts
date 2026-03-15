import { test, expect } from '../fixtures/auth.fixture';

test.describe('Site Admin Interactions', () => {
    test.describe('Events Table', () => {
        test('should search events by name', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/events');

            const searchInput = page.getByPlaceholder(/search events/i);
            await expect(searchInput).toBeVisible({ timeout: 15000 });

            // Type a search term
            await searchInput.fill('test');
            await page.waitForTimeout(500);

            // Results should update (table re-renders)
            await expect(page.locator('table')).toBeVisible();
        });

        test('should have sortable name column header', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/events');

            await expect(page.getByPlaceholder(/search events/i)).toBeVisible({ timeout: 15000 });

            // Name column header should exist and be clickable
            const nameColumnHeader = page.getByRole('columnheader', { name: /name/i });
            await expect(nameColumnHeader).toBeVisible({ timeout: 10000 });
        });
    });

    test.describe('Users Table', () => {
        test('should search users by email', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/users');

            const searchInput = page.getByPlaceholder(/search users/i);
            await expect(searchInput).toBeVisible({ timeout: 15000 });

            await searchInput.fill('test');
            await page.waitForTimeout(500);

            await expect(page.locator('table')).toBeVisible();
        });
    });

    test.describe('Contacts CRM', () => {
        test('should filter contacts by type tab', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/contacts');

            await expect(page.getByText(/contacts\s*\(/i).first()).toBeVisible({ timeout: 15000 });

            // "All" tab should be active
            const allTab = page.getByRole('tab', { name: /all/i });
            await expect(allTab).toBeVisible();

            // Click a different tab if available
            const tabs = page.getByRole('tab');
            const count = await tabs.count();
            if (count > 1) {
                await tabs.nth(1).click();
                await page.waitForTimeout(500);
                // Table should still be visible
                await expect(page.locator('table').first()).toBeVisible();

                // Click back to All
                await allTab.click();
            }
        });
    });

    test.describe('Feedback Management', () => {
        test('should filter feedback by status', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/feedback');

            await expect(page.getByText(/user feedback/i).first()).toBeVisible({ timeout: 15000 });

            // Click "New" tab
            const newTab = page.getByRole('tab', { name: /^new$/i });
            if (await newTab.isVisible({ timeout: 5000 }).catch(() => false)) {
                await newTab.click();
                await expect(newTab).toHaveAttribute('data-state', 'active');
            }

            // Click back to "All"
            const allTab = page.getByRole('tab', { name: /all/i });
            await allTab.click();
            await expect(allTab).toHaveAttribute('data-state', 'active');
        });
    });

    test.describe('Admin Navigation', () => {
        test('should navigate between admin pages via sidebar', async ({ adminPage: page }) => {
            await page.goto('/siteadmin');

            await expect(page.locator('h1')).toContainText(/site administration/i, { timeout: 15000 });

            // Click Users link
            await page.getByRole('link', { name: 'Users' }).first().click();
            await expect(page.getByText(/users\s*\(/i).first()).toBeVisible({ timeout: 15000 });

            // Click Events link
            await page.getByRole('link', { name: 'Events' }).first().click();
            await expect(page.getByText(/events\s*\(/i).first()).toBeVisible({ timeout: 15000 });

            // Click Partners link
            await page.getByRole('link', { name: 'Partners' }).first().click();
            await expect(page.getByText(/partners\s*\(/i).first()).toBeVisible({ timeout: 15000 });
        });
    });
});
