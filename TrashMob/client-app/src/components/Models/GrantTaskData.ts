class GrantTaskData {
    id: string = '00000000-0000-0000-0000-000000000000';
    grantId: string = '';
    title: string = '';
    dueDate: string | null = null;
    isCompleted: boolean = false;
    completedDate: string | null = null;
    sortOrder: number = 0;
    notes: string = '';
    createdByUserId: string = '';
    createdDate: string = '';
    lastUpdatedByUserId: string = '';
    lastUpdatedDate: string = '';
}

export default GrantTaskData;
