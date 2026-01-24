export const Services = Object.freeze({
    TIMEOUT: 6000,
    CACHE: { DISABLE: 100, FOR_ONE_MINUTE: 60 * 1000 },
    BASE_URL:
        process.env.NODE_ENV === 'production'
            ? '/api'
            : 'https://ca-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io/api',
});
export type ServicesType = keyof typeof Services;
