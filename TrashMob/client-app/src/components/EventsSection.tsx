import { FC, useEffect, useState, useRef } from 'react'
import capitalize from 'lodash/capitalize'
import uniq from 'lodash/uniq'

import { MainEvents } from './MainEvents';
import { RouteComponentProps, } from 'react-router-dom';
import { data } from 'azure-maps-control';
import * as MapStore from './../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerPointCollection from './MapControllerPointCollection';
import UserData from './Models/UserData';
import { Button} from 'reactstrap';
import { Container} from 'react-bootstrap';
import {EventFilterSection } from './EventFilterSection';
import {
    useGetEventTypes,
    useGetAllEventsBeingAttendedByUser,
    useGetEvents,
    GetEventsParams,
} from '../services/events';
import { EventTimeFrame, EventTimeLine } from '../enums';

export const defaultFilterParams: GetEventsParams = {
    type: EventTimeLine.All,
    timeFrame: EventTimeFrame.AnyTime,
}

export interface EventsSectionProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventsSection: FC<EventsSectionProps> = ({ isUserLoaded, currentUser, history, location, match }) => {
    const [isMapKeyLoaded, setIsMapKeyLoaded] = useState(false);
    const [center, setCenter] = useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = useState<IAzureMapOptions>();
    const [eventView, setEventView] = useState<string>('list');
    
    const divRef = useRef<HTMLDivElement>(null);

    const [filterParams, setFilterParams] = useState<GetEventsParams>(defaultFilterParams)
    const { data: eventTypeList } = useGetEventTypes()
    
    const {
        data: preliminaryResult
    } = useGetEvents({ type: filterParams.type, timeFrame: EventTimeFrame.AnyTime })
    const {
        data: filteredResult,
        isSuccess: isEventDataLoaded
    } = useGetEvents(filterParams)
    
    const {
        data: myAttendanceList,
        isSuccess: isUserEventDataLoaded,
        refetch: refetchAttendanceList
    } = useGetAllEventsBeingAttendedByUser(currentUser, { enabled: isUserLoaded && !!currentUser });

    /** Generate country dropdown options from preliminary result */
    const countryOptions = uniq(preliminaryResult.map(event => event.country))

    /** Generate state/region dropdown options from filtered result */
    let regionOptions: string[] = []
    if (filterParams.country) {
        regionOptions = uniq(
            filteredResult
                .filter(event => event.country === filterParams.country)
                .map(event => event.region)
            )
    }

    /** Generate city dropdown options from filtered result */
    let cityOptions: string[] = []
    if (filterParams.country && filterParams.state) {
        cityOptions = uniq(
            filteredResult
                .filter(event => event.country === filterParams.country && event.region === filterParams.state)
                .map(event => event.city)
            )
    }

    const eventHeader = `${capitalize(filterParams.type)} Events`

    useEffect(() => {
        window.scrollTo(0, 0);

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

    const handleWhichEvents = (eventTimeline: EventTimeLine) => {
        setFilterParams((prevParams) => {
            return {
                ...prevParams,
                type: eventTimeline
            }
        })
    }

    function handleAttendanceChanged() {
        refetchAttendanceList()
    }

    const backToTop = () => {
        if (divRef.current) {
            divRef.current.scrollIntoView();
        }
    }

    return (
        <>
            <Container fluid className="bg-white p-md-5"  id="events" ref={divRef} >
                <div className="max-width-container mx-auto">
                    <div className="d-flex align-items-center py-4">
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
                        onFiltersChange={filterParams => setFilterParams(filterParams)}
                        countryOptions={countryOptions}
                        regionOptions={regionOptions}
                        cityOptions={cityOptions}
                        eventTypeList={eventTypeList}
                    />
                    <div className="d-flex justify-content-between mb-4 flex-wrap flex-md-nowrap">
                        <h3 className="font-weight-bold flex-grow-1">{eventHeader}</h3>
                        <div className="d-flex align-items-center mt-4">
                            <label className="pr-3 mb-0">
                                <input type="radio" className="mb-0 radio" name="Event view" value="map" onChange={e => setEventView(e.target.value)} checked={eventView === "map"}></input>
                                <span className="px-2">Map view</span>
                            </label>
                            <label className="mb-0">
                                <input type="radio" className="mb-0 radio" name="Event view" value="list" onChange={e => setEventView(e.target.value)} checked={eventView === "list"}></input>
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
                                        <MapControllerPointCollection center={center} multipleEvents={filteredResult} myAttendanceList={myAttendanceList} isUserEventDataLoaded={isUserEventDataLoaded} isEventDataLoaded={isEventDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={""} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} onAttendanceChanged={handleAttendanceChanged} onDetailsSelected={handleDetailsSelected} history={history} location={location} match={match} />
                                    </>
                                </AzureMapsProvider>
                            </div>
                        </>
                    ) : (
                        <>
                            <div className="container-lg">
                                <MainEvents
                                    eventList={filteredResult}
                                    eventTypeList={eventTypeList}
                                    myAttendanceList={myAttendanceList || []}
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