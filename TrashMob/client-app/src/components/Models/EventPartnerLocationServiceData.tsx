import { Guid } from 'guid-typescript';

class EventPartnerLocationServiceData {
    eventId: string = Guid.createEmpty().toString();

    partnerLocationId: string = Guid.createEmpty().toString();

    serviceTypeId: number = 0;

    eventPartnerLocationServiceStatusId: number = 0;
}

export default EventPartnerLocationServiceData;
