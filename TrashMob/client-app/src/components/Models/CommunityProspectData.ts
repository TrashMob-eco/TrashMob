import ProspectContactData from './ProspectContactData';

class CommunityProspectData {
    id: string = '00000000-0000-0000-0000-000000000000';
    name: string = '';
    // Constrained to MunicipalityTypeEnum names — see MUNICIPALITY_TYPES.
    type: string = 'City';
    // Original free-form Type value preserved by the Project 63 migration.
    // Read-only from the UI's perspective.
    typeRaw: string | null = null;
    // Department within the municipality being worked (e.g. Public Works).
    department: string = '';
    // Sales priority (see PROSPECT_PRIORITIES). Null when unset.
    priority: number | null = null;
    // Free-text feedback captured during outreach.
    pricingFeedback: string = '';
    keyObjection: string = '';
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
