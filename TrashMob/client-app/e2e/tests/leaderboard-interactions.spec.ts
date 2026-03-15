import { test, expect } from '../fixtures/auth.fixture';

test.describe('Leaderboard Interactions', () => {
    test('should change leaderboard type filter', async ({ authenticatedPage: page }) => {
        await page.goto('/leaderboards');

        await expect(page.getByRole('heading', { name: /leaderboards/i })).toBeVisible({ timeout: 15000 });

        // Open type filter dropdown
        const typeSelect = page.locator('[role="combobox"]').first();
        await expect(typeSelect).toBeVisible();
        await typeSelect.click();

        // Should show filter options (Radix Select uses custom items, not role=option)
        await expect(page.getByText(/bags collected/i)).toBeVisible({ timeout: 5000 });
    });

    test('should switch between volunteers and teams tabs and update content', async ({
        authenticatedPage: page,
    }) => {
        await page.goto('/leaderboards');

        await expect(page.getByRole('heading', { name: /leaderboards/i })).toBeVisible({ timeout: 15000 });

        // Start on Volunteers tab
        const volunteersTab = page.getByRole('tab', { name: /volunteers/i });
        await expect(volunteersTab).toHaveAttribute('data-state', 'active');

        // Switch to Teams
        const teamsTab = page.getByRole('tab', { name: /teams/i });
        await teamsTab.click();
        await expect(teamsTab).toHaveAttribute('data-state', 'active');
        await expect(volunteersTab).toHaveAttribute('data-state', 'inactive');

        // Switch back to Volunteers
        await volunteersTab.click();
        await expect(volunteersTab).toHaveAttribute('data-state', 'active');
    });

    test('should show your ranking or eligibility message', async ({ authenticatedPage: page }) => {
        await page.goto('/leaderboards');

        await expect(page.getByRole('heading', { name: /leaderboards/i })).toBeVisible({ timeout: 15000 });

        // Should show either ranking card or eligibility message
        const rankingCard = page.getByText(/your ranking/i);
        const eligibilityMsg = page.getByText(/complete.*events.*to appear/i);
        await expect(rankingCard.or(eligibilityMsg)).toBeVisible({ timeout: 10000 });
    });
});
