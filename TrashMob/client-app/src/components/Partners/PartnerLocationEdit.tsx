import * as React from 'react';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Checkbox } from "@/components/ui/checkbox"
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import * as ToolTips from '../../store/ToolTips';
import PartnerLocationData from '../Models/PartnerLocationData';
import * as MapStore from '../../store/MapStore';
import { CreatePartnerLocations, GetPartnerLocations, UpdatePartnerLocations } from '../../services/locations';
import { GoogleMap } from '../Map/GoogleMap';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';
import { useAzureMapSearchAddressReverse } from '../../hooks/useAzureMapSearchAddressReverse';
import { AzureSearchLocationInput, SearchLocationOption } from '../Map/AzureSearchLocationInput';

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

export interface PartnerLocationEditDataProps {
    partnerId: string;
    partnerLocationId: string;
    onCancel: any;
    onSave: any;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerLocationEdit: React.FC<PartnerLocationEditDataProps> = (props) => {

    const { partnerLocationId: locationId } = props
    const { data: item } = useQuery({
        queryKey: GetPartnerLocations({ locationId }).key,
        queryFn: GetPartnerLocations({ locationId }).service,
        select: res => res.data
    });

    console.log({ item })
     const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {},
    });

    const location = form.watch('location');

    const onSubmit: SubmitHandler<FormInputs> = (data) => {
        const body = new PartnerLocationData();
        body.id = locationId;
        body.partnerId = props.partnerId;
        body.name = data.name ?? '';
        body.streetAddress = data.streetAddress ?? '';
        body.city = data.city ?? '';
        body.region = data.region ?? '';
        body.country = data.country ?? '';
        body.postalCode = data.postalCode ?? '';
        body.latitude = data.location.lat ?? 0;
        body.longitude = data.location.lng ?? 0;
        body.isActive = data.isActive;
        body.publicNotes = data.publicNotes ?? '';
        body.privateNotes = data.privateNotes ?? '';
        body.createdByUserId = data.createdByUserId ?? props.currentUser.id;
        body.createdDate = data.createdDate;

        // if (partnerLocationId === Guid.EMPTY) await createPartnerLocations.mutateAsync(body);
        // else await updatePartnerLocations.mutateAsync(body);

        props.onSave();

        // createPartnerRequest.mutateAsync(body);
    };

    // const [partnerLocationId, setPartnerLocationId] = React.useState<string>(Guid.EMPTY);
    // const [locationName, setLocationName] = React.useState<string>('');
    // const [locationNameErrors, setLocationNameErrors] = React.useState<string>('');
    // const [publicNotes, setPublicNotes] = React.useState<string>('');
    // const [publicNotesErrors, setPublicNotesErrors] = React.useState<string>('');
    // const [privateNotes, setPrivateNotes] = React.useState<string>('');
    // const [isPartnerLocationActive, setIsPartnerLocationActive] = React.useState<boolean>(true);
    // const [streetAddress, setStreetAddress] = React.useState<string>('');
    // const [city, setCity] = React.useState<string>('');
    // const [country, setCountry] = React.useState<string>('');
    // const [region, setRegion] = React.useState<string>('');
    // const [postalCode, setPostalCode] = React.useState<string>('');
    // const [latitude, setLatitude] = React.useState<number>(0);
    // const [longitude, setLongitude] = React.useState<number>(0);
    // const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    // const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    // const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    // const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    // const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);

    const createPartnerLocations = useMutation({
        mutationKey: CreatePartnerLocations().key,
        mutationFn: CreatePartnerLocations().service,
    });

    const updatePartnerLocations = useMutation({
        mutationKey: UpdatePartnerLocations().key,
        mutationFn: UpdatePartnerLocations().service,
    });

    React.useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    }, []);

    const map = useMap();

    const handleSelectSearchLocation = React.useCallback(
        async (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            form.setValue('location', { lat, lng: lon });

            // side effect: Move Map Center
            if (map) map.panTo({ lat, lng: lon });
        },
        [map],
    );

    const handleClickMap = React.useCallback((e: MapMouseEvent) => {
        if (e.detail.latLng) {
            const lat = e.detail.latLng.lat;
            const lng = e.detail.latLng.lng;
            form.setValue('location', { lat, lng });
        }
    }, []);

    const handleMarkerDragEnd = React.useCallback((e: google.maps.MapMouseEvent) => {
        if (e.latLng) {
            const lat = e.latLng.lat();
            const lng = e.latLng.lng();
            form.setValue('location', { lat, lng });
        }
    }, []);

    const [azureSubscriptionKey, setAzureSubscriptionKey] = React.useState<string>();
    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: location?.lat,
            long: location?.lng,
            azureKey: azureSubscriptionKey || '',
        },
        { enabled: false },
    );

    // on Marker moved (latitude + longitude changed), do reverse search lat,lng to address
    React.useEffect(() => {
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

    const defaultCenter = { lat: MapStore.defaultLatitude, lng: MapStore.defaultLongitude };
    
    return (
        <div>
            <Card>
                <CardHeader>
                    <CardTitle className='text-2xl text-primary'>Edit Partner Location</CardTitle>
                </CardHeader>
                <CardContent>
                    <TooltipProvider delayDuration={0}>
                        <Form {...form}>
                            <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                                <input type='hidden' name='Id' value={locationId} />
                                <FormField
                                    control={form.control}
                                    name='name'
                                    render={({ field }) => (
                                        <FormItem className='col-span-6'>
                                            <Tooltip>
                                                <TooltipTrigger>
                                                    <FormLabel>Location Name</FormLabel>
                                                </TooltipTrigger>
                                                <TooltipContent className='max-w-64'>
                                                    {ToolTips.PartnerLocationName}
                                                </TooltipContent>
                                            </Tooltip>
                                            <FormControl>
                                                <Input {...field} maxLength={64} />
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
                                            <Tooltip>
                                                <TooltipTrigger>
                                                    <FormLabel>Is Active</FormLabel>
                                                </TooltipTrigger>
                                                <TooltipContent className='max-w-64'>
                                                    {ToolTips.PartnerLocationIsPartnerLocationActive}
                                                </TooltipContent>
                                            </Tooltip>
                                            <FormControl>
                                                <div className="flex items-center space-x-2">
                                                    <Checkbox id="isActive" checked={field.value} onCheckedChange={field.onChange} />
                                                    <label
                                                        htmlFor="isActive"
                                                        className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
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
                                        <FormItem className='col-span-6'>
                                            <Tooltip>
                                                <TooltipTrigger>
                                                    <FormLabel>Public Notes</FormLabel>
                                                </TooltipTrigger>
                                                <TooltipContent className='max-w-64'>
                                                    {ToolTips.PartnerLocationPublicNotes}
                                                </TooltipContent>
                                            </Tooltip>
                                            <FormControl>
                                                <Textarea {...field} maxLength={2048} className='h-24' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <FormField
                                    control={form.control}
                                    name='privateNotes'
                                    render={({ field }) => (
                                        <FormItem className='col-span-6'>
                                            <Tooltip>
                                                <TooltipTrigger>
                                                    <FormLabel>Private Notes</FormLabel>
                                                </TooltipTrigger>
                                                <TooltipContent className='max-w-64'>
                                                    {ToolTips.PartnerLocationPrivateNotes}
                                                </TooltipContent>
                                            </Tooltip>
                                            <FormControl>
                                                <Textarea {...field} maxLength={2048} className='h-24' />
                                            </FormControl>
                                            <FormMessage />
                                        </FormItem>
                                    )}
                                />
                                <div className="col-span-12">
                                    <div>
                                        Click on the map to set the location for your Partner. The location fields above will be
                                    automatically populated.
                                    </div>
                                    <div style={{ position: 'relative', width: '100%' }}>
                                        <GoogleMap defaultCenter={defaultCenter} onClick={handleClickMap}>
                                            <Marker
                                                position={
                                                    location
                                                        ? location
                                                        : defaultCenter
                                                }
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
                                </div>
                                <div className='col-span-12 flex justify-end gap-2'>
                                    <Button variant='secondary' onClick={() => form.reset()}>
                                        Cancel
                                    </Button>
                                    {/* <Button type='submit' disabled={createPartnerRequest.isLoading}>
                                        {createPartnerRequest.isLoading ? (
                                            <Loader2 className='animate-spin' />
                                        ) : null}
                                        Submit
                                    </Button> */}
                                </div>
                            </form>
                        </Form>
                    </TooltipProvider>
                </CardContent>
            </Card>
        </div>
    );
};

const PartnerLocationEditWrapper = (props: PartnerLocationEditDataProps) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <PartnerLocationEdit {...props} />
        </APIProvider>
    );
};

export default PartnerLocationEditWrapper;
