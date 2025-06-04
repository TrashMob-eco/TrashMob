import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import compact from 'lodash/compact';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { Loader2 } from 'lucide-react';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import { Textarea } from '@/components/ui/textarea';
import * as ToolTips from '@/store/ToolTips';
import * as MapStore from '@/store/MapStore';

import { GetEventPickupLocationById, GetEventPickupLocations, UpdateEventPickupLocation } from '@/services/locations';
import { GoogleMap } from '@/components/Map/GoogleMap';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import { useAzureMapSearchAddressReverse } from '@/hooks/useAzureMapSearchAddressReverse';
import { AzureSearchLocationInput, SearchLocationOption } from '@/components/Map/AzureSearchLocationInput';
import { useGetAzureKey } from '@/hooks/useGetAzureKey';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import PickupLocationData from '@/components/Models/PickupLocationData';

interface EditPickupLocationFields {
    id: string;
    eventId: string;
    latitude: number;
    longitude: number;
    hasBeenSubmitted: boolean;
    hasBeenPickedUp: boolean;
    name: string;
    notes: string;

    // Auto
    streetAddress: string;
    city: string;
    region: string;
    postalCode: string;
    country: string;
}

const formSchema = z.object({
    id: z.string(),
    eventId: z.string(),
    latitude: z.number().optional(),
    longitude: z.number().optional(),
    hasBeenSubmitted: z.boolean(),
    hasBeenPickedUp: z.boolean(),
    name: z.string(),
    notes: z.string(),

    streetAddress: z.string().optional(),
    city: z.string().optional(),
    country: z.string().optional(),
    region: z.string().optional(),
    postalCode: z.string().optional(),
});

interface PickupLocationEditFormProps {}

export const PickupLocationEditForm = (props: PickupLocationEditFormProps) => {
    const azureSubscriptionKey = useGetAzureKey();
    const { eventId, locationId } = useParams<{ eventId: string; locationId: string }>() as {
        eventId: string;
        locationId: string;
    };

    const { data: pickupLocation } = useQuery({
        queryKey: GetEventPickupLocationById({ locationId }).key,
        queryFn: GetEventPickupLocationById({ locationId }).service,
        select: (res) => res.data,
    });

    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const { mutate, isLoading: isSubmitting } = useMutation({
        mutationKey: UpdateEventPickupLocation().key,
        mutationFn: UpdateEventPickupLocation().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Pickup Location updated!',
                description: '',
            });
            queryClient.invalidateQueries({
                queryKey: GetEventPickupLocations({ eventId }).key,
                refetchType: 'all',
            });
            navigate(`/eventsummary/${eventId}`);
        },
        onError: async (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Pickup location update fail',
                description: error.message,
            });
        },
    });

    const form = useForm<EditPickupLocationFields>({
        resolver: zodResolver(formSchema),
        defaultValues: {},
    });

    useEffect(() => {
        if (pickupLocation) {
            form.reset({
                ...pickupLocation,
            });
        }
    }, [pickupLocation]);

    const latitude = form.watch('latitude');
    const longitude = form.watch('longitude');
    const addrComponents = form.watch(['streetAddress', 'city', 'region', 'country', 'postalCode']);

    const onSubmit: SubmitHandler<EditPickupLocationFields> = (formValues) => {
        const body = new PickupLocationData();
        body.id = locationId;
        body.eventId = eventId;
        body.name = formValues.name ?? '';
        body.notes = formValues.notes ?? '';
        body.streetAddress = formValues.streetAddress ?? '';
        body.city = formValues.city ?? '';
        body.region = formValues.region ?? '';
        body.country = formValues.country ?? '';
        body.postalCode = formValues.postalCode ?? '';
        body.latitude = formValues.latitude ?? 0;
        body.longitude = formValues.longitude ?? 0;
        body.hasBeenPickedUp = formValues.hasBeenPickedUp;
        body.hasBeenSubmitted = formValues.hasBeenSubmitted;
        mutate(body);
    };

    const map = useMap();

    const handleSelectSearchLocation = useCallback(
        async (location: SearchLocationOption) => {
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

    const handleMarkerDragEnd = useCallback((e: google.maps.MapMouseEvent) => {
        if (e.latLng) {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            form.setValue('latitude', lat);
            form.setValue('longitude', lng);
        }
    }, []);

    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: latitude,
            long: longitude,
            azureKey: azureSubscriptionKey || '',
        },
        { enabled: false },
    );

    // on Marker moved (latitude + longitude changed), do reverse search lat,lng to address
    useEffect(() => {
        const searchAddressReverse = async () => {
            const { data } = await refetchAddressReverse();

            const firstResult = data?.addresses[0];
            if (firstResult) {
                const { streetNameAndNumber, streetName, municipality, country, countrySubdivisionName, postalCode } =
                    firstResult.address;
                form.setValue('streetAddress', streetNameAndNumber || streetName);
                form.setValue('city', municipality);
                form.setValue('country', country);
                form.setValue('region', countrySubdivisionName);
                form.setValue('postalCode', postalCode);
            }
        };
        if (location) searchAddressReverse();
    }, [location]);

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='name'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PickupLocationName} required>
                                Name
                            </FormLabel>
                            <FormControl>
                                <Input {...field} />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='hasBeenPickedUp'
                    render={({ field }) => (
                        <FormItem className='col-span-3'>
                            <FormLabel tooltip={ToolTips.PickupLocationHasBeenPickedUp}>&nbsp;</FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox id='hasBeenPickedUp' checked={field.value} disabled />
                                    <label
                                        htmlFor='hasBeenPickedUp'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Picked up?
                                    </label>
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='hasBeenSubmitted'
                    render={({ field }) => (
                        <FormItem className='col-span-3'>
                            <FormLabel>&nbsp;</FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox id='hasBeenSubmitted' checked={field.value} disabled />
                                    <label
                                        htmlFor='hasBeenSubmitted'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Submitted?
                                    </label>
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='notes'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.PickupLocationNotes} required>
                                Notes
                            </FormLabel>
                            <FormControl>
                                <Textarea {...field} maxLength={1000} className='h-24' />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12'>
                    {latitude && longitude ? (
                        <div className='relative w-full'>
                            <GoogleMap
                                defaultCenter={{ lat: latitude, lng: longitude }}
                                defaultZoom={15}
                                onClick={handleClickMap}
                                style={{ width: '100%', height: 250 }}
                            >
                                <Marker
                                    position={{ lat: latitude, lng: longitude }}
                                    draggable
                                    onDragEnd={handleMarkerDragEnd}
                                />
                            </GoogleMap>
                            {azureSubscriptionKey ? (
                                <div style={{ position: 'absolute', top: 8, left: 8 }}>
                                    <AzureSearchLocationInput
                                        azureKey={azureSubscriptionKey}
                                        onSelectLocation={handleSelectSearchLocation}
                                    />
                                </div>
                            ) : null}
                        </div>
                    ) : null}
                    <div className='text-[0.8rem] font-medium text-muted my-2'>
                        {latitude && longitude
                            ? `${compact(addrComponents).join(', ')}`
                            : `Click on the map to set the location for your Partner.`}
                    </div>
                </div>
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button variant='secondary' data-test='cancel' asChild>
                        <Link to={`/eventsummary/${eventId}`}>Cancel</Link>
                    </Button>
                    <Button type='submit' disabled={isSubmitting}>
                        {isSubmitting ? <Loader2 className='animate-spin' /> : null}
                        Save
                    </Button>
                </div>
            </form>
        </Form>
    );
};

export const PickupLocationEdit = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();
    if (isLoading) return null;
    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <PickupLocationEditForm />
        </APIProvider>
    );
};
