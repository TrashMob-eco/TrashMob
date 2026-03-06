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
    createdByUserId: string | null = null;
    createdDate: string | null = null;
    lastUpdatedByUserId: string | null = null;
    lastUpdatedDate: string | null = null;
}

export default DonationData;
