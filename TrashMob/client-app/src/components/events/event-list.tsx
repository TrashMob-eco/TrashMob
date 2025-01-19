import EventData from '@/components/Models/EventData';
import UserData from '@/components/Models/UserData';
import { EventCard } from './event-card';

interface EventListProps {
    events: EventData[];
    isUserLoaded: boolean;
    currentUser: UserData;
}
export const EventList = ({ events, isUserLoaded, currentUser }: EventListProps) => {
    return (
        <div className='flex flex-col gap-4'>
            {(events || []).map((event) => (
                <EventCard key={event.id} event={event} isUserLoaded={isUserLoaded} currentUser={currentUser} />
            ))}
        </div>
    );
};
