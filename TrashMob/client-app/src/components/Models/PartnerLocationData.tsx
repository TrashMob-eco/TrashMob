import { Guid } from 'guid-typescript';
import PartnerLocationContactData from './PartnerLocationContactData';

class PartnerLocationData {
    id: string = Guid.createEmpty().toString();

    partnerId: string = Guid.createEmpty().toString();

    name: string = '';

    streetAddress: string = '';

    city: string = '';

    region: string = '';

    country: string = '';

    latitude: number = 0;

    longitude: number = 0;

    postalCode: string = '';

    publicNotes: string = '';

    privateNotes: string = '';

    isActive: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();

    partnerLocationContacts: PartnerLocationContactData[] = [];
}

export default PartnerLocationData;
