import * as React from 'react';

import { Route, Switch } from 'react-router';
import { BrowserRouter, RouteComponentProps } from 'react-router-dom';

import { Home } from './components/Home';

// Layout
import { TopMenu } from './components/TopMenu';

import { AboutUs } from './components/AboutUs';
import ContactUs from './components/ContactUs';
import { Faq } from './components/Faq';
import { Footer } from './components/Footer';
import { GettingStarted } from './components/GettingStarted';
import MyDashboard from './components/MyDashboard';
import { Partners } from './components/Partners';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { Sponsors } from './components/Sponsors';
import { TermsOfService } from './components/TermsOfService';
import { initializeIcons } from '@uifabric/icons';
import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { apiConfig, getDefaultHeaders, msalClient } from './store/AuthStore';
import CreateEvent from './components/CreateEvent';
import { EventDetails, MatchParams } from './components/EventDetails';
import { EditEvent } from './components/EditEvent';
import { NoMatch } from './components/NoMatch';
import UserData from './components/Models/UserData';
import * as msal from "@azure/msal-browser";

interface AppProps extends RouteComponentProps<MatchParams> {
}

export const App = (props) => {
    const [isUserLoaded, setIsUserLoaded] = React.useState(false);
    const [currentUser, setCurrentUser] = React.useState<UserData>(new UserData());
    
    initializeIcons();

    msalClient.addEventCallback((message: msal.EventMessage) => {
        if (message.eventType === msal.EventType.LOGIN_SUCCESS) {
            verifyAccount(message.payload as msal.AuthenticationResult)
        }
        if (message.eventType === msal.EventType.LOGOUT_SUCCESS) {
            clearUser();
        }
    });

    function ErrorComponent(error: MsalAuthenticationResult) {
        return <p>An Error Occurred: {error}</p>;
    }

    function LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }

    function renderEditEvent(inp: AppProps ) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <EditEvent {...inp} currentUser={currentUser} isUserLoaded={isUserLoaded}  />
            </MsalAuthenticationTemplate >);
    }

    function clearUser() {
        setIsUserLoaded(false);
        var user = new UserData();
        setCurrentUser(user)
    }

    function verifyAccount(result: msal.AuthenticationResult) {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
            var user = new UserData();

            user.nameIdentifier = result.idTokenClaims["sub"];
            user.userName = result.account?.username ?? "";
            user.city = result.account?.idTokenClaims["city"] ?? "";
            user.region = result.account?.idTokenClaims["region"] ?? "";
            user.country = result.account?.idTokenClaims["country"] ?? "";
            user.postalCode = result.account?.idTokenClaims["postalCode"] ?? "";
            user.givenName = result.account?.idTokenClaims["given_name"] ?? "";
            user.surname = result.account?.idTokenClaims["family_name"] ?? "";
            user.email = result.account?.idTokenClaims["emails"][0] ?? "";

            fetch('api/Users', {
                method: 'POST',
                headers: headers,
                body: JSON.stringify(user)
            })
                .then(response => response.json() as Promise<UserData> | null)
                .then(data => {
                    if (data) {
                        user.id = data.id;
                        user.dateAgreedToPrivacyPolicy = data.dateAgreedToPrivacyPolicy;
                        user.dateAgreedToTermsOfService = data.dateAgreedToTermsOfService;
                        user.memberSince = data.memberSince;
                        user.privacyPolicyVersion = data.privacyPolicyVersion;
                        user.termsOfServiceVersion = data.termsOfServiceVersion;
                        setCurrentUser(user);
                        setIsUserLoaded(true);
                    }

                    //if (user.dateAgreedToPrivacyPolicy < CurrentPrivacyPolicyVersion.versionDate || user.dateAgreedToTermsOfService < CurrentTermsOfServiceVersion.versionDate || user.termsOfServiceVersion === "" || user.privacyPolicyVersion === "") {
                    //    return AgreeToPolicies;
                    // }
                });
        });
    }

    function updateAgreements(tosVersion: string, privacyVersion: string) {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('api/Users/' + currentUser.id, {
                method: 'GET',
                headers: headers,
                body: JSON.stringify(currentUser)
            })
                .then(response => response.json() as Promise<UserData> | null)
                .then(user => {
                    user.dateAgreedToPrivacyPolicy = new Date();
                    user.dateAgreedToTermsOfService = new Date();
                    user.termsOfServiceVersion = tosVersion;
                    user.privacyPolicyVersion = privacyVersion;
                    fetch('api/Users', {
                        method: 'PUT',
                        headers: headers,
                        body: JSON.stringify(user)
                    })
                        .then(response => response.json() as Promise<UserData> | null)
                        .then(data => setCurrentUser(data));
                })
        })
    }

    return (
        <MsalProvider instance={msalClient} >
            <div className="d-flex flex-column h-100">
                <TopMenu {...props} isUserLoaded={isUserLoaded} currentUser={currentUser} />

                <div className="container">
                    <div className="">

                        <BrowserRouter>
                            <Switch>
                                <Route path="/editevent/:eventId" render={(props) => renderEditEvent(props)} />
                                <Route path="/eventdetails/:eventId" component={EventDetails} />
                                <Route path="/createevent">
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}>
                                        <CreateEvent currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                    </MsalAuthenticationTemplate >
                                </Route>
                                <Route exact path="/mydashboard">
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}>
                                        <MyDashboard currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                    </MsalAuthenticationTemplate >
                                </Route>
                                <Route exact path="/aboutus">
                                    <AboutUs />
                                </Route>
                                <Route exact path="/contactus">
                                    <ContactUs />
                                </Route>
                                <Route exact path="/faq">
                                    <Faq />
                                </Route>
                                <Route exact path="/gettingstarted">
                                    <GettingStarted />
                                </Route>
                                <Route exact path="/partners">
                                    <Partners />
                                </Route>
                                <Route exact path="/privacypolicy">
                                    <PrivacyPolicy />
                                </Route>
                                <Route exact path="/sponsors">
                                    <Sponsors />
                                </Route>
                                <Route exact path="/termsofservice">
                                    <TermsOfService />
                                </Route>
                                <Route exact path='/'>
                                    <Home {...props} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                                </Route>
                                <Route>
                                    <NoMatch />
                                </Route>
                            </Switch>
                            <div>
                                <Footer />
                            </div>
                        </BrowserRouter>
                    </div>
                </div>
            </div>
        </MsalProvider>
    );
}
