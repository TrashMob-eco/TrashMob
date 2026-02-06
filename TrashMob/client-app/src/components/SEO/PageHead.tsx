import { Helmet } from 'react-helmet-async';
import { useLocation } from 'react-router';

interface PageHeadProps {
    title?: string;
    description?: string;
}

const BASE_URL = 'https://www.trashmob.eco';
const DEFAULT_DESCRIPTION =
    'Meet up. Clean up. Feel good! Cleaning up Planet Earth, one bucket of trash at a time. Together.';

/**
 * SEO component for managing page-specific meta tags and canonical URLs.
 * Automatically generates canonical URL based on current path.
 */
export const PageHead = ({ title, description }: PageHeadProps) => {
    const location = useLocation();

    // Generate canonical URL from current path (without query params or hash)
    const canonicalUrl = `${BASE_URL}${location.pathname}`;

    // Build the full title
    const fullTitle = title ? `${title} | TrashMob.eco` : 'TrashMob.eco';

    return (
        <Helmet>
            {/* Page Title */}
            <title>{fullTitle}</title>

            {/* Canonical URL - prevents duplicate content issues */}
            <link rel='canonical' href={canonicalUrl} />

            {/* Meta Description */}
            {description ? <meta name='description' content={description} /> : null}

            {/* Open Graph */}
            <meta property='og:url' content={canonicalUrl} />
            {title ? <meta property='og:title' content={fullTitle} /> : null}
            {description ? <meta property='og:description' content={description} /> : null}
        </Helmet>
    );
};

/**
 * Default SEO component that sets canonical URL based on current route.
 * Use this in the app layout to ensure every page has a canonical URL.
 */
export const DefaultPageHead = () => {
    const location = useLocation();
    const canonicalUrl = `${BASE_URL}${location.pathname}`;

    return (
        <Helmet>
            <link rel='canonical' href={canonicalUrl} />
            <meta property='og:url' content={canonicalUrl} />
        </Helmet>
    );
};
