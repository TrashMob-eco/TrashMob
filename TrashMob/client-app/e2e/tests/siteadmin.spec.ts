import { test, expect } from '../fixtures/auth.fixture';

test.describe('Site Administration', () => {
    test.describe('Layout & Navigation', () => {
        test('should display Site Administration heading', async ({ adminPage: page }) => {
            await page.goto('/siteadmin');

            await expect(page.locator('h1')).toContainText(/site administration/i, { timeout: 15000 });
        });

        test('should show admin sidebar navigation groups', async ({ adminPage: page }) => {
            await page.goto('/siteadmin');

            await expect(page.locator('h1')).toContainText(/site administration/i, { timeout: 15000 });

            // Data Management group links
            await expect(page.getByRole('link', { name: 'Users' }).first()).toBeVisible();
            await expect(page.getByRole('link', { name: 'Events' }).first()).toBeVisible();

            // Moderation group links
            await expect(page.getByRole('link', { name: /user feedback/i })).toBeVisible();
        });
    });

    test.describe('Events Management', () => {
        test('should load events page with table headers', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/events');

            await expect(page.getByText(/events\s*\(/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('columnheader', { name: /name/i })).toBeVisible();
            await expect(page.getByRole('columnheader', { name: /status/i })).toBeVisible();
            await expect(page.getByRole('columnheader', { name: /date/i }).first()).toBeVisible();
        });

        test('should have search box', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/events');

            await expect(page.getByPlaceholder(/search events/i)).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Users Management', () => {
        test('should load users page with table headers', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/users');

            await expect(page.getByText(/users\s*\(/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('columnheader', { name: /username/i })).toBeVisible();
            await expect(page.getByRole('columnheader', { name: /email/i })).toBeVisible();
        });

        test('should have search box', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/users');

            await expect(page.getByPlaceholder(/search users/i)).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Partners Management', () => {
        test('should load partners page with table', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/partners');

            await expect(page.getByText(/partners\s*\(/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('columnheader', { name: /name/i })).toBeVisible();
        });
    });

    test.describe('Partner Requests', () => {
        test('should load partner requests page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/partner-requests');

            await expect(page.getByText(/partner requests/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Litter Reports', () => {
        test('should load litter reports page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/litter-reports');

            // Wait for the main content heading (not sidebar nav text)
            await expect(page.locator('main, [role="main"]').first().getByText(/litter reports/i)).toBeVisible({ timeout: 30000 });
        });
    });

    test.describe('Teams', () => {
        test('should load teams page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/teams');

            // Wait for the main content heading (not sidebar nav text)
            await expect(page.locator('main, [role="main"]').first().getByText(/teams/i)).toBeVisible({ timeout: 30000 });
        });
    });

    test.describe('Contacts / CRM', () => {
        test('should load contacts page with tabs and add button', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/contacts');

            await expect(page.getByText(/contacts\s*\(/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('tab', { name: /all/i })).toBeVisible();
            await expect(page.getByRole('link', { name: /add contact/i })).toBeVisible();
        });
    });

    test.describe('Contact Tags', () => {
        test('should load contact tags page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/contact-tags');

            await expect(page.getByText(/contact tags/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Waivers', () => {
        test('should load waivers page with compliance link', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/waivers');

            await expect(page.getByText(/waivers/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('link', { name: /compliance/i })).toBeVisible();
        });
    });

    test.describe('User Feedback', () => {
        test('should load feedback page with status tabs', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/feedback');

            await expect(page.getByText(/user feedback/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('tab', { name: /all/i })).toBeVisible();
        });
    });

    test.describe('Newsletters', () => {
        test('should load newsletters page with create button', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/newsletters');

            await expect(page.getByText(/newsletters/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByRole('button', { name: /create newsletter/i })).toBeVisible();
        });
    });

    test.describe('Email Templates', () => {
        test('should load email templates page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/email-templates');

            await expect(page.getByText(/email templates/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Content Management', () => {
        test('should load content page with editable areas', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/content');

            await expect(page.getByText(/content management/i).first()).toBeVisible({ timeout: 15000 });
            await expect(page.getByText(/hero section/i)).toBeVisible({ timeout: 10000 });
            await expect(page.getByRole('button', { name: /open cms admin/i })).toBeVisible({ timeout: 10000 });
        });
    });

    test.describe('Bulk Invites', () => {
        test('should load bulk invites page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/invites');

            await expect(page.getByText(/invit/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Photo Moderation', () => {
        test('should load photo moderation page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/photo-moderation');

            await expect(page.getByText(/photo moderation/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Donations', () => {
        test('should load donations page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/donations');

            await expect(page.getByText(/donations/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Grants', () => {
        test('should load grants page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/grants');

            await expect(page.getByText(/grants/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Prospects', () => {
        test('should load prospects page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/prospects');

            await expect(page.getByText(/prospects/i).first()).toBeVisible({ timeout: 15000 });
        });
    });

    test.describe('Job Opportunities', () => {
        test('should load job opportunities page', async ({ adminPage: page }) => {
            await page.goto('/siteadmin/job-opportunities');

            await expect(page.getByText(/job opportunities/i).first()).toBeVisible({ timeout: 15000 });
        });
    });
});
