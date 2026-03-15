// Microsoft Azure Maps APIs - proxied through backend for security
// maps

import { AzureMapSearchAddressResult } from '../components/Models/AzureMapSearchAddress';
import { AzureMapSearchAddressReverseResult } from '../components/Models/AzureMapSearchAddressReverse';
import { ApiService } from '.';

export type GetGoogleMapApiKey_Response = string;
export const GetGoogleMapApiKey = () => ({
    key: ['/maps/googlemapkey'],
    service: async () =>
        ApiService('public').fetchData<GetGoogleMapApiKey_Response>({ url: `/v2/maps/googlemapkey`, method: 'get' }),
});

export type GeographicEntityType =
    | 'Country'
    | 'CountrySecondarySubdivision'
    | 'CountrySubdivision'
    | 'CountryTertiarySubdivision'
    | 'Municipality'
    | 'MunicipalitySubdivision'
    | 'Neighbourhood'
    | 'PostalCodeArea';

// Secure proxy endpoints that don't expose the API key

export type SearchAddress_Params = { query: string; entityType?: GeographicEntityType[] };
export type SearchAddress_Response = AzureMapSearchAddressResult;
export const SearchAddress = () => ({
    key: (query: string) => ['SearchAddress', query],
    service: async (params: SearchAddress_Params) => {
        const entityTypeParam = params.entityType ? `&entityType=${params.entityType.join(',')}` : '';
        return ApiService('public').fetchData<SearchAddress_Response>({
            url: `/v2/maps/search?query=${encodeURIComponent(params.query)}${entityTypeParam}`,
            method: 'get',
        });
    },
});

export type ReverseGeocode_Params = { lat: number; long: number };
export type ReverseGeocode_Response = AzureMapSearchAddressReverseResult;
export const ReverseGeocode = () => ({
    key: ['ReverseGeocode'],
    service: async (params: ReverseGeocode_Params) =>
        ApiService('public').fetchData<ReverseGeocode_Response>({
            url: `/v2/maps/reversegeocode?latitude=${params.lat}&longitude=${params.long}`,
            method: 'get',
        }),
});
