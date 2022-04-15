import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker, layer } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/src/layer/HtmlMarkerLayer'
import ReactDOMServer, { renderToString } from "react-dom/server"
import { Button } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import EventAttendeeData from './Models/EventAttendeeData';
import { Position } from '@fluentui/react';

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
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded && !isDataSourceLoaded && isMapReady) {

            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });

            var dataSourceRef = new source.DataSource("mainDataSource", {
                cluster: true,
                clusterMaxZoom: 15,
                clusterRadius: 45
            });
            mapRef.sources.add(dataSourceRef);

            //// Create a bubble layer for rendering clustered data points.
            //var clusterBubbleLayer = new layer.BubbleLayer(dataSourceRef, "bubbleLayer", {
            //    // Scale the size of the clustered bubble based on the number of points in the cluster.
            //    radius: [
            //        'step',
            //        ['get', 'point_count'],
            //        20,         // Default of 20 pixel radius.
            //        100, 30,    // If point_count >= 100, radius is 30 pixels.
            //        750, 40     // If point_count >= 750, radius is 40 pixels.
            //    ],

            //    // Change the color of the cluster based on the value on the point_cluster property of the cluster.
            //    color: [
            //        'step',
            //        ['get', 'point_count'],
            //        'lime',            //Default to lime green. 
            //        100, 'yellow',     //If the point_count >= 100, color is yellow.
            //        750, 'red'        //If the point_count >= 100, color is red.
            //    ],
            //    strokeWidth: 0,
            //    filter: ['has', 'point_count'] //Only rendered data points which have a point_count property, which clusters do.
            //});

            //// Create a layer to render the individual locations.
            //var pointLayer = new layer.SymbolLayer(dataSourceRef, "pointLayer", {
            //    filter: ['!', ['has', 'point_count']], // Filter out clustered points from this layer.   
            //    iconOptions: { allowOverlap: true }
            //});

            //// Create a symbol layer to render the count of locations in a cluster.
            //var clusterLayer = new layer.SymbolLayer(dataSourceRef, "clusterLayer", {
            //    iconOptions: {
            //        image: 'none' //Hide the icon image.
            //    },
            //    textOptions: {
            //        textField: ['get', 'point_count_abbreviated'],
            //        offset: [0, 0.4]
            //    }
            //});

            //// Add the clusterBubbleLayer and two additional layers to the map.
            //mapRef.layers.add([
            //    //clusterBubbleLayer,
            //    //clusterLayer,
            //    pointLayer
            //]);

            props.multipleEvents.forEach(mobEvent => {

                var position = new data.Position(mobEvent.longitude, mobEvent.latitude)
                var point = new data.Point(position);
                var isAtt = 'No';
                if (props.isUserEventDataLoaded) {
                    var isAttending = props.myAttendanceList && (props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0);
                    isAtt = (isAttending ? 'Yes' : 'No');
                }
                else {
                    isAtt = 'Log in to see your status';
                }

                var properties = {
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

                var marker = new HtmlMarker({
                    position: position,
                    draggable: false,
                    properties: properties
                })

                mapRef.events.add('click', marker, function (e) {

                    var properties = e.properties;

                    var popUpHtmlContent = ReactDOMServer.renderToString(getPopUpContent(properties.eventId, properties.eventName, properties.eventDate, properties.streetAddress, properties.city, properties.region, properties.country, properties.postalCode, properties.isAttending));
                    var popUpContent = new DOMParser().parseFromString(popUpHtmlContent, "text/xml");

                    var viewDetailsButton = popUpContent.getElementById("viewDetails");
                    if (viewDetailsButton)
                        viewDetailsButton.addEventListener('click', function () {
                            viewDetails(properties.eventId);
                        });

                    var addAttendeeButton = popUpContent.getElementById("addAttendee");
                    if (addAttendeeButton)
                        addAttendeeButton.addEventListener('click', function () {
                            handleAttend(properties.eventId);
                        });

                    // Create a popup.
                    var popup = new Popup({
                        content: popUpContent.documentElement,
                        position: e.target.position,
                        pixelOffset: [0, -20],
                        closeButton: true
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

                var accounts = msalClient.getAllAccounts();

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

                var request = {
                    scopes: apiConfig.b2cScopes,
                    account: account
                };

                msalClient.acquireTokenSilent(request).then(tokenResponse => {

                    var eventAttendee = new EventAttendeeData();
                    eventAttendee.userId = props.currentUser.id;
                    eventAttendee.eventId = eventId;

                    var data = JSON.stringify(eventAttendee);

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

            function getPopUpContent(eventId: string, eventName: string, eventDate: string, streetAddress: string, city: string, region: string, country: string, postalCode: string, isAttending: string) {

                return (
                    <div className="container card" style={{ padding: "0.5rem" }}>
                        <h4>{eventName}</h4>
                        <table>
                            <tbody>
                                <tr>
                                    <td>Event Date:</td>
                                    <td>{eventDate}</td>
                                </tr>
                                <tr>
                                    <td>Location:</td>
                                    <td>{streetAddress}, {city}, {region}, {country}, {postalCode}</td>
                                </tr>
                                <tr>
                                    <td>
                                        <a id="addAttendee" hidden={!props.isUserLoaded || isAttending === "Yes"} className="action" onClick={() => handleAttend(eventId)}>Register to Attend Event</a>
                                        <label hidden={props.isUserLoaded}>Sign-in required</label>
                                        <label hidden={!props.isUserLoaded || isAttending !== 'Yes'}>Yes</label>
                                    </td>
                                    <td>
                                        <a id="viewDetails" className="action" type="button">View Details</a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
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