export const Services = Object.freeze({
    TIMEOUT: 30000, // 30 seconds - needed for slow DB queries from localhost to Azure SQL
    CACHE: { DISABLE: 100, FOR_ONE_MINUTE: 60 * 1000 },
    // In production: relative /api hits same origin
    // In dev: uses VITE_API_URL env var, defaults to /api (proxied to localhost)
    // UX devs can set VITE_API_URL=https://dev.trashmob.eco/api in .env.local
    BASE_URL: import.meta.env.VITE_API_URL || '/api',
});
export type ServicesType = keyof typeof Services;
