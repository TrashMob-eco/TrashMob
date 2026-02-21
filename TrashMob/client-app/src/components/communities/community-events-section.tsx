import { Link } from 'react-router';
import { Calendar, MapPin, ExternalLink } from 'lucide-react';
import EventData from '@/components/Models/EventData';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';

interface CommunityEventsSectionProps {
    events: EventData[];
    isLoading?: boolean;
}

export const CommunityEventsSection = ({ events, isLoading }: CommunityEventsSectionProps) => {
    const formatDate = (date: Date) => {
        return new Date(date).toLocaleDateString('en-US', {
            weekday: 'short',
            month: 'short',
            day: 'numeric',
            year: 'numeric',
        });
    };

    const formatTime = (date: Date) => {
        return new Date(date).toLocaleTimeString('en-US', {
            hour: 'numeric',
            minute: '2-digit',
        });
    };

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className='text-lg'>Upcoming Events</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className='space-y-3'>
                        {[1, 2, 3].map((i) => (
                            <div key={i} className='animate-pulse'>
                                <div className='h-4 bg-muted rounded w-3/4 mb-2' />
                                <div className='h-3 bg-muted rounded w-1/2' />
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-lg'>Upcoming Events</CardTitle>
                <Link to='/eventsmain'>
                    <Button variant='ghost' size='sm'>
                        View All
                        <ExternalLink className='ml-1 h-3 w-3' />
                    </Button>
                </Link>
            </CardHeader>
            <CardContent>
                {events.length === 0 ? (
                    <p className='text-sm text-muted-foreground'>No upcoming events in this community.</p>
                ) : (
                    <div className='space-y-4'>
                        {events.slice(0, 5).map((event) => (
                            <Link
                                key={event.id}
                                to={`/eventdetails/${event.id}`}
                                className='block p-3 rounded-lg border hover:bg-muted transition-colors'
                            >
                                <h4 className='font-medium text-sm mb-1 line-clamp-1'>{event.name}</h4>
                                <div className='flex items-center gap-1 text-xs text-muted-foreground mb-1'>
                                    <Calendar className='h-3 w-3' />
                                    <span>
                                        {formatDate(event.eventDate)} at {formatTime(event.eventDate)}
                                    </span>
                                </div>
                                <div className='flex items-center gap-1 text-xs text-muted-foreground'>
                                    <MapPin className='h-3 w-3' />
                                    <span className='line-clamp-1'>
                                        {event.city}, {event.region}
                                    </span>
                                </div>
                            </Link>
                        ))}
                        {events.length > 5 && (
                            <p className='text-xs text-muted-foreground text-center'>
                                + {events.length - 5} more events
                            </p>
                        )}
                    </div>
                )}
            </CardContent>
        </Card>
    );
};
