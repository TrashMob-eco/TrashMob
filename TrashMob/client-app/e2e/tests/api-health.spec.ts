import { test, expect } from '@playwright/test';

const BASE_API = process.env.BASE_URL
    ? `${process.env.BASE_URL}/api`
    : 'https://dev.trashmob.eco/api';

test.describe('API Health Checks', () => {
    test('v2 config endpoint returns valid config', async ({ request }) => {
        const res = await request.get(`${BASE_API}/v2/config`);
        expect(res.status()).toBe(200);

        const data = await res.json();
        expect(data).toHaveProperty('authProvider');
        expect(data).toHaveProperty('azureAdEntra');
        expect(data.azureAdEntra).toHaveProperty('clientId');
    });

    test('v2 stats endpoint returns stats', async ({ request }) => {
        const res = await request.get(`${BASE_API}/v2/stats`);
        expect(res.status()).toBe(200);

        const data = await res.json();
        expect(data).toHaveProperty('totalEvents');
        expect(data).toHaveProperty('totalParticipants');
        expect(typeof data.totalEvents).toBe('number');
    });

    test('v2 Google Maps key endpoint returns a key', async ({ request }) => {
        test.info().annotations.push({ type: 'note', description: 'Requires MapsV2Controller googlemapkey endpoint' });
        const res = await request.get(`${BASE_API}/v2/maps/googlemapkey`);
        expect(res.status()).toBe(200);

        const key = await res.text();
        // Key should be a non-empty string (typically starts with "AIza")
        expect(key.length).toBeGreaterThan(0);
        expect(key).not.toContain('error');
    });

    test('v2 events active endpoint returns array', async ({ request }) => {
        const res = await request.get(`${BASE_API}/v2/events/active`);
        expect(res.status()).toBe(200);

        const data = await res.json();
        expect(Array.isArray(data)).toBe(true);
    });

    test('v2 communities featured endpoint returns data', async ({ request }) => {
        const res = await request.get(`${BASE_API}/v2/communities/featured`);
        expect(res.status()).toBe(200);
    });

    test('v2 leaderboard options endpoint returns data', async ({ request }) => {
        const res = await request.get(`${BASE_API}/v2/leaderboards/options`);
        expect(res.status()).toBe(200);
    });
});
