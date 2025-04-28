import { FC, useEffect, useState } from 'react';
import { BrowserRouter, Outlet, Route, Routes, useLocation } from 'react-router';
import { Loader2 } from 'lucide-react';

import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from '@/components/ui/toaster';

import { msalClient } from './store/AuthStore';
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
import { Board } from './pages/board/page';

/** User */
import MyDashboard from './pages/mydashboard';
import { LocationPreferenceWrapper as LocationPreference } from './pages/locationpreference';
import { DeleteMyData } from './pages/deletemydata';
import Waivers from './pages/waivers/page';

/** Events */
import { CreateEventWrapper } from './pages/events/create';
import { EventDetails } from './pages/eventdetails/$eventId/page';
import { EditEventPage } from './pages/events/edit';
import { CancelEvent } from './pages/events/$eventId/delete';
import { EditEventSummary } from './pages/eventsummary/$eventId';
import { PickupLocationCreate } from './pages/eventsummary/$eventId/pickup-locations.create';
import { PickupLocationEdit } from './pages/eventsummary/$eventId/pickup-locations.$locationId.edit';

/** Partners */
import { Partnerships } from './pages/partnerships/page';
import { BecomeAPartnerPage } from './pages/_partnerRequest/becomeapartner';
import { InviteAPartnerPage } from './pages/_partnerRequest/inviteapartner';
import { PartnerRequestDetails } from './pages/partnerrequestdetails/page';
import { PartnerIndex } from './pages/partnerdashboard/$partnerId';
import { PartnerLayout } from './pages/partnerdashboard/$partnerId/_layout';
import { PartnerEdit } from './pages/partnerdashboard/$partnerId/edit';
import { PartnerLocations } from './pages/partnerdashboard/$partnerId/locations';
import { PartnerContacts } from './pages/partnerdashboard/$partnerId/contacts';
import { PartnerContactEdit } from './pages/partnerdashboard/$partnerId/contacts.$contactId.edit';
import { PartnerContactCreate } from './pages/partnerdashboard/$partnerId/contacts.create';
import { PartnerLocationEdit } from './pages/partnerdashboard/$partnerId/locations.$locationId.edit';
import { PartnerLocationCreate } from './pages/partnerdashboard/$partnerId/locations.create';
import { PartnerServices } from './pages/partnerdashboard/$partnerId/services';
import { PartnerServiceEdit } from './pages/partnerdashboard/$partnerId/services.edit';
import { PartnerServiceEnable } from './pages/partnerdashboard/$partnerId/services.enable';
import { PartnerDocuments } from './pages/partnerdashboard/$partnerId/documents';
import { PartnerDocumentEdit } from './pages/partnerdashboard/$partnerId/documents.$documentId.edit';
import { PartnerDocumentCreate } from './pages/partnerdashboard/$partnerId/documents.create';
import { PartnerSocialMediaAccounts } from './pages/partnerdashboard/$partnerId/socials';
import { PartnerSocialAcccountEdit } from './pages/partnerdashboard/$partnerId/socials.$accountId.edit';
import { PartnerSocialAcccountCreate } from './pages/partnerdashboard/$partnerId/socials.create';
import { PartnerAdmins } from './pages/partnerdashboard/$partnerId/admins';
import { PartnerAdminInvite } from './pages/partnerdashboard/$partnerId/admins.invite';

/** SiteAdmin */
import { SiteAdminLayout } from './pages/siteadmin/_layout';
import { SiteAdminUsers } from './pages/siteadmin/users/page';
import { SiteAdminEvents } from './pages/siteadmin/events/page';
import { SiteAdminPartners } from './pages/siteadmin/partners/page';
import { SiteAdminPartnerRequests } from './pages/siteadmin/partner-requests/page';
import { SiteAdminEmailTemplates } from './pages/siteadmin/email-templates';
import { SiteAdminSendNotification } from './pages/siteadmin/send-notification';
import { NoMatch } from './pages/nomatch';

const queryClient = new QueryClient();

const useInitializeApp = () => {
    const [isInitialized, setIsInitialized] = useState(false);
    useEffect(() => {
        if (isInitialized) {
            return;
        }
        setIsInitialized(true);
    }, [isInitialized]);
};

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
            errorComponent={AuthenticationErrorComponent}
            interactionType={InteractionType.Redirect}
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
            <div className='tailwind'>
                <div className='flex justify-center items-center py-16'>
                    <Loader2 className='animate-spin mr-2' /> Loading
                </div>
            </div>
        );
    if (isUserLoaded && !currentUser.isSiteAdmin) return <em>Access Denied</em>;

    return (
        <MsalAuthenticationTemplate
            errorComponent={AuthenticationErrorComponent}
            interactionType={InteractionType.Redirect}
            loadingComponent={AuthenticationLoadingComponent}
        >
            <Outlet />
        </MsalAuthenticationTemplate>
    );
};

export const App: FC = () => {
    useInitializeApp();
    const { currentUser, isUserLoaded, handleUserUpdated } = useLogin();

    return (
        <QueryClientProvider client={queryClient}>
            <MsalProvider instance={msalClient}>
                <div className='flex flex-col h-100'>
                    <BrowserRouter>
                        <ScrollToTop />
                        <SiteHeader currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        <div className='container-fluid px-0'>
                            <Routes>
                                <Route element={<AuthLayout />}>
                                    <Route path='/events'>
                                        <Route element={<CreateEventWrapper />} path='create' />
                                        <Route element={<EditEventPage />} path=':eventId/edit' />
                                    </Route>
                                    <Route path='/eventsummary'>
                                        <Route element={<EditEventSummary />} path=':eventId'>
                                            <Route element={<PickupLocationCreate />} path='pickup-locations/create' />
                                            <Route
                                                element={<PickupLocationEdit />}
                                                path='pickup-locations/:locationId/edit'
                                            />
                                        </Route>
                                    </Route>
                                    <Route path='partnerdashboard'>
                                        <Route element={<div>Partner Dashboard Index</div>} index />
                                        <Route element={<PartnerLayout />} path=':partnerId'>
                                            <Route element={<PartnerIndex />} index />
                                            <Route element={<PartnerEdit />} path='edit' />
                                            <Route element={<PartnerLocations />} path='locations'>
                                                <Route element={<PartnerLocationCreate />} path='create' />
                                                <Route element={<PartnerLocationEdit />} path=':locationId/edit' />
                                            </Route>
                                            <Route element={<PartnerServices />} path='services'>
                                                <Route element={<PartnerServiceEnable />} path='enable' />
                                                <Route element={<PartnerServiceEdit />} path='edit' />
                                            </Route>
                                            <Route element={<PartnerContacts />} path='contacts'>
                                                <Route element={<PartnerContactCreate />} path='create' />
                                                <Route
                                                    element={
                                                        <PartnerContactEdit
                                                            type={PartnerContactType.ORGANIZATION_WIDE}
                                                        />
                                                    }
                                                    path=':contactId/edit'
                                                />
                                                <Route
                                                    element={
                                                        <PartnerContactEdit
                                                            type={PartnerContactType.LOCATION_SPECIFIC}
                                                        />
                                                    }
                                                    path='by-location/:contactId/edit'
                                                />
                                            </Route>
                                            <Route element={<PartnerAdmins />} path='admins'>
                                                <Route element={<PartnerAdminInvite />} path='invite' />
                                            </Route>
                                            <Route element={<PartnerDocuments />} path='documents'>
                                                <Route element={<PartnerDocumentEdit />} path=':documentId/edit' />
                                                <Route element={<PartnerDocumentCreate />} path='create' />
                                            </Route>
                                            <Route element={<PartnerSocialMediaAccounts />} path='socials'>
                                                <Route element={<PartnerSocialAcccountEdit />} path=':accountId/edit' />
                                                <Route element={<PartnerSocialAcccountCreate />} path='create' />
                                            </Route>
                                        </Route>
                                    </Route>

                                    <Route element={<CancelEvent />} path='/cancelevent/:eventId' />
                                    <Route element={<DeleteMyData />} path='/deletemydata' />
                                    <Route element={<MyDashboard />} path='/mydashboard' />
                                    <Route element={<BecomeAPartnerPage />} path='/becomeapartner' />
                                    <Route element={<InviteAPartnerPage />} path='/inviteapartner' />
                                    <Route element={<LocationPreference />} path='/locationpreference' />
                                    <Route element={<Waivers />} path='/waivers' />
                                </Route>
                                <Route element={<AuthSideAdminLayout />}>
                                    <Route element={<SiteAdminLayout />} path='/siteadmin'>
                                        <Route element={<SiteAdminUsers />} path='users' />
                                        <Route element={<SiteAdminEvents />} path='events' />
                                        <Route element={<SiteAdminPartners />} path='partners' />
                                        <Route element={<SiteAdminPartnerRequests />} path='partner-requests' />
                                        <Route element={<SiteAdminEmailTemplates />} path='email-templates' />
                                        <Route element={<SiteAdminSendNotification />} path='send-notifications' />
                                    </Route>
                                </Route>
                                <Route>
                                    <Route
                                        element={<PartnerRequestDetails />}
                                        path='/partnerrequestdetails/:partnerRequestId'
                                    />
                                    <Route element={<EventDetails />} path='/eventdetails/:eventId?' />
                                    <Route element={<Partnerships />} path='/partnerships' />
                                    <Route element={<Shop />} path='/shop' />
                                    <Route element={<Help />} path='/help' />
                                    <Route element={<AboutUs />} path='/aboutus' />
                                    <Route element={<Board />} path='/board' />
                                    <Route element={<ContactUs />} path='/contactus' />
                                    <Route element={<Faq />} path='/faq' />
                                    <Route element={<GettingStarted />} path='/gettingstarted' />
                                    <Route element={<PrivacyPolicy />} path='/privacypolicy' />
                                    <Route element={<TermsOfService />} path='/termsofservice' />
                                    <Route element={<VolunteerOpportunities />} path='/volunteeropportunities' />
                                    <Route element={<Home />} path='/' />
                                </Route>
                                <Route element={<NoMatch />} />
                            </Routes>
                        </div>
                        <SiteFooter />
                    </BrowserRouter>
                </div>
            </MsalProvider>
            <div className='tailwind'>
                <Toaster />
            </div>
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    );
};
