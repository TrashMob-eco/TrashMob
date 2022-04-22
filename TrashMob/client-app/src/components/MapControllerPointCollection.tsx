import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import ReactDOMServer from "react-dom/server"
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import EventAttendeeData from './Models/EventAttendeeData';
import { Button } from 'react-bootstrap';

interface MapControllerProps {
    mapOptions: IAzureMapOptions | undefined
    center: data.Position;
    multipleEvents: EventData[],
    isEventDataLoaded: boolean,
    isMapKeyLoaded: boolean
    eventName: string;
    latitude: number;
    longitude: number;
    onLocationChange: any;
    currentUser: UserData;
    isUserLoaded: boolean;
    onAttendanceChanged: any;
    myAttendanceList: EventData[];
    isUserEventDataLoaded: boolean;
    onDetailsSelected: any;
}

export const MapControllerPointCollection: React.FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);
    const [isDataSourceLoaded, setIsDataSourceLoaded] = React.useState(false);

    useEffect(() => {
        let popup: Popup;

        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded && !isDataSourceLoaded && isMapReady) {

            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });

            const dataSourceRef = new source.DataSource("mainDataSource", {
                cluster: true,
                clusterMaxZoom: 15,
                clusterRadius: 45
            });
            mapRef.sources.add(dataSourceRef);

            popup = new Popup({
                pixelOffset: [0, -20]
            });

            props.multipleEvents.forEach(mobEvent => {

                const position = new data.Position(mobEvent.longitude, mobEvent.latitude)
                const point = new data.Point(position);
                let isAtt = 'No';
                if (props.isUserEventDataLoaded) {
                    var isAttending = props.myAttendanceList && (props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0);
                    isAtt = (isAttending ? 'Yes' : 'No');
                }
                else {
                    isAtt = 'Log in to see your status';
                }

                const properties = {
                    eventId: mobEvent.id,
                    eventName: mobEvent.name,
                    eventDate: mobEvent.eventDate,
                    streetAddress: mobEvent.streetAddress,
                    city: mobEvent.city,
                    region: mobEvent.region,
                    country: mobEvent.country,
                    postalCode: mobEvent.postalCode,
                    isAttending: isAtt,
                    name: mobEvent.name,
                }

                dataSourceRef.add(new data.Feature(point, properties));

                const marker = new HtmlMarker({
                    position: position,
                    draggable: false,
                    properties: properties
                })

                mapRef.events.add('mouseover', marker, function (e) {

                    const popUpHtmlContent = ReactDOMServer.renderToString(getPopUpContent(properties.eventName, properties.eventDate, properties.city, properties.region, properties.country, properties.postalCode, isAtt));
                    const popUpContent = new DOMParser().parseFromString(popUpHtmlContent, "text/html");

                    const viewDetailsButton = popUpContent.getElementById("viewDetails");
                    if (viewDetailsButton)
                        viewDetailsButton.addEventListener('click', function () {
                            viewDetails(properties.eventId);
                        });

                    const addAttendeeButton = popUpContent.getElementById("addAttendee");
                    if (addAttendeeButton)
                        addAttendeeButton.addEventListener('click', function () {
                            handleAttend(properties.eventId);
                        });

                    //Update the content and position of the popup.
                    popup.setOptions({
                        content: popUpContent.documentElement,
                        position: position,
                        closeButton: true,
                    });

                    // Open the popup.
                    if (mapRef) {
                        popup.open(mapRef);
                    }
                });

                mapRef.markers.add(marker);
            })

            setIsDataSourceLoaded(true);

            function viewDetails(eventId: string) {
                props.onDetailsSelected(eventId);
            }

            function handleAttend(eventId: string) {

                const accounts = msalClient.getAllAccounts();

                if (accounts === null || accounts.length === 0) {
                    msalClient.loginRedirect().then(() => {
                        addAttendee(eventId);
                    })
                }
                else {
                    addAttendee(eventId);
                }
            }

            function addAttendee(eventId: string) {

                const account = msalClient.getAllAccounts()[0];

                const request = {
                    scopes: apiConfig.b2cScopes,
                    account: account
                };

                msalClient.acquireTokenSilent(request).then(tokenResponse => {

                    const eventAttendee = new EventAttendeeData();
                    eventAttendee.userId = props.currentUser.id;
                    eventAttendee.eventId = eventId;

                    const data = JSON.stringify(eventAttendee);

                    const headers = getDefaultHeaders('POST');
                    headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    // POST request for Add EventAttendee.  
                    fetch('/api/EventAttendees', {
                        method: 'POST',
                        body: data,
                        headers: headers,
                    }).then((response) => response.json())
                        .then(props.onAttendanceChanged())
                })
            }

            function getPopUpContent(eventName: string, eventDate: Date, city: string, region: string, country: string, postalCode: string, isAttending: string) {
                const date = new Date(eventDate).toLocaleDateString([], { month: "long", day: "2-digit", year: "numeric" });
                const time = new Date(eventDate).toLocaleTimeString([], { timeZoneName: 'short' });
                return (
                    <div className="p-4 map-popup-container">
                        <h4 className="mt-1 font-weight-bold">{eventName}</h4>
                        <div><span className="font-weight-bold">Event Date: </span><span>{date}</span></div>
                        <div><span className="font-weight-bold">Time: </span><span>{time}</span></div>
                        <div><span className="font-weight-bold">Location: </span><span>{city}, {region}, {country}, {postalCode}</span></div>
                        <div>
                            <a id="addAttendee" hidden={!props.isUserLoaded || isAttending === "Yes"} className="action">Register to Attend Event</a>
                            <span hidden={props.isUserLoaded}>Sign-in required</span>
                            <span hidden={!props.isUserLoaded || isAttending !== 'Yes'} className="font-weight-bold">Attending: Yes</span>
                        </div>
                        <button className="btn btn-primary mt-2 w-100">
                            <a id="viewDetails" type="button" className="color-white">View Details</a>
                        </button>
                    </div >
                );
            }
        }
    }, [mapRef,
        props,
        props.center,
        props.isEventDataLoaded,
        props.isMapKeyLoaded,
        props.multipleEvents,
        props.currentUser,
        props.isUserLoaded,
        isDataSourceLoaded,
        isMapReady,
        props.onAttendanceChanged]);

    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }
    return (
        <>
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default MapControllerPointCollection;