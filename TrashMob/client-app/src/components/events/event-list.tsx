import { CalendarRange, ArrowRight, Calendar, MapPin } from 'lucide-react';
import { Link } from 'react-router';
import moment from 'moment';
import compact from 'lodash/compact';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { AttendingBadge, Badge, HostingBadge } from '@/components/ui/badge';
import EventData from '@/components/Models/EventData';
import UserData from '@/components/Models/UserData';
import { RegisterBtn } from '@/components/Customization/RegisterBtn';

import { useGetEventType } from '@/hooks/useGetEventType';

export const EventPlaceAndLocation = (
    event: Partial<Pick<EventData, 'eventDate' | 'streetAddress' | 'city' | 'region'>>,
) => {
    return (
        <div className='grid grid-cols-[24px_1fr] gap-x-3 gap-y-2'>
            {event.eventDate ? (
                <>
                    <Calendar />
                    <div>{moment(event.eventDate).format('MMM DD, YYYY | hh:mm A z')}</div>
                </>
            ) : null}
            <MapPin />
            <div>
                <a
                    className='text-foreground hover:underline'
                    href={`https://google.com/maps/place/${event.streetAddress}+${event.city}+${event.region}+${event.postalCode}+${event.country}`}
                    rel='noopener noreferrer'
                    target='_blank'
                >
                    {compact([event.streetAddress, event.city, event.region]).join(', ')}
                </a>
            </div>
        </div>
    );
};

interface EventListItemProps {
    readonly event: EventData;
    readonly isUserLoaded: boolean;
    readonly currentUser: UserData;
}

export const EventListItem = (props: EventListItemProps) => {
    const { event, isUserLoaded, currentUser } = props;
    const { data: eventType } = useGetEventType(event.eventTypeId);
    return (
        <Card className='!border-[#8AB4AD]' key={event.id}>
            <CardHeader className='!p-4 flex-row'>
                <CardTitle className='grow'>{event.name}</CardTitle>
                <div className='flex gap-2'>
                    {eventType ? (
                        <Badge variant='default'>
                            <CalendarRange /> {eventType.name}
                        </Badge>
                    ) : null}
                    {event.isAttending ? <AttendingBadge /> : null}
                    {event.createdByUserId === currentUser.id ? <HostingBadge /> : null}
                </div>
            </CardHeader>
            <CardContent className='!p-4 !pt-0'>
                <EventPlaceAndLocation {...event} />
            </CardContent>
            <CardFooter className='!p-4 !pt-0 flex justify-between'>
                <Button asChild variant='outline'>
                    <Link to={`/eventdetails/${event.id}`}>
                        View Event <ArrowRight />
                    </Link>
                </Button>
                <RegisterBtn
                    currentUser={currentUser}
                    eventId={event.id}
                    isAttending={event.isAttending ? 'Yes' : 'No'}
                    isEventCompleted={new Date(event.eventDate) < new Date()}
                    isUserLoaded={isUserLoaded}
                />
            </CardFooter>
        </Card>
    );
};

interface EventListProps {
    readonly events: EventData[];
    readonly isUserLoaded: boolean;
    readonly currentUser: UserData;
}

export const EventList = ({ events, isUserLoaded, currentUser }: EventListProps) => {
    return (
        <div className='flex flex-col gap-4'>
            {(events || []).map((event) => (
                <EventListItem currentUser={currentUser} event={event} isUserLoaded={isUserLoaded} key={event.id} />
            ))}
        </div>
    );
};
