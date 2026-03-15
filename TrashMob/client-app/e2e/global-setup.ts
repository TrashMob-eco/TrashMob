import { chromium, FullConfig } from '@playwright/test';
import path from 'path';
import fs from 'fs';

const AUTH_DIR = path.join(__dirname, '.auth');
const USER_AUTH_FILE = path.join(AUTH_DIR, 'user.json');

/**
 * Playwright global setup — runs once before all tests.
 * Logs in via Entra External ID and saves auth state for reuse.
 */
async function globalSetup(config: FullConfig) {
    const email = process.env.E2E_USER_EMAIL;
    const password = process.env.E2E_USER_PASSWORD;

    if (!email || !password) {
        console.log('[global-setup] E2E_USER_EMAIL / E2E_USER_PASSWORD not set — skipping auth setup');
        return;
    }

    // Skip if auth state was saved recently (within 30 minutes)
    if (fs.existsSync(USER_AUTH_FILE)) {
        const stats = fs.statSync(USER_AUTH_FILE);
        const ageMinutes = (Date.now() - stats.mtimeMs) / 60_000;
        if (ageMinutes < 30) {
            console.log(`[global-setup] Auth state is ${Math.round(ageMinutes)}m old — reusing`);
            return;
        }
    }

    console.log('[global-setup] Logging in via Entra External ID...');
    fs.mkdirSync(AUTH_DIR, { recursive: true });

    const baseURL = process.env.BASE_URL || config.projects[0]?.use?.baseURL || 'https://dev.trashmob.eco';

    const browser = await chromium.launch();
    const context = await browser.newContext();
    const page = await context.newPage();

    try {
        // Navigate to app
        await page.goto(baseURL, { waitUntil: 'domcontentloaded' });

        // Click Sign In (wait for it to appear)
        await page.getByRole('button', { name: /sign in/i }).waitFor({ state: 'visible', timeout: 30000 });
        await page.getByRole('button', { name: /sign in/i }).click();
        await page.waitForURL(/.*ciamlogin\.com.*/, { timeout: 15000 });

        // Step 1: Email
        await page.fill('input[name="username"]', email);
        await page.click('#usernamePrimaryButton');

        // Step 2: Password (Microsoft login page)
        await page.waitForSelector('#i0118', { timeout: 10000 });
        await page.fill('#i0118', password);
        await page.click('#idSIButton9');

        // Step 3: "Stay signed in?" — click Yes
        const yesBtn = page.getByRole('button', { name: 'Yes' });
        await yesBtn.waitFor({ state: 'visible', timeout: 15000 });
        await yesBtn.click();

        // Step 4: Wait for redirect back to app
        const baseHost = new URL(baseURL).hostname.replace('www.', '');
        await page.waitForURL(new RegExp(`.*${baseHost.replace('.', '\\.')}.*`), { timeout: 30000 });

        // Step 5: Wait for user to be loaded in the app
        await page.waitForSelector('button[aria-label*="Account menu"]', { timeout: 30000 });

        // Step 6: Capture sessionStorage (MSAL stores tokens there)
        // Playwright's storageState only captures localStorage + cookies, not sessionStorage.
        // We manually extract it and store it alongside the auth state.
        const sessionData = await page.evaluate(() => {
            const data: Record<string, string> = {};
            for (let i = 0; i < sessionStorage.length; i++) {
                const key = sessionStorage.key(i);
                if (key) data[key] = sessionStorage.getItem(key) || '';
            }
            return data;
        });

        // Save auth state (cookies + localStorage)
        await context.storageState({ path: USER_AUTH_FILE });

        // Append sessionStorage to the saved state file
        const stateData = JSON.parse(fs.readFileSync(USER_AUTH_FILE, 'utf-8'));
        stateData.sessionStorage = sessionData;
        stateData.sessionStorageOrigin = baseURL;
        fs.writeFileSync(USER_AUTH_FILE, JSON.stringify(stateData, null, 2));

        console.log(`[global-setup] Auth state saved (${Object.keys(sessionData).length} sessionStorage keys)`);
    } catch (error) {
        console.error('[global-setup] Login failed:', error);
        await page.screenshot({ path: path.join(AUTH_DIR, 'login-failure.png') });
        throw error;
    } finally {
        await browser.close();
    }
}

export default globalSetup;
