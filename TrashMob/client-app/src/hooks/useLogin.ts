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
            if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
                trackAuth('Login', true);
                verifyAccount(message.payload as msal.AuthenticationResult);
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
        if (accounts === null || accounts.length <= 0) {
            return;
        }
        const tokenResponse = await getMsalClientInstance().acquireTokenSilent({
            scopes: getApiConfig().b2cScopes,
            account: accounts[0],
        });
        verifyAccount(tokenResponse);
    }

    function clearUser() {
        setCurrentUser(new UserData());
    }

    async function handleUserUpdated() {
        const { data: user } = await GetUserById({ userId: currentUser?.id }).service();
        setCurrentUser(user || new UserData());
    }

    async function verifyAccount(result: msal.AuthenticationResult) {
        const { userDeleted } = result.idTokenClaims as Record<string, any>;
        if (userDeleted && userDeleted === true) {
            clearUser();
            return;
        }
        const { email } = result.idTokenClaims as Record<string, any>;
        if (!email) {
            console.warn('No email claim found in token — cannot verify account');
            return;
        }

        try {
            const { data: user } = await GetUserByEmail({ email }).service();
            if (user) {
                setCurrentUser(user);
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
