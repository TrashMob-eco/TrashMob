import { test, expect } from '../fixtures/auth.fixture';

test.describe('Event Creation Flow', () => {
    test.describe.configure({ retries: 1 });

    test('should display step 1 (Pick Location) with search and map', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        // Step 1 should be active
        await expect(page.getByText(/pick location/i)).toBeVisible({ timeout: 15000 });

        // Search input should be visible
        await expect(page.getByPlaceholder(/search for address/i).or(page.locator('input[type="text"]').first())).toBeVisible({ timeout: 10000 });
    });

    test('should navigate to step 2 (Edit Detail) and show form fields', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        await expect(page.getByText(/pick location/i)).toBeVisible({ timeout: 15000 });

        // Click Step 2 tab directly
        const step2Tab = page.getByText(/edit detail/i);
        await step2Tab.click();

        // Form fields should be visible
        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });
        await expect(page.locator('textarea[name="description"]').or(page.locator('[name="description"]'))).toBeVisible();
    });

    test('should show event type and visibility dropdowns in step 2', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        await expect(page.getByText(/pick location/i)).toBeVisible({ timeout: 15000 });

        const step2Tab = page.getByText(/edit detail/i);
        await step2Tab.click();

        // Should have event type and visibility selects
        await expect(page.getByText(/event type/i)).toBeVisible({ timeout: 10000 });
        await expect(page.getByText(/visibility/i)).toBeVisible();
    });

    test('should show step 3 (Review) tab', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        await expect(page.getByText(/pick location/i)).toBeVisible({ timeout: 15000 });

        // Review tab should exist
        await expect(page.getByText(/review/i)).toBeVisible();
    });

    test('should validate required fields in step 2', async ({ authenticatedPage: page }) => {
        await page.goto('/events/create');

        await expect(page.getByText(/pick location/i)).toBeVisible({ timeout: 15000 });

        // Go to step 2
        await page.getByText(/edit detail/i).click();
        await expect(page.locator('input[name="name"]')).toBeVisible({ timeout: 10000 });

        // Try to go to step 3 without filling required fields
        await page.getByText(/review/i).click();

        // Should stay on step 2 or show validation errors
        await expect(page.locator('input[name="name"]')).toBeVisible();
    });
});
