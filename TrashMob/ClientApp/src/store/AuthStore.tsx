import * as msal from "@azure/msal-browser";
import { MsalProvider, MsalProviderProps } from "@azure/msal-react";

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