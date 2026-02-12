import * as msal from '@azure/msal-browser';
import { getAppConfig, getCachedConfig, type AuthProvider, type B2CConfig, type EntraConfig } from '../services/config';

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
let entraConfig: EntraConfig | null = null;
let authProvider: AuthProvider = 'b2c';
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
        authProvider = config.authProvider || 'b2c';

        if (authProvider === 'entra' && config.azureAdEntra) {
            entraConfig = config.azureAdEntra;
        } else {
            b2cConfig = config.azureAdB2C || fallbackB2CConfig;
            if (!config.azureAdB2C) {
                console.warn('B2C config not available from server, using fallback');
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
    if (getAuthProvider() === 'entra' && entraConfig) {
        return {
            names: { signUpSignIn: '', deleteUser: '', profileEdit: '' },
            authorities: {
                signUpSignIn: { authority: entraConfig.authority },
                deleteUser: { authority: '' },
                profileEdit: { authority: '' },
            },
            authorityDomain: entraConfig.authorityDomain,
            clientId: entraConfig.clientId,
        };
    }

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
    if (getAuthProvider() === 'entra' && entraConfig) {
        return { b2cScopes: entraConfig.scopes };
    }

    const config = b2cConfig || getCachedConfig()?.azureAdB2C || fallbackB2CConfig;
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

    // DEBUG: Show MSAL config
    alert(`[DEBUG 1] MSAL Initialized\nClient ID: ${clientId}\nAuthority: ${fullAuthority}\nAuthority Domain: ${authorityDomain}\nRedirect URI: ${uri}\nScopes: ${JSON.stringify(getApiConfig().b2cScopes)}`);

    // DEBUG: Handle redirect promise and show result
    const redirectResult = await msalC.handleRedirectPromise();
    if (redirectResult) {
        alert(`[DEBUG 2] Redirect Result Received!\nAccount: ${redirectResult.account?.username}\nEmail claim: ${(redirectResult.idTokenClaims as any)?.email || 'MISSING'}\nAll claims: ${JSON.stringify(Object.keys(redirectResult.idTokenClaims || {}))}`);
    } else {
        const accounts = msalC.getAllAccounts();
        alert(`[DEBUG 2] No redirect result (not returning from auth)\nExisting accounts in cache: ${accounts.length}\n${accounts.map(a => a.username).join(', ')}`);
    }

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

// ========== DEBUG: Auth diagnostics ==========
// Remove this block after debugging is complete
export function debugAuthState(): string {
    const provider = getAuthProvider();
    const policies = getB2CPolicies();
    const apiConfig = getApiConfig();
    const cachedCfg = getCachedConfig();

    const info = [
        `Auth Provider: ${provider}`,
        `Client ID: ${policies.clientId}`,
        `Authority: ${policies.authorities.signUpSignIn.authority}`,
        `Authority Domain: ${policies.authorityDomain}`,
        `Scopes: ${JSON.stringify(apiConfig.b2cScopes)}`,
        `Entra config loaded: ${!!cachedCfg?.azureAdEntra}`,
        `B2C config loaded: ${!!cachedCfg?.azureAdB2C}`,
        `MSAL instance exists: ${!!msalClientInstance}`,
    ].join('\n');

    return info;
}
// ========== END DEBUG ==========
