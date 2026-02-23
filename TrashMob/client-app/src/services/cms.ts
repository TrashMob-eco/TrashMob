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

// Strapi collection response wrapper (for collection types like news)
interface StrapiCollectionResponse<T> {
    data: Array<{ id: number; attributes: T }>;
    meta: { pagination: { page: number; pageSize: number; pageCount: number; total: number } };
}

// News Category types
export interface NewsCategoryData {
    name: string;
    slug: string;
    description?: string;
}

// News Post types
export interface NewsPostData {
    title: string;
    slug: string;
    excerpt: string;
    body: unknown; // Strapi Blocks JSON — rendered by @strapi/blocks-react-renderer
    author: string;
    publishedAt: string;
    isFeatured: boolean;
    estimatedReadTime?: number;
    tags?: string[];
    coverImage?: {
        data?: {
            attributes: {
                url: string;
                alternativeText?: string;
                formats?: Record<string, { url: string; width: number; height: number }>;
            };
        };
    };
    category?: {
        data: {
            id: number;
            attributes: NewsCategoryData;
        } | null;
    };
}

// News service functions
export interface NewsPostsParams {
    page?: number;
    pageSize?: number;
    category?: string;
}

export type GetNewsPosts_Response = StrapiCollectionResponse<NewsPostData>;
export const GetNewsPosts = (params: NewsPostsParams = {}) => ({
    key: ['/cms/news-posts', params],
    service: async () => {
        const searchParams = new URLSearchParams();
        if (params.page) searchParams.set('page', String(params.page));
        if (params.pageSize) searchParams.set('pageSize', String(params.pageSize));
        if (params.category) searchParams.set('category', params.category);
        const qs = searchParams.toString();
        return ApiService('public')
            .fetchData<GetNewsPosts_Response>({ url: `/cms/news-posts${qs ? `?${qs}` : ''}`, method: 'get' })
            .then((res) => res.data)
            .catch(() => ({ data: [], meta: { pagination: { page: 1, pageSize: 10, pageCount: 0, total: 0 } } }));
    },
});

export type GetNewsPostBySlug_Response = StrapiCollectionResponse<NewsPostData>;
export const GetNewsPostBySlug = (slug: string) => ({
    key: ['/cms/news-posts', slug],
    service: async () =>
        ApiService('public')
            .fetchData<GetNewsPostBySlug_Response>({ url: `/cms/news-posts/${slug}`, method: 'get' })
            .then((res) => res.data?.data?.[0]?.attributes ?? null)
            .catch(() => null),
});

export type GetNewsCategories_Response = StrapiCollectionResponse<NewsCategoryData>;
export const GetNewsCategories = () => ({
    key: ['/cms/news-categories'],
    service: async () =>
        ApiService('public')
            .fetchData<GetNewsCategories_Response>({ url: '/cms/news-categories', method: 'get' })
            .then((res) => res.data?.data?.map((item) => item.attributes) ?? [])
            .catch(() => []),
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
