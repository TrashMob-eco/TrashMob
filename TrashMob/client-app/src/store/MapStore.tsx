import { GetMaps } from '../services/maps';

export async function getOption(): Promise<{ subscriptionKey: string }> {
    const authOptions = {
        subscriptionKey: await getKey(),
    };
    return authOptions;
}

export async function getKey(): Promise<string> {
    const key = await GetMaps()
        .service()
        .then((res) => res.data)
        .catch((err) => '');
    return key;
}

export const defaultLongitude: number = -100.01;
export const defaultLatitude: number = 45.01;
export const defaultUserLocationZoom: number = 10;
