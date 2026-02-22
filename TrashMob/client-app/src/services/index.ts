import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import { Services } from '../config/services.config';
import { getApiConfig, getMsalClientInstance, validateToken } from '../store/AuthStore';

const PublicService = axios.create({
    timeout: Services.TIMEOUT,
    baseURL: Services.BASE_URL,
});

const ProtectedService = axios.create({
    timeout: Services.TIMEOUT,
    baseURL: Services.BASE_URL,
});
ProtectedService.interceptors.request.use(
    async (config) => {
        const processedConfig = config;

        // Get & Set Access Token
        const accounts = getMsalClientInstance().getAllAccounts();
        if (accounts === null || accounts.length <= 0) throw new axios.Cancel('User not found!');
        const request = {
            scopes: getApiConfig().scopes,
            account: accounts[0],
        };
        const tokenResponse = await getMsalClientInstance().acquireTokenSilent(request);
        if (!validateToken(tokenResponse.idTokenClaims)) throw new axios.Cancel('User not found!');
        processedConfig.headers.Authorization = `Bearer ${tokenResponse.accessToken}`;

        return processedConfig;
    },
    async (error) => Promise.reject(error),
);

export const ApiService = (serviceType: 'public' | 'protected') => ({
    fetchData<T, K = unknown>(param: AxiosRequestConfig<K>) {
        return new Promise<AxiosResponse<T>>((resolve, reject) => {
            if (serviceType === 'protected') {
                ProtectedService(param)
                    .then((response) => resolve(response))
                    .catch((errors) => reject(errors));
            } else {
                PublicService(param)
                    .then((response) => resolve(response))
                    .catch((errors) => reject(errors));
            }
        });
    },
});
