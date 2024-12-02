import { Link } from 'react-router-dom';
import * as MapStore from '@/store/MapStore';
import { AzureSearchLocationInput, SearchLocationOption } from '@/components/Map/AzureSearchLocationInput';
import { EventsMap } from '@/components/Map';
import { Button } from '@/components/ui/button';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { APIProvider, useMap } from '@vis.gl/react-google-maps';
import { useCallback, useEffect, useState } from 'react';
import UserData from '@/components/Models/UserData';
import { useQuery } from '@tanstack/react-query';
import { GetAllActiveEvents } from '@/services/events';
import { cn } from '@/lib/utils';
import MultiSelect from '@/components/ui/multi-select';
import { useGetEventTypes } from '@/hooks/useGetEventTypes';
import { List, Map, Plus } from 'lucide-react';

interface EventSectionProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const EventSectionComponent = (props: EventSectionProps) => {
    const map = useMap();
    const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>('');
    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    });

    /** Time Ranges */
    const timeRangeOptions = [
        { value: 'today', label: 'Today' },
        { value: 'tomorrow', label: 'Tomorrow' },
        { value: 'this weekend', label: 'This weekend' },
        { value: 'all', label: 'All' },
    ];

    /** Event Types */
    const { data: eventTypes } = useGetEventTypes();
    const eventTypeOptions = (eventTypes || []).map((et) => ({ value: `${et.id}`, label: et.name }));

    /** Statuses */
    const statuses = [
        { value: 'completed', label: 'Completed' },
        { value: 'upcoming', label: 'Upcoming' },
    ];
    /** Filter Parameters */
    const [selectedTimeRange, setSelectedTimeRange] = useState<string>('today');
    const [selectedEventTypes, setSelectedEventTypes] = useState<string[]>([]);
    const [selectedStatuses, setSelectedStatuses] = useState<string[]>([]);
    const [view, setView] = useState<string>('map');

    /** Event List */
    const [events, setEvents] = useState([]);
    const { refetch } = useQuery({
        queryKey: GetAllActiveEvents().key,
        queryFn: GetAllActiveEvents().service,
        staleTime: 100,
        enabled: false,
    });

    useEffect(() => {
        refetch().then((res) => {
            setEvents(res.data?.data || []);
        });
    }, []);

    const handleSelectLocation = useCallback(
        async (location: SearchLocationOption) => {
            const { lat, lon: lng } = location.position;
            if (map) map.panTo({ lat, lng });
        },
        [map],
    );

    return (
        <section id='event-section' className='bg-[#FCFBF8]'>
            <div className='container !py-20'>
                <div className='flex flex-col gap-2'>
                    <div className='flex flex-col md:flex-row items-center gap-4 relative'>
                        <h3 className='my-0 font-semibold'>Upcoming Events near</h3>
                        <div className='relative z-10 h-[60px]'>
                            <AzureSearchLocationInput
                                azureKey={azureSubscriptionKey}
                                className='!rounded-none !border-none !shadow-none !bg-transparent'
                                listClassName='!rounded-lg border md:min-w-[200px] relative shadow-md bg-card !mt-2'
                                renderInput={(inputProps) => (
                                    <div className='w-fit flex flex-row items-center'>
                                        <input
                                            type='text'
                                            {...inputProps}
                                            defaultValue='Seattle, WA'
                                            className='w-72 text-primary placeholder:text-[rgba(150,186,0,0.5)] font-semibold text-4xl mb-0 !py-2 border-b-4 border-primary bg-[#FCFBF8] outline-none'
                                        />
                                        <svg
                                            xmlns='http://www.w3.org/2000/svg'
                                            fill='none'
                                            viewBox='0 0 24 24'
                                            strokeWidth={1.5}
                                            stroke='currentColor'
                                            className='!w-8 !h-8 text-primary'
                                        >
                                            <path
                                                strokeLinecap='round'
                                                strokeLinejoin='round'
                                                d='m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10'
                                            />
                                        </svg>
                                    </div>
                                )}
                                onSelectLocation={handleSelectLocation}
                            />
                        </div>
                    </div>
                    <div className='py-4'>
                        <Tabs
                            defaultValue={selectedTimeRange}
                            onValueChange={setSelectedTimeRange}
                            className='w-full rounded-none bg-transparent p-0'
                        >
                            <TabsList className='bg-transparent gap-2 w-full justify-center md:w-auto md:justify-start '>
                                {timeRangeOptions.map((timeRange) => (
                                    <TabsTrigger
                                        key={timeRange.value}
                                        value={timeRange.value}
                                        className={cn(
                                            'relative !px-2 h-9 rounded-[2px] border-b-2 border-b-transparent bg-transparent font-semibold text-muted-foreground shadow-none transition-none',
                                            "after:content-[''] after:w-0 after:h-0.5 after:absolute after:left-0 after:-bottom-3",
                                            'after:data-[state=active]:bg-[#005B4C] after:data-[state=active]:w-full',
                                            'data-[state=active]:!bg-[#B0CCC8] data-[state=active]:text-foreground',
                                            'transition-all duration-300 ease-in-out',
                                            'after:transition-all after:duration-300 after:ease-in-out',
                                        )}
                                    >
                                        {timeRange.label}
                                    </TabsTrigger>
                                ))}
                            </TabsList>
                        </Tabs>
                    </div>
                    <div className='flex flex-row gap-4 mb-2'>
                        <MultiSelect
                            placeholder='Cleanup types'
                            className='w-48'
                            options={eventTypeOptions}
                            selectedOptions={selectedEventTypes}
                            setSelectedOptions={setSelectedEventTypes}
                        />
                        <MultiSelect
                            placeholder='Status'
                            className='w-36'
                            options={statuses}
                            selectedOptions={selectedStatuses}
                            setSelectedOptions={setSelectedStatuses}
                        />
                        <Button asChild className='hidden md:flex'>
                            <Link to='/manageeventdashboard'>
                                <Plus /> Create Event
                            </Link>
                        </Button>
                        <div className='flex-1' />
                        <ToggleGroup value={view} onValueChange={setView} type='single' variant='outline'>
                            <ToggleGroupItem
                                value='list'
                                className='data-[state=on]:!bg-[#96BA00] data-[state=on]:text-primary-foreground'
                            >
                                <List />
                            </ToggleGroupItem>
                            <ToggleGroupItem
                                value='map'
                                className='data-[state=on]:!bg-[#96BA00] data-[state=on]:text-primary-foreground'
                            >
                                <Map />
                            </ToggleGroupItem>
                        </ToggleGroup>
                    </div>
                    <EventsMap
                        events={events}
                        isUserLoaded={props.isUserLoaded}
                        currentUser={props.currentUser}
                        gestureHandling='cooperative'
                    />
                </div>
            </div>
        </section>
    );
};

export const EventSection = (props: EventSectionProps) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <EventSectionComponent {...props} />
        </APIProvider>
    );
};
