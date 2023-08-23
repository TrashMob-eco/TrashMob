import { FC, useEffect, useState } from 'react'
import { Link, RouteComponentProps } from 'react-router-dom';
import { getEventType } from '../store/eventTypeHelper';
import EventData from './Models/EventData';
import EventTypeData from './Models/EventTypeData';
import UserData from './Models/UserData';
import { RegisterBtn } from './Customization/RegisterBtn';

export interface DisplayEvent {
    id: string;
    name: string;
    eventDate: Date;
    eventTypeId: number;
    city: string;
    region: string;
    country: string;
    isAttending: string;
    creator: string;
}

export interface MainEventsDataProps extends RouteComponentProps {
    eventList: EventData[];
    eventTypeList: EventTypeData[];
    myAttendanceList: EventData[];
    isEventDataLoaded: boolean;
    isUserEventDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onAttendanceChanged: any;
};

export const MainEvents: FC<MainEventsDataProps> = ({ isEventDataLoaded, eventList, isUserEventDataLoaded,
    myAttendanceList, isUserLoaded, eventTypeList, currentUser, onAttendanceChanged, history, location, match }) => {
    const [displayEvents, setDisplayEvents] = useState<DisplayEvent[]>([]);

    useEffect(() => {
        if (isEventDataLoaded && eventList) {
            const list = eventList.map((mobEvent) => {
                const dispEvent: DisplayEvent = {
                    id: mobEvent.id,
                    city: mobEvent.city,
                    region: mobEvent.region,
                    country: mobEvent.country,
                    eventDate: mobEvent.eventDate,
                    eventTypeId: mobEvent.eventTypeId,
                    name: mobEvent.name,
                    creator: mobEvent.createdByUserName,
                    isAttending: ''
                }

                if (isUserEventDataLoaded) {
                    const isAttending = myAttendanceList && (myAttendanceList.findIndex((e) => e.id === mobEvent.id) >= 0);
                    dispEvent.isAttending = (isAttending ? 'Yes' : 'No');
                }
                else {
                    dispEvent.isAttending = 'Log in to see your status';
                }

                return dispEvent;
            });
            setDisplayEvents(list);
        }
    }, [isEventDataLoaded, eventList, myAttendanceList, isUserLoaded, isUserEventDataLoaded])

    const renderEventsList = (events: DisplayEvent[]) => {
        const sortedEvents = events.sort((a, b) => (a.eventDate > b.eventDate) ? 1 : -1)
        return (
            <>
                <ol className="px-1 px-md-5">
                    {sortedEvents.map((mobEvent, i) =>
                        <li className={`d-flex flex-column justify-content-center mb-4 ${i !== sortedEvents.length - 1 ? "border-bottom" : ""}`} key={`event-${i}`}>
                            <div className="d-flex justify-content-between align-items-start align-items-sm-end flex-column flex-sm-row">
                                <h5 className="font-weight-bold font-size-xl">{mobEvent.name}</h5>
                                <span className="font-grey">Created by: {mobEvent.creator}</span>
                            </div>
                            <span className="my-2 event-list-event-type p-2 rounded">{getEventType(eventTypeList, mobEvent.eventTypeId)}</span>
                            <div className="d-flex justify-content-between align-items-start align-items-sm-end mb-4 flex-column flex-sm-row">
                                <div className="d-inline-block font-grey">
                                    <p>{new Date(mobEvent.eventDate).toLocaleDateString("en-US", { month: "long", day: "numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' })}</p>
                                    <span>{mobEvent.city}, {mobEvent.region}, {mobEvent.country}</span>
                                </div>
                                <div className="mt-3 mt-sm-0">
                                    <Link to={'/eventdetails/' + mobEvent.id}><button className="btn btn-outline mr-2 font-weight-bold btn-128">View</button></Link>
                                    <RegisterBtn eventId={mobEvent.id} isAttending={mobEvent.isAttending} isEventCompleted={new Date(mobEvent.eventDate) < new Date()} currentUser={currentUser} onAttendanceChanged={onAttendanceChanged} isUserLoaded={isUserLoaded} history={history} location={location} match={match} ></RegisterBtn>
                                </div>
                            </div>
                        </li>
                    )}
                </ol>
            </>
        )
    }

    return (
        <>
            {!isEventDataLoaded && <p><em>Loading...</em></p>}
            {isEventDataLoaded && renderEventsList(displayEvents)}
        </>
    );
}