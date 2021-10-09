import { useContext, useEffect } from 'react';
import * as React from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore'
import UserData from './Models/UserData';
import { HtmlMarkerLayer } from './HtmlMarkerLayer/SimpleHtmlMarkerLayer'
import { renderToStaticMarkup, renderToString } from "react-dom/server"
import { Button } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import EventAttendeeData from './Models/EventAttendeeData';

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

export const EventCollectionMapController: React.FC<MapControllerProps> = (props) => {
    // Here you use mapRef from context
    const { mapRef, isMapReady } = useContext<IAzureMapsContextProps>(AzureMapsContext);
    const [isDataSourceLoaded, setIsDataSourceLoaded] = React.useState(false);

    useEffect(() => {
        if (mapRef && props.isEventDataLoaded && props.isMapKeyLoaded && !isDataSourceLoaded && isMapReady) {

            // Simple Camera options modification
            mapRef.setCamera({ center: props.center, zoom: MapStore.defaultUserLocationZoom });

            var dataSourceRef = new source.DataSource("mainDataSource", { cluster: true });
            mapRef.sources.add(dataSourceRef);
            setIsDataSourceLoaded(true);

            // Create a reusable popup.
            const popup = new Popup({
                pixelOffset: [0, -20],
                closeButton: true
            });

            // Create a HTML marker layer for rendering data points.
            var markerLayer = new HtmlMarkerLayer(dataSourceRef, "marker1", {
                markerRenderCallback: (id: any, position: data.Position, properties: any) => {
                    // Create an HtmlMarker.
                    const marker = new HtmlMarker({
                        position: position
                    });

                    mapRef.events.add('mouseover', marker, (event: any) => {
                        const marker = event.target as HtmlMarker & { properties: any };
                        const content = marker.properties.cluster
                            ? `Cluster of ${marker.properties.point_count_abbreviated} markers`
                            : marker.properties.content;
                        popup.setOptions({
                            content: content,
                            position: marker.getOptions().position
                        });

                        // Open the popup.
                        if (mapRef) {
                            popup.open(mapRef);
                        }
                    });

                    // mapRef.events.add('mouseout', marker, (event: any) => popup.close());
                    return marker
                },
                clusterRenderCallback: function (id: any, position: any, properties: any) {
                    const markerCluster = new HtmlMarker({
                        position: position,
                        color: 'DarkViolet',
                        text: properties.point_count_abbreviated,
                    });

                    return markerCluster;
                },
                source: dataSourceRef
            });

            //Add marker layer to the map.
            mapRef.layers.add(markerLayer);

            props.multipleEvents.forEach(mobEvent => {
                var position = new data.Point(new data.Position(mobEvent.longitude, mobEvent.latitude));
                var isAtt = 'No';
                if (props.isUserEventDataLoaded) {
                    var isAttending = props.myAttendanceList && (props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0);
                    isAtt = (isAttending ? 'Yes' : 'No');
                }
                else {
                    isAtt = 'Log in to see your status';
                }

                var properties = {
                    content: renderToString(getPopUpContent(mobEvent.id, mobEvent.name, new Date(mobEvent.eventDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' }), mobEvent.streetAddress, mobEvent.city, mobEvent.region, mobEvent.country, mobEvent.postalCode, isAtt, vd)),
                    name: mobEvent.name,
                }
                dataSourceRef.add(new data.Feature(position, properties));
            })

            function vd(eventId: string) {
                props.onDetailsSelected(eventId);
            }

            function getPopUpContent(eventId: string, eventName: string, eventDate: string, streetAddress: string, city: string, region: string, country: string, postalCode: string, isAttending: string, onViewDetails: any) {

                function handleClick() {
                    onViewDetails(eventId)
                }

                return (
                    <div className="container-fluid card">
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
                                        <Button hidden={!props.isUserLoaded || isAttending === "Yes"} className="action" onClick={() => handleAttend(eventId)}>Register to Attend Event</Button>
                                        <label hidden={props.isUserLoaded}>Sign-in required</label>
                                        <label hidden={!props.isUserLoaded || isAttending !== 'Yes'}>Yes</label>
                                    </td>
                                    <td>
                                        <form>
                                            <button className="action" type="button" onClick={() => handleClick()}>View Details</button>
                                        </form>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                );
            }
        }
    }, [mapRef,
        props.center,
        props.isEventDataLoaded,
        props.isMapKeyLoaded,
        props.multipleEvents,
        props.currentUser,
        props.isUserLoaded,
        isDataSourceLoaded,
        isMapReady]);

    function handleLocationChange(e: any) {
        props.onLocationChange(e);
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

    return (
        <>
            <MapComponent mapOptions={props.mapOptions} isMapKeyLoaded={props.isMapKeyLoaded} onLocationChange={handleLocationChange} />
        </>
    );
};

export default EventCollectionMapController;