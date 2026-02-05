// Shareable content types for social sharing

export type ShareableContentType = 'event' | 'team' | 'community';

export interface ShareableContent {
    type: ShareableContentType;
    title: string;
    description: string;
    url: string;
    imageUrl?: string;
    location?: string;
    date?: Date;
}

export type SocialPlatform =
    | 'facebook'
    | 'twitter'
    | 'bluesky'
    | 'linkedin'
    | 'reddit'
    | 'whatsapp'
    | 'email'
    | 'copy'
    | 'qr';
