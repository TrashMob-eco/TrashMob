import { FC, useEffect, useState } from 'react';

import { BrowserRouter, Outlet, Route, Routes, useLocation } from 'react-router';

import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from '@/components/ui/toaster';

import { AboutUs } from './components/Pages/AboutUs';
import EventSummary from './components/EventSummary';
import { Faq } from './components/Faq';
import { GettingStarted } from './components/Pages/GettingStarted';
import MyDashboard from './pages/mydashboard';
import { Board } from './components/Board';
import { msalClient } from './store/AuthStore';
import EventDetails from './components/Pages/EventDetails';
import { NoMatch } from './components/NoMatch';
import PartnerRequest from './components/Partners/PartnerRequest';
import SiteAdmin from './components/Admin/SiteAdmin';
import { Shop } from './components/Shop';

import './custom.css';
import 'react-phone-input-2/lib/style.css';
import Waivers from './components/Waivers/Waivers';
import PartnerRequestDetails from './components/Partners/PartnerRequestDetails';
import { Partnerships } from './components/Partners/Partnerships';
import { Help } from './components/Pages/Help';
import { SiteFooter } from './components/SiteFooter';
import { SiteHeader } from './components/SiteHeader';
import { useLogin } from './hooks/useLogin';

/** 2024 pages */
import { Home } from './pages/_home';
import { CreateEventWrapper } from './pages/events/create';
import { TermsOfService } from './pages/termsofservice';
import { VolunteerOpportunities } from './pages/volunteeropportunities';
import { PartnerEdit } from './pages/partnerdashboard/$partnerId/edit';
import { PartnerLayout } from './pages/partnerdashboard/$partnerId/_layout';
import { PartnerLocations } from './pages/partnerdashboard/$partnerId/locations';
import { PartnerContacts } from './pages/partnerdashboard/$partnerId/contacts';
import { PartnerIndex } from './pages/partnerdashboard/$partnerId';
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
import { PrivacyPolicy } from './pages/privacypolicy';
import { DeleteMyData } from './pages/deletemydata';

import { PartnerContactType } from './enums/PartnerContactType';
import { PartnerAdminInvite } from './pages/partnerdashboard/$partnerId/admins.invite';
import { EditEventPage } from './pages/events/edit';
import { CancelEvent } from './pages/events/$eventId/delete';
import { ContactUsWrapper as ContactUs } from './pages/contactus';
import { LocationPreferenceWrapper as LocationPreference } from './pages/locationpreference';
import { SiteAdminLayout } from './pages/siteadmin/_layout';
import { SiteAdminUsers } from './pages/siteadmin/users';
import { SiteAdminEvents } from './pages/siteadmin/events/page';
import { SiteAdminPartners } from './pages/siteadmin/partners/page';
import { SiteAdminPartnerRequests } from './pages/siteadmin/partner-requests/page';
import { Loader2 } from 'lucide-react';
import { SiteAdminEmailTemplates } from './pages/siteadmin/email-templates';
import { SiteAdminSendNotification } from './pages/siteadmin/send-notification';

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
            <div className='tailwind'>
                <div className='flex justify-center items-center py-16'>
                    <Loader2 className='animate-spin mr-2' /> Loading
                </div>
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

export const App: FC = () => {
    useInitializeApp();
    const { currentUser, isUserLoaded, handleUserUpdated } = useLogin();

    return (
        <QueryClientProvider client={queryClient}>
            <MsalProvider instance={msalClient}>
                <div className='d-flex flex-column h-100'>
                    <BrowserRouter>
                        <ScrollToTop />
                        <SiteHeader currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        <div className='container-fluid px-0'>
                            <Routes>
                                <Route element={<AuthLayout />}>
                                    <Route path='/events'>
                                        <Route path='create' element={<CreateEventWrapper />} />
                                        <Route path=':eventId/edit' element={<EditEventPage />} />
                                    </Route>
                                    <Route path='partnerdashboard'>
                                        <Route index element={<div>Partner Dashboard Index</div>} />
                                        <Route path=':partnerId' element={<PartnerLayout />}>
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
                                                    element={
                                                        <PartnerContactEdit
                                                            type={PartnerContactType.ORGANIZATION_WIDE}
                                                        />
                                                    }
                                                />
                                                <Route
                                                    path='by-location/:contactId/edit'
                                                    element={
                                                        <PartnerContactEdit
                                                            type={PartnerContactType.LOCATION_SPECIFIC}
                                                        />
                                                    }
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
                                    <Route
                                        path='/eventsummary/:eventId?'
                                        element={<EventSummary currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route path='/cancelevent/:eventId' element={<CancelEvent />} />
                                    <Route path='/deletemydata' element={<DeleteMyData />} />
                                    <Route
                                        path='/mydashboard'
                                        element={<MyDashboard currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route path='/becomeapartner' element={<PartnerRequest mode='become' />} />
                                    <Route path='/inviteapartner' element={<PartnerRequest mode='send' />} />
                                    <Route
                                        path='/siteadmin'
                                        element={<SiteAdmin currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route element={<AuthSideAdminLayout />}>
                                        <Route path='/admin' element={<SiteAdminLayout />}>
                                            <Route path='users' element={<SiteAdminUsers />} />
                                            <Route path='events' element={<SiteAdminEvents />} />
                                            <Route path='partners' element={<SiteAdminPartners />} />
                                            <Route path='partner-requests' element={<SiteAdminPartnerRequests />} />
                                            <Route path='email-templates' element={<SiteAdminEmailTemplates />} />
                                            <Route path='send-notifications' element={<SiteAdminSendNotification />} />
                                        </Route>
                                    </Route>
                                    <Route path='/locationpreference' element={<LocationPreference />} />
                                    <Route
                                        path='/waivers'
                                        element={
                                            isUserLoaded ? (
                                                <Waivers currentUser={currentUser} onUserUpdated={handleUserUpdated} />
                                            ) : null
                                        }
                                    />
                                </Route>
                                <Route>
                                    <Route
                                        path='/partnerrequestdetails/:partnerRequestId'
                                        element={<PartnerRequestDetails />}
                                    />
                                    <Route path='/eventdetails/:eventId?' element={<EventDetails />} />
                                    <Route path='/partnerships' element={<Partnerships />} />
                                    <Route path='/shop' element={<Shop />} />
                                    <Route path='/help' element={<Help />} />
                                    <Route path='/aboutus' element={<AboutUs />} />
                                    <Route path='/board' element={<Board />} />
                                    <Route path='/contactus' element={<ContactUs />} />
                                    <Route path='/faq' element={<Faq />} />
                                    <Route path='/gettingstarted' element={<GettingStarted />} />
                                    <Route path='/privacypolicy' element={<PrivacyPolicy />} />
                                    <Route path='/termsofservice' element={<TermsOfService />} />
                                    <Route path='/volunteeropportunities' element={<VolunteerOpportunities />} />
                                    <Route path='/' element={<Home />} />
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
