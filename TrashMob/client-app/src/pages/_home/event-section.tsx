import { Link } from 'react-router';
import toNumber from 'lodash/toNumber';
import * as MapStore from '@/store/MapStore';
import { AzureSearchLocationInput, SearchLocationOption } from '@/components/Map/AzureSearchLocationInput';
import { EventsMap } from '@/components/events/event-map';
import { Button } from '@/components/ui/button';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useMap } from '@vis.gl/react-google-maps';
import { useCallback, useEffect, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { GetFilteredEvents, GetFilteredEvents_Params } from '@/services/events';
import { cn } from '@/lib/utils';
import { Select, SelectContent, SelectItemAlt, SelectTrigger, SelectValue } from '@/components/ui/select';
import { List, Map, Plus, Pencil } from 'lucide-react';
import { useGetDefaultMapCenter } from '@/hooks/useGetDefaultMapCenter';
import { AzureMapSearchAddressReverse } from '@/services/maps';
import { GetAllEventsBeingAttendedByUser } from '@/services/events';

import { EventList } from '@/components/events/event-list';
import { useLogin } from '@/hooks/useLogin';
import {
    getCompletedTimeranges,
    getAllUpcomingTimerange,
    getUpcomingTimeranges,
    getLastDaysTimerange,
} from './utils/timerange';

enum EventStatusFilter {
    UPCOMING = 'upcoming',
    COMPLETED = 'completed',
    CANCELED = 'canceled',
}

interface EventSectionProps {}

/** Event List */
const useGetFilteredEvents = (params: GetFilteredEvents_Params) => {
    return useQuery({
        queryKey: GetFilteredEvents(params).key,
        queryFn: GetFilteredEvents(params).service,
        select: (res) => res.data || [],
    });
};

export const EventSection = (props: EventSectionProps) => {
    const { isUserLoaded, currentUser } = useLogin();
    const map = useMap();
    const defaultMapCenter = useGetDefaultMapCenter();

    const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>('');
    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    });

    /** Statuses */
    const statuses = [
        { value: EventStatusFilter.UPCOMING, label: 'Upcoming' },
        { value: EventStatusFilter.COMPLETED, label: 'Completed' },
    ];

    /** Filter Parameters */
    const [selectedStatuses, setSelectedStatuses] = useState<string>(EventStatusFilter.UPCOMING);

    // Default timerange is This weekend
    const [selectedTimeRange, setSelectedTimeRange] = useState<string>(getAllUpcomingTimerange());

    const [selectedLocation, setSelectedLocation] = useState<SearchLocationOption>();
    const [view, setView] = useState<string>('map');

    /** Time Ranges */
    const timeRangeOptions =
        selectedStatuses === EventStatusFilter.UPCOMING ? getUpcomingTimeranges() : getCompletedTimeranges();

    /** Event List */
    const [startDate, endDate] = selectedTimeRange.split('|');
    const { data: events } = useGetFilteredEvents({ startDate, endDate, city: selectedLocation?.address.municipality });

    // Load and add user's attendance to events
    const { data: myAttendanceList } = useQuery({
        queryKey: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).key,
        queryFn: GetAllEventsBeingAttendedByUser({ userId: currentUser.id }).service,
        select: (res) => res.data,
    });

    const eventsWithAttendance = (events || []).map((event) => {
        const isAttending: boolean = (myAttendanceList || []).some((ev) => ev.id === event.id);
        return { ...event, isAttending };
    });

    /**
     * Side Effect
     */
    // Side Effect 1: Reverse Search City from lat,lng
    useEffect(() => {
        if (!azureSubscriptionKey) return;

        AzureMapSearchAddressReverse()
            .service({
                azureKey: azureSubscriptionKey,
                lat: defaultMapCenter.lat,
                long: defaultMapCenter.lng,
            })
            .then((response) => {
                const result = response.data.addresses[0];
                const [lat, lon] = result.position.split(',').map(toNumber);
                const location = {
                    id: result.id,
                    displayAddress: result.address.freeformAddress,
                    address: result.address,
                    position: { lat, lon },
                };
                setSelectedLocation(location);
            });
    }, [defaultMapCenter, azureSubscriptionKey]);

    // Side Effect 2: When eventStatusFilter change, set timeRangeOption accordingly
    useEffect(() => {
        if (selectedStatuses === EventStatusFilter.COMPLETED) {
            setSelectedTimeRange(getLastDaysTimerange(90));
        } else {
            setSelectedTimeRange(getAllUpcomingTimerange());
        }
    }, [timeRangeOptions, selectedStatuses]);

    /**
     * Events
     */
    const handleSelectSearchLocation = useCallback(
        async (location: SearchLocationOption) => {
            setSelectedLocation(location);
            const { lat, lon: lng } = location.position;
            if (map) map.panTo({ lat, lng });
        },
        [map],
    );

    return (
        <section className='bg-[#FCFBF8]' id='events'>
            <div className='container !py-20'>
                <div className='flex flex-col gap-2'>
                    <div className='flex flex-col md:flex-row items-center gap-4 relative'>
                        <h3 className='my-0 font-semibold'>Events near</h3>
                        <div className='relative z-10 h-[60px]'>
                            <AzureSearchLocationInput
                                azureKey={azureSubscriptionKey}
                                className='!rounded-none !border-none !shadow-none !bg-transparent'
                                entityType={['Municipality']}
                                listClassName='!rounded-lg border md:min-w-[200px] relative shadow-md bg-card !mt-2'
                                onSelectLocation={handleSelectSearchLocation}
                                placeholder={selectedLocation ? selectedLocation.address.municipality : 'Location ...'}
                                renderInput={(inputProps) => (
                                    <div className='w-fit flex flex-row items-center'>
                                        <input
                                            type='text'
                                            {...inputProps}
                                            className='w-72 text-primary placeholder:text-primary/50 font-semibold text-4xl mb-0 !py-2 border-b-4 border-primary bg-[#FCFBF8] outline-none'
                                        />
                                        <Pencil className='!w-8 !h-8 text-white fill-primary' />
                                    </div>
                                )}
                            />
                        </div>
                        <div className='grow flex justify-end'>
                            <Button asChild className='md:flex'>
                                <Link to='/events/create'>
                                    <Plus /> Create Event
                                </Link>
                            </Button>
                        </div>
                    </div>
                    <div className='py-4'>
                        <Tabs
                            className='w-full rounded-none bg-transparent p-0'
                            defaultValue={selectedStatuses}
                            onValueChange={setSelectedStatuses}
                        >
                            <TabsList className='bg-transparent gap-2 w-full justify-center md:w-auto md:justify-start '>
                                {statuses.map((status) => (
                                    <TabsTrigger
                                        className={cn(
                                            'relative !px-2 h-9 rounded-[2px] border-b-2 border-b-transparent bg-transparent font-semibold text-muted-foreground shadow-none transition-none',
                                            "after:content-[''] after:w-0 after:h-0.5 after:absolute after:left-0 after:-bottom-3",
                                            'after:data-[state=active]:bg-[#005B4C] after:data-[state=active]:w-full',
                                            'data-[state=active]:!bg-[#B0CCC8] data-[state=active]:text-foreground',
                                            'transition-all duration-300 ease-in-out',
                                            'after:transition-all after:duration-300 after:ease-in-out',
                                        )}
                                        key={status.value}
                                        value={status.value}
                                    >
                                        {status.label}
                                    </TabsTrigger>
                                ))}
                            </TabsList>
                        </Tabs>
                    </div>
                    <div className='flex flex-row gap-4 mb-2'>
                        <Select onValueChange={setSelectedTimeRange} value={selectedTimeRange}>
                            <SelectTrigger className='w-48'>
                                <SelectValue placeholder='Time' />
                            </SelectTrigger>
                            <SelectContent>
                                {timeRangeOptions.map((timeRange) => (
                                    <SelectItemAlt key={timeRange.value} value={timeRange.value}>
                                        {timeRange.label}
                                    </SelectItemAlt>
                                ))}
                            </SelectContent>
                        </Select>

                        <div className='flex-1' />
                        <ToggleGroup onValueChange={setView} type='single' value={view} variant='outline'>
                            <ToggleGroupItem
                                className='data-[state=on]:bg-primary data-[state=on]:text-primary-foreground'
                                value='list'
                            >
                                <List />
                            </ToggleGroupItem>
                            <ToggleGroupItem
                                className='data-[state=on]:bg-primary data-[state=on]:text-primary-foreground'
                                value='map'
                            >
                                <Map />
                            </ToggleGroupItem>
                        </ToggleGroup>
                    </div>
                    <div>
                        {(eventsWithAttendance || []).length} events found in{' '}
                        {selectedLocation?.address.municipality || 'your area'}
                    </div>
                    {view === 'map' ? (
                        <EventsMap
                            currentUser={currentUser}
                            defaultCenter={
                                selectedLocation
                                    ? { lat: selectedLocation.position.lat, lng: selectedLocation.position.lon }
                                    : undefined
                            }
                            defaultZoom={13}
                            events={eventsWithAttendance || []}
                            gestureHandling='greedy'
                            isUserLoaded={isUserLoaded}
                        />
                    ) : (
                        <EventList
                            currentUser={currentUser}
                            events={eventsWithAttendance || []}
                            isUserLoaded={isUserLoaded}
                        />
                    )}
                </div>
            </div>
        </section>
    );
};
