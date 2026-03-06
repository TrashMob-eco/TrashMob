class ContactNoteData {
    id: string = '00000000-0000-0000-0000-000000000000';
    contactId: string = '';
    noteType: number = 1;
    subject: string = '';
    body: string = '';
    createdByUserId: string | null = null;
    createdDate: string | null = null;
    lastUpdatedByUserId: string | null = null;
    lastUpdatedDate: string | null = null;
}

export default ContactNoteData;
