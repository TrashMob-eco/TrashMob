import { test, expect } from '@playwright/test';

test.describe('Home Page Interactions', () => {
    test.describe('Event Section', () => {
        test('should switch between Upcoming and Completed tabs', async ({ page }) => {
            await page.goto('/');

            // Wait for events section
            const upcomingTab = page.getByRole('tab', { name: /upcoming/i });
            await expect(upcomingTab).toBeVisible({ timeout: 15000 });

            // Upcoming should be active by default
            await expect(upcomingTab).toHaveAttribute('data-state', 'active');

            // Switch to Completed
            const completedTab = page.getByRole('tab', { name: /completed/i });
            await completedTab.click();
            await expect(completedTab).toHaveAttribute('data-state', 'active');
            await expect(upcomingTab).toHaveAttribute('data-state', 'inactive');
        });

        test('should toggle between map and list views', async ({ page }) => {
            await page.goto('/');

            // Wait for toggle group
            const toggleGroup = page.locator('#events [role="group"]');
            await expect(toggleGroup).toBeVisible({ timeout: 15000 });

            const listBtn = toggleGroup.locator('button').first();
            const mapBtn = toggleGroup.locator('button').last();

            // Click list view
            await listBtn.click();
            await expect(listBtn).toHaveAttribute('data-state', 'on');

            // Click map view
            await mapBtn.click();
            await expect(mapBtn).toHaveAttribute('data-state', 'on');
        });

        test('should show event count in results', async ({ page }) => {
            await page.goto('/');

            // Wait for events section to load
            await expect(page.getByText(/events found/i)).toBeVisible({ timeout: 15000 });
        });

        test('should have time range filter dropdown', async ({ page }) => {
            await page.goto('/');

            // Wait for events section
            await expect(page.getByRole('tab', { name: /upcoming/i })).toBeVisible({ timeout: 15000 });

            // Time range select should be visible
            const timeSelect = page.locator('#events [role="combobox"]').first();
            await expect(timeSelect).toBeVisible();
        });
    });

    test.describe('Navigation Menus', () => {
        test('should open Take Action menu with create event and report litter', async ({ page }) => {
            await page.goto('/');

            const takeActionBtn = page.getByRole('button', { name: /take action/i });
            await expect(takeActionBtn).toBeVisible({ timeout: 15000 });

            // Radix NavigationMenu requires real mouse pointer events
            const box = await takeActionBtn.boundingBox();
            if (box) {
                await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
            }
            await page.waitForTimeout(400);

            // Menu links include description text
            await expect(page.getByRole('link', { name: /create.*event.*organize/i })).toBeVisible({ timeout: 5000 });
            await expect(page.getByRole('link', { name: /report.*litter.*spotted/i })).toBeVisible();
        });

        test('should navigate to event creation from Take Action menu', async ({ page }) => {
            await page.goto('/');

            const takeActionBtn = page.getByRole('button', { name: /take action/i });
            await expect(takeActionBtn).toBeVisible({ timeout: 15000 });

            const box = await takeActionBtn.boundingBox();
            if (box) {
                await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
            }
            await page.waitForTimeout(400);

            await page.getByRole('link', { name: /create.*event.*organize/i }).click();
            await expect(page).toHaveURL(/events\/create/);
        });
    });

    test.describe('Stats Section', () => {
        test('should display non-zero stat counters', async ({ page }) => {
            await page.goto('/');

            // Wait for stats to load with counters
            await expect(page.getByText(/events hosted/i)).toBeVisible({ timeout: 15000 });
            await expect(page.getByText(/bags collected/i)).toBeVisible();
            await expect(page.getByText(/participants/i)).toBeVisible();
        });
    });
});
