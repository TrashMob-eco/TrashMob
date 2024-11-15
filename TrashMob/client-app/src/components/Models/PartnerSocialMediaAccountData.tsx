import { Guid } from 'guid-typescript';

class PartnerSocialMediaAccountData {
    id: string = Guid.createEmpty().toString();

    partnerId: string = Guid.createEmpty().toString();

    socialMediaAccountTypeId: number = 0;

    accountIdentifier: string = '';

    isActive: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default PartnerSocialMediaAccountData;
