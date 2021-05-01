import * as React from 'react'

import { MainEvents } from './MainEvents';
import { MainCarousel } from './MainCarousel';
import { Link, RouteComponentProps } from 'react-router-dom';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import UserData from './Models/UserData';

export interface HomeProps extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const Home: React.FC<HomeProps> = (props) => {
    const [eventList, setEventList] = React.useState<EventData[]>([]);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [myAttendanceList, setMyAttendanceList] = React.useState<EventData[]>([]);
    const [isDataLoaded, setIsDataLoaded] = React.useState(false);
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState(false);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();

    const headers = getDefaultHeaders('GET');
    getEventTypes();

    fetch('api/Events/active', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<EventData[]>)
        .then(data => {
            setEventList(data);
            setIsDataLoaded(true);
        });

    MapStore.getOption().then(opts => {
        setMapOptions(opts);
        setIsMapKeyLoaded(true);
    })

    React.useEffect(() => {
        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, [])

    React.useEffect(() => {
        // If the user is logged in, get the events they are attending
        var accounts = msalClient.getAllAccounts();

        if (accounts !== null && accounts.length > 0) {
            var request = {
                scopes: apiConfig.b2cScopes,
                account: accounts[0]
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('api/events/eventsuserisattending/' + props.currentUser.id, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<EventData[]>)
                    .then(data => {
                        setMyAttendanceList(data);
                        setIsDataLoaded(true);
                    })
            });
        }
    }, [props.isUserLoaded, props.currentUser]);

    function getEventTypes() {
        const headers = getDefaultHeaders('GET');

        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            });
    }

    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    return (
        <div>
            <div>
                <MainCarousel />
            </div>
            <div>
                <div>
                    <Link to="/createevent">Create a New Event</Link>
                </div>
                <div style={{ width: 50 + '%' }}>
                    <MainEvents eventList={eventList} eventTypeList={eventTypeList} myAttendanceList={myAttendanceList} isDataLoaded={isDataLoaded} isUserLoaded={props.isUserLoaded} currentUser={props.currentUser} />
                </div>
                <div style={{ width: 50 + '%' }}>
                    <AzureMapsProvider>
                        <>
                            <MapController center={center} multipleEvents={eventList} isDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUserId={props.currentUser.id} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div>
        </div>
    );
}
