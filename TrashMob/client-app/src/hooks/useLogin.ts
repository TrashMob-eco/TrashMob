import UserData from '@/components/Models/UserData';
import { Guid } from 'guid-typescript';
import { useEffect, useState } from 'react';
import * as msal from '@azure/msal-browser';
import { getApiConfig, getMsalClientInstance } from '@/store/AuthStore';
import { GetUserByEmail, GetUserById, GetUserByObjectId } from '@/services/users';
import { useFeatureMetrics } from './useFeatureMetrics';

const EMPTY_GUID = Guid.createEmpty().toString();

export const useLogin = () => {
    const [callbackId, setCallbackId] = useState('');
    const [currentUser, setCurrentUser] = useState<UserData>(new UserData());
    const isUserLoaded = !!currentUser.id && currentUser.id !== EMPTY_GUID;
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
        try {
            const tokenResponse = await getMsalClientInstance().acquireTokenSilent({
                scopes: getApiConfig().b2cScopes,
                account: accounts[0],
            });
            verifyAccount(tokenResponse);
        } catch (err: any) {
            console.warn('acquireTokenSilent failed:', err);
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

        const { userDeleted } = claims;
        if (userDeleted && userDeleted === true) {
            clearUser();
            return;
        }
        const { email } = claims;
        const objectId = claims.oid;

        if (!email && !objectId) {
            console.warn('No email or oid claim found in token — cannot verify account');
            return;
        }

        try {
            const user = await fetchUser(email, objectId);
            if (user) {
                setCurrentUser(user);
            }
        } catch (error) {
            // On first sign-up, the backend auto-creates the user during auth validation.
            // If the first call fails (e.g. transient error), retry once after a short delay.
            console.warn('First user lookup attempt failed, retrying...', error);
            try {
                await new Promise<void>((resolve) => {
                    setTimeout(resolve, 1000);
                });
                const user = await fetchUser(email, objectId);
                if (user) {
                    setCurrentUser(user);
                }
            } catch (retryError) {
                console.error('Failed to verify account after retry', retryError);
            }
        }
    }

    async function fetchUser(email: string | undefined, objectId: string | undefined): Promise<UserData | null> {
        // Try email lookup first, fall back to ObjectId
        if (email) {
            const { data: user } = await GetUserByEmail({ email }).service();
            if (user) return user;
        }
        if (objectId) {
            const { data: user } = await GetUserByObjectId({ objectId }).service();
            if (user) return user;
        }
        return null;
    }

    return {
        isUserLoaded,
        currentUser,
        handleUserUpdated,
    };
};
