import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchEvents } from './components/FetchEvents';
import { AddEvent } from './components/AddEvent';

import './custom.css'
import { About } from './components/About';
import { ContactUs } from './components/ContactUs';
import { Faq } from './components/Faq';
import { GettingStarted } from './components/GettingStarted';
import { Partners } from './components/Partners';
import { PrivacyPolicy } from './components/PrivacyPolicy';
import { Sponsors } from './components/Sponsors';
import { TermsOfService } from './components/TermsOfService';
import { UserDashboard } from './components/UserDashboard';
import { UserStories } from './components/UserStories';

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
            <Route path='/about' component={About} />  
            <Route path='/addevent' component={AddEvent} />
            <Route path='/contactus' component={ContactUs} />
            <Route path='/event/edit/:eventid' component={AddEvent} />
            <Route path='/faq' component={Faq} />
            <Route path='/fetchevents' component={FetchEvents} />
            <Route path='/gettingstarted' component={GettingStarted} />
            <Route path='/partners' component={Partners} />
            <Route path='/privacypolicy' component={PrivacyPolicy} />  
            <Route path='/sponsors' component={Sponsors} />
            <Route path='/termsofservice' component={TermsOfService} />
            <Route path='/userdashboard' component={UserDashboard} />  
            <Route path='/userstories' component={UserStories} />  
      </Layout>
    );
  }
}
