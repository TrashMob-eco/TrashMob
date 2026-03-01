class ContactData {
    id: string = '00000000-0000-0000-0000-000000000000';
    firstName: string = '';
    lastName: string = '';
    email: string = '';
    phone: string = '';
    organizationName: string = '';
    title: string = '';
    address: string = '';
    city: string = '';
    region: string = '';
    postalCode: string = '';
    country: string = '';
    contactType: number = 1;
    source: string = '';
    userId: string | null = null;
    partnerId: string | null = null;
    notes: string = '';
    isActive: boolean = true;
    createdByUserId: string = '';
    createdDate: string = '';
    lastUpdatedByUserId: string = '';
    lastUpdatedDate: string = '';
}

export default ContactData;
