// Microsoft Azure APIs
// maps

import axios from "axios";
import SearchAddressData from "../components/Models/SearchAddressData";
import AddressData from "../components/Models/AddressData";
import { ApiService } from ".";

export type GetMaps_Response = string;
export const GetMaps = () => ({ key: ['/maps'], service: async () => ApiService('public').fetchData<GetMaps_Response>({ url: `/maps`, method: 'get' }) });

export type AzureMapSearchAddress_Params = { azureKey: string; query: string; }
export type AzureMapSearchAddress_Response = SearchAddressData;
export const AzureMapSearchAddress = () => ({ key: ['AzureMapSearchAddress'], service: async (params: AzureMapSearchAddress_Params) => axios.get<AzureMapSearchAddress_Response>(`https://atlas.microsoft.com/search/address/json?typeahead=true&subscription-key=${params.azureKey}&api-version=1.0&query=${params.query}`) });

export type AzureMapSearchAddressReverse_Params = { azureKey: string; lat: number; long: number; }
export type AzureMapSearchAddressReverse_Response = AddressData;
export const AzureMapSearchAddressReverse = () => ({ key: ['AzureMapSearchAddressReverse'], service: async (params: AzureMapSearchAddressReverse_Params) => axios.get<AzureMapSearchAddressReverse_Response>(`https://atlas.microsoft.com/search/address/reverse/json?subscription-key=${params.azureKey}&api-version=1.0&query=${params.lat},${params.long}`) });
