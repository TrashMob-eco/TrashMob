import { Guid } from 'guid-typescript';

class DisplayEventPartnerLocationData {
    eventId: string = Guid.createEmpty().toString();

    partnerLocationId: string = Guid.createEmpty().toString();

    eventPartnerLocationStatusId: number = 0;

    createdByUserId: string = '';

    partnerName: string = '';

    partnerLocationName: string = '';

    partnerLocationNotes: string = '';

    partnerServicesEngaged: string = '';
}

export default DisplayEventPartnerLocationData;
