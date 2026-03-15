import { test, expect } from '../fixtures/base.fixture';
import AxeBuilder from '@axe-core/playwright';

/**
 * Automated accessibility (WCAG 2.2 AA) scans using axe-core.
 * These tests catch ~30-40% of accessibility issues automatically,
 * including color contrast, missing labels, ARIA errors, heading order, etc.
 *
 * Note: Automated tools cannot catch all accessibility issues.
 * Manual testing with screen readers (NVDA, VoiceOver) is still needed.
 */
test.describe('Accessibility (WCAG 2.2 AA)', () => {
    test.describe.configure({ retries: 1 });

    async function scanPage(page: import('@playwright/test').Page, url: string) {
        await page.goto(url);
        // Wait for content to load
        await page.waitForLoadState('networkidle');
        // Give dynamic content time to render
        await page.waitForTimeout(2000);

        const results = await new AxeBuilder({ page })
            .withTags(['wcag2a', 'wcag2aa', 'wcag22aa'])
            .exclude('.azure-maps-control-container') // Exclude third-party map controls
            .analyze();

        return results;
    }

    test('home page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');

        if (critical.length > 0) {
            const summary = critical
                .map((v) => `[${v.impact}] ${v.id}: ${v.description} (${v.nodes.length} instances)`)
                .join('\n');
            console.log('Accessibility violations:\n' + summary);
        }

        expect(critical).toEqual([]);
    });

    test('about us page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/aboutus');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');
        expect(critical).toEqual([]);
    });

    test('litter reports page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/litterreports');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');
        expect(critical).toEqual([]);
    });

    test('FAQ page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/faq');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');
        expect(critical).toEqual([]);
    });

    test('privacy policy page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/privacypolicy');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');
        expect(critical).toEqual([]);
    });

    test('getting started page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/gettingstarted');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');
        expect(critical).toEqual([]);
    });

    test('news page should have no critical accessibility violations', async ({ page }) => {
        const results = await scanPage(page, '/news');

        const critical = results.violations.filter((v) => v.impact === 'critical' || v.impact === 'serious');
        expect(critical).toEqual([]);
    });

    test('home page should report all violations for audit', async ({ page }) => {
        const results = await scanPage(page, '/');

        // Log all violations (including minor/moderate) for tracking
        if (results.violations.length > 0) {
            console.log(`\nTotal violations: ${results.violations.length}`);
            for (const violation of results.violations) {
                console.log(`  [${violation.impact}] ${violation.id}: ${violation.help} (${violation.nodes.length} instances)`);
            }
        }

        // This test always passes — it's for audit/reporting purposes
        // The other tests enforce zero critical/serious violations
        expect(results.violations).toBeDefined();
    });
});
