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

export const EventPlaceAndLocation = (event: EventData) => {
    return (
        <div className='grid grid-cols-[24px_1fr] gap-x-3 gap-y-2'>
            <Calendar />
            <div>{moment(event.eventDate).format('MMM DD, YYYY | hh:mm A z')}</div>
            <MapPin />
            <div>
                <a
                    href={`https://google.com/maps/place/${event.streetAddress}+${event.city}+${event.region}+${event.postalCode}+${event.country}`}
                    target='_blank'
                    className='text-foreground hover:underline'
                    rel='noopener noreferrer'
                >
                    {compact([event.streetAddress, event.city, event.region]).join(', ')}
                </a>
            </div>
        </div>
    );
};

interface EventListItemProps {
    event: EventData;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventListItem = (props: EventListItemProps) => {
    const { event, isUserLoaded, currentUser } = props;
    const { data: eventType } = useGetEventType(event.eventTypeId);
    return (
        <Card key={event.id} className='!border-[#8AB4AD]'>
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
                <Button variant='outline' asChild>
                    <Link to={`/eventdetails/${event.id}`}>
                        View Event <ArrowRight />
                    </Link>
                </Button>
                <RegisterBtn
                    eventId={event.id}
                    isUserLoaded={isUserLoaded}
                    currentUser={currentUser}
                    isAttending={event.isAttending ? 'Yes' : 'No'}
                    isEventCompleted={new Date(event.eventDate) < new Date()}
                />
            </CardFooter>
        </Card>
    );
};

interface EventListProps {
    events: EventData[];
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventList = ({ events, isUserLoaded, currentUser }: EventListProps) => {
    return (
        <div className='flex flex-col gap-4'>
            {(events || []).map((event) => (
                <EventListItem key={event.id} event={event} isUserLoaded={isUserLoaded} currentUser={currentUser} />
            ))}
        </div>
    );
};
