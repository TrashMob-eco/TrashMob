import ProspectContactData from './ProspectContactData';

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
    // Legacy primary-contact shortcuts: still read on POST/PUT by the backend to upsert
    // the primary contact, populated on read from the primary ProspectContact. Frontend
    // forms continue to use these for the time being; the dedicated contacts list (below)
    // is the source of truth for everything else.
    contactEmail: string = '';
    contactName: string = '';
    contactTitle: string = '';
    contactPhone: string = '';
    pipelineStage: number = 0;
    fitScore: number = 0;
    notes: string = '';
    lastContactedDate: string | null = null;
    nextFollowUpDate: string | null = null;
    convertedPartnerId: string | null = null;
    contacts: ProspectContactData[] = [];
    createdDate: string | null = null;
    lastUpdatedDate: string | null = null;
}

export default CommunityProspectData;
