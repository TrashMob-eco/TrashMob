export type AzureMapSearchAddressResultItem = {
    type: string
    id: string
    score: string
    address: {
      streetNumber: string
      streetName: string
      municipalitySubdivision: string
      municipality: string
      countrySecondarySubdivision: string
      countryTertiarySubdivision: string
      countrySubdivision: string
      postalCode: string
      extendedPostalCode: string
      countryCode: string
      country: string
      countryCodeISO3: string
      freeformAddress: string
      countrySubdivisionName: string
    },
    position: {
      lat: number
      lon: number
    }
  }
  
  export type AzureMapSearchAddressResult = {
    summary: {
      queryTime: number
      numResults: number
      totalResults: number
    }
    results: AzureMapSearchAddressResultItem[]
  }