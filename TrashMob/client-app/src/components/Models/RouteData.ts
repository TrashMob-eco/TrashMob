export interface SortableLocation {
    sortOrder: number;
    latitude: number;
    longitude: number;
}

export interface DisplayAnonymizedRoute {
    id: string;
    eventId: string;
    startTime: string;
    endTime: string;
    totalDistanceMeters: number;
    durationMinutes: number;
    bagsCollected: number | null;
    weightCollected: number | null;
    weightUnitId: number | null;
    densityGramsPerMeter: number | null;
    densityColor: string;
    locations: SortableLocation[];
}

export interface DisplayEventRouteStats {
    eventId: string;
    totalRoutes: number;
    totalDistanceMeters: number;
    totalDurationMinutes: number;
    uniqueContributors: number;
    totalBagsCollected: number;
    totalWeightCollected: number;
    totalWeightUnitId: number;
    coverageAreaSquareMeters: number;
    averageDensityGramsPerMeter: number | null;
    maxDensityGramsPerMeter: number | null;
}

export interface DisplayEventAttendeeRoute {
    id: string;
    eventId: string;
    userId: string;
    startTime: string;
    endTime: string;
    totalDistanceMeters: number;
    durationMinutes: number;
    privacyLevel: string;
    isTrimmed: boolean;
    trimStartMeters: number;
    trimEndMeters: number;
    bagsCollected: number | null;
    weightCollected: number | null;
    weightUnitId: number | null;
    notes: string | null;
    expiresDate: string | null;
    isTimeTrimmed: boolean;
    originalEndTime: string | null;
    originalTotalDistanceMeters: number | null;
    originalDurationMinutes: number | null;
    densityGramsPerMeter: number | null;
    densityColor: string;
    locations: SortableLocation[];
}

export interface DisplayUserRouteHistory {
    routeId: string;
    eventId: string;
    eventName: string;
    eventDate: string;
    totalDistanceMeters: number;
    durationMinutes: number;
    privacyLevel: string;
    bagsCollected: number | null;
    weightCollected: number | null;
    weightUnitId: number | null;
    densityGramsPerMeter: number | null;
    densityColor: string;
    isTimeTrimmed: boolean;
    eventLatitude: number;
    eventLongitude: number;
    startTime: string;
    endTime: string;
    locations: SortableLocation[];
}

export interface EventSummaryPrefill {
    numberOfBags: number;
    pickedWeight: number;
    pickedWeightUnitId: number;
    durationInMinutes: number;
    actualNumberOfAttendees: number;
    hasRouteData: boolean;
}
