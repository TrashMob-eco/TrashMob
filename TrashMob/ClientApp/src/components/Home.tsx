import { Component } from 'react';
import * as React from 'react'

import { MainEvents } from './MainEvents';
import { MainCarousel } from './MainCarousel';
import NearbyEventsMap from './NearbyEventsMap';
import { Link } from 'react-router-dom';

interface PropsType { };

interface DataState { };

export class Home extends Component<PropsType, DataState> {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <div>
                    <MainCarousel />
                </div>
                <div>
                    <div>
                        <Link to="/createevent">Create a New Event</Link>
                    </div>
                    <div style={{ width: 50+'%' }}>
                        <MainEvents />
                    </div>
                    <div style={{ width: 50+'%' }}>
                        <NearbyEventsMap />
                    </div>
                </div>
            </div>
        );
    }
}
