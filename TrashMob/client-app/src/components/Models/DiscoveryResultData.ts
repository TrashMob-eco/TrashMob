import DiscoveredProspectData from './DiscoveredProspectData';

class DiscoveryResultData {
    prospects: DiscoveredProspectData[] = [];
    tokensUsed: number = 0;
    message: string | null = null;
}

export default DiscoveryResultData;
