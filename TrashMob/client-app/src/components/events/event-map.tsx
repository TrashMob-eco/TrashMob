import { useRef, useState } from 'react';
import { AdvancedMarker, InfoWindow, MapProps } from '@vis.gl/react-google-maps';
import { GoogleMapWithKey as GoogleMap } from '../Map/GoogleMap';
import { EventDetailInfoWindowHeader, EventDetailInfoWindowContent } from '../Map/EventInfoWindowContent';
import EventData from '../Models/EventData';
import UserData from '../Models/UserData';
import LitterReportData from '../Models/LitterReportData';
import { LitterReportStatusEnum } from '../Models/LitterReportStatus';
import { useQuery } from '@tanstack/react-query';
import { GetAllEventsBeingAttendedByUser } from '@/services/events';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import moment from 'moment';
import { EventPin } from './event-pin';
import { LitterReportPin, litterReportColors } from '../litterreports/litter-report-pin';
import {
    LitterReportInfoWindowHeader,
    LitterReportInfoWindowContent,
} from '../litterreports/litter-report-info-window';

const colors = {
    upcoming: '#1D4ED8',
    completed: '#005345',
    canceled: '#B52D2D',
};

const getLitterReportColor = (statusId: number): string => {
    switch (statusId) {
        case LitterReportStatusEnum.New:
            return litterReportColors.new;
        case LitterReportStatusEnum.Assigned:
            return litterReportColors.assigned;
        case LitterReportStatusEnum.Cleaned:
            return litterReportColors.cleaned;
        case LitterReportStatusEnum.Cancelled:
            return litterReportColors.cancelled;
        default:
            return litterReportColors.new;
    }
};

interface EventsMapProps extends MapProps {
    id?: string;
    events: EventData[];
    isUserLoaded: boolean;
    currentUser: UserData;
    litterReports?: LitterReportData[];
    showLitterReports?: boolean;
}

export const EventsMap = (props: EventsMapProps) => {
    const { id, events, isUserLoaded, currentUser, gestureHandling, litterReports, showLitterReports, ...rest } = props;

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
    const eventMarkersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const litterReportMarkersRef = useRef<Record<string, google.maps.marker.AdvancedMarkerElement>>({});
    const [showingEventId, setShowingEventId] = useState<string>('');
    const [showingLitterReportId, setShowingLitterReportId] = useState<string>('');
    const showingEvent = eventsWithAttendance.find((event) => event.id === showingEventId);
    const showingLitterReport = (litterReports || []).find((report) => report.id === showingLitterReportId);
    const { ref, isInViewPort } = useIsInViewport();

    // Get first image with coordinates for each litter report (for pin placement)
    const litterReportsWithLocation = (litterReports || [])
        .map((report) => {
            const imageWithLocation = report.litterImages?.find((img) => img.latitude && img.longitude);
            if (!imageWithLocation) return null;
            return {
                ...report,
                latitude: imageWithLocation.latitude!,
                longitude: imageWithLocation.longitude!,
            };
        })
        .filter(Boolean) as (LitterReportData & { latitude: number; longitude: number })[];

    const handleEventMarkerHover = (eventId: string) => {
        setShowingEventId(eventId);
        setShowingLitterReportId('');
    };

    const handleLitterReportMarkerHover = (reportId: string) => {
        setShowingLitterReportId(reportId);
        setShowingEventId('');
    };

    return (
        <div ref={ref}>
            <GoogleMap id={id} gestureHandling={gestureHandling} {...rest}>
                {/* Event Markers */}
                {eventsWithAttendance.map((event) => {
                    const isCompleted = moment(event.eventDate).isBefore(new Date());
                    return (
                        <AdvancedMarker
                            key={event.id}
                            ref={(el) => {
                                eventMarkersRef.current[event.id] = el!;
                            }}
                            className={cn({
                                'animate-[bounce_1s_both_3s]': isInViewPort,
                            })}
                            position={{ lat: event.latitude, lng: event.longitude }}
                            onMouseEnter={() => handleEventMarkerHover(event.id)}
                        >
                            <EventPin color={isCompleted ? colors.completed : colors.upcoming} size={48} />
                        </AdvancedMarker>
                    );
                })}

                {/* Litter Report Markers */}
                {showLitterReports
                    ? litterReportsWithLocation.map((report) => (
                          <AdvancedMarker
                              key={`litter-${report.id}`}
                              ref={(el) => {
                                  litterReportMarkersRef.current[report.id] = el!;
                              }}
                              className={cn({
                                  'animate-[bounce_1s_both_3s]': isInViewPort,
                              })}
                              position={{ lat: report.latitude, lng: report.longitude }}
                              onMouseEnter={() => handleLitterReportMarkerHover(report.id)}
                          >
                              <LitterReportPin color={getLitterReportColor(report.litterReportStatusId)} size={32} />
                          </AdvancedMarker>
                      ))
                    : null}

                {/* Event Info Window */}
                {showingEvent ? (
                    <InfoWindow
                        anchor={eventMarkersRef.current[showingEventId]}
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

                {/* Litter Report Info Window */}
                {showingLitterReport ? (
                    <InfoWindow
                        anchor={litterReportMarkersRef.current[showingLitterReportId]}
                        headerContent={<LitterReportInfoWindowHeader name={showingLitterReport.name} />}
                        onClose={() => setShowingLitterReportId('')}
                    >
                        <LitterReportInfoWindowContent report={showingLitterReport} />
                    </InfoWindow>
                ) : null}
            </GoogleMap>
        </div>
    );
};
