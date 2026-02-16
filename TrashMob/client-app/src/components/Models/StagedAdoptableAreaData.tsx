import { Guid } from 'guid-typescript';

export type ReviewStatus = 'Pending' | 'Approved' | 'Rejected';
export type ConfidenceLevel = 'High' | 'Medium' | 'Low';

class StagedAdoptableAreaData {
    id: string = Guid.createEmpty().toString();

    batchId: string = Guid.EMPTY;

    partnerId: string = Guid.EMPTY;

    name: string = '';

    description: string = '';

    areaType: string = '';

    geoJson: string = '';

    centerLatitude: number = 0;

    centerLongitude: number = 0;

    reviewStatus: ReviewStatus = 'Pending';

    confidence: ConfidenceLevel = 'Medium';

    isPotentialDuplicate: boolean = false;

    duplicateOfName: string = '';

    osmId: string = '';

    osmTags: string = '';

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default StagedAdoptableAreaData;
