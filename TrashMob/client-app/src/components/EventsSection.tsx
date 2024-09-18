import { FC, useEffect, useState, useCallback, useRef } from 'react'
import capitalize from 'lodash/capitalize'

import { MainEvents } from './MainEvents';
import { RouteComponentProps, } from 'react-router-dom';
import EventData from './Models/EventData';
import { data } from 'azure-maps-control';
import * as MapStore from './../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerPointCollection from './MapControllerPointCollection';
import UserData from './Models/UserData';
import { Button} from 'reactstrap';
import { Container} from 'react-bootstrap';
import {EventFilterSection } from './EventFilterSection';
import { useQuery } from '@tanstack/react-query';
import { GetAllEventsBeingAttendedByUser, GetEventTypes } from '../services/events';
import { Services } from '../config/services.config';
import { ApiService } from '../services';
import dayjs from 'dayjs';
import isBetween from "dayjs/plugin/isBetween"
import { EventTimeFrame, EventTimeLine } from '../enums';
dayjs.extend(isBetween)

export interface EventsSectionProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export type EventFilterParams = {
    type: EventTimeLine
    country?: string
    state?: string
    cities?: string[]
    cleanTypes: string[]
    timeFrame: EventTimeFrame
}

export const defaultFilterParams: EventFilterParams = {
    type: EventTimeLine.All,
    timeFrame: EventTimeFrame.AnyTime,
    cleanTypes: []
}

const isInTimeFrame = (event: EventData, timeFrame: EventTimeFrame) => {
    const now = dayjs()
    const eventDate = dayjs(event.eventDate)

    switch (timeFrame) {
        case EventTimeFrame.Next24Hours:
            return eventDate.isBetween(now, now.add(24, 'hours'), "hour", "[]")
        case EventTimeFrame.NextWeek:
            return eventDate.isBetween(now, now.add(7, 'days'), "day", "[]")
        case EventTimeFrame.NextMonth:
            return eventDate.isBetween(now, now.add(1, 'months'), "day", "[]")
        case EventTimeFrame.Past24Hours:
            return eventDate.isBetween(now.subtract(1, 'day'), now, "day", "[]")
        case EventTimeFrame.PastWeek:
            return eventDate.isBetween(now.subtract(7, 'day'), now, "day", "[]")
        case EventTimeFrame.PastMonth:
            return eventDate.isBetween(now.subtract(1, 'months'), now, "day", "[]")
        default:
            return true;
    }
}

const useQueryEventTypes = () => {
    return useQuery({ 
        queryKey: GetEventTypes().key,
        queryFn: GetEventTypes().service,
        initialData: () => [],
        staleTime: Services.CACHE.DISABLE,
    })
}

const useQueryEvents = (filterParams: EventFilterParams = defaultFilterParams) => {
    return useQuery({
        queryKey: ["events", filterParams],
        queryFn: async ({ queryKey }) => {
            const params = queryKey[1] as EventFilterParams 

            const url: string = {
                [EventTimeLine.All]: '/Events/notcanceled',
                [EventTimeLine.Completed]: '/Events/completed',
                [EventTimeLine.Upcoming]: '/Events/active'
            }[params.type]

            const { data: events } = await ApiService('public').fetchData<EventData[]>({ url, method: 'get' })
            const filteredEvents = events
                .filter(event => !params.country || event.country === params.country)
                .filter(event => !params.country || !params.state || event.region === params.state)
                .filter(event => !params.country || !params.state || !params.cities || params.cities.includes(event.city))
                .filter(event => params.cleanTypes.length === 0 || params.cleanTypes.includes(`${event.eventTypeId}`))
                .filter(event => !params.timeFrame || isInTimeFrame(event, params.timeFrame))

            console.log({ params, events, filteredEvents })
            return filteredEvents
        },
        initialData: () => []
    })
}

const generateLocationMap = (events: EventData[]) => {
    return events.reduce((accuMap, event) => {
        const {country, region, city} = event;
        if (!accuMap.has(country)) {
            accuMap.set(country, new Map<string, Set<string>>());
        }

        const stateMap = accuMap.get(country);
        if(stateMap) {
            if(!stateMap.has(region)) {
                stateMap.set(region, new Set<string>());
            }
            
            const citySet = stateMap.get(region);
            if(citySet) citySet.add(city);
        }
        return accuMap
    }, new Map<string, Map<string, Set<string>>>())
}

export const EventsSection: FC<EventsSectionProps> = ({ isUserLoaded, currentUser, history, location, match }) => {
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState(false);
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [eventView, setEventView] = useState<string>('list');
    
    const [myAttendanceList, setMyAttendanceList] = useState<EventData[]>([]);
    const [isUserEventDataLoaded, setIsUserEventDataLoaded] = useState(false);
    const [forceReload, setForceReload] = useState(false);
    
    const divRef = useRef<HTMLDivElement>(null);

    const [filterParams, setFilterParams] = useState<EventFilterParams>(defaultFilterParams)
    const { data: eventTypeList } = useQueryEventTypes()
    const { data: presentEventList, isSuccess: isEventDataLoaded } = useQueryEvents(filterParams)
    const locationMap = generateLocationMap(presentEventList)
    const eventHeader = `${capitalize(filterParams.type)} Events`

    const getEventsBeingAttendedByUser = useQuery({ 
        queryKey: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).key,
        queryFn: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).service, 
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    });

    useEffect(()=>{
        setForceReload(false);
    },[presentEventList])

    useEffect(() => {
        window.scrollTo(0, 0);

        if (isUserLoaded && currentUser) {
            setMyAttendanceList([]);
            setIsUserEventDataLoaded(false);
            // If the user is logged in, get the events they are attending
            getEventsBeingAttendedByUser.refetch().then(res => {
                setMyAttendanceList(res.data?.data || []);
                setIsUserEventDataLoaded(true);
            })
        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                const point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, [isUserLoaded, currentUser])

    const handleLocationChange = (point: data.Position) => {
        // do nothing
    }

    const handleDetailsSelected = (eventId: string) => {
        history.push("eventdetails/" + eventId);
    }

    const handleEventView = (view: string) => {
        setEventView(view);
    }

    const handleWhichEvents = (eventTimeline: EventTimeLine) => {
        setFilterParams((prevParams) => {
            return {
                ...prevParams,
                type: eventTimeline
            }
        })
    }

    function handleAttendanceChanged() {
        setMyAttendanceList([]);
        setIsUserEventDataLoaded(false);
        if (!isUserLoaded || !currentUser) return;
        // If the user is logged in, get the events they are attending
        getEventsBeingAttendedByUser.refetch().then(res => {
            setMyAttendanceList(res.data?.data || []);
            setIsUserEventDataLoaded(true);
        })
    }

    const onFiltersChange = useCallback((country: string, state: string, cities: string[], cleanTypes: string[], timeFrame: EventTimeFrame) => {
        console.log(`onFilterChange`, { country, state, cities, cleanTypes, timeFrame })
        setFilterParams((prevParams) => {
            return {
                ...prevParams,
                country,
                state,
                cities,
                cleanTypes,
                timeFrame,
            }
        })
        
    },[setFilterParams]);

    const backToTop = () =>{
        if(divRef.current)
        {
            divRef.current.scrollIntoView();
        }
    }

    return (
        <>
            <Container fluid className="bg-white p-md-5"  id="events" ref={divRef} >
                <div className="max-width-container mx-auto">
                    <div className="d-flex align-items-center mt-4">
                        <label className="mb-0">
                            <input  type="radio" className="mb-0 radio" name="Which events" value={EventTimeLine.Upcoming}
                                onChange={e => handleWhichEvents(e.target.value as EventTimeLine)}
                                checked={filterParams.type === EventTimeLine.Upcoming}
                            />
                            <span className="px-2">Upcoming Events</span>
                        </label>
                        <label className="pr-3 mb-0">
                            <input type="radio" className="mb-0 radio" name="Which events" value={EventTimeLine.Completed}
                                onChange={e => handleWhichEvents(e.target.value as EventTimeLine)}
                                checked={filterParams.type === EventTimeLine.Completed}
                            />
                            <span className="px-2">Completed Events</span>
                        </label>
                        <label className="pr-3 mb-0">
                            <input type="radio" className="mb-0 radio" name="Which events" value={EventTimeLine.All}
                                onChange={e => handleWhichEvents(e.target.value as EventTimeLine)}
                                checked={filterParams.type === EventTimeLine.All}
                            />
                            <span className="px-2">All Events</span>
                        </label>
                    </div>
                    <EventFilterSection
                        filterParams={filterParams}
                        defaultFilterParams={defaultFilterParams}
                        onResetFilters={() => setFilterParams(defaultFilterParams)}
                        updateEventsByFilters={onFiltersChange}
                        locationMap={locationMap}
                        eventTypeList={eventTypeList}
                    />
                    <div className="d-flex justify-content-between mb-4 flex-wrap flex-md-nowrap">
                        <h3 className="font-weight-bold flex-grow-1">{eventHeader}</h3>
                        <div className="d-flex align-items-center mt-4">
                            <label className="pr-3 mb-0">
                                <input type="radio" className="mb-0 radio" name="Event view" value="map" onChange={e => handleEventView(e.target.value)} checked={eventView === "map"}></input>
                                <span className="px-2">Map view</span>
                            </label>
                            <label className="mb-0">
                                <input type="radio" className="mb-0 radio" name="Event view" value="list" onChange={e => handleEventView(e.target.value)} checked={eventView === "list"}></input>
                                <span className="px-2">List view</span>
                            </label>
                        </div>
                    </div>
                    {eventView === 'map' ? (
                        <>
                            <Button color='primary' className='mb-2' onClick={() => history.push("/manageeventdashboard")}>Create a New Event</Button>
                            <div className="w-100 m-0">
                                <AzureMapsProvider>
                                    <>
                                        <MapControllerPointCollection forceReload={forceReload} center={center} multipleEvents={presentEventList} myAttendanceList={myAttendanceList} isUserEventDataLoaded={isUserEventDataLoaded} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} onDetailsSelected={handleDetailsSelected} history={history} location={location} match={match} />
                                    </>
                                </AzureMapsProvider>
                            </div>
                        </>
                    ) : (
                        <>
                            <div className="container-lg">
                                <MainEvents
                                    eventList={presentEventList || []}
                                    eventTypeList={eventTypeList}
                                    myAttendanceList={myAttendanceList}
                                    isEventDataLoaded={isEventDataLoaded}
                                    isUserEventDataLoaded={isUserEventDataLoaded}
                                    isUserLoaded={isUserLoaded}
                                    currentUser={currentUser}
                                    onAttendanceChanged={handleAttendanceChanged}
                                    history={history}
                                    location={location}
                                    match={match}
                                    backToTop={backToTop}
                                />
                            </div>
                        </>
                    )}
                </div>
            </Container>
        </>
    );
}