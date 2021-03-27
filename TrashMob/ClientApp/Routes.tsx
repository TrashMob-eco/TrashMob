import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './src/components/Layout';
import { Home } from './src/components/Home';
import { FetchEvents } from './src/components/FetchEvents';
import { AddEvent } from './src/components/AddEvent';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/fetchevents' component={FetchEvents} />
    <Route path='/addevent' component={AddEvent} />
    <Route path='/event/edit/:eventid' component={AddEvent} />
</Layout>;