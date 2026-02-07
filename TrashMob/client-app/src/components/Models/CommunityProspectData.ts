class CommunityProspectData {
    id: string = '00000000-0000-0000-0000-000000000000';
    name: string = '';
    type: string = 'Municipality';
    city: string = '';
    region: string = '';
    country: string = 'United States';
    latitude: number | null = null;
    longitude: number | null = null;
    population: number | null = null;
    website: string = '';
    contactEmail: string = '';
    contactName: string = '';
    contactTitle: string = '';
    pipelineStage: number = 0;
    fitScore: number = 0;
    notes: string = '';
    lastContactedDate: string | null = null;
    nextFollowUpDate: string | null = null;
    convertedPartnerId: string | null = null;
    createdDate: string = '';
    lastUpdatedDate: string = '';
}

export default CommunityProspectData;
