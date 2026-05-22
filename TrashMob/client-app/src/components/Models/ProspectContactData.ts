class ProspectContactData {
    id: string = '00000000-0000-0000-0000-000000000000';
    prospectId: string = '';
    name: string = '';
    title: string | null = null;
    email: string | null = null;
    phone: string | null = null;
    role: string | null = null;
    contactStatus: number = 0;
    isPrimary: boolean = false;
    referredByContactId: string | null = null;
    notes: string | null = null;
    createdDate: string | null = null;
    lastUpdatedDate: string | null = null;
}

export default ProspectContactData;
