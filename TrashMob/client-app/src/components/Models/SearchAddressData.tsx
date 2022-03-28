class SearchAddressData {
    "summary": {
        "queryTime": number,
        "numResults": number,
        "totalResults": number,
    };
    "results": [
        {
            "type": "",
            "id": "",
            "score": "",
            "address": {
                "streetNumber": "",
                "streetName": "",
                "municipalitySubdivision": string,
                "municipality": string,
                "countrySecondarySubdivision": string,
                "countryTertiarySubdivision": string,
                "countrySubdivision": string,
                "postalCode": string,
                "extendedPostalCode": string,
                "countryCode": string,
                "country": string,
                "countryCodeISO3": string,
                "freeformAddress": string,
                "countrySubdivisionName": string,
            },
            "position": {
                "lat": number,
                "lon": number
            }
        }
    ]
}

export default SearchAddressData;