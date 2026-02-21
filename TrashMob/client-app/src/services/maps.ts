// Microsoft Azure Maps APIs - proxied through backend for security
// maps

import { AzureMapSearchAddressResult } from '../components/Models/AzureMapSearchAddress';
import { AzureMapSearchAddressReverseResult } from '../components/Models/AzureMapSearchAddressReverse';
import { ApiService } from '.';

/**
 * @deprecated Use SearchAddress instead - this endpoint exposes the API key
 */
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

export type GeographicEntityType =
    | 'Country'
    | 'CountrySecondarySubdivision'
    | 'CountrySubdivision'
    | 'CountryTertiarySubdivision'
    | 'Municipality'
    | 'MunicipalitySubdivision'
    | 'Neighbourhood'
    | 'PostalCodeArea';

// New secure proxy endpoints that don't expose the API key

export type SearchAddress_Params = { query: string; entityType?: GeographicEntityType[] };
export type SearchAddress_Response = AzureMapSearchAddressResult;
export const SearchAddress = () => ({
    key: (query: string) => ['SearchAddress', query],
    service: async (params: SearchAddress_Params) => {
        const entityTypeParam = params.entityType ? `&entityType=${params.entityType.join(',')}` : '';
        return ApiService('public').fetchData<SearchAddress_Response>({
            url: `/maps/search?query=${encodeURIComponent(params.query)}${entityTypeParam}`,
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
            url: `/maps/reversegeocode?latitude=${params.lat}&longitude=${params.long}`,
            method: 'get',
        }),
});

// Legacy functions - kept for backward compatibility during migration
// These will be removed after all usages are migrated to the proxy endpoints

/**
 * @deprecated Use SearchAddress instead - this function exposes the API key in the browser
 */
export type AzureMapSearchAddress_Params = { azureKey: string; query: string; entityType?: GeographicEntityType[] };
export type AzureMapSearchAddress_Response = AzureMapSearchAddressResult;
export const AzureMapSearchAddress = () => ({
    key: (query: string) => ['AzureMapSearchAddress', query],
    service: async (params: AzureMapSearchAddress_Params) => {
        // Redirect to the secure proxy endpoint (ignoring azureKey)
        const entityTypeParam = params.entityType ? `&entityType=${params.entityType.join(',')}` : '';
        return ApiService('public').fetchData<AzureMapSearchAddress_Response>({
            url: `/maps/search?query=${encodeURIComponent(params.query)}${entityTypeParam}`,
            method: 'get',
        });
    },
});

/**
 * @deprecated Use ReverseGeocode instead - this function exposes the API key in the browser
 */
export type AzureMapSearchAddressReverse_Params = { azureKey: string; lat: number; long: number };
export type AzureMapSearchAddressReverse_Response = AzureMapSearchAddressReverseResult;
export const AzureMapSearchAddressReverse = () => ({
    key: ['AzureMapSearchAddressReverse'],
    service: async (params: AzureMapSearchAddressReverse_Params) => {
        // Redirect to the secure proxy endpoint (ignoring azureKey)
        return ApiService('public').fetchData<AzureMapSearchAddressReverse_Response>({
            url: `/maps/reversegeocode?latitude=${params.lat}&longitude=${params.long}`,
            method: 'get',
        });
    },
});
