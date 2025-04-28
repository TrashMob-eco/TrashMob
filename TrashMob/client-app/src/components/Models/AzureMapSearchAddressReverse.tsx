export type AzureMapSearchAddressReverseResultItem = {
    address: {
        streetName: string;
        streetNameAndNumber: string;
        routeNumbers: [];
        countryCode: string;
        countrySubdivision: string;
        countrySecondarySubdivision: string;
        municipality: string;
        postalName: string;
        postalCode: string;
        country: string;
        countryCodeISO3: string;
        freeformAddress: string;
        boundingBox: {
            northEast: string;
            southWest: string;
            entity: string;
        };
        countrySubdivisionName: string;
    };
    id: string;
    position: string;
    dataSources: {
        geometry: {
            id: string;
        };
    };
    entityType: string;
};

export type AzureMapSearchAddressReverseResult = {
    summary: {
        queryTime: number;
        numResults: number;
    };
    addresses: AzureMapSearchAddressReverseResultItem[];
};
