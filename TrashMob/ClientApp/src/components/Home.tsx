import React from 'react';
import { FetchEvents } from './FetchEvents';  
import { MainCarousel } from './MainCarousel';
import { NearbyEventsMap } from './NearbyEventsMap';

interface PropsType { };

interface DataState { };

export class Home extends React.Component<PropsType, DataState> {
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
