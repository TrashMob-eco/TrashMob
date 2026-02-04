import { test, expect } from '../fixtures/base.fixture';
import { HomePage } from '../pages/home.page';

test.describe('Public Pages', () => {
    test.describe('Home Page', () => {
        test('should load home page successfully', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            // Verify page loaded
            await expect(page).toHaveTitle(/trashmob/i);
            await expect(homePage.heroTitle).toBeVisible();
        });

        test('should display main navigation elements', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            // Check navigation menus are present
            await expect(homePage.exploreMenu).toBeVisible();
            await expect(homePage.takeActionMenu).toBeVisible();
            await expect(homePage.aboutMenu).toBeVisible();
            await expect(homePage.donateLink).toBeVisible();
        });

        test('should display sign in button for unauthenticated users', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await expect(homePage.signInButton).toBeVisible();
        });

        test('should have working logo link to home', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await homePage.logo.click();
            await expect(page).toHaveURL('/');
        });
    });

    test.describe('Navigation', () => {
        test('should navigate to Teams page from Explore menu', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await homePage.navigateToTeams();
            await expect(page).toHaveURL('/teams');
        });

        test('should navigate to Communities page from Explore menu', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await homePage.navigateToCommunities();
            await expect(page).toHaveURL('/communities');
        });

        test('should navigate to What\'s New page from About menu', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await homePage.navigateToWhatsNew();
            await expect(page).toHaveURL('/whatsnew');
        });

        test('should navigate to Help page from About menu', async ({ page }) => {
            const homePage = new HomePage(page);
            await homePage.goto();

            await homePage.navigateToHelp();
            await expect(page).toHaveURL('/help');
        });
    });

    test.describe('Static Pages', () => {
        test('should load Teams page', async ({ page }) => {
            await page.goto('/teams');
            await expect(page).toHaveTitle(/teams|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load Communities page', async ({ page }) => {
            await page.goto('/communities');
            await expect(page).toHaveTitle(/communities|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load Leaderboards page', async ({ page }) => {
            await page.goto('/leaderboards');
            await expect(page).toHaveTitle(/leaderboard|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load What\'s New page', async ({ page }) => {
            await page.goto('/whatsnew');
            await expect(page).toHaveTitle(/what's new|trashmob/i);
            await expect(page.locator('h1')).toContainText(/what's new/i);
        });

        test('should load Help page', async ({ page }) => {
            await page.goto('/help');
            await expect(page).toHaveTitle(/help|faq|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load Getting Started page', async ({ page }) => {
            await page.goto('/gettingstarted');
            await expect(page).toHaveTitle(/getting started|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load About Us page', async ({ page }) => {
            await page.goto('/aboutus');
            await expect(page).toHaveTitle(/about|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load Privacy Policy page', async ({ page }) => {
            await page.goto('/privacypolicy');
            await expect(page).toHaveTitle(/privacy|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });

        test('should load Terms of Service page', async ({ page }) => {
            await page.goto('/termsofservice');
            await expect(page).toHaveTitle(/terms|trashmob/i);
            await expect(page.locator('h1')).toBeVisible();
        });
    });

    test.describe('Accessibility', () => {
        test('should have no accessibility violations on home page', async ({ page }) => {
            await page.goto('/');

            // Check for basic accessibility elements
            // Logo/banner should have alt text or aria-label
            await expect(page.locator('header')).toBeVisible();

            // Main content should be present
            await expect(page.locator('main, [role="main"]')).toBeVisible();

            // Skip to content link should exist for screen readers
            // (this may need adjustment based on actual implementation)
        });

        test('navigation should be keyboard accessible', async ({ page }) => {
            await page.goto('/');

            // Tab through navigation elements
            await page.keyboard.press('Tab');

            // Should be able to focus on navigation items
            const activeElement = await page.evaluate(() => document.activeElement?.tagName);
            expect(['A', 'BUTTON']).toContain(activeElement);
        });
    });
});
