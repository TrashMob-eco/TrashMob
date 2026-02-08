import CommunityData from '@/components/Models/CommunityData';

// RegionType enum values (mirrors backend RegionTypeEnum)
export const RegionType = {
    City: 0,
    County: 1,
    State: 2,
    Province: 3,
    Region: 4,
    Country: 5,
} as const;

export function getLocation(community: CommunityData): string {
    if (community.regionType === RegionType.County && community.countyName) {
        return [community.countyName, community.region, community.country].filter(Boolean).join(', ');
    }
    if (community.regionType === RegionType.State || community.regionType === RegionType.Province) {
        return [community.region, community.country].filter(Boolean).join(', ');
    }
    // City (default) or null regionType — backward compatible
    return [community.city, community.region, community.country].filter(Boolean).join(', ') || 'Location not specified';
}

export function getRegionTypeLabel(regionType: number | null): string | null {
    switch (regionType) {
        case RegionType.County:
            return 'County';
        case RegionType.State:
            return 'State';
        case RegionType.Province:
            return 'Province';
        case RegionType.Region:
            return 'Region';
        case RegionType.Country:
            return 'Country';
        default:
            return null; // City or null — no badge needed
    }
}
