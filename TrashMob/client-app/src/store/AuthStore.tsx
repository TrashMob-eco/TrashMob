import * as msal from '@azure/msal-browser';
import { getAppConfig, getCachedConfig, type AuthProvider, type B2CConfig, type EntraConfig } from '../services/config';

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
let b2cConfig: B2CConfig | null = null;
let entraConfig: EntraConfig | null = null;
let authProvider: AuthProvider = 'entra';
let initPromise: Promise<void> | null = null;

// Initialize auth configuration from backend
async function initializeAuth(): Promise<void> {
    if (b2cConfig || entraConfig) {
        return; // Already initialized
    }

    if (initPromise) {
        return initPromise; // Initialization in progress
    }

    initPromise = getAppConfig().then((config) => {
        authProvider = config.authProvider || 'entra';

        if (authProvider === 'entra') {
            entraConfig = config.azureAdEntra || fallbackEntraConfig;
            if (!config.azureAdEntra) {
                console.warn('Entra config not available from server, using fallback');
            }
        } else {
            b2cConfig = config.azureAdB2C || null;
            if (!config.azureAdB2C) {
                console.warn('B2C config not available from server');
            }
        }
    });

    return initPromise;
}

// Get the current auth provider
export function getAuthProvider(): AuthProvider {
    return getCachedConfig()?.authProvider || authProvider;
}

// Get the B2C config (synchronous, returns fallback if not yet loaded)
export function getB2CPolicies(): {
    names: { signUpSignIn: string; deleteUser: string; profileEdit: string };
    authorities: {
        signUpSignIn: { authority: string };
        deleteUser: { authority: string };
        profileEdit: { authority: string };
    };
    authorityDomain: string;
    clientId: string;
} {
    // When using Entra, return config with empty policy authorities
    // Profile edit and account deletion move to in-app (Graph API) in Phase 2
    const effectiveEntra = entraConfig || getCachedConfig()?.azureAdEntra || fallbackEntraConfig;
    if (getAuthProvider() === 'entra') {
        return {
            names: { signUpSignIn: '', deleteUser: '', profileEdit: '' },
            authorities: {
                signUpSignIn: { authority: effectiveEntra.authority },
                deleteUser: { authority: '' },
                profileEdit: { authority: '' },
            },
            authorityDomain: effectiveEntra.authorityDomain,
            clientId: effectiveEntra.clientId,
        };
    }

    const config = b2cConfig || getCachedConfig()?.azureAdB2C;
    if (!config) {
        throw new Error('B2C auth provider selected but no B2C config available');
    }

    return {
        names: config.policies,
        authorities: {
            signUpSignIn: { authority: config.authorities.signUpSignIn },
            deleteUser: { authority: config.authorities.deleteUser },
            profileEdit: { authority: config.authorities.profileEdit },
        },
        authorityDomain: config.authorityDomain,
        clientId: config.clientId,
    };
}

export function getApiConfig(): { b2cScopes: string[] } {
    if (getAuthProvider() === 'entra') {
        const config = entraConfig || getCachedConfig()?.azureAdEntra || fallbackEntraConfig;
        return { b2cScopes: config.scopes };
    }

    const config = b2cConfig || getCachedConfig()?.azureAdB2C;
    if (!config) {
        throw new Error('B2C auth provider selected but no B2C config available');
    }
    return {
        b2cScopes: config.scopes,
    };
}

export async function GetMsalClient(navigateToLoginRequestUrl: boolean): Promise<msal.PublicClientApplication> {
    const { host } = window.location;
    const { protocol } = window.location;

    const uri = `${protocol}//${host}`;

    const policies = getB2CPolicies();
    const { clientId } = policies;
    const fullAuthority = policies.authorities.signUpSignIn.authority;
    const { authorityDomain } = policies;

    const msalC = new msal.PublicClientApplication({
        auth: {
            clientId,
            authority: fullAuthority,
            postLogoutRedirectUri: '/',
            navigateToLoginRequestUrl,
            knownAuthorities: [authorityDomain],
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
    if (!idTokenClaims.hasOwnProperty('email')) {
        return false;
    }

    return true;
}

export const tokenRequest = {
    get scopes() {
        return getApiConfig().b2cScopes;
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
