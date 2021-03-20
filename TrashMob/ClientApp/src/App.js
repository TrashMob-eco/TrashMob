import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchMobEvent } from './components/FetchMobEvent';
import { AddMobEvent } from './components/AddMobEvent';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
            <Route path='/fetchmobevent' component={FetchMobEvent} />
            <Route path='/addmobevent' component={AddMobEvent} />
            <Route path='/mobevent/edit/:eventid' component={AddMobEvent} />  
      </Layout>
    );
  }
}
