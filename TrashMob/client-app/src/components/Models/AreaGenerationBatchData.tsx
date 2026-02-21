import { Guid } from 'guid-typescript';

export type BatchStatus = 'Queued' | 'Discovering' | 'Processing' | 'Complete' | 'Failed' | 'Cancelled';

class AreaGenerationBatchData {
    id: string = Guid.createEmpty().toString();

    partnerId: string = Guid.EMPTY;

    category: string = '';

    status: BatchStatus = 'Queued';

    discoveredCount: number = 0;

    processedCount: number = 0;

    skippedCount: number = 0;

    stagedCount: number = 0;

    approvedCount: number = 0;

    rejectedCount: number = 0;

    createdCount: number = 0;

    errorMessage: string = '';

    completedDate: Date | null = null;

    boundsNorth: number | null = null;

    boundsSouth: number | null = null;

    boundsEast: number | null = null;

    boundsWest: number | null = null;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default AreaGenerationBatchData;
