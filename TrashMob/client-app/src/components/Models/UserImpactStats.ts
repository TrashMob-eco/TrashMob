export interface UserEventMetricsSummary {
    eventId: string;
    eventName: string;
    eventDate: Date;
    bagsCollected: number;
    weightPounds: number;
    durationMinutes: number;
    status: string;
}

export default interface UserImpactStats {
    totalBagsCollected: number;
    totalWeightPounds: number;
    totalWeightKilograms: number;
    totalDurationMinutes: number;
    eventsWithMetrics: number;
    eventBreakdown: UserEventMetricsSummary[];
}
