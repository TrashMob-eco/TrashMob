import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './src/components/Layout';
import { Home } from './src/components/Home';
import { FetchMobEvent } from './src/components/FetchMobEvent';
import { AddMobEvent } from './src/components/AddMobEvent';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
    <Route path='/fetchmobevent' component={FetchMobEvent} />
    <Route path='/addmobevent' component={AddMobEvent} />
    <Route path='/mobevent/edit/:eventid' component={AddMobEvent} />
</Layout>;