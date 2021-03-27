import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchEvents } from './components/FetchEvents';
import { AddEvent } from './components/AddEvent';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
            <Route path='/fetchevents' component={FetchEvents} />
            <Route path='/addevent' component={AddEvent} />
            <Route path='/event/edit/:eventid' component={AddEvent} />  
      </Layout>
    );
  }
}
