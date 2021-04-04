import { IAzureMapOptions } from 'react-azure-maps'
import { AuthenticationType } from 'azure-maps-control'

export async function getOption(): Promise<IAzureMapOptions> {
    var authOptions = {
        authType: AuthenticationType.subscriptionKey,
        subscriptionKey: await getKey()
    }
    return authOptions;
}

async function getKey(): Promise<string> {
    var key = '';

    fetch('api/maps', {
        method: 'GET',
        headers: {
            Allow: 'GET',
            Accept: 'application/json',
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.json() as Promise<string>)
        .then(data => {
            key = data;
        });
    return key;
}