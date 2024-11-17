import { AccountInfo, InteractionRequiredAuthError } from '@azure/msal-browser';
import { GetUserByEmail } from '@/services/users';
import { useQuery } from '@tanstack/react-query';
import { useMsal } from '@azure/msal-react';
import UserData from '@/components/Models/UserData';
import { getApiConfig } from '@/store/AuthStore';

type TrashmobTokenClaims = AccountInfo['idTokenClaims'] & {
    email: string;
    userDeleted?: boolean;
};

type TrashmobAccountInfo = AccountInfo & {
    idTokenClaims?: TrashmobTokenClaims;
};

const emptyUser = new UserData()

const useGetProfile = (account: TrashmobAccountInfo, email: string) => {
    const { instance } = useMsal(); // Hook to access MSAL accounts

    return useQuery<UserData>({
        queryKey: email ? GetUserByEmail({ email }).key : [],
        queryFn: async () => {
            if (!email) {
                throw new Error('No email found in idToken');
            }

            // Ensure token is valid before making API requests
            try {
                const response = await instance.acquireTokenSilent({
                    account,
                    scopes: getApiConfig().b2cScopes,
                });

                if (!response.accessToken) {
                    throw new Error('No access token retrieved');
                }

                const { data } = await GetUserByEmail({ email }).service();
                return data ?? emptyUser;
            } catch (error) {
                if (error instanceof InteractionRequiredAuthError) {
                    console.error('Interactive login required for token refresh.');
                } else {
                    console.error('Error fetching user profile:', error);
                }
                return emptyUser
            }
        },
        initialData: emptyUser, // Empty UserData
        initialDataUpdatedAt: 0,
        enabled: !!email,
    });
};

export const useLogin = () => {
    const { accounts } = useMsal(); // Hook to access MSAL accounts
    const account = accounts[0] as TrashmobAccountInfo;

    const email = account?.idTokenClaims?.email ?? '';

    const { data: profile, refetch: refetchProfile } = useGetProfile(account, email);

    async function handleUserUpdated() {
        refetchProfile();
    }

    return {
        isUserLoaded: !!profile?.email,
        currentUser: profile,
        handleUserUpdated,
    };
};
