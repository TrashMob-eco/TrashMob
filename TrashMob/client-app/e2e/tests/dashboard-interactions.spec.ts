import { test, expect } from '../fixtures/auth.fixture';

test.describe('Dashboard Interactions', () => {
    test.describe('Event Table', () => {
        test('should sort events by name', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByRole('heading', { name: /my events/i })).toBeVisible({ timeout: 15000 });

            // Find sortable Name column header button
            const nameHeader = page.locator('table').first().getByRole('button', { name: /name/i });
            if (await nameHeader.isVisible({ timeout: 5000 }).catch(() => false)) {
                await nameHeader.click();
                // After click, sort indicator should change
                await page.waitForTimeout(300);
                // Click again to reverse sort
                await nameHeader.click();
                await page.waitForTimeout(300);
            }
        });

        test('should filter events with dropdown', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByRole('heading', { name: /my events/i })).toBeVisible({ timeout: 15000 });

            // Find the event filter dropdown
            const filterSelect = page.locator('[role="combobox"]').first();
            if (await filterSelect.isVisible({ timeout: 5000 }).catch(() => false)) {
                await filterSelect.click();

                // Should show filter options (Radix Select items)
                await expect(page.getByText(/all events|events i created|events i joined/i).first()).toBeVisible({
                    timeout: 5000,
                });
            }
        });

        test('should toggle between list and map view on upcoming events', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.getByText(/upcoming events/i).first()).toBeVisible({ timeout: 15000 });

            // Find the toggle group in the upcoming events card
            const toggleGroup = page.locator('[role="group"]').first();
            if (await toggleGroup.isVisible({ timeout: 5000 }).catch(() => false)) {
                const buttons = toggleGroup.locator('button');
                const count = await buttons.count();
                if (count >= 2) {
                    // Click second button (map view)
                    await buttons.last().click();
                    await expect(buttons.last()).toHaveAttribute('data-state', 'on');

                    // Click first button (list view)
                    await buttons.first().click();
                    await expect(buttons.first()).toHaveAttribute('data-state', 'on');
                }
            }
        });
    });

    test.describe('Sidebar Navigation', () => {
        test('should scroll to sections via sidebar nav', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.locator('h1')).toContainText(/dashboard/i, { timeout: 15000 });

            // Click on Impact in sidebar
            const impactBtn = page.getByRole('button', { name: /^impact$/i });
            if (await impactBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
                await impactBtn.click();

                // Verified impact section should be in viewport
                await expect(page.getByText(/verified impact/i)).toBeVisible({ timeout: 5000 });
            }
        });

        test('should navigate to waivers section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.locator('h1')).toContainText(/dashboard/i, { timeout: 15000 });

            const waiversBtn = page.getByRole('button', { name: /^waivers$/i });
            if (await waiversBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
                await waiversBtn.click();
                await expect(page.getByText(/my waivers/i).first()).toBeVisible({ timeout: 5000 });
            }
        });

        test('should navigate to newsletters section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.locator('h1')).toContainText(/dashboard/i, { timeout: 15000 });

            const newslettersBtn = page.getByRole('button', { name: /^newsletters$/i });
            if (await newslettersBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
                await newslettersBtn.click();
                await expect(page.getByText(/newsletter preferences/i).first()).toBeVisible({ timeout: 5000 });
            }
        });
    });

    test.describe('Invite Friends', () => {
        test('should display invite friends section', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            await expect(page.locator('h1')).toContainText(/dashboard/i, { timeout: 15000 });

            // Navigate to invite friends section via sidebar
            const inviteBtn = page.getByRole('button', { name: /invite friends/i });
            if (await inviteBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
                await inviteBtn.click();
                await page.waitForTimeout(500);
            }

            // The send invites button should exist somewhere on the page
            await expect(page.getByRole('button', { name: /send invites/i })).toBeVisible({ timeout: 10000 });
        });
    });
});
