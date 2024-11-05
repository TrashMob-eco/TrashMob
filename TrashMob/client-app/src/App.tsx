import { FC, useEffect, useState } from 'react';

import { Route, Switch } from 'react-router';
import { BrowserRouter, RouteComponentProps } from 'react-router-dom';

import { initializeIcons } from '@uifabric/icons';
import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import Home from './components/Pages/Home';

import { AboutUs } from './components/Pages/AboutUs';
import ContactUs from './components/Pages/ContactUs';
import EventSummary from './components/EventSummary';
import { Faq } from './components/Faq';
import { GettingStarted } from './components/Pages/GettingStarted';
import MyDashboard from './components/Pages/MyDashboard';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { TermsOfService } from './components/Pages/TermsOfService';
import { Board } from './components/Board';
import { VolunteerOpportunities } from './components/VolunteerOpportunities';
import { msalClient } from './store/AuthStore';
import EventDetails, { DetailsMatchParams } from './components/Pages/EventDetails';
import { NoMatch } from './components/NoMatch';
import LocationPreference from './components/Pages/LocationPreference';
import PartnerDashboard, { PartnerDashboardMatchParams } from './components/Partners/PartnerDashboard';
import PartnerRequest from './components/Partners/PartnerRequest';
import SiteAdmin from './components/Admin/SiteAdmin';
import ManageEventDashboard, {
    ManageEventDashboardMatchParams,
} from './components/EventManagement/ManageEventDashboard';
import { Shop } from './components/Shop';
import { EventSummaries } from './components/EventSummaries';
import { CancelEvent, CancelEventMatchParams } from './components/EventManagement/CancelEvent';

import './custom.css';
import 'react-phone-input-2/lib/style.css';
import DeleteMyData from './components/Pages/DeleteMyData';
import Waivers from './components/Waivers/Waivers';
import PartnerRequestDetails, { PartnerRequestDetailsMatchParams } from './components/Partners/PartnerRequestDetails';
import { Partnerships } from './components/Partners/Partnerships';
import { Help } from './components/Pages/Help';
import SiteHeader from './components/SiteHeader';
import { useLogin } from './hooks/useLogin';
import { SiteFooter } from './components/SiteFooter/SiteFooter';

interface AppProps extends RouteComponentProps<ManageEventDashboardMatchParams> {}

interface PartnerProps extends RouteComponentProps<PartnerDashboardMatchParams> {}

interface CancelProps extends RouteComponentProps<CancelEventMatchParams> {}

interface DetailsProps extends RouteComponentProps<DetailsMatchParams> {}

interface PartnerRequestDetailsProps extends RouteComponentProps<PartnerRequestDetailsMatchParams> {}

interface DeleteMyDataProps extends RouteComponentProps {}

const queryClient = new QueryClient();

const useInitializeApp = () => {
    const [isInitialized, setIsInitialized] = useState(false);
    useEffect(() => {
        if (isInitialized) {
            return;
        }
        setIsInitialized(true);
        initializeIcons();
    }, [isInitialized]);
};

export const App: FC = () => {
    useInitializeApp();
    const { currentUser, isUserLoaded, handleUserUpdated } = useLogin();

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

    function renderEditEvent(inp: AppProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}
            >
                <ManageEventDashboard {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate>
        );
    }

    function renderPartnerDashboard(inp: PartnerProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}
            >
                <PartnerDashboard {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate>
        );
    }

    function renderEventDetails(inp: DetailsProps) {
        return <EventDetails {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />;
    }

    function renderPartnerRequestDetails(inp: PartnerRequestDetailsProps) {
        return <PartnerRequestDetails {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />;
    }

    function renderEventSummary(inp: AppProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}
            >
                <EventSummary {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate>
        );
    }

    function renderCancelEvent(inp: CancelProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}
            >
                <CancelEvent {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate>
        );
    }

    function renderDeleteMyData(inp: DeleteMyDataProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}
            >
                <DeleteMyData {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate>
        );
    }

    return (
        <QueryClientProvider client={queryClient}>
            <MsalProvider instance={msalClient}>
                <div className='d-flex flex-column h-100'>
                    <BrowserRouter>
                        <SiteHeader currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        <div className='container-fluid px-0'>
                            <Switch>
                                <Route
                                    path='/manageeventdashboard/:eventId?'
                                    render={(props: AppProps) => renderEditEvent(props)}
                                />
                                <Route
                                    path='/partnerdashboard/:partnerId?'
                                    render={(props: PartnerProps) => renderPartnerDashboard(props)}
                                />
                                <Route
                                    path='/partnerrequestdetails/:partnerRequestId'
                                    render={(props: PartnerRequestDetailsProps) => renderPartnerRequestDetails(props)}
                                />
                                <Route
                                    path='/eventsummary/:eventId?'
                                    render={(props: AppProps) => renderEventSummary(props)}
                                />
                                <Route
                                    path='/eventdetails/:eventId'
                                    render={(props: DetailsProps) => renderEventDetails(props)}
                                />
                                <Route
                                    path='/cancelevent/:eventId'
                                    render={(props: CancelProps) => renderCancelEvent(props)}
                                />
                                <Route
                                    path='/deletemydata'
                                    render={(props: DeleteMyDataProps) => renderDeleteMyData(props)}
                                />
                                <Route exact path='/mydashboard'>
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}
                                    >
                                        <MyDashboard currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                    </MsalAuthenticationTemplate>
                                </Route>
                                <Route exact path='/becomeapartner'>
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}
                                    >
                                        <PartnerRequest
                                            currentUser={currentUser}
                                            isUserLoaded={isUserLoaded}
                                            mode='become'
                                        />
                                    </MsalAuthenticationTemplate>
                                </Route>
                                <Route exact path='/inviteapartner'>
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}
                                    >
                                        <PartnerRequest
                                            currentUser={currentUser}
                                            isUserLoaded={isUserLoaded}
                                            mode='send'
                                        />
                                    </MsalAuthenticationTemplate>
                                </Route>
                                <Route exact path='/siteadmin'>
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}
                                    >
                                        <SiteAdmin currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                    </MsalAuthenticationTemplate>
                                </Route>
                                <Route exact path='/locationpreference'>
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}
                                    >
                                        <LocationPreference
                                            currentUser={currentUser}
                                            isUserLoaded={isUserLoaded}
                                            onUserUpdated={handleUserUpdated}
                                        />
                                    </MsalAuthenticationTemplate>
                                </Route>
                                <Route exact path='/waivers'>
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}
                                    >
                                        {isUserLoaded ? <Waivers currentUser={currentUser} /> : null}
                                    </MsalAuthenticationTemplate>
                                </Route>
                                <Route exact path='/partnerships'>
                                    <Partnerships />
                                </Route>
                                <Route exact path='/shop'>
                                    <Shop />
                                </Route>
                                <Route exact path='/help'>
                                    <Help />
                                </Route>
                                <Route exact path='/aboutus'>
                                    <AboutUs />
                                </Route>
                                <Route exact path='/board'>
                                    <Board />
                                </Route>
                                <Route exact path='/contactus'>
                                    <ContactUs />
                                </Route>
                                <Route exact path='/eventsummaries'>
                                    <EventSummaries />
                                </Route>
                                <Route exact path='/faq'>
                                    <Faq />
                                </Route>
                                <Route exact path='/gettingstarted'>
                                    <GettingStarted />
                                </Route>
                                <Route exact path='/privacypolicy'>
                                    <PrivacyPolicy />
                                </Route>
                                <Route exact path='/termsofservice'>
                                    <TermsOfService />
                                </Route>
                                <Route exact path='/volunteeropportunities'>
                                    <VolunteerOpportunities />
                                </Route>
                                <Route exact path='/'>
                                    <Home
                                        currentUser={currentUser}
                                        isUserLoaded={isUserLoaded}
                                        onUserUpdated={handleUserUpdated}
                                    />
                                </Route>
                                <Route>
                                    <NoMatch />
                                </Route>
                            </Switch>
                        </div>
                        <SiteFooter />
                    </BrowserRouter>
                </div>
            </MsalProvider>
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    );
};
