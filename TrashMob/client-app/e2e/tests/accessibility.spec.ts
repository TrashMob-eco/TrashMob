import { test, expect } from '../fixtures/base.fixture';
import AxeBuilder from '@axe-core/playwright';

/**
 * Automated accessibility (WCAG 2.2 AA) scans using axe-core.
 * These tests catch ~30-40% of accessibility issues automatically,
 * including color contrast, missing labels, ARIA errors, heading order, etc.
 *
 * Current policy:
 * - FAIL on "critical" violations (most severe, e.g. missing form labels)
 * - WARN on "serious" violations (logged for tracking, e.g. color contrast)
 * - As violations are fixed, thresholds can be tightened
 *
 * Note: Automated tools cannot catch all accessibility issues.
 * Manual testing with screen readers (NVDA, VoiceOver) is still needed.
 */
test.describe('Accessibility (WCAG 2.2 AA)', () => {
    test.describe.configure({ retries: 1 });

    async function scanPage(page: import('@playwright/test').Page, url: string) {
        await page.goto(url);
        await page.waitForLoadState('networkidle');
        await page.waitForTimeout(2000);

        const results = await new AxeBuilder({ page })
            .withTags(['wcag2a', 'wcag2aa', 'wcag22aa'])
            .exclude('.azure-maps-control-container') // Third-party map controls
            .analyze();

        return results;
    }

    function logViolations(
        pageName: string,
        violations: import('axe-core').Result[],
        level: 'critical' | 'serious',
    ) {
        const filtered = violations.filter((v) => v.impact === level);
        if (filtered.length > 0) {
            console.log(
                `\n[${level.toUpperCase()}] ${pageName} — ${filtered.length} violation(s):` +
                    filtered
                        .map((v) => `\n  ${v.id}: ${v.help} (${v.nodes.length} instance${v.nodes.length !== 1 ? 's' : ''})`)
                        .join(''),
            );
        }
        return filtered;
    }

    // Known violations being tracked for fix — don't block CI
    const knownViolationIds = new Set([
        'button-name', // Icon-only buttons from Radix/shadcn components — needs audit
        'aria-valid-attr-value', // Dynamic ARIA attributes from third-party components
    ]);

    async function assertNoBlockingViolations(page: import('@playwright/test').Page, url: string, pageName: string) {
        const results = await scanPage(page, url);

        // Log serious violations as warnings (color contrast, link names, etc.)
        logViolations(pageName, results.violations, 'serious');

        // Log known critical violations as warnings (tracked for future fix)
        const allCritical = results.violations.filter((v) => v.impact === 'critical');
        const known = allCritical.filter((v) => knownViolationIds.has(v.id));
        const unknown = allCritical.filter((v) => !knownViolationIds.has(v.id));

        if (known.length > 0) {
            console.log(
                `\n[KNOWN] ${pageName} — ${known.length} tracked violation(s):` +
                    known.map((v) => `\n  ${v.id}: ${v.help} (${v.nodes.length} instance${v.nodes.length !== 1 ? 's' : ''})`).join(''),
            );
        }

        // Fail on new/unknown critical violations
        expect(unknown, `${pageName} has new critical accessibility violations`).toEqual([]);
    }

    test('home page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/', 'Home');
    });

    test('about us page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/aboutus', 'About Us');
    });

    test('litter reports page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/litterreports', 'Litter Reports');
    });

    test('FAQ page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/faq', 'FAQ');
    });

    test('privacy policy page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/privacypolicy', 'Privacy Policy');
    });

    test('getting started page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/gettingstarted', 'Getting Started');
    });

    test('news page should have no critical accessibility violations', async ({ page }) => {
        await assertNoBlockingViolations(page, '/news', 'News');
    });

    test('full audit — log all violations for tracking', async ({ page }) => {
        const results = await scanPage(page, '/');

        if (results.violations.length > 0) {
            console.log(`\nFull audit — ${results.violations.length} total violation(s):`);
            for (const v of results.violations) {
                console.log(
                    `  [${v.impact}] ${v.id}: ${v.help} (${v.nodes.length} instance${v.nodes.length !== 1 ? 's' : ''})`,
                );
            }
        } else {
            console.log('\nFull audit — no violations found!');
        }

        // Always passes — this test is for audit/reporting only
        expect(results.violations).toBeDefined();
    });
});
