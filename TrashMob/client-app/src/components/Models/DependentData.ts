class DependentData {
    id: string = '00000000-0000-0000-0000-000000000000';
    parentUserId: string = '';
    firstName: string = '';
    lastName: string = '';
    dateOfBirth: string = '';
    relationship: string = 'Parent';
    medicalNotes: string = '';
    emergencyContactPhone: string = '';
    email: string = '';
    isActive: boolean = true;
    privoConsentStatus: number | null = null;
}

export default DependentData;
