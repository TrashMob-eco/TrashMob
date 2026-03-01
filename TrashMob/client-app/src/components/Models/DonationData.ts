class DonationData {
    id: string = '00000000-0000-0000-0000-000000000000';
    contactId: string = '';
    amount: number = 0;
    donationDate: string = '';
    donationType: number = 1;
    campaign: string = '';
    isRecurring: boolean = false;
    recurringFrequency: number | null = null;
    pledgeId: string | null = null;
    inKindDescription: string = '';
    matchingGiftEmployer: string = '';
    notes: string = '';
    receiptSent: boolean = false;
    thankYouSent: boolean = false;
    createdByUserId: string = '';
    createdDate: string = '';
    lastUpdatedByUserId: string = '';
    lastUpdatedDate: string = '';
}

export default DonationData;
