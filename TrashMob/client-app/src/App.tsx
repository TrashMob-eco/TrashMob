import { FC, useEffect, useState } from 'react';

import { Route, Switch } from 'react-router';
import { BrowserRouter, RouteComponentProps } from 'react-router-dom';

import Home from './components/Pages/Home';

// Layout
import TopMenu from './components/TopMenu';

import { AboutUs } from './components/Pages/AboutUs';
import ContactUs from './components/Pages/ContactUs';
import EventSummary from './components/EventSummary';
import { Faq } from './components/Faq';
import { Footer } from './components/Footer';
import { GettingStarted } from './components/Pages/GettingStarted';
import MyDashboard from './components/Pages/MyDashboard';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { TermsOfService } from './components/Pages/TermsOfService';
import { Board } from './components/Board';
import { VolunteerOpportunities } from './components/VolunteerOpportunities';
import { initializeIcons } from '@uifabric/icons';
import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from './store/AuthStore';
import { EventDetails, DetailsMatchParams } from './components/Pages/EventDetails';
import { NoMatch } from './components/NoMatch';
import UserData from './components/Models/UserData';
import * as msal from "@azure/msal-browser";
import { Guid } from 'guid-typescript';
import LocationPreference from './components/Pages/LocationPreference';
import PartnerDashboard, { PartnerDashboardMatchParams } from './components/Partners/PartnerDashboard';
import PartnerRequest from './components/Partners/PartnerRequest';
import SiteAdmin from './components/Admin/SiteAdmin';
import ManageEventDashboard, { ManageEventDashboardMatchParams } from './components/EventManagement/ManageEventDashboard';
import { Shop } from './components/Shop';
import { EventSummaries } from './components/EventSummaries';
import { CancelEvent, CancelEventMatchParams } from './components/EventManagement/CancelEvent';

import './custom.css';
import 'react-phone-input-2/lib/style.css'
import DeleteMyData from './components/Pages/DeleteMyData';
import Waivers from './components/Waivers/Waivers';
import WaiversReturn from './components/Waivers/WaiversReturn';
import PartnerRequestDetails, { PartnerRequestDetailsMatchParams } from './components/Partners/PartnerRequestDetails';
import { Partnerships } from './components/Partners/Partnerships';
import { Help } from './components/Pages/Help';

interface AppProps extends RouteComponentProps<ManageEventDashboardMatchParams> {
}

interface PartnerProps extends RouteComponentProps<PartnerDashboardMatchParams> {
}

interface CancelProps extends RouteComponentProps<CancelEventMatchParams> {
}

interface DetailsProps extends RouteComponentProps<DetailsMatchParams> {
}

interface PartnerRequestDetailsProps extends RouteComponentProps<PartnerRequestDetailsMatchParams> {
}

interface WaiversReturnProps extends RouteComponentProps {
}

interface DeleteMyDataProps extends RouteComponentProps {
}

export const App: FC = () => {
    const [isUserLoaded, setIsUserLoaded] = useState(false);
    const [currentUser, setCurrentUser] = useState<UserData>(new UserData());

    useEffect(() => {
        initializeIcons();

        msalClient.addEventCallback((message: msal.EventMessage) => {
            if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
                verifyAccount(message.payload as msal.AuthenticationResult)
            }
            if (message.eventType === msal.EventType.LOGOUT_SUCCESS) {
                clearUser();
            }
        });

        var userStr = sessionStorage.getItem('user');
        if (userStr) {
            var user = JSON.parse(userStr);
            setCurrentUser(user);
            if (user.id === Guid.EMPTY) {
                setIsUserLoaded(false);
            } else {
                setIsUserLoaded(true);
            }
        }
    },
        // eslint-disable-next-line
        []);

    function ErrorComponent(error: MsalAuthenticationResult) {
        return <p>An Error Occurred: {error}</p>;
    }

    function LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }

    function renderEditEvent(inp: AppProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <ManageEventDashboard {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderPartnerDashboard(inp: PartnerProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <PartnerDashboard {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderEventDetails(inp: DetailsProps) {
        return (
            <EventDetails {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
        );
    }

    function renderPartnerRequestDetails(inp: PartnerRequestDetailsProps) {
        return (
            <PartnerRequestDetails {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
        );
    }

    function renderEventSummary(inp: AppProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <EventSummary {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderCancelEvent(inp: CancelProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <CancelEvent {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }

    function renderWaiversReturn(inp: WaiversReturnProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <WaiversReturn {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} onUserUpdated={handleUserUpdated} />
            </MsalAuthenticationTemplate >);
    }

    function renderDeleteMyData(inp: DeleteMyDataProps) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <DeleteMyData {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded} />
            </MsalAuthenticationTemplate >);
    }


    function clearUser() {
        const user = new UserData();
        setCurrentUser(user)
        sessionStorage.setItem('user', JSON.stringify(user));
        setIsUserLoaded(false);
    }

    function handleUserUpdated() {
        const account = msalClient.getAllAccounts()[0];

        var apiConfig = getApiConfig();

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        setIsUserLoaded(false);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            if (!validateToken(tokenResponse.idTokenClaims)) {
                return;
            }

            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Users/' + currentUser.id, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<UserData>)
                .then(data => {
                    setCurrentUser(data);
                    setIsUserLoaded(true);
                    sessionStorage.setItem('user', JSON.stringify(data));
                });
        });
    }

    function verifyAccount(result: msal.AuthenticationResult) {

        var userDeleted = result.idTokenClaims["userDeleted"];

        if (userDeleted && userDeleted === true) {
            clearUser();
            return;
        }

        var email = result.idTokenClaims["email"];
        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        const request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {

            if (!validateToken(tokenResponse.idTokenClaims)) {
                return;
            }

            const method = 'GET';
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
            const user = new UserData();

            fetch('/api/Users/getuserbyemail/' + encodeURIComponent(email), {
                method: method,
                headers: headers
            })
                .then(response => response.json() as Promise<UserData> | null)
                .then(data => {
                    if (data) {
                        user.id = data.id;
                        user.userName = data.userName;
                        user.givenName = data.givenName;
                        user.dateAgreedToTrashMobWaiver = data.dateAgreedToTrashMobWaiver;
                        user.memberSince = data.memberSince;
                        user.trashMobWaiverVersion = data.trashMobWaiverVersion;
                        user.isSiteAdmin = data.isSiteAdmin;
                        user.email = data.email;
                        setCurrentUser(user);
                        setIsUserLoaded(true);
                        sessionStorage.setItem('user', JSON.stringify(user));
                    }
                });
        });
    }

    return (
        <MsalProvider instance={msalClient} >
            <div className="d-flex flex-column h-100">
                <BrowserRouter>
                    <TopMenu isUserLoaded={isUserLoaded} currentUser={currentUser} />
                    <div className="container-fluid px-0">
                        <Switch>
                            <Route path="/manageeventdashboard/:eventId?" render={(props: AppProps) => renderEditEvent(props)} />
                            <Route path="/partnerdashboard/:partnerId?" render={(props: PartnerProps) => renderPartnerDashboard(props)} />
                            <Route path="/partnerrequestdetails/:partnerRequestId" render={(props: PartnerRequestDetailsProps) => renderPartnerRequestDetails(props)} />
                            <Route path="/eventsummary/:eventId?" render={(props: AppProps) => renderEventSummary(props)} />
                            <Route path="/eventdetails/:eventId" render={(props: DetailsProps) => renderEventDetails(props)} />
                            <Route path="/cancelevent/:eventId" render={(props: CancelProps) => renderCancelEvent(props)} />
                            <Route path="/waiversreturn/:envelopeId?" render={(props: WaiversReturnProps) => renderWaiversReturn(props)} />
                            <Route path="/deletemydata" render={(props: DeleteMyDataProps) => renderDeleteMyData(props)} />
                            <Route exact path="/mydashboard">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <MyDashboard currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/becomeapartner">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <PartnerRequest currentUser={currentUser} isUserLoaded={isUserLoaded} mode="become" />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/inviteapartner">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <PartnerRequest currentUser={currentUser} isUserLoaded={isUserLoaded} mode="send" />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/siteadmin">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <SiteAdmin currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/locationpreference">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <LocationPreference currentUser={currentUser} isUserLoaded={isUserLoaded} onUserUpdated={handleUserUpdated} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/waivers">
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <Waivers currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </MsalAuthenticationTemplate >
                            </Route>
                            <Route exact path="/partnerships">
                                <Partnerships />
                            </Route>
                            <Route exact path="/shop">
                                <Shop />
                            </Route>
                            <Route exact path="/help">
                                <Help />
                            </Route>
                            <Route exact path="/aboutus">
                                <AboutUs />
                            </Route>
                            <Route exact path="/board">
                                <Board />
                            </Route>
                            <Route exact path="/contactus">
                                <ContactUs />
                            </Route>
                            <Route exact path="/eventsummaries">
                                <EventSummaries />
                            </Route>
                            <Route exact path="/faq">
                                <Faq />
                            </Route>
                            <Route exact path="/gettingstarted">
                                <GettingStarted />
                            </Route>
                            <Route exact path="/privacypolicy">
                                <PrivacyPolicy />
                            </Route>
                            <Route exact path="/termsofservice">
                                <TermsOfService />
                            </Route>
                            <Route exact path="/volunteeropportunities">
                                <VolunteerOpportunities />
                            </Route>
                            <Route exact path='/'>
                                <Home currentUser={currentUser} isUserLoaded={isUserLoaded} onUserUpdated={handleUserUpdated} />
                            </Route>
                            <Route>
                                <NoMatch />
                            </Route>
                        </Switch>
                    </div>
                    <Footer />
                </BrowserRouter>
            </div>
        </MsalProvider>
    );
}
