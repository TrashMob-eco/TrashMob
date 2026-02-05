import { Page } from '@playwright/test';

/**
 * Generate a unique test identifier for creating unique test data
 */
export function uniqueId(): string {
    return `e2e-${Date.now()}-${Math.random().toString(36).substring(2, 8)}`;
}

/**
 * Generate a future date string in YYYY-MM-DD format
 * @param daysFromNow - Number of days in the future
 */
export function futureDate(daysFromNow: number = 7): string {
    const date = new Date();
    date.setDate(date.getDate() + daysFromNow);
    return date.toISOString().split('T')[0];
}

/**
 * Generate a past date string in YYYY-MM-DD format
 * @param daysAgo - Number of days in the past
 */
export function pastDate(daysAgo: number = 7): string {
    const date = new Date();
    date.setDate(date.getDate() - daysAgo);
    return date.toISOString().split('T')[0];
}

/**
 * Wait for network requests to settle
 * Useful after actions that trigger API calls
 */
export async function waitForNetworkIdle(page: Page, timeout: number = 5000): Promise<void> {
    await page.waitForLoadState('networkidle', { timeout });
}

/**
 * Wait for a toast notification to appear
 */
export async function waitForToast(page: Page, textPattern?: RegExp): Promise<void> {
    const toastSelector = '[data-testid="toast"], [role="status"], .toast';
    await page.locator(toastSelector).waitFor({ state: 'visible' });

    if (textPattern) {
        await page.locator(toastSelector).filter({ hasText: textPattern }).waitFor({ state: 'visible' });
    }
}

/**
 * Dismiss any visible toast notifications
 */
export async function dismissToasts(page: Page): Promise<void> {
    const closeButtons = page.locator('[data-testid="toast"] button[aria-label*="close"], .toast button');
    const count = await closeButtons.count();
    for (let i = 0; i < count; i++) {
        await closeButtons.nth(i).click();
    }
}

/**
 * Check if an element is in the viewport
 */
export async function isInViewport(page: Page, selector: string): Promise<boolean> {
    return page.evaluate((sel) => {
        const element = document.querySelector(sel);
        if (!element) return false;

        const rect = element.getBoundingClientRect();
        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
        );
    }, selector);
}

/**
 * Scroll to an element
 */
export async function scrollToElement(page: Page, selector: string): Promise<void> {
    await page.locator(selector).scrollIntoViewIfNeeded();
}

/**
 * Take a screenshot with a descriptive name
 */
export async function takeScreenshot(page: Page, name: string): Promise<void> {
    await page.screenshot({ path: `test-results/screenshots/${name}.png`, fullPage: true });
}
