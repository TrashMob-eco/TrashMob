import { chromium, FullConfig, Browser, BrowserContext, Page } from '@playwright/test';
import path from 'path';
import fs from 'fs';

const AUTH_DIR = path.join(__dirname, '.auth');
const USER_AUTH_FILE = path.join(AUTH_DIR, 'user.json');
const ADMIN_AUTH_FILE = path.join(AUTH_DIR, 'admin.json');

/**
 * Logs in a single user via Entra External ID and saves auth state.
 */
async function loginAndSaveState(
    browser: Browser,
    baseURL: string,
    email: string,
    password: string,
    authFile: string,
    label: string,
): Promise<void> {
    // Skip if auth state was saved recently (within 30 minutes)
    if (fs.existsSync(authFile)) {
        const stats = fs.statSync(authFile);
        const ageMinutes = (Date.now() - stats.mtimeMs) / 60_000;
        if (ageMinutes < 30) {
            console.log(`[global-setup] ${label} auth state is ${Math.round(ageMinutes)}m old — reusing`);
            return;
        }
    }

    console.log(`[global-setup] Logging in ${label} via Entra External ID...`);

    const context = await browser.newContext();
    const page = await context.newPage();

    try {
        // Navigate to app
        await page.goto(baseURL, { waitUntil: 'domcontentloaded' });

        // Click Sign In
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

        // Step 5: Wait for user to be loaded
        await page.waitForSelector('button[aria-label*="Account menu"]', { timeout: 30000 });

        // Step 6: Capture sessionStorage (MSAL stores tokens there)
        const sessionData = await page.evaluate(() => {
            const data: Record<string, string> = {};
            for (let i = 0; i < sessionStorage.length; i++) {
                const key = sessionStorage.key(i);
                if (key) data[key] = sessionStorage.getItem(key) || '';
            }
            return data;
        });

        // Save auth state (cookies + localStorage)
        await context.storageState({ path: authFile });

        // Append sessionStorage to the saved state file
        const stateData = JSON.parse(fs.readFileSync(authFile, 'utf-8'));
        stateData.sessionStorage = sessionData;
        stateData.sessionStorageOrigin = baseURL;
        fs.writeFileSync(authFile, JSON.stringify(stateData, null, 2));

        console.log(`[global-setup] ${label} auth state saved (${Object.keys(sessionData).length} sessionStorage keys)`);
    } catch (error) {
        console.error(`[global-setup] ${label} login failed:`, error);
        await page.screenshot({ path: path.join(AUTH_DIR, `${label}-login-failure.png`) });
        throw error;
    } finally {
        await context.close();
    }
}

/**
 * Playwright global setup — runs once before all tests.
 * Logs in standard user and admin via Entra External ID and saves auth state.
 */
async function globalSetup(config: FullConfig) {
    fs.mkdirSync(AUTH_DIR, { recursive: true });

    const baseURL = process.env.BASE_URL || config.projects[0]?.use?.baseURL || 'https://dev.trashmob.eco';
    const browser = await chromium.launch();

    try {
        // Login standard user
        const userEmail = process.env.E2E_USER_EMAIL;
        const userPassword = process.env.E2E_USER_PASSWORD;
        if (userEmail && userPassword) {
            await loginAndSaveState(browser, baseURL, userEmail, userPassword, USER_AUTH_FILE, 'user');
        } else {
            console.log('[global-setup] E2E_USER_EMAIL / E2E_USER_PASSWORD not set — skipping user auth');
        }

        // Login admin user
        const adminEmail = process.env.E2E_ADMIN_EMAIL;
        const adminPassword = process.env.E2E_ADMIN_PASSWORD;
        if (adminEmail && adminPassword) {
            await loginAndSaveState(browser, baseURL, adminEmail, adminPassword, ADMIN_AUTH_FILE, 'admin');
        } else {
            console.log('[global-setup] E2E_ADMIN_EMAIL / E2E_ADMIN_PASSWORD not set — skipping admin auth');
        }
    } finally {
        await browser.close();
    }
}

export default globalSetup;
