import { Page, Locator } from '@playwright/test';

/**
 * Page Object for the Home page (/)
 */
export class HomePage {
    readonly page: Page;

    // Header elements
    readonly logo: Locator;
    readonly signInButton: Locator;
    readonly signUpButton: Locator;
    readonly userMenuButton: Locator;

    // Navigation menu items
    readonly exploreMenu: Locator;
    readonly takeActionMenu: Locator;
    readonly aboutMenu: Locator;
    readonly donateLink: Locator;

    // Hero section
    readonly heroTitle: Locator;
    readonly heroSubtitle: Locator;
    readonly getStartedButton: Locator;

    // Events section
    readonly eventsSection: Locator;
    readonly eventCards: Locator;

    // Stats section
    readonly statsSection: Locator;

    constructor(page: Page) {
        this.page = page;

        // Header
        this.logo = page.locator('a[href="/"]').first();
        this.signInButton = page.getByRole('button', { name: /sign in/i });
        this.signUpButton = page.getByRole('button', { name: /sign up|get started/i });
        this.userMenuButton = page.locator('[data-testid="user-menu"], [aria-label="User menu"]');

        // Navigation
        this.exploreMenu = page.getByRole('button', { name: /explore/i });
        this.takeActionMenu = page.getByRole('button', { name: /take action/i });
        this.aboutMenu = page.getByRole('button', { name: /about/i });
        this.donateLink = page.getByRole('link', { name: /donate/i });

        // Hero
        this.heroTitle = page.locator('h1').first();
        this.heroSubtitle = page.locator('[class*="hero"] p, section:first-of-type p').first();
        this.getStartedButton = page.getByRole('link', { name: /get started|join/i });

        // Events
        this.eventsSection = page.locator('#events, [data-testid="events-section"]');
        this.eventCards = page.locator('[data-testid="event-card"], .event-card');

        // Stats
        this.statsSection = page.locator('[data-testid="stats-section"], .stats-section');
    }

    async goto() {
        await this.page.goto('/');
    }

    async isLoaded() {
        await this.heroTitle.waitFor({ state: 'visible' });
    }

    // Navigation helpers
    async openExploreMenu() {
        await this.exploreMenu.click();
        // Wait for menu content to be visible
        await this.page.waitForSelector('[data-radix-menu-content], [role="menu"]', { state: 'visible', timeout: 5000 }).catch(() => {});
    }

    async openTakeActionMenu() {
        await this.takeActionMenu.click();
        await this.page.waitForSelector('[data-radix-menu-content], [role="menu"]', { state: 'visible', timeout: 5000 }).catch(() => {});
    }

    async openAboutMenu() {
        await this.aboutMenu.click();
        await this.page.waitForSelector('[data-radix-menu-content], [role="menu"]', { state: 'visible', timeout: 5000 }).catch(() => {});
    }

    async navigateToEvents() {
        await this.openExploreMenu();
        await this.page.getByRole('link', { name: /^events$/i }).click();
    }

    async navigateToTeams() {
        await this.openExploreMenu();
        await this.page.getByRole('link', { name: /^teams$/i }).click();
    }

    async navigateToCommunities() {
        await this.openExploreMenu();
        await this.page.getByRole('link', { name: /^communities$/i }).click();
    }

    async navigateToCreateEvent() {
        await this.openTakeActionMenu();
        await this.page.getByRole('link', { name: /create event/i }).click();
    }

    async navigateToReportLitter() {
        await this.openTakeActionMenu();
        await this.page.getByRole('link', { name: /report litter/i }).click();
    }

    async navigateToWhatsNew() {
        await this.openAboutMenu();
        await this.page.getByRole('link', { name: /what's new/i }).click();
    }

    async navigateToHelp() {
        await this.openAboutMenu();
        await this.page.getByRole('link', { name: /help.*faq/i }).click();
    }

    async clickSignIn() {
        await this.signInButton.click();
    }

    async clickDonate() {
        await this.donateLink.click();
    }
}
