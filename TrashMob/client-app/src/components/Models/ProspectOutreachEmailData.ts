class ProspectOutreachEmailData {
    id: string = '00000000-0000-0000-0000-000000000000';
    prospectId: string = '';
    cadenceStep: number = 0;
    subject: string = '';
    htmlBody: string = '';
    status: string = 'Draft';
    sentDate: string | null = null;
    openedDate: string | null = null;
    clickedDate: string | null = null;
    errorMessage: string = '';
    createdDate: string | null = null;
}

export default ProspectOutreachEmailData;
