import UserData from '@/components/Models/UserData';
import { useEffect, useState } from 'react';
import * as msal from '@azure/msal-browser';
import { getApiConfig, msalClient } from '@/store/AuthStore';
import { GetUserByEmail, GetUserById } from '@/services/users';

export const useLogin = () => {
    const [callbackId, setCallbackId] = useState('');
    const [currentUser, setCurrentUser] = useState<UserData>(new UserData());
    const isUserLoaded = !!currentUser.email;

    useEffect(() => {
        if (callbackId) {
            return;
        }
        const id = msalClient.addEventCallback((message: msal.EventMessage) => {
            if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
                verifyAccount(message.payload as msal.AuthenticationResult);
            }
            if (message.eventType === msal.EventType.LOGOUT_SUCCESS) {
                clearUser();
            }
        });
        setCallbackId(id ?? '');
        initialLogin();
        return () => msalClient.removeEventCallback(callbackId);
    }, [callbackId]);

    async function initialLogin() {
        const accounts = msalClient.getAllAccounts();
        if (accounts === null || accounts.length <= 0) {
            return;
        }
        const tokenResponse = await msalClient.acquireTokenSilent({
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
        const { data: user } = await GetUserByEmail({ email }).service();
        if (!user) {
            return;
        }
        setCurrentUser(user);
    }

    return {
        isUserLoaded,
        currentUser,
        handleUserUpdated,
    };
};
