import { useEffect, useRef, useState } from 'react';
import { AdvancedMarker, InfoWindow, useMap } from '@vis.gl/react-google-maps';
import { GoogleMap } from './GoogleMap';
import { EventDetailInfoWindowHeader, EventDetailInfoWindowContent } from './EventInfoWindowContent';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { useQuery } from '@tanstack/react-query';
import { GetAllEventsBeingAttendedByUser } from '../../services/events';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';

interface EventsMapProps {
    id?: string;
    events: EventData[];
    isUserLoaded: boolean;
    currentUser: UserData;
    gestureHandling?: string;
}

export const EventsMap = (props: EventsMapProps) => {
    const { id, events, isUserLoaded, currentUser, gestureHandling } = props;

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

    const { ref, isInViewPort } = useIsInViewport();

    return (
        <div ref={ref}>
            <GoogleMap id={id} gestureHandling={gestureHandling}>
                {eventsWithAttendance.map((event) => (
                    <AdvancedMarker
                        key={event.id}
                        ref={(el) => {
                            markersRef.current[event.id] = el!;
                        }}
                        className={cn({
                            'animate-[bounce_1s_both_3s]': isInViewPort,
                        })}
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
        </div>
    );
};
