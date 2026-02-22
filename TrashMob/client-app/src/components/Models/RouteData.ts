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
