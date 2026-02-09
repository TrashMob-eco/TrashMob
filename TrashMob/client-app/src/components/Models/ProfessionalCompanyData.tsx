import { Guid } from 'guid-typescript';

class ProfessionalCompanyData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    contactEmail: string = '';

    contactPhone: string = '';

    partnerId: string = Guid.EMPTY;

    isActive: boolean = true;

    createdByUserId: string = Guid.EMPTY;

    createdDate: Date = new Date();

    lastUpdatedByUserId: string = Guid.EMPTY;

    lastUpdatedDate: Date = new Date();
}

export default ProfessionalCompanyData;
