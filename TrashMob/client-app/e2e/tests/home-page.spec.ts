import { test, expect } from '../fixtures/base.fixture';
import { HomePage } from '../pages/home.page';

test.describe('Home Page Content', () => {
    test.describe('Hero Section', () => {
        test('should display hero section with CTA buttons', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            // Hero should have a "Join us today" or similar CTA link
            await expect(page.getByRole('link', { name: /join us|get started/i })).toBeVisible();
        });

        test('should have working Getting Started link in hero', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            const ctaLink = page.getByRole('link', { name: /join us today/i });
            if (await ctaLink.isVisible()) {
                await ctaLink.click();
                await expect(page).toHaveURL('/gettingstarted');
            }
        });
    });

    test.describe('Stats Section', () => {
        test('should display stats section with counters', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            // Stats section should have a gray background section with stat cards
            const statsSection = page.locator('section.bg-background');
            await expect(statsSection).toBeVisible();

            // Should display stat labels
            await expect(page.getByText('Events Hosted')).toBeVisible();
            await expect(page.getByText('Bags Collected')).toBeVisible();
            await expect(page.getByText('Participants')).toBeVisible();
            await expect(page.getByText('Hours Spent')).toBeVisible();
        });
    });

    test.describe('What is TrashMob Section', () => {
        test('should display introduction section', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            const introSection = page.locator('#introduction');
            await expect(introSection).toBeVisible();

            // Should have the heading
            await expect(page.getByRole('heading', { name: /what is a trashmob/i })).toBeVisible();

            // Should have CTA buttons (scoped to intro section to avoid strict mode violation with Getting Started's "Learn more")
            await expect(introSection.getByRole('link', { name: /learn more/i })).toBeVisible();
        });
    });

    test.describe('Events Section', () => {
        test('should display events section with search', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            // Events section heading
            await expect(page.getByRole('heading', { name: /events near/i })).toBeVisible();

            // Should have tabs for Upcoming and Completed
            await expect(page.getByRole('tab', { name: /upcoming/i })).toBeVisible();
            await expect(page.getByRole('tab', { name: /completed/i })).toBeVisible();
        });
    });

    test.describe('Getting Started Section', () => {
        test('should display getting started section', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await expect(page.getByRole('heading', { name: /getting started/i })).toBeVisible();

            // Should display the three requirements
            await expect(page.getByText('Work gloves')).toBeVisible();
            await expect(page.getByText('A bucket')).toBeVisible();
            await expect(page.getByText('A good attitude')).toBeVisible();
        });
    });
});
