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

export interface AppConfig {
    applicationInsightsKey: string | null;
    azureAdB2C: B2CConfig | null;
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
    configPromise = fetch('/api/config')
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
            // Return a default config that will cause auth to fail gracefully
            const defaultConfig: AppConfig = {
                applicationInsightsKey: null,
                azureAdB2C: null,
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
