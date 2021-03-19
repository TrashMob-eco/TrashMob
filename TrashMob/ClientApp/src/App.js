import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchCleanupEvent } from './components/FetchCleanupEvent';
import { AddCleanupEvent } from './components/AddCleanupEvent';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
            <Route path='/fetchcleanupevent' component={FetchCleanupEvent} />
            <Route path='/addcleanupevent' component={AddCleanupEvent} />
            <Route path='/cleanupevent/edit/:eventid' component={AddCleanupEvent} />  
      </Layout>
    );
  }
}
