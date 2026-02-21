export type MetricsStatus = 'Pending' | 'Approved' | 'Rejected' | 'Adjusted';

export default interface EventAttendeeMetricsData {
    id: string;
    eventId: string;
    userId: string;

    // Metrics
    bagsCollected?: number;
    pickedWeight?: number;
    pickedWeightUnitId?: number;
    durationMinutes?: number;
    notes?: string;

    // Approval workflow
    status: MetricsStatus;
    reviewedByUserId?: string;
    reviewedDate?: Date;
    rejectionReason?: string;

    // Adjusted values
    adjustedBagsCollected?: number;
    adjustedPickedWeight?: number;
    adjustedPickedWeightUnitId?: number;
    adjustedDurationMinutes?: number;
    adjustmentReason?: string;

    // Audit fields
    createdByUserId?: string;
    createdDate?: Date;
    lastUpdatedByUserId?: string;
    lastUpdatedDate?: Date;

    // For display (joined from User)
    userName?: string;
}
