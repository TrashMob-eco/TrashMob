/**
 * PRIVO Integration E2E Tests
 *
 * Tests the PRIVO consent management flows against the live dev environment.
 * Uses the authenticated test user and PRIVO INT environment.
 *
 * Prerequisites:
 * - PRIVO INT credentials in dev Key Vault
 * - PRIVO webhook endpoint configured
 * - dev.trashmob.eco whitelisted in PRIVO
 *
 * Note: Tests that require the PRIVO verification widget (external site)
 * cannot be fully automated. Those tests verify the setup and API calls
 * but stop at the redirect point.
 */
import { test, expect } from '../fixtures/auth.fixture';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

test.describe('PRIVO Integration', () => {
    test.describe.configure({ retries: 1 });

    test.describe('API Endpoints', () => {
        test('PRIVO enabled endpoint returns feature flag', async ({ authenticatedPage: page }) => {
            const res = await page.request.get(`${BASE_API}/v2/privo/enabled`);
            expect(res.status()).toBe(200);

            const data = await res.json();
            expect(data).toHaveProperty('enabled');
            expect(typeof data.enabled).toBe('boolean');
        });

        test('verification status returns 200 or 204', async ({ authenticatedPage: page }) => {
            const res = await page.request.get(`${BASE_API}/v2/privo/status`);
            // 200 = has consent record, 204 = no consent record (both valid)
            expect([200, 204]).toContain(res.status());
        });

        test('permissions endpoint returns 200 or 204', async ({ authenticatedPage: page }) => {
            const res = await page.request.get(`${BASE_API}/v2/privo/permissions`);
            // 200 = minor with permissions, 204 = adult or no PRIVO record
            expect([200, 204]).toContain(res.status());
        });

        test('status refresh returns 200 or 204', async ({ authenticatedPage: page }) => {
            const res = await page.request.post(`${BASE_API}/v2/privo/status/refresh`);
            expect([200, 204]).toContain(res.status());
        });
    });

    test.describe('Adult Identity Verification UI', () => {
        test('dashboard shows Identity Verification card', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');

            // Wait for dashboard to load
            await expect(page.getByText(/my dependents/i).first()).toBeVisible({ timeout: 15000 });

            // Should show either "Verify My Identity" or "Identity Verified" depending on state
            const verifyCard = page
                .getByText(/identity verification/i)
                .first()
                .or(page.getByText(/identity verified/i).first());

            await expect(verifyCard).toBeVisible({ timeout: 10000 });
        });

        test('verify button initiates PRIVO flow', async ({ authenticatedPage: page }) => {
            // Skip if user already has a consent record (any status)
            const statusRes = await page.request.get(`${BASE_API}/v2/privo/status`);
            test.skip(statusRes.status() === 200, 'User already has consent record — skip initiation test');

            // Check if PRIVO is enabled
            const enabledRes = await page.request.get(`${BASE_API}/v2/privo/enabled`);
            const enabledData = await enabledRes.json();
            test.skip(!enabledData.enabled, 'PRIVO not enabled on this environment');

            await page.goto('/mydashboard');
            await expect(page.getByText(/my dependents/i).first()).toBeVisible({ timeout: 15000 });

            const verifyBtn = page.getByRole('button', { name: /verify my identity/i });
            const isVisible = await verifyBtn.isVisible({ timeout: 5000 }).catch(() => false);
            test.skip(!isVisible, 'Verify button not visible (may already be verified or pending)');

            // Click and verify the API call succeeds (will redirect to PRIVO)
            const [response] = await Promise.all([
                page.waitForResponse((r) => r.url().includes('/v2/privo/verify') && r.request().method() === 'POST'),
                verifyBtn.click(),
            ]);

            expect(response.status()).toBe(200);
            const consent = await response.json();
            expect(consent).toHaveProperty('id');
            expect(consent).toHaveProperty('consentType', 1); // AdultVerification
            expect(consent).toHaveProperty('status', 1); // Pending
        });
    });

    test.describe('Dependent Consent UI', () => {
        test('dependents card shows consent status for 13+ dependents', async ({ authenticatedPage: page }) => {
            await page.goto('/mydashboard');
            await expect(page.getByText(/my dependents/i).first()).toBeVisible({ timeout: 15000 });

            // Check if user has any dependents
            const dependentsRes = await page.request.get(`${BASE_API}/v2/users/me/dependents`);
            if (dependentsRes.status() !== 200) {
                test.skip(true, 'Cannot fetch dependents');
            }

            // If dependents exist, the card should show
            const depSection = page.getByText(/my dependents/i).first();
            await expect(depSection).toBeVisible();
        });

        test('consent shield button calls PRIVO API for 13+ dependent', async ({ authenticatedPage: page }) => {
            // Check parent is verified (204 = no consent record)
            const statusRes = await page.request.get(`${BASE_API}/v2/privo/status`);
            let isVerified = false;
            if (statusRes.status() === 200) {
                const statusText = await statusRes.text();
                if (statusText) {
                    isVerified = JSON.parse(statusText).status === 2;
                }
            }
            test.skip(!isVerified, 'Parent must be verified first');

            await page.goto('/mydashboard');
            await expect(page.getByText(/my dependents/i).first()).toBeVisible({ timeout: 15000 });

            // Look for the shield (consent) button — indicates a 13+ dependent needs consent
            const shieldBtn = page.locator('button[title="Start parental consent via PRIVO"]').first();
            const hasShield = await shieldBtn.isVisible({ timeout: 5000 }).catch(() => false);
            test.skip(!hasShield, 'No 13+ dependents needing consent');

            // Click and verify API call
            const [response] = await Promise.all([
                page.waitForResponse(
                    (r) => r.url().includes('/v2/privo/consent/child/') && r.request().method() === 'POST',
                ),
                shieldBtn.click(),
            ]);

            expect(response.status()).toBe(200);
            const consent = await response.json();
            expect(consent).toHaveProperty('consentType', 2); // ParentInitiatedChild
        });
    });

    test.describe('Minor Account Restrictions', () => {
        // These tests verify that minor users see restricted UI
        // They require logging in as a minor — use the child test account if available

        test('dashboard dependents section shows correct card based on user type', async ({
            authenticatedPage: page,
        }) => {
            await page.goto('/mydashboard');

            // Wait for page load
            const dashboardContent = page.locator('#dependents, #routes, #newsletters').first();
            await expect(dashboardContent).toBeVisible({ timeout: 15000 });

            // Check user type via API
            const userRes = await page.request.get(`${BASE_API}/v2/users/me`);
            if (userRes.status() !== 200) {
                test.skip(true, 'Cannot fetch current user');
            }

            const user = await userRes.json();

            if (user.isMinor) {
                // Minor should see "Minor Account" card, NOT "Add Dependent"
                await expect(page.getByText(/minor account/i)).toBeVisible({ timeout: 5000 });
                await expect(page.getByRole('button', { name: /add dependent/i })).not.toBeVisible();
            } else {
                // Adult should see dependents management
                await expect(page.getByText(/my dependents/i).first()).toBeVisible({ timeout: 5000 });
            }
        });
    });

    test.describe('PRIVO Callback Page', () => {
        test('callback page renders without auth', async ({ page }) => {
            await page.goto('/privo/callback?status=success');

            await expect(page.getByText(/verification submitted/i)).toBeVisible({ timeout: 10000 });
            await expect(page.getByRole('link', { name: /go to dashboard/i })).toBeVisible();
        });

        test('callback page shows generic message without status param', async ({ page }) => {
            await page.goto('/privo/callback');

            await expect(page.getByText(/verification/i).first()).toBeVisible({ timeout: 10000 });
            await expect(
                page.getByRole('link', { name: /go to dashboard/i }).first(),
            ).toBeVisible();
        });
    });

    test.describe('Webhook Processing', () => {
        test('webhook endpoint rejects missing API key', async ({ request }) => {
            const res = await request.post(`${BASE_API}/v2/webhooks/privo/consent`, {
                data: {
                    id: '00000000-0000-0000-0000-000000000000',
                    timestamp: new Date().toISOString(),
                    sid: 'test-sid',
                    event_types: ['consent_request_created'],
                    consent_identifiers: ['test-consent-id'],
                },
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            expect(res.status()).toBe(401);
        });

        test('webhook endpoint rejects wrong API key', async ({ request }) => {
            const res = await request.post(`${BASE_API}/v2/webhooks/privo/consent`, {
                data: {
                    id: '00000000-0000-0000-0000-000000000000',
                    timestamp: new Date().toISOString(),
                    sid: 'test-sid',
                    event_types: ['consent_request_created'],
                    consent_identifiers: ['test-consent-id'],
                },
                headers: {
                    'Content-Type': 'application/json',
                    'X-Api-Key': 'wrong-key-value',
                },
            });

            expect(res.status()).toBe(401);
        });
    });
});
