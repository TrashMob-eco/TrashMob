import * as React from 'react';

import { Route, Switch } from 'react-router';
import { BrowserRouter } from 'react-router-dom';

import { Home } from './components/Home';

// Layout
import { TopMenu } from './components/TopMenu';

import './custom.css'
import { About } from './components/About';
import { ContactUs } from './components/ContactUs';
import { Faq } from './components/Faq';
import { Footer } from './components/Footer';
import { GettingStarted } from './components/GettingStarted';
import { MyDashboard } from './components/MyDashboard';
import { Partners } from './components/Partners';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { Sponsors } from './components/Sponsors';
import { TermsOfService } from './components/TermsOfService';
import { initializeIcons } from '@uifabric/icons';
import { MsalAuthenticationResult, MsalAuthenticationTemplate, MsalProvider } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { msalClient } from './store/AuthStore';
import { CreateEvent } from './components/CreateEvent';
import { EventDetails } from './components/EventDetails';

export const App = () => {

    initializeIcons();

    function ErrorComponent(error: MsalAuthenticationResult) {
        return <p>An Error Occurred: {error}</p>;
    }

    function LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }

    return (
        <MsalProvider instance={msalClient} >
            <div className="d-flex flex-column h-100">
                <TopMenu />
                <div className="container-fluid flex-grow-1 d-flex">
                    <div className="row flex-fill flex-column flex-sm-row">

                        <BrowserRouter basename="/" >
                            <Switch>
                                <Route exact path='/'>
                                    <Home />
                                </Route>
                                <Route exact path="/about">
                                    <About />
                                </Route>
                                <Route>
                                <MsalAuthenticationTemplate
                                    interactionType={InteractionType.Redirect}
                                    errorComponent={ErrorComponent}
                                    loadingComponent={LoadingComponent}>
                                    <Route path="/createevent/:eventId?" component={CreateEvent} />
                                </MsalAuthenticationTemplate >
                                 </Route>
                                <Route exact path="/contactus">
                                    <ContactUs />
                                </Route>
                                <Route path="/eventdetails/:eventId?" component={EventDetails} />
                                <Route exact path="/faq">
                                    <Faq />
                                </Route>
                                <Route exact path="/gettingstarted">
                                    <GettingStarted />
                                </Route>
                                <Route exact path="/mydashboard">
                                    <MsalAuthenticationTemplate
                                        interactionType={InteractionType.Redirect}
                                        errorComponent={ErrorComponent}
                                        loadingComponent={LoadingComponent}>                                        
                                        <MyDashboard />
                                    </MsalAuthenticationTemplate >
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
