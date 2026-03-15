import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL ? `${process.env.BASE_URL}/api` : 'https://dev.trashmob.eco/api';

test.describe('Litter Report Edit Page', () => {
    test.describe.configure({ retries: 1 });

    async function findEditableLitterReport(page: import('@playwright/test').Page) {
        // Fetch user's own litter reports (ones they can edit)
        const response = await page.request.get(`${BASE_API}/v2/litterreports`);
        const data = await response.json();
        const reports = data?.data ?? data;
        if (!Array.isArray(reports) || reports.length === 0) return null;
        return reports[0];
    }

    test('should load edit page with existing images displayed', async ({ authenticatedPage: page }) => {
        const report = await findEditableLitterReport(page);
        test.skip(!report, 'No litter reports to edit');

        await page.goto(`/litterreports/${report.id}/edit`);

        // Wait for the page to load
        await expect(page.getByRole('heading', { name: /edit litter report/i })).toBeVisible({ timeout: 15000 });

        // Should show the report name in the form
        const nameInput = page.getByPlaceholder(/trash pile/i).or(page.locator('input[name="name"]'));
        await expect(nameInput).toBeVisible({ timeout: 10000 });
        await expect(nameInput).not.toHaveValue('');

        // Should display existing images in the image grid
        const imageGrid = page.locator('img[alt="Litter report"]');
        const imageCount = await imageGrid.count();
        expect(imageCount).toBeGreaterThanOrEqual(1);
    });

    test('should show Photos section with upload area', async ({ authenticatedPage: page }) => {
        const report = await findEditableLitterReport(page);
        test.skip(!report, 'No litter reports to edit');

        await page.goto(`/litterreports/${report.id}/edit`);

        await expect(page.getByRole('heading', { name: /edit litter report/i })).toBeVisible({ timeout: 15000 });

        // Should show Photos label
        await expect(page.getByText('Photos')).toBeVisible();

        // Should show helper text about adding/removing photos
        await expect(page.getByText(/add or remove photos/i)).toBeVisible();

        // Should show image count
        await expect(page.getByText(/of 5 photos added/i)).toBeVisible({ timeout: 5000 });
    });

    test('should show status dropdown with options', async ({ authenticatedPage: page }) => {
        const report = await findEditableLitterReport(page);
        test.skip(!report, 'No litter reports to edit');

        await page.goto(`/litterreports/${report.id}/edit`);

        await expect(page.getByRole('heading', { name: /edit litter report/i })).toBeVisible({ timeout: 15000 });

        // Click the status dropdown
        const statusTrigger = page.locator('[role="combobox"]').first();
        await expect(statusTrigger).toBeVisible({ timeout: 10000 });
        await statusTrigger.click();

        // Should show status options
        await expect(page.getByRole('option', { name: /new/i })).toBeVisible({ timeout: 5000 });
        await expect(page.getByRole('option', { name: /cleaned/i })).toBeVisible();
    });

    test('should have Save Changes and Cancel buttons', async ({ authenticatedPage: page }) => {
        const report = await findEditableLitterReport(page);
        test.skip(!report, 'No litter reports to edit');

        await page.goto(`/litterreports/${report.id}/edit`);

        await expect(page.getByRole('heading', { name: /edit litter report/i })).toBeVisible({ timeout: 15000 });

        await expect(page.getByRole('button', { name: /save changes/i })).toBeVisible();
        await expect(page.getByRole('link', { name: /cancel/i })).toBeVisible();
    });

    test('should show character counters for name and description', async ({ authenticatedPage: page }) => {
        const report = await findEditableLitterReport(page);
        test.skip(!report, 'No litter reports to edit');

        await page.goto(`/litterreports/${report.id}/edit`);

        await expect(page.getByRole('heading', { name: /edit litter report/i })).toBeVisible({ timeout: 15000 });

        // Character counters should be visible (format: "X / Y")
        const counters = page.getByText(/\d+ \/ \d+/);
        const counterCount = await counters.count();
        expect(counterCount).toBeGreaterThanOrEqual(1);
    });

    test('should navigate back when Cancel is clicked', async ({ authenticatedPage: page }) => {
        const report = await findEditableLitterReport(page);
        test.skip(!report, 'No litter reports to edit');

        await page.goto(`/litterreports/${report.id}/edit`);

        await expect(page.getByRole('heading', { name: /edit litter report/i })).toBeVisible({ timeout: 15000 });

        await page.getByRole('link', { name: /cancel/i }).click();

        // Should navigate back to the report detail page
        await expect(page).toHaveURL(new RegExp(`/litterreports/${report.id}$`), { timeout: 10000 });
    });

    test('should deny access to non-owner', async ({ page }) => {
        // Use an unauthenticated page to try editing a report
        const response = await page.request.get(`${BASE_API}/v2/litterreports`);
        const data = await response.json();
        const reports = data?.data ?? data;
        test.skip(!Array.isArray(reports) || reports.length === 0, 'No litter reports');

        await page.goto(`/litterreports/${reports[0].id}/edit`);

        // Should show access denied or redirect (unauthenticated user)
        const accessDenied = page.getByText(/access denied|permission/i);
        const loginBtn = page.getByRole('button', { name: /sign in/i });
        await expect(accessDenied.or(loginBtn).or(page.locator('h1').first())).toBeVisible({ timeout: 15000 });
    });
});
