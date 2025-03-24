class AddressData {
    summary: {
        queryTime: number;
        numResults: number;
    };

    addresses: [
        {
            address: {
                streetName: '';
                streetNameAndNumber: '';
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
            position: string;
            dataSources: {
                geometry: {
                    id: string;
                };
            };
            entityType: string;
        },
    ];
}

export default AddressData;
