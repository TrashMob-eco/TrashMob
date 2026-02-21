// Configuration service - fetches app config from backend

export interface B2CConfig {
    clientId: string;
    authorityDomain: string;
    policies: {
        signUpSignIn: string;
        deleteUser: string;
        profileEdit: string;
    };
    authorities: {
        signUpSignIn: string;
        deleteUser: string;
        profileEdit: string;
    };
    scopes: string[];
}

export interface EntraConfig {
    clientId: string;
    authorityDomain: string;
    authority: string;
    scopes: string[];
}

export type AuthProvider = 'b2c' | 'entra';

export interface AppConfig {
    applicationInsightsKey: string | null;
    authProvider?: AuthProvider;
    azureAdB2C?: B2CConfig | null;
    azureAdEntra?: EntraConfig | null;
}

let cachedConfig: AppConfig | null = null;
let configPromise: Promise<AppConfig> | null = null;

export async function getAppConfig(): Promise<AppConfig> {
    // Return cached config if available
    if (cachedConfig) {
        return cachedConfig;
    }

    // Return existing promise if fetch is in progress
    if (configPromise) {
        return configPromise;
    }

    // Fetch config from backend
    // Use VITE_API_URL if set (for local dev against remote backend), otherwise /api (proxied)
    const configUrl = import.meta.env.VITE_API_URL ? `${import.meta.env.VITE_API_URL}/config` : '/api/config';
    configPromise = fetch(configUrl)
        .then((response) => {
            if (!response.ok) {
                throw new Error(`Failed to fetch config: ${response.status}`);
            }
            return response.json();
        })
        .then((config: AppConfig) => {
            cachedConfig = config;
            return config;
        })
        .catch((error) => {
            console.error('Failed to fetch app config:', error);
            // Return a default config — defaults to Entra (fallback config in AuthStore handles the rest)
            const defaultConfig: AppConfig = {
                applicationInsightsKey: null,
                authProvider: 'entra',
            };
            cachedConfig = defaultConfig;
            return defaultConfig;
        });

    return configPromise;
}

// Synchronous getter for cached config (returns null if not yet loaded)
export function getCachedConfig(): AppConfig | null {
    return cachedConfig;
}
