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

    // Navigation helpers - Radix NavigationMenu requires real mouse events to open.
    // Playwright's .hover() doesn't always trigger the pointer events Radix listens for,
    // but mouse.move to the element center does.
    private async hoverNavTrigger(trigger: Locator) {
        const box = await trigger.boundingBox();
        if (box) {
            await this.page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
        }
        await this.page.waitForTimeout(400); // Allow animation
    }

    async openExploreMenu() {
        await this.hoverNavTrigger(this.exploreMenu);
    }

    async openTakeActionMenu() {
        await this.hoverNavTrigger(this.takeActionMenu);
    }

    async openAboutMenu() {
        await this.hoverNavTrigger(this.aboutMenu);
    }

    async navigateToEvents() {
        await this.openExploreMenu();
        await this.page.getByRole('link', { name: 'Events' }).click();
    }

    async navigateToTeams() {
        await this.openExploreMenu();
        // Use the nav menu link specifically (contains description text)
        await this.page.getByRole('link', { name: /Teams.*join.*team/i }).click();
    }

    async navigateToCommunities() {
        await this.openExploreMenu();
        await this.page.getByRole('link', { name: /Communities.*community programs/i }).click();
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
        await this.page.getByRole('link', { name: 'Help' }).click();
    }

    async navigateToFaq() {
        await this.openAboutMenu();
        // Use more specific locator to avoid footer FAQ link
        await this.page.getByRole('link', { name: /FAQ.*frequently/i }).click();
    }

    async clickSignIn() {
        await this.signInButton.click();
    }

    async clickDonate() {
        await this.donateLink.click();
    }
}
