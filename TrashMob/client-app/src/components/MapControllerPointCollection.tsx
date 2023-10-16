import { FC, useContext, useEffect, useState } from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import ReactDOMServer from "react-dom/server"
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../store/AuthStore';
import EventAttendeeData from './Models/EventAttendeeData';
import { getEventType } from '../store/eventTypeHelper';
import { RegisterBtn } from './Customization/RegisterBtn';
import { RouteComponentProps } from 'react-router-dom';
interface MapControllerProps extends RouteComponentProps {
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
    forceReload: boolean;
}

export const MapControllerPointCollection: FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);
    const [isDataSourceLoaded, setIsDataSourceLoaded] = useState(false);

    useEffect(() => {
        if (props.forceReload) {
            // mapRef?.sources.clear();
            setIsDataSourceLoaded(false);
        }
    }, [props.forceReload]);

    useEffect(() => {
        let popup: Popup;
        
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded && !isDataSourceLoaded && isMapReady) {

            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });

            var dataSource = mapRef.sources.getById("mainDataSource") as source.DataSource

            if (!dataSource) {
                dataSource = new source.DataSource("mainDataSource", {
                    cluster: true,
                    clusterMaxZoom: 15,
                    clusterRadius: 45
                });

                mapRef.sources.add(dataSource);
            }

            mapRef.markers.clear();
            dataSource.clear();

            popup = new Popup({
                pixelOffset: [0, -20]
            });

            props.multipleEvents.forEach(mobEvent => {

                const position = new data.Position(mobEvent.longitude, mobEvent.latitude)
                const point = new data.Point(position);
                let isAtt = 'No';
                if (props.isUserLoaded) {
                    var isAttending = props.myAttendanceList && (props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0);
                    isAtt = (isAttending ? 'Yes' : 'No');
                }
                else {
                    isAtt = 'Log in to see your status';
                }

                var isEventComplete = false;
                let currentTime = new Date();
                if (new Date(mobEvent.eventDate) < currentTime) {
                    isEventComplete = true;
                }

                const properties = {
                    eventId: mobEvent.id,
                    eventName: mobEvent.name,
                    eventDate: mobEvent.eventDate,
                    eventTypeList: '',
                    eventTypeId: mobEvent.eventTypeId,
                    streetAddress: mobEvent.streetAddress,
                    city: mobEvent.city,
                    region: mobEvent.region,
                    country: mobEvent.country,
                    postalCode: mobEvent.postalCode,
                    isAttending: isAtt,
                    name: mobEvent.name,
                    creator: mobEvent.createdByUserName,
                    isEventComplete: isEventComplete
                }

                const headers = getDefaultHeaders('GET');
                fetch('/api/eventtypes', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json()).then(data => {
                        const type = getEventType(data, properties.eventTypeId)
                        properties.eventTypeList = type;
                    });

                dataSource.add(new data.Feature(point, properties));

                const marker = new HtmlMarker({
                    position: position,
                    draggable: false,
                    properties: properties,
                    color: (!isEventComplete ? "#96ba00" : "grey")
                })

                mapRef.events.add('mouseover', marker, function (e) {

                    const popUpHtmlContent = ReactDOMServer.renderToString(getPopUpContent(properties.eventId, properties.eventName, properties.eventTypeList, properties.eventDate, properties.city, properties.region, properties.country, properties.postalCode, properties.creator, isAtt));
                    const popUpContent = new DOMParser().parseFromString(popUpHtmlContent, "text/html");
                    const body = popUpContent.querySelector('body');
                    body?.classList.add('p-4', 'map-popup-container');

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
                    var apiConfig = getApiConfig();
                    msalClient.loginRedirect({
                        scopes: apiConfig.b2cScopes
                    }).then(() => {
                        addAttendee(eventId);
                    })
                }
                else {
                    addAttendee(eventId);
                }
            }

            function addAttendee(eventId: string) {

                const account = msalClient.getAllAccounts()[0];
                var apiConfig = getApiConfig();

                const request = {
                    scopes: apiConfig.b2cScopes,
                    account: account
                };

                msalClient.acquireTokenSilent(request).then(tokenResponse => {

                    if (!validateToken(tokenResponse.idTokenClaims)) {
                        return;
                    }

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

            function getPopUpContent(eventId: string, eventName: string, eventType: string, eventDate: Date, city: string, region: string, country: string, postalCode: string, creator: string, isAttending: string) {
                const date = new Date(eventDate).toLocaleDateString([], { month: "long", day: "2-digit", year: "numeric" });
                const time = new Date(eventDate).toLocaleTimeString([], { timeZoneName: 'short' });
                return (
                    <>
                        <div>
                            <h5 className="mt-1 font-weight-bold">{eventName}</h5>
                            <p className="my-3 event-list-event-type p-2 rounded">{eventType}</p>
                            <p className="m-0">{date}, {time}</p>
                            <p className="m-0">
                                {city ? city + "," : ""} {region ? region + "," : ""} {country ? country + "," : ""} {postalCode ? postalCode : ""}
                            </p>
                        </div>
                        <div className="d-flex justify-content-between mt-2">
                            <span className="align-self-end">Created by {creator}</span>
                            <button className="btn btn-outline">
                                <a id="viewDetails" type="button" href={'/eventdetails/' + eventId}>View Details</a>
                            </button>
                            <RegisterBtn eventId={eventId} isAttending={isAttending} isEventCompleted={new Date(eventDate) < new Date()} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} onAttendanceChanged={props.onAttendanceChanged} history={props.history} location={props.location} match={props.match}></RegisterBtn>
                        </div>
                    </>
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
        props.onAttendanceChanged,
        props.myAttendanceList,
        props.isUserEventDataLoaded
    ]);

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