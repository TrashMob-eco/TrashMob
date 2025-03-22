import { useCallback, useEffect, Fragment, useState } from 'react';
import { useParams, Link } from 'react-router';
import moment from 'moment';

import compact from 'lodash/compact';
import take from 'lodash/take';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Switch } from '@/components/ui/switch';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Select, SelectItem, SelectContent, SelectTrigger, SelectValue } from '@/components/ui/select';
import { DatePicker } from '@/components/ui/datepicker';
import { Textarea } from '@/components/ui/textarea';
import { TimePicker } from '@/components/ui/timepicker';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';

import { ManageEventDashboardLayout } from '../_layout';
import { MapMouseEvent, Marker } from '@vis.gl/react-google-maps';
import * as Constants from '@/components/Models/Constants';
import EventData from '@/components/Models/EventData';
import { AzureMapSearchAddressReverse, AzureMapSearchAddressReverse_Params } from '@/services/maps';
import { useGetAzureKey } from '@/hooks/useGetAzureKey';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Check, CircleDashed, UserRoundCheck } from 'lucide-react';
import EventPartnerLocationServiceData from '@/components/Models/EventPartnerLocationServiceData';
import { useEditEventPageQueries } from './useEditEventPageQueries';
import { useEditEventPageMutations } from './useEditEventPageMutations';

const createMomentFromDateAndTime = (eventDate: Date, eventTimeStart: string) => {
    const eventDateMoment = moment(eventDate);
    const start = moment(`${eventDateMoment.format('YYYY-MM-DD')} ${eventTimeStart}`);
    return start;
};

const updateEventSchema = z.object({
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

export const EditEventPage = () => {
    const { eventId } = useParams<{ eventId: string }>() as { eventId: string };
    const azureKey = useGetAzureKey();

    const {
        // Masterdata
        eventTypes,
        serviceTypes,
        serviceStatuses,

        // Event data
        event,
        eventAttendees,
        eventPartnerLocations,
        servicesByLocation,
    } = useEditEventPageQueries(eventId);

    const { updateEvent, createEventPartnerLocationService, deleteEventPartnerLocationService } =
        useEditEventPageMutations();

    const [showAllAttendees, setShowAllAttendees] = useState<boolean>(false);

    //  This will handle the submit form event.
    const requestService = (serviceTypeId: number, partnerLocationId: string, partnerLocName: string) => {
        const body = new EventPartnerLocationServiceData();
        body.eventId = eventId;
        body.partnerLocationId = partnerLocationId;
        body.serviceTypeId = serviceTypeId;
        body.eventPartnerLocationServiceStatusId = Constants.EventPartnerLocationServiceStatusRequested;

        createEventPartnerLocationService.mutateAsync(body);
    };

    const removeServiceRequest = (serviceTypeId: number, partnerLocationId: string, partnerLocName: string) => {
        deleteEventPartnerLocationService.mutateAsync({ eventId, partnerLocationId, serviceTypeId });
    };

    const form = useForm<z.infer<typeof updateEventSchema>>({
        resolver: zodResolver(updateEventSchema),
        defaultValues: {},
    });

    useEffect(() => {
        if (!event) return;

        form.reset({
            name: event.name,
            description: event.description,
            eventDate: event.eventDate,
            eventTimeStart: moment(event.eventDate).format('hh:mm'),
            eventTimeEnd: moment(event.eventDate)
                .add(event.durationHours, 'hours')
                .add(event.durationMinutes, 'minutes')
                .format('hh:mm'),
            eventTypeId: `${event.eventTypeId}`,
            streetAddress: event.streetAddress,
            city: event.city,
            region: event.region,
            country: event.country,
            postalCode: event.postalCode,
            latitude: event.latitude,
            longitude: event.longitude,
            maxNumberOfParticipants: event.maxNumberOfParticipants,
            isEventPublic: event.isEventPublic,
            createdByUserId: event.createdByUserId,
            eventStatusId: event.eventStatusId,
        });
    }, [event]);

    function onSubmit(formValues: z.infer<typeof updateEventSchema>) {
        const body = new EventData();

        const start = createMomentFromDateAndTime(formValues.eventDate, formValues.eventTimeStart);
        const end = createMomentFromDateAndTime(formValues.eventDate, formValues.eventTimeEnd);
        const diffMinutes = end.diff(start, 'minutes');
        const durationHours = Math.floor(diffMinutes / 60);
        const durationMinutes = diffMinutes % 60;
        body.name = formValues.name ?? '';
        body.description = formValues.description ?? '';
        body.eventDate = start.toDate();
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
        body.eventStatusId = formValues.eventStatusId;

        updateEvent.mutate(body);
    }

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

    const numAttendees = (eventAttendees || []).length;
    const visibleEventAttendees = showAllAttendees ? eventAttendees || [] : take(eventAttendees, 10);
    return (
        <ManageEventDashboardLayout
            title='Edit an event'
            aside={
                <>
                    <Card>
                        <CardHeader>
                            <CardTitle>Attendees ({numAttendees})</CardTitle>
                        </CardHeader>
                        <CardContent className='grid gap-6'>
                            {visibleEventAttendees.map((attendee) => (
                                <div key={attendee.id} className='flex items-center justify-between space-x-4'>
                                    <div className='flex items-center space-x-2'>
                                        <span className='relative flex shrink-0 overflow-hidden rounded-full h-8 w-8'>
                                            <UserRoundCheck />
                                        </span>
                                        <div className='grow'>
                                            <p className='text-sm font-medium leading-none m-0'>{attendee.userName}</p>
                                            <p className='text-sm text-muted-foreground m-0'>
                                                {attendee.city} {attendee.country}
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            ))}
                            {!showAllAttendees && numAttendees > 10 ? (
                                <Button variant='link' onClick={() => setShowAllAttendees(true)}>
                                    Show all {numAttendees} attendees
                                </Button>
                            ) : null}
                            {showAllAttendees && numAttendees > 10 ? (
                                <Button variant='link' onClick={() => setShowAllAttendees(false)}>
                                    Show less
                                </Button>
                            ) : null}
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader>
                            <CardTitle>Available Partner Services</CardTitle>
                        </CardHeader>
                        <CardContent className='grid'>
                            {(eventPartnerLocations || []).map((partnerLocation, locIndex) => {
                                const locationServices = servicesByLocation[locIndex].data || [];
                                return (
                                    <Fragment key={`${partnerLocation.partnerLocationId}`}>
                                        <div className='text-sm font-semibold text-sidebar-foreground/70 py-2'>
                                            {partnerLocation.partnerName} - {partnerLocation.partnerLocationName}
                                        </div>
                                        {locationServices.map((service) => {
                                            const serviceType = serviceTypes?.find(
                                                (st) => st.id === service.serviceTypeId,
                                            );
                                            const status = serviceStatuses?.find(
                                                (st) => st.id === service.eventPartnerLocationServiceStatusId,
                                            );
                                            if (!status || !serviceType) return;
                                            return (
                                                <div
                                                    key={`${partnerLocation.partnerLocationId}-${service.serviceTypeId}`}
                                                    className='flex items-center justify-between space-x-4 py-2'
                                                >
                                                    <div className='flex items-center w-full space-x-2'>
                                                        <span className='relative flex shrink-0 overflow-hidden rounded-full h-8 w-8'>
                                                            {status.id === 0 ? (
                                                                <CircleDashed color='#c7c7c7' />
                                                            ) : (
                                                                <Check color='#96ba00' />
                                                            )}
                                                        </span>
                                                        <div className='grow'>
                                                            <p className='text-sm font-medium leading-none m-0'>
                                                                {serviceType?.name}
                                                            </p>
                                                            <p className='text-xs text-muted-foreground m-0'>
                                                                {status.id === 0 ? 'Not Requested' : 'Requested'}
                                                            </p>
                                                        </div>
                                                        <div>
                                                            {status.id === 0 ? (
                                                                <Button
                                                                    size='sm'
                                                                    variant='outline'
                                                                    onClick={() =>
                                                                        requestService(
                                                                            service.serviceTypeId,
                                                                            partnerLocation.partnerLocationId,
                                                                            partnerLocation.partnerLocationName,
                                                                        )
                                                                    }
                                                                >
                                                                    Request
                                                                </Button>
                                                            ) : (
                                                                <Button
                                                                    size='sm'
                                                                    variant='destructive'
                                                                    onClick={() =>
                                                                        removeServiceRequest(
                                                                            service.serviceTypeId,
                                                                            partnerLocation.partnerLocationId,
                                                                            partnerLocation.partnerLocationName,
                                                                        )
                                                                    }
                                                                >
                                                                    Remove
                                                                </Button>
                                                            )}
                                                        </div>
                                                    </div>
                                                </div>
                                            );
                                        })}
                                    </Fragment>
                                );
                            })}
                        </CardContent>
                    </Card>
                </>
            }
        >
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>
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
                        {event ? (
                            <>
                                <div className='col-span-12'>
                                    <GoogleMap
                                        id='locationPicker'
                                        defaultCenter={{ lat: event.latitude, lng: event.longitude }}
                                        defaultZoom={16}
                                        style={{ width: '100%', height: '300px' }}
                                        onClick={handleClickMap}
                                    >
                                        <Marker
                                            position={{ lat: event.latitude, lng: event.longitude }}
                                            draggable
                                            onDragEnd={handleMarkerDragEnd}
                                        />
                                    </GoogleMap>
                                    <FormDescription className='flex justify-between'>
                                        <span>
                                            {compact([
                                                event.streetAddress,
                                                event.city,
                                                event.region,
                                                event.country,
                                            ]).join(', ')}
                                        </span>
                                        <span>
                                            ({event.latitude}, {event.longitude})
                                        </span>
                                    </FormDescription>
                                </div>
                            </>
                        ) : null}
                        <div className='col-span-12 flex gap-2 justify-end my-4'>
                            <Button type='button' variant='outline' asChild>
                                <Link to='/mydashboard'>Back</Link>
                            </Button>
                            <Button type='submit'>Save</Button>
                        </div>
                    </div>
                </form>
            </Form>
        </ManageEventDashboardLayout>
    );
};
