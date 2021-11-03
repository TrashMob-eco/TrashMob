import * as React from 'react'
import { getDefaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import DisplayEventSummary from './Models/DisplayEventSummary';
import EventTypeData from './Models/EventTypeData';

export const EventSummaries: React.FC = () => {
    const [displaySummaries, setDisplaySummaries] = React.useState<DisplayEventSummary[]>([]);
    const [eventTypeList, setEventTypeList] = React.useState<EventTypeData[]>([]);
    const [isEventSummaryDataLoaded, setIsEventSummaryDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/eventtypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setEventTypeList(data);
            })
            .then(() => {

            fetch('/api/eventsummaries', {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<DisplayEventSummary[]>)
                .then(data => {
                    setDisplaySummaries(data);
                    setIsEventSummaryDataLoaded(true);
                });
        })
    }, [])

    function renderEventSummariesTable(events: DisplayEventSummary[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Date</th>
                            <th>Event Type</th>
                            <th>Address</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                            <th>Attendees</th>
                            <th>Total Bags</th>
                            <th>Duration in Minutes</th>
                        </tr>
                    </thead>
                    <tbody>
                        {events.sort((a, b) => (a.eventDate > b.eventDate) ? 1 : -1).map(mobEvent =>
                            <tr key={mobEvent.id}>
                                <td>{mobEvent.name}</td>
                                <td>{new Date(mobEvent.eventDate).toLocaleDateString("en-US", {month:"long", day:"numeric", year: 'numeric', hour: 'numeric', minute: 'numeric' })}</td>
                                <td>{getEventType(eventTypeList, mobEvent.eventTypeId)}</td>
                                <td>{mobEvent.streetAddress}</td>
                                <td>{mobEvent.city}</td>
                                <td>{mobEvent.region}</td>
                                <td>{mobEvent.country}</td>
                                <td>{mobEvent.postalCode}</td>
                                <td>{mobEvent.totalAttendees}</td>
                                <td>{mobEvent.totalBags}</td>
                                <td>{mobEvent.durationInMinutes}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <>
            <div>
                {!isEventSummaryDataLoaded && <p><em>Loading...</em></p>}
                {isEventSummaryDataLoaded && renderEventSummariesTable(displaySummaries)}
            </div>
        </>
    );
}