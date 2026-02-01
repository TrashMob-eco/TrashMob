/**
 * Application Insights service wrapper
 * Provides typed access to the global appInsights object initialized in index.html
 */

// Type definition for the global appInsights SDK
interface AppInsightsSDK {
    trackEvent: (event: { name: string; properties?: Record<string, string | number | boolean> }) => void;
    trackPageView: (pageView: { name: string; properties?: Record<string, string | number | boolean> }) => void;
    trackException: (exception: { exception: Error; properties?: Record<string, string> }) => void;
    trackTrace: (trace: { message: string; properties?: Record<string, string> }) => void;
    flush: () => void;
}

// Declare the global appInsights object created by the SDK snippet in index.html
declare global {
    interface Window {
        appInsights?: AppInsightsSDK;
    }
}

/**
 * Get the Application Insights SDK instance
 * Returns null if not initialized
 */
export function getAppInsights(): AppInsightsSDK | null {
    return window.appInsights || null;
}

/**
 * Check if Application Insights is available
 */
export function isAppInsightsAvailable(): boolean {
    return !!window.appInsights;
}

/**
 * Track a custom event in Application Insights
 */
export function trackEvent(name: string, properties?: Record<string, string | number | boolean>): void {
    const appInsights = getAppInsights();
    if (appInsights) {
        appInsights.trackEvent({ name, properties });
    }
}

/**
 * Track a page view in Application Insights
 */
export function trackPageView(name: string, properties?: Record<string, string | number | boolean>): void {
    const appInsights = getAppInsights();
    if (appInsights) {
        appInsights.trackPageView({ name, properties });
    }
}

/**
 * Track an exception in Application Insights
 */
export function trackException(error: Error, properties?: Record<string, string>): void {
    const appInsights = getAppInsights();
    if (appInsights) {
        appInsights.trackException({ exception: error, properties });
    }
}

/**
 * Flush any pending telemetry
 */
export function flush(): void {
    const appInsights = getAppInsights();
    if (appInsights) {
        appInsights.flush();
    }
}
