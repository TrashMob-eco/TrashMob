import { useCallback } from 'react';
import { trackEvent, trackPageView as trackPageViewFn } from '../services/appInsights';

/**
 * Event categories for feature metrics
 */
export type MetricCategory =
    | 'Auth'
    | 'Event'
    | 'Attendance'
    | 'LitterReport'
    | 'Partner'
    | 'Team'
    | 'Search'
    | 'Navigation'
    | 'Feedback';

/**
 * Common actions for feature metrics
 */
export type MetricAction =
    | 'View'
    | 'Click'
    | 'Submit'
    | 'Success'
    | 'Error'
    | 'Create'
    | 'Edit'
    | 'Delete'
    | 'Cancel'
    | 'Register'
    | 'Unregister'
    | 'Login'
    | 'Logout'
    | 'Signup'
    | 'Search'
    | 'Filter'
    | 'Join'
    | 'Leave';

/**
 * Feature metric event structure
 */
export interface FeatureMetricEvent {
    /** Category of the feature (e.g., 'Event', 'Auth', 'LitterReport') */
    category: MetricCategory;
    /** Action performed (e.g., 'Create', 'Submit', 'View') */
    action: MetricAction | string;
    /** Optional target/subject of the action */
    target?: string;
    /** Additional properties for the event */
    properties?: Record<string, string | number | boolean>;
}

/**
 * Build the event name from category, action, and target
 * Format: {Category}_{Action}_{Target}
 */
function buildEventName(event: FeatureMetricEvent): string {
    const parts = [event.category, event.action];
    if (event.target) {
        parts.push(event.target);
    }
    return parts.join('_');
}

/**
 * Hook for tracking feature usage metrics
 *
 * @example
 * ```tsx
 * function CreateEventPage() {
 *   const { track } = useFeatureMetrics();
 *
 *   const handleSubmit = async (data) => {
 *     track({
 *       category: 'Event',
 *       action: 'Create',
 *       target: 'Submit',
 *       properties: { eventType: data.eventType }
 *     });
 *     // ... submit logic
 *   };
 * }
 * ```
 */
export function useFeatureMetrics() {
    /**
     * Track a feature usage event
     */
    const track = useCallback((event: FeatureMetricEvent) => {
        const eventName = buildEventName(event);
        const properties: Record<string, string | number | boolean> = {
            ...event.properties,
            timestamp: new Date().toISOString(),
            category: event.category,
            action: event.action,
        };

        if (event.target) {
            properties.target = event.target;
        }

        trackEvent(eventName, properties);
    }, []);

    /**
     * Track a page view
     */
    const trackPageView = useCallback((pageName: string, properties?: Record<string, string | number | boolean>) => {
        trackPageViewFn(pageName, {
            ...properties,
            timestamp: new Date().toISOString(),
        });
    }, []);

    /**
     * Track authentication events
     */
    const trackAuth = useCallback(
        (action: 'Login' | 'Logout' | 'Signup', success: boolean, properties?: Record<string, string>) => {
            track({
                category: 'Auth',
                action,
                target: success ? 'Success' : 'Error',
                properties,
            });
        },
        [track],
    );

    /**
     * Track event lifecycle actions (create, edit, cancel, complete)
     */
    const trackEventAction = useCallback(
        (
            action: 'Create' | 'Edit' | 'Delete' | 'Cancel' | 'View',
            eventId?: string,
            properties?: Record<string, string | number | boolean>,
        ) => {
            track({
                category: 'Event',
                action,
                properties: {
                    ...properties,
                    ...(eventId && { eventId }),
                },
            });
        },
        [track],
    );

    /**
     * Track attendance actions (register, unregister, check-in)
     */
    const trackAttendance = useCallback(
        (
            action: 'Register' | 'Unregister',
            eventId: string,
            properties?: Record<string, string | number | boolean>,
        ) => {
            track({
                category: 'Attendance',
                action,
                properties: {
                    ...properties,
                    eventId,
                },
            });
        },
        [track],
    );

    /**
     * Track litter report actions
     */
    const trackLitterReport = useCallback(
        (
            action: 'Create' | 'Edit' | 'Delete' | 'View',
            litterReportId?: string,
            properties?: Record<string, string | number | boolean>,
        ) => {
            track({
                category: 'LitterReport',
                action,
                properties: {
                    ...properties,
                    ...(litterReportId && { litterReportId }),
                },
            });
        },
        [track],
    );

    /**
     * Track team actions
     */
    const trackTeam = useCallback(
        (
            action: 'Create' | 'Edit' | 'Delete' | 'View' | 'Join' | 'Leave',
            teamId?: string,
            properties?: Record<string, string | number | boolean>,
        ) => {
            track({
                category: 'Team',
                action,
                properties: {
                    ...properties,
                    ...(teamId && { teamId }),
                },
            });
        },
        [track],
    );

    /**
     * Track search actions
     */
    const trackSearch = useCallback(
        (searchType: string, resultCount: number, properties?: Record<string, string | number | boolean>) => {
            track({
                category: 'Search',
                action: 'Search',
                target: searchType,
                properties: {
                    ...properties,
                    resultCount,
                },
            });
        },
        [track],
    );

    return {
        track,
        trackPageView,
        trackAuth,
        trackEventAction,
        trackAttendance,
        trackLitterReport,
        trackTeam,
        trackSearch,
    };
}

export default useFeatureMetrics;
