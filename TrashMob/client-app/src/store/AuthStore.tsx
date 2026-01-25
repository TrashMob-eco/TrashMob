import * as msal from '@azure/msal-browser';
import { getAppConfig, getCachedConfig, type B2CConfig } from '../services/config';

// Fallback B2C configuration for when config cannot be loaded
// Uses dev settings as fallback since they're safer for testing
const fallbackB2CConfig: B2CConfig = {
    clientId: 'e46d67ba-fe46-40f4-b222-2f982b2bb112',
    authorityDomain: 'TrashMobDev.b2clogin.com',
    policies: {
        signUpSignIn: 'B2C_1A_TM_SIGNUP_SIGNIN',
        deleteUser: 'B2C_1A_TM_DEREGISTER',
        profileEdit: 'B2C_1A_TM_PROFILEEDIT',
    },
    authorities: {
        signUpSignIn: 'https://TrashMobDev.b2clogin.com/TrashMobDev.onmicrosoft.com/B2C_1A_TM_SIGNUP_SIGNIN',
        deleteUser: 'https://TrashMobDev.b2clogin.com/TrashMobDev.onmicrosoft.com/B2C_1A_TM_DEREGISTER',
        profileEdit: 'https://TrashMobDev.b2clogin.com/TrashMobDev.onmicrosoft.com/B2C_1A_TM_PROFILEEDIT',
    },
    scopes: [
        'https://TrashMobDev.onmicrosoft.com/api/TrashMob.Read',
        'https://TrashMobDev.onmicrosoft.com/api/TrashMob.Writes',
        'email',
    ],
};

// Cached MSAL client and config
let msalClientInstance: msal.PublicClientApplication | null = null;
let b2cConfig: B2CConfig | null = null;
let initPromise: Promise<void> | null = null;

// Initialize auth configuration from backend
async function initializeAuth(): Promise<void> {
    if (b2cConfig) {
        return; // Already initialized
    }

    if (initPromise) {
        return initPromise; // Initialization in progress
    }

    initPromise = getAppConfig().then((config) => {
        b2cConfig = config.azureAdB2C || fallbackB2CConfig;
        if (!config.azureAdB2C) {
            console.warn('B2C config not available from server, using fallback');
        }
    });

    return initPromise;
}

// Get the B2C config (synchronous, returns fallback if not yet loaded)
export function getB2CPolicies(): {
    names: { signUpSignIn: string; deleteUser: string; profileEdit: string };
    authorities: { signUpSignIn: { authority: string }; deleteUser: { authority: string }; profileEdit: { authority: string } };
    authorityDomain: string;
    clientId: string;
} {
    const config = b2cConfig || getCachedConfig()?.azureAdB2C || fallbackB2CConfig;

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
    const config = b2cConfig || getCachedConfig()?.azureAdB2C || fallbackB2CConfig;
    return {
        b2cScopes: config.scopes,
    };
}

export function GetMsalClient(navigateToLoginRequestUrl: boolean): msal.PublicClientApplication {
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

    return msalC;
}

// Get or create the MSAL client (uses cached instance after first call)
export function getMsalClientInstance(): msal.PublicClientApplication {
    if (!msalClientInstance) {
        msalClientInstance = GetMsalClient(true);
    }
    return msalClientInstance;
}

// Initialize auth and return the MSAL client
export async function initializeMsalClient(): Promise<msal.PublicClientApplication> {
    await initializeAuth();
    return getMsalClientInstance();
}

// Legacy export for backward compatibility
// Note: This creates the client immediately with whatever config is available
// For proper initialization, use initializeMsalClient() instead
export const msalClient: msal.PublicClientApplication = GetMsalClient(true);

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
