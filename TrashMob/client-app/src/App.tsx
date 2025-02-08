import { FC, useEffect, useState } from 'react';

import { BrowserRouter, Outlet, Route, Routes, useLocation } from 'react-router';

import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from '@/components/ui/toaster';

import Home from './components/Pages/Home';

import { AboutUs } from './components/Pages/AboutUs';
import ContactUs from './components/Pages/ContactUs';
import EventSummary from './components/EventSummary';
import { Faq } from './components/Faq';
import { GettingStarted } from './components/Pages/GettingStarted';
import MyDashboard from './pages/mydashboard';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { Board } from './components/Board';
import { msalClient } from './store/AuthStore';
import EventDetails from './components/Pages/EventDetails';
import { NoMatch } from './components/NoMatch';
import LocationPreference from './components/Pages/LocationPreference';
import PartnerRequest from './components/Partners/PartnerRequest';
import SiteAdmin from './components/Admin/SiteAdmin';
import ManageEventDashboard from './components/EventManagement/ManageEventDashboard';
import { Shop } from './components/Shop';
import { CancelEvent } from './components/EventManagement/CancelEvent';

import './custom.css';
import 'react-phone-input-2/lib/style.css';
import DeleteMyData from './components/Pages/DeleteMyData';
import Waivers from './components/Waivers/Waivers';
import PartnerRequestDetails from './components/Partners/PartnerRequestDetails';
import { Partnerships } from './components/Partners/Partnerships';
import { Help } from './components/Pages/Help';
import { SiteFooter } from './components/SiteFooter';
import { SiteHeader } from './components/SiteHeader';
import { useLogin } from './hooks/useLogin';

/** 2024 pages */
import { Home as Home2024 } from './pages/Home';
import { TermsOfService } from './pages/termsofservice';
import { VolunteerOpportunities } from './pages/volunteeropportunities';
import { PartnerEdit } from './pages/partnerdashboard/$partnerId/edit';
import { PartnerLayout } from './pages/partnerdashboard/$partnerId/_layout';
import { PartnerLocations } from './pages/partnerdashboard/$partnerId/locations';
import { PartnerContacts } from './pages/partnerdashboard/$partnerId/contacts';
import { PartnerAdmins } from './components/Partners/PartnerAdmins';
import { PartnerDocuments } from './components/Partners/PartnerDocuments';
import { PartnerSocialMediaAccounts } from './components/Partners/PartnerSocialMediaAccounts';
import { PartnerIndex } from './pages/partnerdashboard/$partnerId';
import { PartnerContactEdit } from './pages/partnerdashboard/$partnerId/contacts.$contactId.edit';
import { PartnerContactCreate } from './pages/partnerdashboard/$partnerId/contacts.create';
import { PartnerLocationEdit } from './pages/partnerdashboard/$partnerId/locations.$locationId.edit';
import { PartnerLocationCreate } from './pages/partnerdashboard/$partnerId/locations.create';
import { PartnerServices } from './pages/partnerdashboard/$partnerId/services';
import { PartnerServiceEdit } from './pages/partnerdashboard/$partnerId/services.edit';
import { PartnerServiceEnable } from './pages/partnerdashboard/$partnerId/services.enable';
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

const AuthLayout = () => {
    function ErrorComponent(error: MsalAuthenticationResult) {
        return (
            <p>
                An Error Occurred:
                {error}
            </p>
        );
    }
    function LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }
    return (
        <MsalAuthenticationTemplate
            interactionType={InteractionType.Redirect}
            errorComponent={ErrorComponent}
            loadingComponent={LoadingComponent}
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
                                    <Route
                                        path='manageeventdashboard/:eventId?'
                                        element={
                                            <ManageEventDashboard
                                                currentUser={currentUser}
                                                isUserLoaded={isUserLoaded}
                                            />
                                        }
                                    />
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
                                                <Route path=':contactId/edit' element={<PartnerContactEdit />} />
                                            </Route>
                                            <Route path='admins' element={<PartnerAdmins />} />
                                            <Route path='documents' element={<PartnerDocuments />} />
                                            <Route path='socials' element={<PartnerSocialMediaAccounts />} />
                                        </Route>
                                    </Route>
                                    <Route
                                        path='/eventsummary/:eventId?'
                                        element={<EventSummary currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route
                                        path='/cancelevent/:eventId'
                                        element={<CancelEvent currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route
                                        path='/deletemydata'
                                        element={<DeleteMyData currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route
                                        path='/mydashboard'
                                        element={<MyDashboard currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route
                                        path='/becomeapartner'
                                        element={
                                            <PartnerRequest
                                                currentUser={currentUser}
                                                isUserLoaded={isUserLoaded}
                                                mode='become'
                                            />
                                        }
                                    />
                                    <Route
                                        path='/inviteapartner'
                                        element={
                                            <PartnerRequest
                                                currentUser={currentUser}
                                                isUserLoaded={isUserLoaded}
                                                mode='send'
                                            />
                                        }
                                    />
                                    <Route
                                        path='/siteadmin'
                                        element={<SiteAdmin currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route
                                        path='/locationpreference'
                                        element={
                                            <LocationPreference
                                                currentUser={currentUser}
                                                isUserLoaded={isUserLoaded}
                                                onUserUpdated={handleUserUpdated}
                                            />
                                        }
                                    />
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
                                    <Route
                                        path='/eventdetails/:eventId?'
                                        element={<EventDetails currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
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
                                    <Route
                                        path='/2024'
                                        element={<Home2024 currentUser={currentUser} isUserLoaded={isUserLoaded} />}
                                    />
                                    <Route
                                        path='/'
                                        element={
                                            <Home
                                                currentUser={currentUser}
                                                isUserLoaded={isUserLoaded}
                                                onUserUpdated={handleUserUpdated}
                                            />
                                        }
                                    />
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
