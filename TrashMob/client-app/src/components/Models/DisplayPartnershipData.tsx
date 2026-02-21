import { Guid } from 'guid-typescript';

class DisplayPartnershipData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    partnerRequestStatusId: number = 0;

    partnerStatusId: number = 0;

    // Community Home Page Properties
    homePageEnabled: boolean = false;

    slug: string = '';
}

export default DisplayPartnershipData;
