import { Component } from 'react';
import * as React from 'react'
import { FetchEvents } from './FetchEvents';  
import { MainCarousel } from './MainCarousel';
import { NearbyEventsMap } from './NearbyEventsMap';

export class Home extends Component {
    static displayName = Home.name;

    render() {
        return (
            <div>
                <div>
                    <MainCarousel />
                </div>
                <div>
                    <FetchEvents />
                </div>
                <div>
                    <NearbyEventsMap />
                </div>
            </div>
        );
    }
}
