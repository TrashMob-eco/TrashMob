import { useCallback, useEffect, useState } from 'react';
import { ManageEventDashboardLayout } from './_layout';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Switch } from '@/components/ui/switch';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { DatePicker } from '@/components/ui/datepicker';
import { Textarea } from '@/components/ui/textarea';
import { TimePicker } from '@/components/ui/timepicker';
import { GoogleMap } from '@/components/Map/GoogleMap';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import {
    AzureSearchLocationInputWithKey as AzureSearchLocationInput,
    SearchLocationOption,
} from '@/components/Map/AzureSearchLocationInput';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { Select, SelectItem, SelectContent, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useGetEventTypes } from '@/hooks/useGetEventTypes';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { cn } from '@/lib/utils';
import { EventStatusActive } from '@/components/Models/Constants';
import moment from 'moment';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { CreateEvent, GetUserEvents } from '@/services/events';
import EventData from '@/components/Models/EventData';
import { AzureMapSearchAddressReverse, AzureMapSearchAddressReverse_Params } from '@/services/maps';
import { useGetAzureKey } from '@/hooks/useGetAzureKey';
import { AzureMapSearchAddressReverseResultItem } from '@/components/Models/AzureMapSearchAddressReverse';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { useNavigate } from 'react-router';
import { ToastAction } from '@/components/ui/toast';

const createEventSchema = z.object({
    name: z.string().min(2, {
        message: 'Event name must be at least 2 characters.',
    }),
    description: z.string(),
    eventDate: z.date(),
    eventTimeStart: z.string(),
    eventTimeEnd: z.string(),
    eventTypeId: z.string(),
    streetAddress: z.string().optional(),
    city: z.string().optional(),
    region: z.string().optional(),
    country: z.string().optional(),
    postalCode: z.string().optional(),
    latitude: z.number(),
    longitude: z.number(),
    maxNumberOfParticipants: z.number().min(0),
    isEventPublic: z.boolean(),
    createdByUserId: z.string(),
    eventStatusId: z.number(),
    locationConfirmed: z.boolean(),
});

export const CreateEventPage = () => {
    const azureKey = useGetAzureKey();
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: eventTypes } = useGetEventTypes();
    const steps = [
        { key: 'pick-location', label: 'Pick Location' },
        { key: 'edit-detail', label: 'Edit Detail' },
        { key: 'review', label: 'Review' },
    ];
    const [step, setStep] = useState<string>('pick-location');

    const createEvent = useMutation({
        mutationKey: CreateEvent().key,
        mutationFn: CreateEvent().service,
        onSuccess: (data, variable) => {
            toast({
                duration: 10000,
                variant: 'primary',
                title: `Event ${variable.name} created!`,
                description: `${moment(variable.eventDate).format('dddd, MMMM Do YYYY [at] h:mm a')}`,
            });
            queryClient.invalidateQueries({
                queryKey: GetUserEvents({ userId: currentUser.id }).key,
                refetchType: 'all',
            });
            navigate('/mydashboard', {
                state: {
                    newEventCreated: true,
                },
            });
        },
    });

    const form = useForm<z.infer<typeof createEventSchema>>({
        resolver: zodResolver(createEventSchema),
        defaultValues: {
            eventTypeId: '1',
            eventDate: moment().startOf('isoWeek').add(1, 'week').day('saturday').toDate(),
            eventTimeStart: '9:00',
            eventTimeEnd: '11:00',
            isEventPublic: true,
            maxNumberOfParticipants: 10,
            locationConfirmed: false,
            createdByUserId: `1`,
            eventStatusId: EventStatusActive,
        },
    });

    const [previewValues, setPreviewValues] = useState<z.infer<typeof createEventSchema> | null>(null);

    const saveDetail = async () => {
        // Validate location & detail
        const validated = await form.trigger([
            'name',
            'description',
            'eventDate',
            'eventTimeStart',
            'eventTimeEnd',
            'maxNumberOfParticipants',
            'isEventPublic',
            'latitude',
            'longitude',
        ]);
        if (validated) {
            // Snapshot current value for preview
            const currentValues = form.getValues();
            setPreviewValues(currentValues);
            setStep('review');
        }
    };

    function onSubmit(formValues: z.infer<typeof createEventSchema>) {
        const body = new EventData();

        const eventDateMoment = moment(formValues.eventDate);
        const start = moment(`${eventDateMoment.format('YYYY-MM-DD')} ${formValues.eventTimeStart}`);
        const end = moment(`${eventDateMoment.format('YYYY-MM-DD')} ${formValues.eventTimeEnd}`);
        const diffMinutes = end.diff(start, 'minutes');
        const durationHours = Math.floor(diffMinutes / 60);
        const durationMinutes = diffMinutes % 60;
        body.name = formValues.name ?? '';
        body.description = formValues.description ?? '';
        body.eventDate = formValues.eventDate;
        body.durationHours = durationHours ?? 2;
        body.durationMinutes = durationMinutes ?? 0;
        body.eventTypeId = Number(formValues.eventTypeId);
        body.streetAddress = formValues.streetAddress ?? '';
        body.city = formValues.city ?? '';
        body.region = formValues.region ?? '';
        body.country = formValues.country ?? '';
        body.postalCode = formValues.postalCode ?? '';
        body.latitude = formValues.latitude ?? 0;
        body.longitude = formValues.longitude ?? 0;
        body.maxNumberOfParticipants = formValues.maxNumberOfParticipants ?? 0;
        body.isEventPublic = formValues.isEventPublic;
        body.createdByUserId = currentUser.id;
        body.eventStatusId = formValues.eventStatusId;

        createEvent.mutate(body);
    }

    const map = useMap('locationPicker');

    const handleSelectSearchLocation = useCallback(
        (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            form.setValue('latitude', lat);
            form.setValue('longitude', lon);

            // side effect: Move Map Center
            if (map) map.panTo({ lat, lng: lon });
        },
        [map],
    );

    const handleClickMap = useCallback((e: MapMouseEvent) => {
        if (e.detail.latLng) {
            const lat = e.detail.latLng.lat;
            const lng = e.detail.latLng.lng;
            form.setValue('latitude', lat);
            form.setValue('longitude', lng);
        }
    }, []);

    const handleMarkerDragEnd = useCallback(async (e: google.maps.MapMouseEvent) => {
        if (e.latLng) {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            form.setValue('latitude', lat);
            form.setValue('longitude', lng);
        }
    }, []);

    const latitude = form.watch('latitude');
    const longitude = form.watch('longitude');

    const searchAddressReverse = async (params: AzureMapSearchAddressReverse_Params) => {
        const { data } = await AzureMapSearchAddressReverse().service(params);
        const firstResult = data?.addresses[0];
        if (firstResult) {
            form.setValue('streetAddress', firstResult.address.streetNameAndNumber);
            form.setValue('city', firstResult.address.municipality);
            form.setValue('region', firstResult.address.countrySubdivisionName);
            form.setValue('country', firstResult.address.country);
            form.setValue('postalCode', firstResult.address.postalCode);

            // Set Event default name
            form.setValue('name', `Clean up ${firstResult.address.streetName}, ${firstResult.address.municipality}`);
        }
    };

    useEffect(() => {
        if (azureKey && latitude && longitude) {
            const params: AzureMapSearchAddressReverse_Params = {
                lat: latitude,
                long: longitude,
                azureKey: azureKey,
            };
            searchAddressReverse(params);
        }
    }, [latitude, longitude, azureKey]);

    return (
        <ManageEventDashboardLayout title='Create an event'>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>
                    <Tabs value={step}>
                        <TabsList className='mb-4 mt-4 bg-transparent gap-x-8 w-full'>
                            {steps.map((step, i) => (
                                <TabsTrigger
                                    key={step.key}
                                    value={step.key}
                                    className={cn(
                                        'grow border-t-2 border-muted bg-transparent rounded-none shadow-none flex flex-col items-start !pl-0 !pt-4',
                                        'data-[state=active]:!border-[#96BA00]',
                                    )}
                                >
                                    <div className='text-primary'>Step {i + 1}</div>
                                    <div className='text-muted text-sm'>{step.label}</div>
                                </TabsTrigger>
                            ))}
                        </TabsList>
                        <TabsContent value='pick-location'>
                            <div className='grid gap-4 grid-cols-12'>
                                <FormItem className='col-span-12'>
                                    <FormControl>
                                        <AzureSearchLocationInput
                                            className='w-full'
                                            placeholder='Search for address'
                                            onSelectLocation={handleSelectSearchLocation}
                                        />
                                    </FormControl>
                                </FormItem>
                                {latitude && longitude && (
                                    <>
                                        <FormItem className='col-span-12'>
                                            <GoogleMap
                                                id='locationPicker'
                                                defaultCenter={{ lat: latitude, lng: longitude }}
                                                defaultZoom={16}
                                                style={{ width: '100%', height: '300px' }}
                                                onClick={handleClickMap}
                                            >
                                                <Marker
                                                    position={{ lat: latitude, lng: longitude }}
                                                    draggable
                                                    onDragEnd={handleMarkerDragEnd}
                                                />
                                            </GoogleMap>
                                            <FormDescription>
                                                Drag marker or click on map to move marker to precise location.
                                            </FormDescription>
                                        </FormItem>
                                        <div className='col-span-12 flex gap-2 justify-end'>
                                            <Button type='button' onClick={() => setStep('edit-detail')}>
                                                Next
                                            </Button>
                                        </div>
                                    </>
                                )}
                            </div>
                        </TabsContent>
                        <TabsContent value='edit-detail'>
                            <div className='grid gap-4 grid-cols-12'>
                                <FormField
                                    control={form.control}
                                    name='name'
                                    render={({ field }) => (
                                        <FormItem className='col-span-12'>
                                            <FormLabel>Event Name</FormLabel>
                                            <FormControl>
                                                <Input placeholder='New Event' {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='eventTypeId'
                                    render={({ field }) => (
                                        <FormItem className='col-span-6'>
                                            <FormLabel>Event Type</FormLabel>
                                            <FormControl>
                                                <Select value={field.value} onValueChange={field.onChange}>
                                                    <SelectTrigger className='w-full'>
                                                        <SelectValue placeholder='Clean up Type' />
                                                    </SelectTrigger>
                                                    <SelectContent>
                                                        {(eventTypes || []).map((type) => (
                                                            <SelectItem key={type.id} value={`${type.id}`}>
                                                                {type.name}
                                                            </SelectItem>
                                                        ))}
                                                    </SelectContent>
                                                </Select>
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='isEventPublic'
                                    render={({ field }) => (
                                        <FormItem className='col-span-3'>
                                            <FormLabel>Is Public Event</FormLabel>
                                            <FormControl>
                                                <div className='flex h-[36px] items-center'>
                                                    <Switch checked={field.value} onCheckedChange={field.onChange} />
                                                </div>
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='maxNumberOfParticipants'
                                    render={({ field }) => (
                                        <FormItem className='col-span-3'>
                                            <FormLabel>Max attendee</FormLabel>
                                            <FormControl>
                                                <Input type='number' {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='eventDate'
                                    render={({ field }) => (
                                        <FormItem className='col-span-6'>
                                            <FormLabel>Date</FormLabel>
                                            <FormControl>
                                                <DatePicker {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='eventTimeStart'
                                    render={({ field }) => (
                                        <FormItem className='col-span-3'>
                                            <FormLabel>Start time</FormLabel>
                                            <FormControl>
                                                <TimePicker {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='eventTimeEnd'
                                    render={({ field }) => (
                                        <FormItem className='col-span-3'>
                                            <FormLabel>End time</FormLabel>
                                            <FormControl>
                                                <TimePicker {...field} />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='description'
                                    render={({ field }) => (
                                        <FormItem className='col-span-12'>
                                            <FormLabel>Description</FormLabel>
                                            <FormControl>
                                                <Textarea
                                                    placeholder='Type your message here.'
                                                    className='resize-none'
                                                    {...field}
                                                />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <div className='col-span-12 flex gap-2 justify-end'>
                                    <Button type='button' variant='outline' onClick={() => setStep('pick-location')}>
                                        Back
                                    </Button>
                                    <Button type='button' onClick={saveDetail}>
                                        Next
                                    </Button>
                                </div>
                            </div>
                        </TabsContent>
                        <TabsContent value='review'>
                            {previewValues && (
                                <>
                                    <h4 className='text-lg'>{previewValues.name}</h4>
                                    <p>
                                        {eventTypes?.find((type) => `${type.id}` === previewValues.eventTypeId)?.name}
                                    </p>
                                    <p>
                                        {previewValues.latitude}, {previewValues.longitude}
                                    </p>
                                    <p>
                                        {moment(previewValues.eventDate).format('dddd, MMMM D, YYYY')}{' '}
                                        {previewValues.eventTimeStart} - {previewValues.eventTimeEnd}
                                    </p>
                                    <GoogleMap
                                        defaultCenter={{ lat: latitude, lng: longitude }}
                                        defaultZoom={16}
                                        style={{ width: '100%', height: '300px' }}
                                    >
                                        <Marker position={{ lat: latitude, lng: longitude }} />
                                    </GoogleMap>
                                </>
                            )}

                            <div className='col-span-12 flex gap-2 justify-end my-4'>
                                <Button type='button' variant='outline' onClick={() => setStep('edit-detail')}>
                                    Back
                                </Button>
                                <Button type='submit'>Create Event</Button>
                            </div>
                        </TabsContent>
                    </Tabs>
                </form>
            </Form>
        </ManageEventDashboardLayout>
    );
};

export const CreateEventWrapper = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <CreateEventPage />
        </APIProvider>
    );
};
