import * as React from 'react';

import { Route, RouteComponentProps, Switch } from 'react-router';
import { BrowserRouter } from 'react-router-dom';

import { Home } from './components/Home';

// Layout
import { TopMenu } from './components/TopMenu';

import { About } from './components/About';
import { ContactUs } from './components/ContactUs';
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
import { msalClient } from './store/AuthStore';
import CreateEvent from './components/CreateEvent';
import { EventDetails } from './components/EventDetails';
import { EditEvent, EditEventProps } from './components/EditEvent';
import { NoMatch } from './components/NoMatch';

export const App = () => {

    initializeIcons();

    function ErrorComponent(error: MsalAuthenticationResult) {
        return <p>An Error Occurred: {error}</p>;
    }

    function LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }

    function renderEditEvent(inp: EditEventProps ) {
        return (
            <MsalAuthenticationTemplate
                interactionType={InteractionType.Redirect}
                errorComponent={ErrorComponent}
                loadingComponent={LoadingComponent}>
                <EditEvent {...inp} />
            </MsalAuthenticationTemplate >);
    }

    return (
        <MsalProvider instance={msalClient} >
            <div className="d-flex flex-column h-100">
                <TopMenu />
                <div className="container flex-grow-1 d-flex">
                    <div className="row flex-fill flex-column flex-sm-row">

                        <BrowserRouter>
                            <Switch>
                                <Route path="/editevent/:eventId" render={(props) => renderEditEvent(props)} />
                                <Route path="/eventdetails/:eventId" component={EventDetails} />
                                <Route path="/createevent">
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}>
                                        <CreateEvent />
                                    </MsalAuthenticationTemplate >
                                </Route>
                                <Route exact path="/mydashboard">
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}>
                                        <MyDashboard />
                                    </MsalAuthenticationTemplate >
                                </Route>
                                <Route exact path="/about">
                                    <About />
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
                                    <Home />
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
