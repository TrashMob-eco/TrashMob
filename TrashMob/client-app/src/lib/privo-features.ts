/** PRIVO feature identifiers for permission gating. */
export const PrivoFeature = {
    Account: 'trashmobservice_account',
    Leaderboard: 'trashmobservice_leaderboard',
    Social: 'trashmobservice_social',
    Newsletter: 'trashmobservice_newsletter',
    Notifications: 'trashmobservice_notifications',
    Geolocation: 'trashmobservice_geolocation',
    Team: 'trashmobservice_team',
    PhotoUploads: 'trashmobservice_photo_uploads',
} as const;

export type PrivoFeatureId = (typeof PrivoFeature)[keyof typeof PrivoFeature];
