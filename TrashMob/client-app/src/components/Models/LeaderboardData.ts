// Leaderboard data types

export interface LeaderboardEntry {
    entityId: string;
    entityName: string;
    entityType: string;
    rank: number;
    score: number;
    formattedScore: string;
    region?: string;
    city?: string;
    profilePhotoUrl?: string;
}

export interface LeaderboardResponse {
    leaderboardType: string;
    timeRange: string;
    locationScope: string;
    locationValue?: string;
    computedDate: string;
    totalEntries: number;
    entries: LeaderboardEntry[];
}

export interface UserRankResponse {
    leaderboardType: string;
    timeRange: string;
    rank?: number;
    score?: number;
    formattedScore?: string;
    totalRanked: number;
    isEligible: boolean;
    ineligibleReason?: string;
}

export interface TeamRankResponse {
    teamId: string;
    teamName?: string;
    leaderboardType: string;
    timeRange: string;
    rank?: number;
    score?: number;
    formattedScore?: string;
    totalRanked: number;
    isEligible: boolean;
    ineligibleReason?: string;
}

export interface LeaderboardOptions {
    types: string[];
    timeRanges: string[];
    locationScopes: string[];
}

// Display labels for leaderboard types
export const LeaderboardTypeLabels: Record<string, string> = {
    Events: 'Events Attended',
    Bags: 'Bags Collected',
    Weight: 'Weight Picked (lbs)',
    Hours: 'Hours Volunteered',
};

// Display labels for time ranges
export const TimeRangeLabels: Record<string, string> = {
    Week: 'This Week',
    Month: 'This Month',
    Year: 'This Year',
    AllTime: 'All Time',
};

// Display labels for location scopes
export const LocationScopeLabels: Record<string, string> = {
    Global: 'Global',
    Region: 'Region',
    City: 'City',
};
