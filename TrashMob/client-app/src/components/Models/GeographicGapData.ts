class GeographicGapData {
    city: string = '';
    region: string = '';
    country: string = '';
    eventCount: number = 0;
    nearestPartnerDistanceMiles: number | null = null;
    averageLatitude: number | null = null;
    averageLongitude: number | null = null;
}

export default GeographicGapData;
