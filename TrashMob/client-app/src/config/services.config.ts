export const Services = Object.freeze({
    TIMEOUT: 6000,
    CACHE: { DISABLE: 100, FOR_ONE_MINUTE: 60 * 1000 },
    BASE_URL: process.env.NODE_ENV === 'production' ? '/api' : 'https://as-tm-dev-westus2.azurewebsites.net/api',
});
export type ServicesType = keyof typeof Services;
