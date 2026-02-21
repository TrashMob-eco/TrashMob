import { ApiService } from '.';

// Strapi response wrapper type
interface StrapiResponse<T> {
    data: {
        id: number;
        attributes: T;
    } | null;
    meta: Record<string, unknown>;
}

// Hero Section types
export interface HeroSectionData {
    tagline?: string;
    primaryButtonText: string;
    primaryButtonLink: string;
    secondaryButtonText?: string;
    secondaryButtonLink?: string;
    googlePlayUrl?: string;
    appStoreUrl?: string;
    backgroundImage?: {
        data?: {
            attributes: {
                url: string;
                alternativeText?: string;
            };
        };
    };
}

// What Is TrashMob Section types
export interface WhatIsTrashmobData {
    heading: string;
    description: string;
    youtubeVideoUrl?: string;
    primaryButtonText?: string;
    primaryButtonLink?: string;
    secondaryButtonText?: string;
    secondaryButtonLink?: string;
}

// Getting Started Section types
export interface RequirementItem {
    id: number;
    icon: string;
    label: string;
}

export interface GettingStartedData {
    heading: string;
    subheading?: string;
    requirements?: RequirementItem[];
    ctaButtonText?: string;
    ctaButtonLink?: string;
}

// Service functions
export type GetHeroSection_Response = StrapiResponse<HeroSectionData>;
export const GetHeroSection = () => ({
    key: ['/cms/hero-section'],
    service: async () =>
        ApiService('public')
            .fetchData<GetHeroSection_Response>({ url: '/cms/hero-section', method: 'get' })
            .then((res) => res.data?.data?.attributes ?? null)
            .catch(() => null),
});

export type GetWhatIsTrashmob_Response = StrapiResponse<WhatIsTrashmobData>;
export const GetWhatIsTrashmob = () => ({
    key: ['/cms/what-is-trashmob'],
    service: async () =>
        ApiService('public')
            .fetchData<GetWhatIsTrashmob_Response>({ url: '/cms/what-is-trashmob', method: 'get' })
            .then((res) => res.data?.data?.attributes ?? null)
            .catch(() => null),
});

export type GetGettingStarted_Response = StrapiResponse<GettingStartedData>;
export const GetGettingStarted = () => ({
    key: ['/cms/getting-started'],
    service: async () =>
        ApiService('public')
            .fetchData<GetGettingStarted_Response>({ url: '/cms/getting-started', method: 'get' })
            .then((res) => res.data?.data?.attributes ?? null)
            .catch(() => null),
});

// Admin URL service (protected)
export type GetCmsAdminUrl_Response = { adminUrl: string };
export const GetCmsAdminUrl = () => ({
    key: ['/cms/admin-url'],
    service: async () =>
        ApiService('protected')
            .fetchData<GetCmsAdminUrl_Response>({ url: '/cms/admin-url', method: 'get' })
            .then((res) => res.data),
});
