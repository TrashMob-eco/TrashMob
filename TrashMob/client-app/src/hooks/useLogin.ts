import UserData from '@/components/Models/UserData';
import { useCallback, useEffect, useState } from 'react';
import * as msal from '@azure/msal-browser';
import { getApiConfig, msalClient } from '@/store/AuthStore';
import { GetUserByEmail, GetUserById } from '@/services/users';

export const useLogin = () => {
    const [callbackId, setCallbackId] = useState('');
    const [isUserLoading, setUserIsLoading] = useState<boolean>(false)
    const [currentUser, setCurrentUser] = useState<UserData>(new UserData());
    const isUserLoaded = !!currentUser.email;

    const login = useCallback(() => {
        const apiConfig = getApiConfig();

        msalClient.loginRedirect({
            scopes: apiConfig.b2cScopes,
        });

        setUserIsLoading(true)
    }, [msalClient, setUserIsLoading])

    useEffect(() => {
        console.log('useEffect')

        if (callbackId) {
            return;
        }
        const id = msalClient.addEventCallback((message: msal.EventMessage) => {
            if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
                setUserIsLoading(true)
                verifyAccount(message.payload as msal.AuthenticationResult);
            }
            if (message.eventType === msal.EventType.LOGOUT_SUCCESS) {
                clearUser();
            }
        });
        setCallbackId(id ?? '');
        initialLogin();
        return () => msalClient.removeEventCallback(callbackId);
    }, [callbackId, setUserIsLoading]);

    async function initialLogin() {
        console.log('initialLogin')
        const accounts = msalClient.getAllAccounts();
        if (accounts === null || accounts.length <= 0) {
            return;
        }

        setUserIsLoading(true)
        const tokenResponse = await msalClient.acquireTokenSilent({
            scopes: getApiConfig().b2cScopes,
            account: accounts[0],
        });
        await verifyAccount(tokenResponse);
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
        setUserIsLoading(false)
        setCurrentUser(user);
    }

    return {
        isUserLoaded,
        isUserLoading,
        currentUser,
        login,
        handleUserUpdated,
    };
};
