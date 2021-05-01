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
    const [isEventDataLoaded, setIsEventDataLoaded] = React.useState(false);
    const [isUserEventDataLoaded, setIsUserEventDataLoaded] = React.useState(false);
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState(false);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');
        fetch('api/eventtypes', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            });

        fetch('api/Events/active', {
            method: 'GET',
            headers: headers
        })
            .then(response => response.json() as Promise<EventData[]>)
            .then(data => {
                setEventList(data);
                setIsEventDataLoaded(true);
            });

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

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

        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);

        if (!props.isUserLoaded || !props.currentUser) {
            return;
        }

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
                        setIsUserEventDataLoaded(true);
                    })
            });
        }
    }, [props.isUserLoaded, props.currentUser]);

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
                    <MainEvents eventList={eventList} eventTypeList={eventTypeList} myAttendanceList={myAttendanceList} isEventDataLoaded={isEventDataLoaded} isUserEventDataLoaded={isUserEventDataLoaded} isUserLoaded={isUserLoaded} currentUser={currentUser} />
                </div>
                <div style={{ width: 50 + '%' }}>
                    <AzureMapsProvider>
                        <>
                            <MapController center={center} multipleEvents={eventList} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div>
        </div>
    );
}
