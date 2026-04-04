class DependentData {
    id: string = '';
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
