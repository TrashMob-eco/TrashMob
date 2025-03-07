import { useRef, useState } from 'react';
import { AdvancedMarker, InfoWindow, MapProps } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '../Map/GoogleMap';
import { EventDetailInfoWindowHeader, EventDetailInfoWindowContent } from '../Map/EventInfoWindowContent';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import { useQuery } from '@tanstack/react-query';
import { GetAllEventsBeingAttendedByUser } from '@/services/events';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import moment from 'moment';
import { EventPin } from './event-pin';

const colors = {
    upcoming: '#1D4ED8',
    completed: '#005345',
    canceled: '#B52D2D',
};

interface EventsMapProps extends MapProps {
    id?: string;
    events: EventData[];
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventsMap = (props: EventsMapProps) => {
    const { id, events, isUserLoaded, currentUser, gestureHandling, ...rest } = props;

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
            <GoogleMap id={id} gestureHandling={gestureHandling} {...rest}>
                {eventsWithAttendance.map((event) => {
                    const isCompleted = moment(event.eventDate).isBefore(new Date());
                    return (
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
                        >
                            <EventPin color={isCompleted ? colors.completed : colors.upcoming} size={48} />
                        </AdvancedMarker>
                    );
                })}
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
