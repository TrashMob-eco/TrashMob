import * as msal from '@azure/msal-browser';
import { getAppConfig, getCachedConfig, type EntraConfig } from '../services/config';

// Fallback Entra External ID configuration for when config cannot be loaded
// Uses dev settings as fallback since they're safer for testing
const fallbackEntraConfig: EntraConfig = {
    clientId: '1e6ae74d-0160-4a01-9d75-04048e03b17e',
    authorityDomain: 'trashmobecodev.ciamlogin.com',
    authority: 'https://trashmobecodev.ciamlogin.com/',
    scopes: [
        'https://TrashMobEcoDev.onmicrosoft.com/api/TrashMob.Read',
        'https://TrashMobEcoDev.onmicrosoft.com/api/TrashMob.Writes',
        'email',
    ],
};

// Cached MSAL client and config
let msalClientInstance: msal.PublicClientApplication | null = null;
let entraConfig: EntraConfig | null = null;
let initPromise: Promise<void> | null = null;

// Initialize auth configuration from backend
async function initializeAuth(): Promise<void> {
    if (entraConfig) {
        return; // Already initialized
    }

    if (initPromise) {
        return initPromise; // Initialization in progress
    }

    initPromise = getAppConfig().then((config) => {
        entraConfig = config.azureAdEntra || fallbackEntraConfig;
        if (!config.azureAdEntra) {
            console.warn('Entra config not available from server, using fallback');
        }
    });

    return initPromise;
}

function getEntraConfig(): EntraConfig {
    return entraConfig || getCachedConfig()?.azureAdEntra || fallbackEntraConfig;
}

export function getApiConfig(): { scopes: string[] } {
    return { scopes: getEntraConfig().scopes };
}

export async function GetMsalClient(navigateToLoginRequestUrl: boolean): Promise<msal.PublicClientApplication> {
    const { host } = window.location;
    const { protocol } = window.location;

    const uri = `${protocol}//${host}`;

    const config = getEntraConfig();

    const msalC = new msal.PublicClientApplication({
        auth: {
            clientId: config.clientId,
            authority: config.authority,
            postLogoutRedirectUri: '/',
            navigateToLoginRequestUrl,
            knownAuthorities: [config.authorityDomain],
            redirectUri: uri,
        },
        cache: {
            cacheLocation: 'sessionStorage',
            storeAuthStateInCookie: false,
        },
        system: {
            loggerOptions: {
                loggerCallback: (level, message, containsPii) => {
                    if (containsPii) {
                        console.info('Note: Logging message contained PII');
                        return;
                    }
                    switch (level) {
                        case msal.LogLevel.Error:
                            console.error(message);
                            return;
                        case msal.LogLevel.Info:
                            console.info(message);
                            return;
                        case msal.LogLevel.Verbose:
                            console.debug(message);
                            return;
                        case msal.LogLevel.Warning:
                            console.warn(message);
                    }
                },
            },
        },
    });

    await msalC.initialize();

    return msalC;
}

// Get the cached MSAL client (must be initialized first via initializeMsalClient)
export function getMsalClientInstance(): msal.PublicClientApplication {
    if (!msalClientInstance) {
        throw new Error('MSAL client not initialized. Call initializeMsalClient() first.');
    }
    return msalClientInstance;
}

// Initialize auth and return the MSAL client
export async function initializeMsalClient(): Promise<msal.PublicClientApplication> {
    await initializeAuth();
    if (!msalClientInstance) {
        msalClientInstance = await GetMsalClient(true);
    }
    return msalClientInstance;
}

export function validateToken(idTokenClaims: object): boolean {
    // CIAM id_tokens may not include an email claim — accept tokens with either email or oid
    if (!idTokenClaims.hasOwnProperty('email') && !idTokenClaims.hasOwnProperty('oid')) {
        return false;
    }

    return true;
}

export const tokenRequest = {
    get scopes() {
        return getApiConfig().scopes;
    },
};

export function getDefaultHeaders(method: string): Headers {
    const headers = new Headers();
    headers.append('Allow', method);
    headers.append('Accept', 'application/json, text/plain');
    headers.append('Content-Type', 'application/json');
    return headers;
}

// Pre-initialize auth config on module load
initializeAuth().catch((error) => {
    console.error('Failed to initialize auth config:', error);
});
