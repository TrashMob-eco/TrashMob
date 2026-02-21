export interface PublicAttendeeMetrics {
    userId: string;
    userName: string;
    bagsCollected?: number;
    weightPounds?: number;
    durationMinutes?: number;
    status: string;
}

export default interface EventMetricsPublicSummary {
    eventId: string;
    totalBagsCollected: number;
    totalWeightPounds: number;
    totalDurationMinutes: number;
    contributorCount: number;
    contributors: PublicAttendeeMetrics[];
}
