// Achievement data types

export interface AchievementDto {
    id: number;
    name: string;
    displayName: string;
    description: string;
    category: string;
    iconUrl?: string;
    points: number;
    isEarned: boolean;
    earnedDate?: string;
}

export interface UserAchievementsResponse {
    userId: string;
    totalPoints: number;
    earnedCount: number;
    totalCount: number;
    achievements: AchievementDto[];
}

export interface NewAchievementNotification {
    achievement: AchievementDto;
    earnedDate: string;
}

export interface AchievementType {
    id: number;
    name: string;
    displayName: string;
    description: string;
    category: string;
    iconUrl?: string;
    criteria: string;
    points: number;
    displayOrder: number;
    isActive: boolean;
}

// Display labels for achievement categories
export const AchievementCategoryLabels: Record<string, string> = {
    Participation: 'Participation',
    Impact: 'Impact',
    Special: 'Special',
};

// Category icons (emoji fallbacks)
export const AchievementCategoryIcons: Record<string, string> = {
    Participation: '🎯',
    Impact: '🌟',
    Special: '⭐',
};
