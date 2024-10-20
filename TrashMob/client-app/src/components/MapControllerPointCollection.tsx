import { FC, useContext, useEffect, useState } from 'react';
import { AzureMapsContext, IAzureMapOptions, IAzureMapsContextProps } from 'react-azure-maps';
import { data, source, Popup, HtmlMarker } from 'azure-maps-control';
import ReactDOMServer from 'react-dom/server';
import { RouteComponentProps } from 'react-router-dom';
import { useMutation, useQuery } from '@tanstack/react-query';
import MapComponent from './MapComponent';
import EventData from './Models/EventData';
import * as MapStore from '../store/MapStore';
import UserData from './Models/UserData';
import { getApiConfig, msalClient } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { RegisterBtn } from './Customization/RegisterBtn';
import { AddEventAttendee, GetEventTypes } from '../services/events';
import { Services } from '../config/services.config';
import WaiverData from './Models/WaiverData';
import { GetTrashMobWaivers } from '../services/waivers';

interface MapControllerProps extends RouteComponentProps {
    mapOptions: IAzureMapOptions | undefined;
    center: data.Position;
    multipleEvents: EventData[];
    isEventDataLoaded: boolean;
    isMapKeyLoaded: boolean;
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
    const [waiver, setWaiver] = useState<WaiverData>();

    const getEventTypes = useQuery({
        queryKey: GetEventTypes().key,
        queryFn: GetEventTypes().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getTrashMobWaivers = useQuery({
        queryKey: GetTrashMobWaivers().key,
        queryFn: GetTrashMobWaivers().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const addEventAttendee = useMutation({
        mutationKey: AddEventAttendee().key,
        mutationFn: AddEventAttendee().service,
    });

    useEffect(() => {
        getTrashMobWaivers.refetch().then((res) => {
            setWaiver(res.data?.data);
        });
    }, []);

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
            mapRef.setCamera({
                center: props.center,
                zoom: MapStore.defaultUserLocationZoom,
            });

            let dataSource = mapRef.sources.getById('mainDataSource') as source.DataSource;

            if (!dataSource) {
                dataSource = new source.DataSource('mainDataSource', {
                    cluster: true,
                    clusterMaxZoom: 15,
                    clusterRadius: 45,
                });

                mapRef.sources.add(dataSource);
            }

            mapRef.markers.clear();
            dataSource.clear();

            popup = new Popup({
                pixelOffset: [0, -20],
            });

            props.multipleEvents.forEach((mobEvent) => {
                const position = new data.Position(mobEvent.longitude, mobEvent.latitude);
                const point = new data.Point(position);
                let isAtt = 'No';
                if (props.isUserLoaded) {
                    const isAttending =
                        props.myAttendanceList && props.myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0;
                    isAtt = isAttending ? 'Yes' : 'No';
                } else {
                    isAtt = 'Log in to see your status';
                }

                let isEventComplete = false;
                const currentTime = new Date();
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
                    isEventComplete,
                };

                getEventTypes.refetch().then(async (res) => {
                    if (res.data === undefined) throw new Error();
                    const type = getEventType(res.data.data, properties.eventTypeId);
                    properties.eventTypeList = type;
                });

                dataSource.add(new data.Feature(point, properties));

                const marker = new HtmlMarker({
                    position,
                    draggable: false,
                    properties,
                    color: !isEventComplete ? '#96ba00' : 'grey',
                });

                mapRef.events.add('mouseover', marker, (e) => {
                    const popUpHtmlContent = ReactDOMServer.renderToString(
                        getPopUpContent(
                            properties.eventId,
                            properties.eventName,
                            properties.eventTypeList,
                            properties.eventDate,
                            properties.city,
                            properties.region,
                            properties.country,
                            properties.postalCode,
                            properties.creator,
                            isAtt,
                        ),
                    );
                    const popUpContent = new DOMParser().parseFromString(popUpHtmlContent, 'text/html');
                    const body = popUpContent.querySelector('body');
                    body?.classList.add('p-4', 'map-popup-container');

                    const viewDetailsButton = popUpContent.getElementById('viewDetails');
                    if (viewDetailsButton) {
                        viewDetailsButton.addEventListener('click', () => {
                            viewDetails(properties.eventId);
                        });
                    }

                    const addAttendeeButton = popUpContent.getElementById('addAttendee');
                    if (addAttendeeButton) {
                        addAttendeeButton.addEventListener('click', () => {
                            handleAttend(properties.eventId);
                        });
                    }

                    // Update the content and position of the popup.
                    popup.setOptions({
                        content: popUpContent.documentElement,
                        position,
                        closeButton: true,
                    });

                    // Open the popup.
                    if (mapRef) {
                        popup.open(mapRef);
                    }
                });

                mapRef.markers.add(marker);
            });

            setIsDataSourceLoaded(true);

            function viewDetails(eventId: string) {
                props.onDetailsSelected(eventId);
            }

            function handleAttend(eventId: string) {
                const accounts = msalClient.getAllAccounts();

                if (accounts === null || accounts.length === 0) {
                    const apiConfig = getApiConfig();
                    msalClient
                        .loginRedirect({
                            scopes: apiConfig.b2cScopes,
                        })
                        .then(() => {
                            addAttendee(eventId);
                        });
                } else {
                    addAttendee(eventId);
                }
            }

            function addAttendee(eventId: string) {
                addEventAttendee
                    .mutateAsync({
                        userId: props.currentUser.id,
                        eventId,
                    })
                    .then(() => props.onAttendanceChanged());
            }

            function getPopUpContent(
                eventId: string,
                eventName: string,
                eventType: string,
                eventDate: Date,
                city: string,
                region: string,
                country: string,
                postalCode: string,
                creator: string,
                isAttending: string,
            ) {
                const date = new Date(eventDate).toLocaleDateString([], {
                    month: 'long',
                    day: '2-digit',
                    year: 'numeric',
                });
                const time = new Date(eventDate).toLocaleTimeString([], {
                    timeZoneName: 'short',
                });
                return (
                    <>
                        <div>
                            <h5 className='mt-1 font-weight-bold'>{eventName}</h5>
                            <p className='my-3 event-list-event-type p-2 rounded'>{eventType}</p>
                            <p className='m-0'>
                                {date},{time}
                            </p>
                            <p className='m-0'>
                                {city ? `${city},` : ''} {region ? `${region},` : ''} {country ? `${country},` : ''}{' '}
                                {postalCode || ''}
                            </p>
                        </div>
                        <div className='d-flex justify-content-between mt-2'>
                            <span className='align-self-end'>
                                Created by
                                {creator}
                            </span>
                            <button className='btn btn-outline'>
                                <a id='viewDetails' type='button' href={`/eventdetails/${eventId}`}>
                                    View Details
                                </a>
                            </button>
                            <RegisterBtn
                                eventId={eventId}
                                isAttending={isAttending}
                                isEventCompleted={new Date(eventDate) < new Date()}
                                currentUser={props.currentUser}
                                isUserLoaded={props.isUserLoaded}
                                onAttendanceChanged={props.onAttendanceChanged}
                                history={props.history}
                                location={props.location}
                                match={props.match}
                                addEventAttendee={addEventAttendee}
                                waiverData={waiver}
                            />
                        </div>
                    </>
                );
            }
        }
    }, [
        mapRef,
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
        props.isUserEventDataLoaded,
    ]);

    function handleLocationChange(e: any) {
        props.onLocationChange(e);
    }
    return (
        <MapComponent
            mapOptions={props.mapOptions}
            isMapKeyLoaded={props.isMapKeyLoaded}
            onLocationChange={handleLocationChange}
        />
    );
};

export default MapControllerPointCollection;
