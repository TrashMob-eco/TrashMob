import { FC, Suspense, lazy, useEffect, useState } from 'react';
import { BrowserRouter, Outlet, Route, Routes, useLocation } from 'react-router';
import { Loader2 } from 'lucide-react';
import { HelmetProvider } from 'react-helmet-async';

import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from '@/components/ui/toaster';
import { FeedbackWidget } from './components/FeedbackWidget/FeedbackWidget';
import { ErrorBoundary } from './components/ErrorBoundary';
import { DefaultPageHead } from './components/SEO/PageHead';

import { initializeMsalClient } from './store/AuthStore';
import { Shop } from './components/Shop';

import 'react-phone-input-2/lib/style.css';
import { SiteFooter } from './components/SiteFooter';
import { SiteHeader } from './components/SiteHeader';
import { useLogin } from './hooks/useLogin';
import { PartnerContactType } from './enums/PartnerContactType';

/** 2024 pages */
import { Home } from './pages/_home';
import { TermsOfService } from './pages/termsofservice';
import { VolunteerOpportunities } from './pages/volunteeropportunities';
import { PrivacyPolicy } from './pages/privacypolicy';
import { Help } from './pages/help/page';
import { Faq } from './pages/faq/page';
import { AboutUs } from './pages/aboutus/page';
import { ContactUsWrapper as ContactUs } from './pages/contactus';
import { GettingStarted } from './pages/gettingstarted/page';
import { WhatsNew } from './pages/whatsnew/page';
import { Board } from './pages/board/page';

/** User */
import MyDashboard from './pages/MyDashboard';
import { LocationPreference } from './pages/locationpreference';
import { DeleteMyData } from './pages/deletemydata';
import Waivers from './pages/waivers/page';

/** Events */
import { CreateEventWrapper } from './pages/events/create';
import { EventDetails } from './pages/eventdetails/$eventId/page';
import { AttendeeMetricsReview } from './pages/eventdetails/$eventId/attendee-metrics/page';
import { EditEventPage } from './pages/events/edit';
import { CancelEvent } from './pages/events/$eventId/delete';
import { EditEventSummary } from './pages/eventsummary/$eventId';
import { PickupLocationCreate } from './pages/eventsummary/$eventId/pickup-locations.create';
import { PickupLocationEdit } from './pages/eventsummary/$eventId/pickup-locations.$locationId.edit';

/** Litter Reports */
import { LitterReportsPage } from './pages/litterreports';
import { LitterReportDetailPage } from './pages/litterreports/$litterReportId';
import { LitterReportEditPage } from './pages/litterreports/$litterReportId/edit';
import { CreateLitterReportPage } from './pages/litterreports/create';

/** Teams */
import { TeamsPage } from './pages/teams';
import { TeamDetailPage } from './pages/teams/$teamId';
import { CreateTeamPage } from './pages/teams/create';
import { TeamEditPage } from './pages/teams/$teamId/edit';
import { TeamInvitesPage } from './pages/teams/$teamId/invites';
import { TeamInviteDetailsPage } from './pages/teams/$teamId/invites/$batchId';

/** Leaderboards */
import { LeaderboardsPage } from './pages/leaderboards/page';

/** Achievements */
import { AchievementsPage } from './pages/achievements/page';

/** Communities - Public pages */
import { CommunitiesPage } from './pages/communities';
import { CommunityDetailPage } from './pages/communities/$slug';

/** Partners - Public pages */
import { Partnerships } from './pages/partnerships/page';
import { BecomeAPartnerPage } from './pages/_partnerRequest/becomeapartner';
import { InviteAPartnerPage } from './pages/_partnerRequest/inviteapartner';
import { PartnerRequestDetails } from './pages/partnerrequestdetails/page';

/** Unsubscribe */
import { UnsubscribePage } from './pages/unsubscribe';
import { NoMatch } from './pages/nomatch';

// Lazy loading fallback component
const LazyLoadingFallback = () => (
    <div className='flex justify-center items-center py-16'>
        <Loader2 className='animate-spin mr-2' /> Loading...
    </div>
);

/** Community Admin - Lazy loaded */
const CommunityAdminLayout = lazy(() =>
    import('./pages/communities/$slug/admin/_layout').then((m) => ({ default: m.CommunityAdminLayout })),
);
const CommunityAdminDashboard = lazy(() =>
    import('./pages/communities/$slug/admin').then((m) => ({ default: m.CommunityAdminDashboard })),
);
const CommunityContentEdit = lazy(() =>
    import('./pages/communities/$slug/admin/content').then((m) => ({ default: m.CommunityContentEdit })),
);
const CommunityAdminInvites = lazy(() =>
    import('./pages/communities/$slug/admin/invites').then((m) => ({ default: m.CommunityAdminInvites })),
);
const CommunityAdminInviteDetails = lazy(() =>
    import('./pages/communities/$slug/admin/invites/$batchId').then((m) => ({
        default: m.CommunityAdminInviteDetails,
    })),
);

/** Partner Dashboard - Lazy loaded (partner admin pages) */
const PartnerIndex = lazy(() =>
    import('./pages/partnerdashboard/$partnerId').then((m) => ({ default: m.PartnerIndex })),
);
const PartnerLayout = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/_layout').then((m) => ({ default: m.PartnerLayout })),
);
const PartnerEdit = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/edit').then((m) => ({ default: m.PartnerEdit })),
);
const PartnerLocations = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/locations').then((m) => ({ default: m.PartnerLocations })),
);
const PartnerContacts = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/contacts').then((m) => ({ default: m.PartnerContacts })),
);
const PartnerContactEdit = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/contacts.$contactId.edit').then((m) => ({
        default: m.PartnerContactEdit,
    })),
);
const PartnerContactCreate = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/contacts.create').then((m) => ({ default: m.PartnerContactCreate })),
);
const PartnerLocationEdit = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/locations.$locationId.edit').then((m) => ({
        default: m.PartnerLocationEdit,
    })),
);
const PartnerLocationCreate = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/locations.create').then((m) => ({ default: m.PartnerLocationCreate })),
);
const PartnerServices = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/services').then((m) => ({ default: m.PartnerServices })),
);
const PartnerServiceEdit = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/services.edit').then((m) => ({ default: m.PartnerServiceEdit })),
);
const PartnerServiceEnable = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/services.enable').then((m) => ({ default: m.PartnerServiceEnable })),
);
const PartnerDocuments = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/documents').then((m) => ({ default: m.PartnerDocuments })),
);
const PartnerDocumentEdit = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/documents.$documentId.edit').then((m) => ({
        default: m.PartnerDocumentEdit,
    })),
);
const PartnerDocumentCreate = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/documents.create').then((m) => ({ default: m.PartnerDocumentCreate })),
);
const PartnerSocialMediaAccounts = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/socials').then((m) => ({ default: m.PartnerSocialMediaAccounts })),
);
const PartnerSocialAcccountEdit = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/socials.$accountId.edit').then((m) => ({
        default: m.PartnerSocialAcccountEdit,
    })),
);
const PartnerSocialAcccountCreate = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/socials.create').then((m) => ({
        default: m.PartnerSocialAcccountCreate,
    })),
);
const PartnerAdmins = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/admins').then((m) => ({ default: m.PartnerAdmins })),
);
const PartnerAdminInvite = lazy(() =>
    import('./pages/partnerdashboard/$partnerId/admins.invite').then((m) => ({ default: m.PartnerAdminInvite })),
);

/** SiteAdmin - Lazy loaded (admin-only pages) */
const SiteAdminLayout = lazy(() => import('./pages/siteadmin/_layout').then((m) => ({ default: m.SiteAdminLayout })));
const SiteAdminUsers = lazy(() => import('./pages/siteadmin/users/page').then((m) => ({ default: m.SiteAdminUsers })));
const SiteAdminEvents = lazy(() =>
    import('./pages/siteadmin/events/page').then((m) => ({ default: m.SiteAdminEvents })),
);
const SiteAdminPartners = lazy(() =>
    import('./pages/siteadmin/partners/page').then((m) => ({ default: m.SiteAdminPartners })),
);
const SiteAdminTeams = lazy(() => import('./pages/siteadmin/teams/page').then((m) => ({ default: m.SiteAdminTeams })));
const SiteAdminPartnerRequests = lazy(() =>
    import('./pages/siteadmin/partner-requests/page').then((m) => ({ default: m.SiteAdminPartnerRequests })),
);
const SiteAdminJobOpportunities = lazy(() =>
    import('./pages/siteadmin/job-opportunities/page').then((m) => ({ default: m.SiteAdminJobOpportunities })),
);
const SiteAdminJobOpportunityCreate = lazy(() =>
    import('./pages/siteadmin/job-opportunities/create').then((m) => ({ default: m.SiteAdminJobOpportunityCreate })),
);
const SiteAdminJobOpportunityEdit = lazy(() =>
    import('./pages/siteadmin/job-opportunities/$jobId.edit').then((m) => ({ default: m.SiteAdminJobOpportunityEdit })),
);
const SiteAdminEmailTemplates = lazy(() =>
    import('./pages/siteadmin/email-templates').then((m) => ({ default: m.SiteAdminEmailTemplates })),
);
const SiteAdminSendNotification = lazy(() =>
    import('./pages/siteadmin/send-notification').then((m) => ({ default: m.SiteAdminSendNotification })),
);
const SiteAdminContent = lazy(() => import('./pages/siteadmin/content').then((m) => ({ default: m.SiteAdminContent })));
const SiteAdminLitterReports = lazy(() =>
    import('./pages/siteadmin/litter-reports/page').then((m) => ({ default: m.SiteAdminLitterReports })),
);
const SiteAdminFeedback = lazy(() =>
    import('./pages/siteadmin/feedback/page').then((m) => ({ default: m.SiteAdminFeedback })),
);
const SiteAdminPhotoModeration = lazy(() =>
    import('./pages/siteadmin/photo-moderation/page').then((m) => ({ default: m.SiteAdminPhotoModeration })),
);
const SiteAdminWaivers = lazy(() =>
    import('./pages/siteadmin/waivers/page').then((m) => ({ default: m.SiteAdminWaivers })),
);
const SiteAdminWaiverCreate = lazy(() =>
    import('./pages/siteadmin/waivers/create').then((m) => ({ default: m.SiteAdminWaiverCreate })),
);
const SiteAdminWaiverEdit = lazy(() =>
    import('./pages/siteadmin/waivers/$waiverId.edit').then((m) => ({ default: m.SiteAdminWaiverEdit })),
);
const WaiverComplianceDashboard = lazy(() =>
    import('./pages/siteadmin/waivers/compliance').then((m) => ({ default: m.WaiverComplianceDashboard })),
);
const SiteAdminInvites = lazy(() =>
    import('./pages/siteadmin/invites/page').then((m) => ({ default: m.SiteAdminInvites })),
);
const SiteAdminInviteDetails = lazy(() =>
    import('./pages/siteadmin/invites/$batchId').then((m) => ({ default: m.SiteAdminInviteDetails })),
);
const SiteAdminNewsletters = lazy(() =>
    import('./pages/siteadmin/newsletters/page').then((m) => ({ default: m.SiteAdminNewsletters })),
);
const SiteAdminProspects = lazy(() =>
    import('./pages/siteadmin/prospects/page').then((m) => ({ default: m.SiteAdminProspects })),
);
const SiteAdminProspectCreate = lazy(() =>
    import('./pages/siteadmin/prospects/create').then((m) => ({ default: m.SiteAdminProspectCreate })),
);
const SiteAdminProspectEdit = lazy(() =>
    import('./pages/siteadmin/prospects/$prospectId.edit').then((m) => ({ default: m.SiteAdminProspectEdit })),
);
const SiteAdminProspectDetail = lazy(() =>
    import('./pages/siteadmin/prospects/$prospectId').then((m) => ({ default: m.SiteAdminProspectDetail })),
);
const SiteAdminProspectDiscovery = lazy(() =>
    import('./pages/siteadmin/prospects/discovery').then((m) => ({ default: m.SiteAdminProspectDiscovery })),
);
const SiteAdminProspectImport = lazy(() =>
    import('./pages/siteadmin/prospects/import').then((m) => ({ default: m.SiteAdminProspectImport })),
);
const SiteAdminProspectAnalytics = lazy(() =>
    import('./pages/siteadmin/prospects/analytics').then((m) => ({ default: m.SiteAdminProspectAnalytics })),
);

const queryClient = new QueryClient();

// Component for Listening to pathname change, then scroll to top
function ScrollToTop() {
    const { pathname } = useLocation();

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [pathname]);

    return null;
}

function AuthenticationLoadingComponent() {
    return <p>Authentication in progress...</p>;
}

function AuthenticationErrorComponent(result: MsalAuthenticationResult) {
    return (
        <p>
            An Error Occurred:
            {result.error?.errorCode} {result.error?.errorMessage}
        </p>
    );
}

const AuthLayout = () => {
    return (
        <MsalAuthenticationTemplate
            interactionType={InteractionType.Redirect}
            errorComponent={AuthenticationErrorComponent}
            loadingComponent={AuthenticationLoadingComponent}
        >
            <Outlet />
        </MsalAuthenticationTemplate>
    );
};

const AuthSideAdminLayout = () => {
    const { currentUser, isUserLoaded } = useLogin();
    if (!isUserLoaded)
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Loading
            </div>
        );
    if (isUserLoaded && !currentUser.isSiteAdmin) return <em>Access Denied</em>;

    return (
        <MsalAuthenticationTemplate
            interactionType={InteractionType.Redirect}
            errorComponent={AuthenticationErrorComponent}
            loadingComponent={AuthenticationLoadingComponent}
        >
            <Outlet />
        </MsalAuthenticationTemplate>
    );
};

// Inner component that uses MSAL hooks - must be rendered inside MsalProvider
const AppContent: FC = () => {
    const { currentUser, isUserLoaded } = useLogin();

    return (
        <div className='flex flex-col h-100'>
            <BrowserRouter>
                <ScrollToTop />
                <DefaultPageHead />
                <SiteHeader currentUser={currentUser} isUserLoaded={isUserLoaded} />
                <div className='container-fluid px-0'>
                    <Routes>
                        <Route element={<AuthLayout />}>
                            <Route path='/events'>
                                <Route path='create' element={<CreateEventWrapper />} />
                                <Route path=':eventId/edit' element={<EditEventPage />} />
                            </Route>
                            <Route path='/eventsummary'>
                                <Route path=':eventId' element={<EditEventSummary />}>
                                    <Route path='pickup-locations/create' element={<PickupLocationCreate />} />
                                    <Route path='pickup-locations/:locationId/edit' element={<PickupLocationEdit />} />
                                </Route>
                            </Route>
                            <Route path='partnerdashboard'>
                                <Route index element={<div>Partner Dashboard Index</div>} />
                                <Route
                                    path=':partnerId'
                                    element={
                                        <Suspense fallback={<LazyLoadingFallback />}>
                                            <PartnerLayout />
                                        </Suspense>
                                    }
                                >
                                    <Route index element={<PartnerIndex />} />
                                    <Route path='edit' element={<PartnerEdit />} />
                                    <Route path='locations' element={<PartnerLocations />}>
                                        <Route path='create' element={<PartnerLocationCreate />} />
                                        <Route path=':locationId/edit' element={<PartnerLocationEdit />} />
                                    </Route>
                                    <Route path='services' element={<PartnerServices />}>
                                        <Route path='enable' element={<PartnerServiceEnable />} />
                                        <Route path='edit' element={<PartnerServiceEdit />} />
                                    </Route>
                                    <Route path='contacts' element={<PartnerContacts />}>
                                        <Route path='create' element={<PartnerContactCreate />} />
                                        <Route
                                            path=':contactId/edit'
                                            element={<PartnerContactEdit type={PartnerContactType.ORGANIZATION_WIDE} />}
                                        />
                                        <Route
                                            path='by-location/:contactId/edit'
                                            element={<PartnerContactEdit type={PartnerContactType.LOCATION_SPECIFIC} />}
                                        />
                                    </Route>
                                    <Route path='admins' element={<PartnerAdmins />}>
                                        <Route path='invite' element={<PartnerAdminInvite />} />
                                    </Route>
                                    <Route path='documents' element={<PartnerDocuments />}>
                                        <Route path=':documentId/edit' element={<PartnerDocumentEdit />} />
                                        <Route path='create' element={<PartnerDocumentCreate />} />
                                    </Route>
                                    <Route path='socials' element={<PartnerSocialMediaAccounts />}>
                                        <Route path=':accountId/edit' element={<PartnerSocialAcccountEdit />} />
                                        <Route path='create' element={<PartnerSocialAcccountCreate />} />
                                    </Route>
                                </Route>
                            </Route>

                            <Route path='/cancelevent/:eventId' element={<CancelEvent />} />
                            <Route path='/deletemydata' element={<DeleteMyData />} />
                            <Route path='/mydashboard' element={<MyDashboard />} />
                            <Route path='/becomeapartner' element={<BecomeAPartnerPage />} />
                            <Route path='/inviteapartner' element={<InviteAPartnerPage />} />
                            <Route path='/locationpreference' element={<LocationPreference />} />
                            <Route path='/waivers' element={<Waivers />} />
                            <Route path='/achievements' element={<AchievementsPage />} />
                            <Route path='/litterreports/create' element={<CreateLitterReportPage />} />
                            <Route path='/litterreports/:litterReportId/edit' element={<LitterReportEditPage />} />
                            <Route path='/teams/create' element={<CreateTeamPage />} />
                            <Route path='/teams/:teamId/edit' element={<TeamEditPage />} />
                            <Route path='/teams/:teamId/invites' element={<TeamInvitesPage />} />
                            <Route path='/teams/:teamId/invites/:batchId' element={<TeamInviteDetailsPage />} />
                            <Route
                                path='/communities/:slug/admin'
                                element={
                                    <Suspense fallback={<LazyLoadingFallback />}>
                                        <CommunityAdminLayout />
                                    </Suspense>
                                }
                            >
                                <Route index element={<CommunityAdminDashboard />} />
                                <Route path='content' element={<CommunityContentEdit />} />
                                <Route path='invites' element={<CommunityAdminInvites />} />
                                <Route path='invites/:batchId' element={<CommunityAdminInviteDetails />} />
                            </Route>
                        </Route>
                        <Route element={<AuthSideAdminLayout />}>
                            <Route
                                path='/siteadmin'
                                element={
                                    <Suspense fallback={<LazyLoadingFallback />}>
                                        <SiteAdminLayout />
                                    </Suspense>
                                }
                            >
                                <Route path='users' element={<SiteAdminUsers />} />
                                <Route path='events' element={<SiteAdminEvents />} />
                                <Route path='partners' element={<SiteAdminPartners />} />
                                <Route path='teams' element={<SiteAdminTeams />} />
                                <Route path='litter-reports' element={<SiteAdminLitterReports />} />
                                <Route path='partner-requests' element={<SiteAdminPartnerRequests />} />
                                <Route path='job-opportunities' element={<SiteAdminJobOpportunities />}>
                                    <Route path=':jobId/edit' element={<SiteAdminJobOpportunityEdit />} />
                                    <Route path='create' element={<SiteAdminJobOpportunityCreate />} />
                                </Route>
                                <Route path='email-templates' element={<SiteAdminEmailTemplates />} />
                                <Route path='send-notifications' element={<SiteAdminSendNotification />} />
                                <Route path='content' element={<SiteAdminContent />} />
                                <Route path='feedback' element={<SiteAdminFeedback />} />
                                <Route path='photo-moderation' element={<SiteAdminPhotoModeration />} />
                                <Route path='waivers' element={<SiteAdminWaivers />}>
                                    <Route path=':waiverId/edit' element={<SiteAdminWaiverEdit />} />
                                    <Route path='create' element={<SiteAdminWaiverCreate />} />
                                </Route>
                                <Route path='waivers/compliance' element={<WaiverComplianceDashboard />} />
                                <Route path='invites' element={<SiteAdminInvites />} />
                                <Route path='invites/:batchId' element={<SiteAdminInviteDetails />} />
                                <Route path='newsletters' element={<SiteAdminNewsletters />} />
                                <Route path='prospects' element={<SiteAdminProspects />} />
                                <Route path='prospects/create' element={<SiteAdminProspectCreate />} />
                                <Route path='prospects/discovery' element={<SiteAdminProspectDiscovery />} />
                                <Route path='prospects/import' element={<SiteAdminProspectImport />} />
                                <Route path='prospects/analytics' element={<SiteAdminProspectAnalytics />} />
                                <Route path='prospects/:prospectId' element={<SiteAdminProspectDetail />} />
                                <Route path='prospects/:prospectId/edit' element={<SiteAdminProspectEdit />} />
                            </Route>
                        </Route>
                        <Route>
                            <Route
                                path='/partnerrequestdetails/:partnerRequestId'
                                element={<PartnerRequestDetails />}
                            />
                            <Route path='/eventdetails/:eventId?' element={<EventDetails />} />
                            <Route path='/eventdetails/:eventId/attendee-metrics' element={<AttendeeMetricsReview />} />
                            <Route path='/litterreports' element={<LitterReportsPage />} />
                            <Route path='/litterreports/:litterReportId' element={<LitterReportDetailPage />} />
                            <Route path='/teams' element={<TeamsPage />} />
                            <Route path='/teams/:teamId' element={<TeamDetailPage />} />
                            <Route path='/leaderboards' element={<LeaderboardsPage />} />
                            <Route path='/communities' element={<CommunitiesPage />} />
                            <Route path='/communities/:slug' element={<CommunityDetailPage />} />
                            <Route path='/partnerships' element={<Partnerships />} />
                            <Route path='/shop' element={<Shop />} />
                            <Route path='/help' element={<Help />} />
                            <Route path='/aboutus' element={<AboutUs />} />
                            <Route path='/board' element={<Board />} />
                            <Route path='/contactus' element={<ContactUs />} />
                            <Route path='/faq' element={<Faq />} />
                            <Route path='/gettingstarted' element={<GettingStarted />} />
                            <Route path='/whatsnew' element={<WhatsNew />} />
                            <Route path='/privacypolicy' element={<PrivacyPolicy />} />
                            <Route path='/termsofservice' element={<TermsOfService />} />
                            <Route path='/volunteeropportunities' element={<VolunteerOpportunities />} />
                            <Route path='/unsubscribe' element={<UnsubscribePage />} />
                            <Route path='/' element={<Home />} />
                        </Route>
                        <Route path='*' element={<NoMatch />} />
                    </Routes>
                </div>
                <SiteFooter />
            </BrowserRouter>
        </div>
    );
};

export const App: FC = () => {
    const [msalClient, setMsalClient] = useState<PublicClientApplication | null>(null);
    const [isInitializing, setIsInitializing] = useState(true);

    // Initialize MSAL client after config is loaded from backend
    useEffect(() => {
        initializeMsalClient()
            .then((client) => {
                setMsalClient(client);
                setIsInitializing(false);
            })
            .catch((error) => {
                console.error('Failed to initialize MSAL client:', error);
                setIsInitializing(false);
            });
    }, []);

    // Show loading while MSAL is initializing
    if (isInitializing || !msalClient) {
        return (
            <div className='flex justify-center items-center py-16 min-h-screen'>
                <Loader2 className='animate-spin mr-2' /> Loading...
            </div>
        );
    }

    return (
        <HelmetProvider>
            <QueryClientProvider client={queryClient}>
                <MsalProvider instance={msalClient}>
                    <ErrorBoundary>
                        <AppContent />
                    </ErrorBoundary>
                </MsalProvider>
                <Toaster />
                <FeedbackWidget />
                <ReactQueryDevtools initialIsOpen={false} />
            </QueryClientProvider>
        </HelmetProvider>
    );
};
