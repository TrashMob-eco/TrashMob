import { Component } from 'react';
import * as React from 'react';

import { Route, Switch } from 'react-router';
import { BrowserRouter } from 'react-router-dom';

import { Home } from './components/Home';
import { FetchEvents } from './components/FetchEvents';
import { AddEvent } from './components/AddEvent';

// Layout
import { TopMenu } from './layout/TopMenu';

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
import { UserStories } from './components/UserStories';
import { MsalAuthenticationTemplate , UnauthenticatedTemplate, MsalProvider, MsalAuthenticationResult } from '@azure/msal-react';
import { InteractionType } from '@azure/msal-browser';
import { msalClient } from './store/AuthStore';
import { initializeIcons } from '@uifabric/icons';

export default class App extends Component {
    constructor(props: any) {
        super(props);
        this.state = { loading: true }
    }

    componentDidMount() {
            this.setState({ loading: false });
    }

    static displayName = App.name;

    private ErrorComponent(error: MsalAuthenticationResult) {
        return <p>An Error Occurred: {error}</p>;
    }

    private LoadingComponent() {
        return <p>Authentication in progress...</p>;
    }

    render() {
        initializeIcons();

        return (
            <MsalProvider instance={msalClient} >
                <div className="d-flex flex-column h-100">
                    <TopMenu />

                    <div className="container-fluid flex-grow-1 d-flex">
                        <div className="row flex-fill flex-column flex-sm-row">

                            <BrowserRouter>
                                {/*<MsalAuthenticationTemplate*/}
                                {/*    interactionType={InteractionType.Popup}*/}
                                {/*    errorComponent={this.ErrorComponent}*/}
                                {/*    loadingComponent={this.LoadingComponent}>*/}
                                {/*    <Switch>*/}
                                {/*        */}{/*<Route path="/addevent">*/}
                                {/*        */}{/*    <AddEvent />*/}
                                {/*        */}{/*</Route>*/}
                                {/*        <Route path="/mydashboard">*/}
                                {/*            <MyDashboard />*/}
                                {/*        </Route>*/}
                                {/*    </Switch>*/}
                                {/*</MsalAuthenticationTemplate >*/}
                                <UnauthenticatedTemplate>
                                    <Switch>
                                        <Route path='/'>
                                            <Home />
                                        </Route>
                                        <Route path="/about">
                                            <About />
                                        </Route>
                                        <Route path="/contactus">
                                            <ContactUs />
                                        </Route>
                                        <Route path="/faq">
                                            <Faq />
                                        </Route>
                                        <Route path="/fetchevents">
                                            <FetchEvents />
                                        </Route>
                                        <Route path="/gettingstarted">
                                            <GettingStarted />
                                        </Route>
                                        <Route path="/mydashboard">
                                            <MyDashboard />
                                        </Route>
                                        <Route path="/partners">
                                            <Partners />
                                        </Route>
                                        <Route path="/privacypolicy">
                                            <PrivacyPolicy />
                                        </Route>
                                        <Route path="/sponsors">
                                            <Sponsors />
                                        </Route>
                                        <Route path="/termsofservice">
                                            <TermsOfService />
                                        </Route>
                                        <Route path="/userstories">
                                            <UserStories />
                                        </Route>
                                    </Switch>
                                </UnauthenticatedTemplate>
                                <Footer />
                            </BrowserRouter>
                        </div>
                    </div>
                </div>
            </MsalProvider>
        );
    }
}
