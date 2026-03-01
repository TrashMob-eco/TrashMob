class PledgeData {
    id: string = '00000000-0000-0000-0000-000000000000';
    contactId: string = '';
    totalAmount: number = 0;
    startDate: string = '';
    endDate: string | null = null;
    frequency: number = 1;
    status: number = 1;
    notes: string = '';
    createdByUserId: string = '';
    createdDate: string = '';
    lastUpdatedByUserId: string = '';
    lastUpdatedDate: string = '';
}

export default PledgeData;
