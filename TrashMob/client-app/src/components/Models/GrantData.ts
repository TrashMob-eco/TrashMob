class GrantData {
    id: string = '00000000-0000-0000-0000-000000000000';
    funderName: string = '';
    programName: string = '';
    description: string = '';
    amountMin: number | null = null;
    amountMax: number | null = null;
    amountAwarded: number | null = null;
    status: number = 1;
    submissionDeadline: string | null = null;
    awardDate: string | null = null;
    reportingDeadline: string | null = null;
    renewalDate: string | null = null;
    funderContactId: string | null = null;
    grantUrl: string = '';
    notes: string = '';
    createdByUserId: string = '';
    createdDate: string = '';
    lastUpdatedByUserId: string = '';
    lastUpdatedDate: string = '';
}

export default GrantData;
