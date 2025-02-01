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

import { GetLocationsByPartner, GetPartnerLocations, UpdatePartnerLocations } from '@/services/locations';
import PartnerLocationData from '@/components/Models/PartnerLocationData';
import { GoogleMap } from '@/components/Map/GoogleMap';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { useAzureMapSearchAddressReverse } from '@/hooks/useAzureMapSearchAddressReverse';
import { AzureSearchLocationInput, SearchLocationOption } from '@/components/Map/AzureSearchLocationInput';

interface FormInputs {
    partnerId: string;
    name: string;
    isActive: boolean;
    publicNotes: string;
    privateNotes: string;
    location: { lat: number; lng: number };

    // Auto
    streetAddress: string;
    city: string;
    region: string;
    postalCode: string;
    country: string;
}

const formSchema = z.object({
    partnerId: z.string(),
    name: z.string({ required_error: 'Location Name cannot be empty.' }),
    isActive: z.boolean(),
    publicNotes: z.string({ required_error: 'Public notes cannot be empty.' }),
    privateNotes: z.string(),
    location: z.object({
        lat: z.number().optional(),
        lng: z.number().optional(),
    }),
    streetAddress: z.string().optional(),
    city: z.string().optional(),
    country: z.string().optional(),
    region: z.string().optional(),
    postalCode: z.string().optional(),
});

const useGetPartnerLocationById = (locationId: string) =>
    useQuery({
        queryKey: GetPartnerLocations({ locationId }).key,
        queryFn: GetPartnerLocations({ locationId }).service,
        select: (res) => res.data,
    });

interface PartnerLocationEditFormProps {
    azureSubscriptionKey: string;
}

export const PartnerLocationEditForm = (props: PartnerLocationEditFormProps) => {
    const { azureSubscriptionKey } = props;
    const { partnerId, locationId } = useParams<{ partnerId: string; locationId: string }>() as {
        partnerId: string;
        locationId: string;
    };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const { data: currentValues, isLoading } = useGetPartnerLocationById(locationId);
    const { mutate, isLoading: isSubmitting } = useMutation({
        mutationKey: UpdatePartnerLocations().key,
        mutationFn: UpdatePartnerLocations().service,
        onSuccess: () => {
            toast({
                variant: 'primary',
                title: 'Location saved!',
                description: '',
            });
            queryClient.invalidateQueries({
                queryKey: GetLocationsByPartner({ partnerId }).key,
                refetchType: 'all',
            });
            navigate(`/partnerdashboard/${partnerId}/locations`);
        },
    });

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            location: { lat: MapStore.defaultLatitude, lng: MapStore.defaultLongitude },
        },
    });

    useEffect(() => {
        if (currentValues) {
            form.reset({
                ...currentValues,
                location: {
                    lat: currentValues.latitude,
                    lng: currentValues.longitude,
                },
            });
        }
    }, [currentValues]);

    const location = form.watch('location');
    const addrComponents = form.watch(['streetAddress', 'city', 'region', 'country', 'postalCode']);

    const onSubmit: SubmitHandler<FormInputs> = (formValues) => {
        const body = new PartnerLocationData();
        body.id = locationId;
        body.partnerId = partnerId;
        body.name = formValues.name ?? '';
        body.streetAddress = formValues.streetAddress ?? '';
        body.city = formValues.city ?? '';
        body.region = formValues.region ?? '';
        body.country = formValues.country ?? '';
        body.postalCode = formValues.postalCode ?? '';
        body.latitude = formValues.location.lat ?? 0;
        body.longitude = formValues.location.lng ?? 0;
        body.isActive = formValues.isActive;
        body.publicNotes = formValues.publicNotes ?? '';
        body.privateNotes = formValues.privateNotes ?? '';
        mutate(body);
    };

    const map = useMap();

    const handleSelectSearchLocation = useCallback(
        async (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            form.setValue('location', { lat, lng: lon });

            // side effect: Move Map Center
            if (map) map.panTo({ lat, lng: lon });
        },
        [map],
    );

    const handleClickMap = useCallback((e: MapMouseEvent) => {
        if (e.detail.latLng) {
            const lat = e.detail.latLng.lat;
            const lng = e.detail.latLng.lng;
            form.setValue('location', { lat, lng });
        }
    }, []);

    const handleMarkerDragEnd = useCallback((e: google.maps.MapMouseEvent) => {
        if (e.latLng) {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            form.setValue('location', { lat, lng });
        }
    }, []);

    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: location?.lat,
            long: location?.lng,
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

    if (isLoading) {
        return <Loader2 className='animate-spin mx-auto my-10' />;
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                <FormField
                    control={form.control}
                    name='name'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerContactName} required>
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
                    name='isActive'
                    render={({ field }) => (
                        <FormItem className='col-span-6'>
                            <FormLabel tooltip={ToolTips.PartnerLocationIsPartnerLocationActive}>Is Active</FormLabel>
                            <FormControl>
                                <div className='flex items-center space-x-2 h-9'>
                                    <Checkbox id='isActive' checked={field.value} onCheckedChange={field.onChange} />
                                    <label
                                        htmlFor='isActive'
                                        className='text-sm mb-0 font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70'
                                    >
                                        Active
                                    </label>
                                </div>
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='publicNotes'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.PartnerLocationPublicNotes} required>
                                Public Notes
                            </FormLabel>
                            <FormControl>
                                <Textarea {...field} maxLength={1000} className='h-24' />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name='privateNotes'
                    render={({ field }) => (
                        <FormItem className='col-span-12'>
                            <FormLabel tooltip={ToolTips.PartnerLocationPrivateNotes}>Private Notes</FormLabel>
                            <FormControl>
                                <Textarea {...field} maxLength={1000} className='h-24' />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <div className='col-span-12'>
                    <div className='relative w-full'>
                        <GoogleMap
                            defaultCenter={location}
                            defaultZoom={15}
                            onClick={handleClickMap}
                            style={{ width: '100%', height: 250 }}
                        >
                            <Marker position={location} draggable onDragEnd={handleMarkerDragEnd} />
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
                    <div className='text-[0.8rem] font-medium text-muted my-2'>
                        {location
                            ? `${compact(addrComponents).join(', ')}`
                            : `Click on the map to set the location for your Partner.`}
                    </div>
                </div>
                <div className='col-span-12 flex justify-end gap-2'>
                    <Button variant='secondary' data-test='cancel' asChild>
                        <Link to={`/partnerdashboard/${partnerId}/locations`}>Cancel</Link>
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

export const PartnerLocationEdit = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();
    const [azureSubscriptionKey, setAzureSubscriptionKey] = useState<string>();

    useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    }, []);

    if (isLoading || !azureSubscriptionKey) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <PartnerLocationEditForm azureSubscriptionKey={azureSubscriptionKey} />
        </APIProvider>
    );
};
