import * as msal from "@azure/msal-browser";

export const msalClient: msal.PublicClientApplication = new msal.PublicClientApplication(
    {
        auth:
        {
            clientId: '0a1647a4-c758-4964-904f-a9b66958c071',
            authority: 'https://trashmob.b2clogin.com/Trashmob.onmicrosoft.com/b2c_1_signupsignin1',
            postLogoutRedirectUri: "/"
        },
        cache: { cacheLocation: "sessionStorage", storeAuthStateInCookie: false},
        system: {
            loggerOptions: {
                loggerCallback: (level, message, containsPii) => {
                    if (containsPii) {
                        console.info("Note: Logging message contained PII");
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
                            return;
                    }
                }
            }
        }
    }
);

export const apiConfig = {
    b2cScopes: ["https://Trashmob.onmicrosoft.com/api/TrashMob.Read", "https://Trashmob.onmicrosoft.com/api/Trashmob.Writes" ],
};

export const tokenRequest = {
    scopes: apiConfig.b2cScopes
}

export function getDefaultHeaders(method: string) : Headers {
    var headers = new Headers();
    headers.append('Allow', method);
    headers.append('Accept', 'application/json, text/plain');
    headers.append('Content-Type', 'application/json');
    return headers;
}