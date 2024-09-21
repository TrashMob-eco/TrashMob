import { Guid } from 'guid-typescript';

class DisplayPartnerLocationEventServiceData {
    eventId: string = Guid.createEmpty().toString();

    partnerLocationId: string = Guid.createEmpty().toString();

    serviceTypeId: number = 0;

    eventPartnerLocationStatusId: number = 0;

    partnerName: string = '';

    partnerLocationName: string = '';

    eventName: string = '';

    eventDescription: string = '';

    eventDate: Date = new Date();

    eventStreetAddress: string = '';

    eventCity: string = '';

    eventRegion: string = '';

    eventCountry: string = '';

    eventPostalCode: string = '';
}

export default DisplayPartnerLocationEventServiceData;
