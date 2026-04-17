const CONSENT_KEY = 'trashmob_cookie_consent';

interface CookieConsent {
    analytics: boolean;
    timestamp: string;
}

export function getConsent(): CookieConsent | null {
    try {
        const raw = localStorage.getItem(CONSENT_KEY);
        if (!raw) return null;
        return JSON.parse(raw) as CookieConsent;
    } catch {
        return null;
    }
}

export function setConsent(analytics: boolean): void {
    const consent: CookieConsent = { analytics, timestamp: new Date().toISOString() };
    localStorage.setItem(CONSENT_KEY, JSON.stringify(consent));
}

const GA4_MEASUREMENT_ID = 'G-T9F2NDS042';

let clarityLoaded = false;
let appInsightsLoaded = false;
let ga4Loaded = false;

export function loadClarity(): void {
    if (clarityLoaded || !import.meta.env.PROD) return;
    clarityLoaded = true;

    (function (c: Window, l: Document, a: string, r: string, i: string) {
        const w = c as unknown as Record<string, unknown>;
        w[a] =
            w[a] ||
            function (...args: unknown[]) {
                ((w[a] as { q?: unknown[] }).q = (w[a] as { q?: unknown[] }).q || []).push(args);
            };
        const t = l.createElement(r) as HTMLScriptElement;
        t.async = true;
        t.src = 'https://www.clarity.ms/tag/' + i;
        const y = l.getElementsByTagName(r)[0];
        y?.parentNode?.insertBefore(t, y);
    })(window, document, 'clarity', 'script', 'az7h8gs917');
}

export function loadGoogleAnalytics(): void {
    if (ga4Loaded || !import.meta.env.PROD) return;
    ga4Loaded = true;

    const script = document.createElement('script');
    script.async = true;
    script.src = `https://www.googletagmanager.com/gtag/js?id=${GA4_MEASUREMENT_ID}`;
    document.head.appendChild(script);

    const w = window as unknown as Record<string, unknown>;
    if (!w.dataLayer) w.dataLayer = [];
    const dataLayer = w.dataLayer as unknown[];
    function gtag(...args: unknown[]) {
        dataLayer.push(args);
    }
    gtag('js', new Date());
    gtag('config', GA4_MEASUREMENT_ID);
}

export function loadAppInsights(): void {
    if (appInsightsLoaded) return;
    appInsightsLoaded = true;

    import('../services/config')
        .then(({ getAppConfig }) => getAppConfig())
        .then((config) => {
            const instrumentationKey = config.applicationInsightsKey;
            if (!instrumentationKey) {
                console.warn('Application Insights instrumentation key not available');
                return;
            }

            const script = document.createElement('script');
            script.src = 'https://js.monitor.azure.com/scripts/b/ai.2.min.js';
            script.crossOrigin = 'anonymous';
            script.onload = () => {
                const ai = (window as unknown as Record<string, unknown>).appInsights;
                if (ai && typeof (ai as { trackPageView?: () => void }).trackPageView === 'function') {
                    (ai as { trackPageView: (opts: Record<string, unknown>) => void }).trackPageView({});
                }
            };

            // Initialize the SDK snippet inline before the script loads
            initAppInsightsSnippet(instrumentationKey);
            document.head.appendChild(script);
        })
        .catch((error: unknown) => {
            console.warn('Failed to fetch Application Insights config:', error);
        });
}

function initAppInsightsSnippet(instrumentationKey: string): void {
    const w = window as unknown as Record<string, unknown>;
    const sdkName = 'appInsights';
    w.appInsightsSDK = sdkName;

    const appInsights = {
        config: { instrumentationKey },
        initialize: true,
        queue: [] as Array<() => void>,
        sv: '5',
        version: 2,
        SeverityLevel: { Verbose: 0, Information: 1, Warning: 2, Error: 3, Critical: 4 },
    } as Record<string, unknown>;

    // Create stub methods that queue calls until SDK loads
    const methods = [
        'trackEvent',
        'trackPageView',
        'trackException',
        'trackTrace',
        'trackDependencyData',
        'trackMetric',
        'trackPageViewPerformance',
        'startTrackPage',
        'stopTrackPage',
        'startTrackEvent',
        'stopTrackEvent',
        'addTelemetryInitializer',
        'setAuthenticatedUserContext',
        'clearAuthenticatedUserContext',
        'flush',
    ];

    for (const method of methods) {
        appInsights[method] = function (...args: unknown[]) {
            (appInsights.queue as Array<() => void>).push(() => {
                (appInsights[method] as (...a: unknown[]) => void)?.apply(appInsights, args);
            });
        };
    }

    w[sdkName] = appInsights;
}

function fireGoogleAdsConversion(sendTo: string, value?: number): void {
    const w = window as unknown as Record<string, unknown>;
    const dataLayer = w.dataLayer as unknown[] | undefined;
    if (!dataLayer) return;

    function gtag(...args: unknown[]) {
        dataLayer!.push(args);
    }
    const params: Record<string, unknown> = { send_to: sendTo };
    if (value !== undefined) {
        params.value = value;
        params.currency = 'USD';
    }
    gtag('event', 'conversion', params);
}

/** Fire a Google Ads conversion event for new sign-ups. */
export function trackSignUpConversion(): void {
    fireGoogleAdsConversion('AW-18035648449/V0zjCL3Hh48cEMHPiJhD', 1.0);
}

/** Fire a Google Ads conversion event for event RSVPs. */
export function trackRsvpConversion(): void {
    fireGoogleAdsConversion('AW-18035648449/uhNBCKDbpJwcEMHPiJhD');
}

/** Fire a Google Ads conversion event for event creation by organizers. */
export function trackEventCreatedConversion(): void {
    fireGoogleAdsConversion('AW-18035648449/zEXMCKbbpJwcEMHPiJhD');
}

/** Fire a Google Ads conversion event for city partner inquiries. */
export function trackPartnerInquiryConversion(): void {
    fireGoogleAdsConversion('AW-18035648449/98NDCKPbpJwcEMHPiJhD');
}

/** Initialize analytics scripts if the user has previously consented. */
export function initAnalytics(): void {
    const consent = getConsent();
    if (consent?.analytics) {
        loadClarity();
        loadAppInsights();
        loadGoogleAnalytics();
    }
}
