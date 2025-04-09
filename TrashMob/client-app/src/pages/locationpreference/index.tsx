import { useMutation, useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router';
import { FormEvent, useCallback, useEffect, useRef, useState } from 'react';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Loader2 } from 'lucide-react';

import * as ToolTips from '@/store/ToolTips';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormField, FormItem, FormMessage } from '@/components/ui/form';
import { EnhancedFormLabel as FormLabel } from '@/components/ui/custom/form';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItemAlt, SelectTrigger, SelectValue } from '@/components/ui/select';
import { MarkerWithInfoWindow, EventInfoWindowContent } from '@/components/Map';
import { GoogleMap } from '@/components/Map/GoogleMap';
import { Button } from '@/components/ui/button';
import { GetUserById, UpdateUser } from '@/services/users';
import { AzureSearchLocationInput, SearchLocationOption } from '@/components/Map/AzureSearchLocationInput';
import { APIProvider, MapMouseEvent, useMap } from '@vis.gl/react-google-maps';
import { useAzureMapSearchAddressReverse } from '@/hooks/useAzureMapSearchAddressReverse';
import { useMapStore } from '@/hooks/useMapStore';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import UserData from '@/components/Models/UserData';
import moment from 'moment';

enum DistanceUnit {
    KM = 'km',
    MILE = 'mi',
}

const locationPreferenceSchema = z.object({
    latitude: z.number(),
    longitude: z.number(),
    city: z.string(),
    region: z.string(),
    country: z.string(),
    postalCode: z.string(),
    travelLimitForLocalEvents: z
        .number()
        .min(0, 'Travel limit must be greater than or equal to 0.')
        .max(1000, 'Travel limit must be less than 1000.'),
    prefersMetric: z.boolean(),
});

export const LocationPreference = () => {
    const navigate = useNavigate();
    const { toast } = useToast();
    const { currentUser, handleUserUpdated } = useLogin();
    const { azureSubscriptionKey } = useMapStore();

    const { data: user } = useQuery({
        queryKey: GetUserById({ userId: currentUser.id }).key,
        queryFn: GetUserById({ userId: currentUser.id }).service,
        select: (res) => res.data,
    });

    const updateUser = useMutation({
        mutationKey: UpdateUser().key,
        mutationFn: UpdateUser().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'User Location Preference updated!',
            });
            handleUserUpdated();
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: `Unknown error occured while checking user name. Please try again. Error: ${error.message}`,
            });
        },
    });

    const form = useForm<z.infer<typeof locationPreferenceSchema>>({
        resolver: zodResolver(locationPreferenceSchema),
        defaultValues: {},
    });

    useEffect(() => {
        if (!user) return;

        form.reset({
            latitude: user.latitude,
            longitude: user.longitude,
            city: user.city,
            region: user.region,
            postalCode: user.postalCode,
            country: user.country,
            prefersMetric: user.prefersMetric,
            travelLimitForLocalEvents: user.travelLimitForLocalEvents,
        });
    }, [user]);

    const onSubmit = useCallback(
        (values: z.infer<typeof locationPreferenceSchema>) => {
            if (!currentUser) return;

            const body = new UserData();
            body.id = currentUser.id;
            body.userName = currentUser.userName ?? '';
            body.email = currentUser.email ?? '';
            body.dateAgreedToTrashMobWaiver = new Date(currentUser.dateAgreedToTrashMobWaiver);
            body.memberSince = new Date(currentUser.memberSince);
            body.trashMobWaiverVersion = currentUser.trashMobWaiverVersion;

            body.city = values.city ?? '';
            body.region = values.region ?? '';
            body.country = values.country ?? '';
            body.postalCode = values.postalCode ?? '';
            body.latitude = values.latitude;
            body.longitude = values.longitude;
            body.prefersMetric = values.prefersMetric;
            body.travelLimitForLocalEvents = values.travelLimitForLocalEvents;

            updateUser.mutateAsync(body);
        },
        [currentUser],
    );

    const handleCancel = useCallback(
        (event: FormEvent<HTMLElement>) => {
            event.preventDefault();
            navigate('/');
        },
        [navigate],
    );

    /** Map */
    const map = useMap();
    const radiusRef = useRef<google.maps.Circle>();

    const latitude = form.watch('latitude');
    const longitude = form.watch('longitude');
    const travelLimitForLocalEvents = form.watch('travelLimitForLocalEvents');
    const prefersMetric = form.watch('prefersMetric');

    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: latitude,
            long: longitude,
            azureKey: azureSubscriptionKey,
        },
        { enabled: false },
    );

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

    // on Marker moved (latitude + longitude changed), do reverse search lat,lng to address
    useEffect(() => {
        const searchAddressReverse = async () => {
            const { data } = await refetchAddressReverse();

            const firstResult = data?.addresses[0];
            if (firstResult) {
                form.setValue('city', firstResult.address.municipality);
                form.setValue('country', firstResult.address.country);
                form.setValue('region', firstResult.address.countrySubdivisionName);
                form.setValue('postalCode', firstResult.address.postalCode);
            }
        };
        if (latitude && longitude) searchAddressReverse();
    }, [latitude, longitude]);

    // On Map Initialized, add circle polygon
    useEffect(() => {
        if (!map || radiusRef.current) return;

        const radiusCircle = new google.maps.Circle({
            strokeColor: '#005C4C',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#005C4C',
            fillOpacity: 0.2,
            clickable: false,
            map,
        });
        radiusRef.current = radiusCircle;
    }, [map]);

    // On radius, lat, lng changed, update radius polygon
    useEffect(() => {
        if (map && radiusRef.current) {
            radiusRef.current.setCenter({ lat: latitude, lng: longitude });

            // Note: radius unit is meter.
            radiusRef.current.setRadius(travelLimitForLocalEvents * (prefersMetric ? 1000 : 1600));
        }
    }, [map, radiusRef, latitude, longitude, travelLimitForLocalEvents, prefersMetric]);

    const date = moment().format('LL');
    const time = moment().format('LTS');

    if (!user) return null;

    return (
        <div className='tailwind'>
            <HeroSection Title='Set your location' Description='Get notified for events near you!' />
            <div className='container mx-auto py-5'>
                <Card>
                    <CardHeader>
                        <CardTitle>Location preferences</CardTitle>
                    </CardHeader>
                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-2'>
                            <CardContent>
                                <div className='grid grid-cols-12 gap-4'>
                                    <div className='col-span-12'>
                                        {latitude && longitude ? (
                                            <div className='relative'>
                                                <GoogleMap
                                                    defaultCenter={{ lat: latitude, lng: longitude }}
                                                    onClick={handleClickMap}
                                                >
                                                    <MarkerWithInfoWindow
                                                        position={{ lat: latitude, lng: longitude }}
                                                        draggable
                                                        onDragEnd={handleMarkerDragEnd}
                                                        infoWindowTrigger='hover'
                                                        infoWindowProps={{
                                                            headerDisabled: true,
                                                        }}
                                                        infoWindowContent={
                                                            <EventInfoWindowContent
                                                                title="User's Base Location"
                                                                date={date}
                                                                time={time}
                                                            />
                                                        }
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
                                    </div>
                                    <FormField
                                        control={form.control}
                                        name='travelLimitForLocalEvents'
                                        render={({ field }) => (
                                            <FormItem className='col-span-4'>
                                                <FormLabel
                                                    tooltip={ToolTips.LocationPreferenceTravelLimitForLocalEvents}
                                                >
                                                    Maximum event radius info
                                                </FormLabel>
                                                <FormControl>
                                                    <Input
                                                        {...field}
                                                        type='number'
                                                        onChange={(e) => field.onChange(e.target.valueAsNumber)}
                                                    />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='prefersMetric'
                                        render={({ field }) => (
                                            <FormItem className='col-span-2'>
                                                <FormLabel> </FormLabel>
                                                <FormControl>
                                                    <Select
                                                        value={field.value ? DistanceUnit.KM : DistanceUnit.MILE}
                                                        onValueChange={(unit) =>
                                                            field.onChange(unit === DistanceUnit.KM)
                                                        }
                                                    >
                                                        <SelectTrigger className='w-full'>
                                                            <SelectValue placeholder='Unit' />
                                                        </SelectTrigger>
                                                        <SelectContent>
                                                            <SelectItemAlt value={DistanceUnit.KM}>km</SelectItemAlt>
                                                            <SelectItemAlt value={DistanceUnit.MILE}>mi</SelectItemAlt>
                                                        </SelectContent>
                                                    </Select>
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='city'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <FormLabel tooltip={ToolTips.LocationPreferenceCity}>City</FormLabel>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='region'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <FormLabel tooltip={ToolTips.LocationPreferenceRegion}>State</FormLabel>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='postalCode'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <FormLabel tooltip={ToolTips.LocationPreferencePostalCode}>
                                                    Postal code
                                                </FormLabel>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                </div>
                            </CardContent>
                            <CardFooter className='flex gap-2 justify-end'>
                                <Button variant='outline' onClick={handleCancel}>
                                    Discard
                                </Button>
                                <Button type='submit' disabled={updateUser.isLoading} variant='default'>
                                    {updateUser.isLoading ? <Loader2 className='animate-spin' /> : null}
                                    Save
                                </Button>
                            </CardFooter>
                        </form>
                    </Form>
                </Card>
            </div>
        </div>
    );
};

export const LocationPreferenceWrapper = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <LocationPreference />
        </APIProvider>
    );
};
