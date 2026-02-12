import UserData from '@/components/Models/UserData';
import { useEffect, useState } from 'react';
import * as msal from '@azure/msal-browser';
import { getApiConfig, getMsalClientInstance } from '@/store/AuthStore';
import { GetUserByEmail, GetUserById } from '@/services/users';
import { useFeatureMetrics } from './useFeatureMetrics';

export const useLogin = () => {
    const [callbackId, setCallbackId] = useState('');
    const [currentUser, setCurrentUser] = useState<UserData>(new UserData());
    const isUserLoaded = !!currentUser.email;
    const { trackAuth } = useFeatureMetrics();

    useEffect(() => {
        if (callbackId) {
            return;
        }
        const id = getMsalClientInstance().addEventCallback((message: msal.EventMessage) => {
            // DEBUG: Log all MSAL events
            alert(`[DEBUG 4] MSAL Event: ${message.eventType}\nError: ${message.error?.message || 'none'}`);

            if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
                const result = message.payload as msal.AuthenticationResult;
                alert(`[DEBUG 5] LOGIN_SUCCESS!\nAccount: ${result.account?.username}\nEmail claim: ${(result.idTokenClaims as any)?.email || 'MISSING'}\nAll claims: ${JSON.stringify(Object.keys(result.idTokenClaims || {}))}`);
                trackAuth('Login', true);
                verifyAccount(result);
            }
            if (message.eventType === msal.EventType.LOGOUT_SUCCESS) {
                trackAuth('Logout', true);
                clearUser();
            }
        });
        setCallbackId(id ?? '');
        initialLogin();
        return () => getMsalClientInstance().removeEventCallback(callbackId);
    }, [callbackId]);

    async function initialLogin() {
        const accounts = getMsalClientInstance().getAllAccounts();
        alert(`[DEBUG 6] initialLogin called\nAccounts in cache: ${accounts?.length || 0}\n${accounts?.map(a => `${a.username} (${a.homeAccountId})`).join('\n') || 'none'}`);
        if (accounts === null || accounts.length <= 0) {
            return;
        }
        try {
            const tokenResponse = await getMsalClientInstance().acquireTokenSilent({
                scopes: getApiConfig().b2cScopes,
                account: accounts[0],
            });
            alert(`[DEBUG 7] acquireTokenSilent success\nEmail claim: ${(tokenResponse.idTokenClaims as any)?.email || 'MISSING'}\nAll claims: ${JSON.stringify(Object.keys(tokenResponse.idTokenClaims || {}))}`);
            verifyAccount(tokenResponse);
        } catch (err: any) {
            alert(`[DEBUG 7] acquireTokenSilent FAILED\nError: ${err.message || err}`);
        }
    }

    function clearUser() {
        setCurrentUser(new UserData());
    }

    async function handleUserUpdated() {
        const { data: user } = await GetUserById({ userId: currentUser?.id }).service();
        setCurrentUser(user || new UserData());
    }

    async function verifyAccount(result: msal.AuthenticationResult) {
        const claims = result.idTokenClaims as Record<string, any>;
        alert(`[DEBUG 8] verifyAccount called\nAll claims: ${JSON.stringify(claims, null, 2)}`);

        const { userDeleted } = claims;
        if (userDeleted && userDeleted === true) {
            alert('[DEBUG 8b] User marked as deleted — clearing');
            clearUser();
            return;
        }
        const { email } = claims;
        if (!email) {
            alert(`[DEBUG 9] NO EMAIL CLAIM!\nThis is why sign-in fails.\nAvailable claims: ${Object.keys(claims).join(', ')}\nFull claims: ${JSON.stringify(claims)}`);
            console.warn('No email claim found in token — cannot verify account');
            return;
        }

        alert(`[DEBUG 9] Email found: ${email}\nCalling GetUserByEmail...`);

        try {
            const { data: user } = await GetUserByEmail({ email }).service();
            if (user) {
                alert(`[DEBUG 10] User loaded!\nID: ${user.id}\nName: ${user.userName}\nEmail: ${user.email}`);
                setCurrentUser(user);
            } else {
                alert(`[DEBUG 10] GetUserByEmail returned null/empty for: ${email}`);
            }
        } catch (error) {
            // On first sign-up, the backend auto-creates the user during auth validation.
            // If the first call fails (e.g. transient error), retry once after a short delay.
            console.warn('First GetUserByEmail attempt failed, retrying...', error);
            try {
                await new Promise<void>((resolve) => {
                    setTimeout(resolve, 1000);
                });
                const { data: user } = await GetUserByEmail({ email }).service();
                if (user) {
                    setCurrentUser(user);
                }
            } catch (retryError) {
                console.error('Failed to verify account after retry', retryError);
            }
        }
    }

    return {
        isUserLoaded,
        currentUser,
        handleUserUpdated,
    };
};
