import { Guid } from 'guid-typescript';

class TeamPhotoData {
    id: string = Guid.createEmpty().toString();

    teamId: string = Guid.createEmpty().toString();

    imageUrl: string = '';

    caption: string = '';

    uploadedByUserId: string = Guid.EMPTY;

    uploadedDate: Date = new Date();

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default TeamPhotoData;
