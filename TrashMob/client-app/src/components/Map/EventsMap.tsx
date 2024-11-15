import { AdvancedMarker, InfoWindow, useMap } from '@vis.gl/react-google-maps';
import { GoogleMap } from './GoogleMap';
import { EventDetailInfoWindowHeader, EventDetailInfoWindowContent } from './EventInfoWindowContent';
import EventData from '../Models/EventData';
import { useEffect, useRef, useState } from 'react';
import UserData from '../Models/UserData';
import { useQuery } from '@tanstack/react-query';
import { GetAllEventsBeingAttendedByUser } from '../../services/events';

interface EventsMapProps {
    id?: string;
    events: EventData[];
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventsMap = (props: EventsMapProps) => {
    const { id, events, isUserLoaded, currentUser } = props;

    // Load and add user's attendance to events
    const { data: myAttendanceList } = useQuery({
        queryKey: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).key,
        queryFn: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).service,
        select: (res) => res.data,
    });

    const eventsWithAttendance = events.map((event) => {
        const isAttending: boolean = (myAttendanceList || []).some((ev) => ev.id === event.id);
        return { ...event, isAttending };
    });

    const map = useMap(id);

    // Fit Map to show all events markers
    useEffect(() => {
        if (map && events.length) {
            let bounds = new google.maps.LatLngBounds();
            for (let event of events) {
                bounds.extend({ lat: event.latitude, lng: event.longitude });
            }
            map.fitBounds(bounds);
        }
    }, [map, events]);

    /**
     *  Show Event InfoWindow when user hover marker
     *  Note: Use shared infoWindow to prevent multiple InfoWindow opening at the same time.
     */
    const markersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const [showingEventId, setShowingEventId] = useState<string>('');
    const showingEvent = eventsWithAttendance.find((event) => event.id === showingEventId);

    return (
        <GoogleMap id={id}>
            {eventsWithAttendance.map((event) => (
                <AdvancedMarker
                    key={event.id}
                    ref={(el) => {
                        markersRef.current[event.id] = el!;
                    }}
                    position={{ lat: event.latitude, lng: event.longitude }}
                    onMouseEnter={(e) => setShowingEventId(event.id)}
                />
            ))}
            {showingEvent ? (
                <InfoWindow
                    anchor={markersRef.current[showingEventId]}
                    headerContent={<EventDetailInfoWindowHeader {...showingEvent} />}
                    onClose={() => setShowingEventId('')}
                >
                    <EventDetailInfoWindowContent
                        event={showingEvent}
                        hideTitle
                        isUserLoaded={isUserLoaded}
                        currentUser={currentUser}
                    />
                </InfoWindow>
            ) : null}
        </GoogleMap>
    );
};
