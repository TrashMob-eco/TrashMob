import { test as base } from '@playwright/test';

/**
 * Base test fixture extending Playwright's test with custom fixtures.
 * Use this for tests that don't require authentication.
 */
export const test = base.extend({
    // Add any custom fixtures here as needed
});

export { expect } from '@playwright/test';
