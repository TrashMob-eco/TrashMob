import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router';
import { Calendar, MapPin } from 'lucide-react';
import EventData from '@/components/Models/EventData';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Select, SelectContent, SelectItemAlt, SelectTrigger, SelectValue } from '@/components/ui/select';
import {
    getUpcomingTimeranges,
    getCompletedTimeranges,
    getAllUpcomingTimerange,
    getLastDaysTimerange,
} from '@/pages/_home/utils/timerange';
import moment from 'moment';

type EventTab = 'upcoming' | 'completed';
const INITIAL_SHOW_COUNT = 5;

interface CommunityEventsSectionProps {
    events: EventData[];
    isLoading?: boolean;
}

export const CommunityEventsSection = ({ events, isLoading }: CommunityEventsSectionProps) => {
    const [tab, setTab] = useState<EventTab>('upcoming');
    const [timeRange, setTimeRange] = useState<string>(getAllUpcomingTimerange());
    const [showAll, setShowAll] = useState(false);

    // Reset time range and showAll when tab changes
    useEffect(() => {
        setShowAll(false);
        if (tab === 'completed') {
            setTimeRange(getLastDaysTimerange(90));
        } else {
            setTimeRange(getAllUpcomingTimerange());
        }
    }, [tab]);

    const timeRangeOptions = tab === 'upcoming' ? getUpcomingTimeranges() : getCompletedTimeranges();

    const filteredEvents = useMemo(() => {
        const now = moment();
        const [startDate, endDate] = timeRange.split('|');
        const start = moment(startDate);
        const end = moment(endDate).endOf('day');

        return events.filter((event) => {
            const eventMoment = moment(event.eventDate);
            const isUpcoming = eventMoment.isAfter(now);

            if (tab === 'upcoming' && !isUpcoming) return false;
            if (tab === 'completed' && isUpcoming) return false;

            return eventMoment.isBetween(start, end, undefined, '[]');
        });
    }, [events, tab, timeRange]);

    const displayEvents = showAll ? filteredEvents : filteredEvents.slice(0, INITIAL_SHOW_COUNT);

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
                    <CardTitle className='text-lg'>Events</CardTitle>
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

    const emptyText =
        tab === 'upcoming'
            ? 'No upcoming events in this community.'
            : 'No completed events match the selected filters.';

    return (
        <Card>
            <CardHeader className='space-y-3'>
                <CardTitle className='text-lg'>Events</CardTitle>
                <div className='flex flex-col sm:flex-row gap-3'>
                    <Tabs value={tab} onValueChange={(v) => setTab(v as EventTab)}>
                        <TabsList>
                            <TabsTrigger value='upcoming'>Upcoming</TabsTrigger>
                            <TabsTrigger value='completed'>Completed</TabsTrigger>
                        </TabsList>
                    </Tabs>
                    <Select value={timeRange} onValueChange={setTimeRange}>
                        <SelectTrigger className='w-44'>
                            <SelectValue />
                        </SelectTrigger>
                        <SelectContent>
                            {timeRangeOptions.map((opt) => (
                                <SelectItemAlt key={opt.value} value={opt.value}>
                                    {opt.label}
                                </SelectItemAlt>
                            ))}
                        </SelectContent>
                    </Select>
                </div>
            </CardHeader>
            <CardContent>
                {filteredEvents.length === 0 ? (
                    <p className='text-sm text-muted-foreground'>{emptyText}</p>
                ) : (
                    <div className='space-y-4'>
                        {displayEvents.map((event) => (
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
                        {filteredEvents.length > INITIAL_SHOW_COUNT ? (
                            <Button
                                variant='ghost'
                                size='sm'
                                className='w-full'
                                onClick={() => setShowAll((prev) => !prev)}
                            >
                                {showAll ? 'Show fewer' : `Show all ${filteredEvents.length} events`}
                            </Button>
                        ) : null}
                    </div>
                )}
            </CardContent>
        </Card>
    );
};
