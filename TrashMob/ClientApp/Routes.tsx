import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './src/components/Layout';
import { Home } from './src/components/Home';
import { FetchEvent } from './src/components/FetchEvent';
import { AddEvent } from './src/components/AddEvent';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/fetchmobevent' component={FetchEvent} />
    <Route path='/addmobevent' component={AddEvent} />
    <Route path='/mobevent/edit/:eventid' component={AddEvent} />
</Layout>;