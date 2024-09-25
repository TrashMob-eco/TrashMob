// Microsoft Azure APIs
// maps

import axios from 'axios';
import { AzureMapSearchAddressResult } from '../components/Models/AzureMapSearchAddress';
import { AzureMapSearchAddressReverseResult } from '../components/Models/AzureMapSearchAddressReverse';
import { ApiService } from '.';

export type GetMaps_Response = string;
export const GetMaps = () => ({
    key: ['/maps'],
    service: async () => ApiService('public').fetchData<GetMaps_Response>({ url: `/maps`, method: 'get' }),
});


export type GetGoogleMapApiKey_Response = string;
export const GetGoogleMapApiKey = () => ({
    key: ['/maps/googlemapkey'],
    service: async () => ApiService('public').fetchData<GetMaps_Response>({ url: `/maps/googlemapkey`, method: 'get' }),
});

export type AzureMapSearchAddress_Params = { azureKey: string; query: string };
export type AzureMapSearchAddress_Response = AzureMapSearchAddressResult;
export const AzureMapSearchAddress = () => ({
    key: (query: string) => ['AzureMapSearchAddress', query],
    service: async (params: AzureMapSearchAddress_Params) =>
        axios.get<AzureMapSearchAddress_Response>(
            `https://atlas.microsoft.com/search/address/json?typeahead=true&subscription-key=${params.azureKey}&api-version=1.0&query=${params.query}`,
        ),
});

export type AzureMapSearchAddressReverse_Params = { azureKey: string; lat: number; long: number };
export type AzureMapSearchAddressReverse_Response = AzureMapSearchAddressReverseResult;
export const AzureMapSearchAddressReverse = () => ({
    key: (lat: number, lng: number) => ['AzureMapSearchAddressReverse', { lat, lng }],
    service: async (params: AzureMapSearchAddressReverse_Params) => 
        axios.get<AzureMapSearchAddressReverse_Response>(
            `https://atlas.microsoft.com/search/address/reverse/json?subscription-key=${params.azureKey}&api-version=1.0&query=${params.lat},${params.long}`,
        ),
});
