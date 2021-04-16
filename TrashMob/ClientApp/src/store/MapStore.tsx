import { IAzureMapOptions } from 'react-azure-maps'
import { AuthenticationType } from 'azure-maps-control'
import { defaultHeaders } from './AuthStore';

export async function getOption(): Promise<IAzureMapOptions> {
    var authOptions = {
        authType: AuthenticationType.subscriptionKey,
        subscriptionKey: await getKey()
    }
    return authOptions;
}

async function getKey(): Promise<string> {
    var key = '';

    const headers = defaultHeaders('GET');

    await fetch('api/maps', {
        method: 'GET',
        headers: headers,
    })
        .then(response => response.json() as Promise<string>)
        .then(data => {
            key = data;
        });
    return key;
}