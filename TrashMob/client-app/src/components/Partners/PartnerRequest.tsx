import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { SubmitHandler, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Form as ShadcnForm, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Tooltip as ShadcnTooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import PhoneInput from 'react-phone-input-2';

import { APIProvider, MapMouseEvent, useMap } from '@vis.gl/react-google-maps';
import { useMutation } from '@tanstack/react-query';
import * as ToolTips from '../../store/ToolTips';
import PartnerRequestData from '../Models/PartnerRequestData';
import UserData from '../Models/UserData';
import * as Constants from '../Models/Constants';
import * as MapStore from '../../store/MapStore';
import { CreatePartnerRequest } from '../../services/partners';
import { GoogleMap } from '../Map/GoogleMap';
import { Marker } from '@vis.gl/react-google-maps';
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';
import { AzureSearchLocationInput, SearchLocationOption } from '../Map/AzureSearchLocationInput';
import { useAzureMapSearchAddressReverse } from '../../hooks/useAzureMapSearchAddressReverse';
import { PartnerType } from '@/enums/PartnerType';

enum PartnerRequestMode {
    SEND = 'send',
    REQUEST = 'request',
}

interface PartnerRequestProps extends RouteComponentProps<any> {
    mode: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

interface FormInputs {
    name: string;
    partnerTypeId: PartnerType;
    email: string;
    website: string;
    phone: string;
    notes: string;
    location: { lat: number; lng: number };

    // Auto
    streetAddress: string;
    city: string;
    country: string;
    region: string;
    postalCode: string;
}

const formSchema = z.object({
    name: z.string({ required_error: 'Name cannot be blank.' }),
    partnerTypeId: z.string(),
    email: z
        .string({ required_error: 'Email cannot be blank.' })
        .email({ message: 'Please enter valid email address.' }),
    website: z.string().url({ message: 'Please enter valid website.' }).optional(),
    phone: z.string().regex(Constants.RegexPhoneNumber, { message: 'Please enter a valid phone number.' }).optional(),
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

export const PartnerRequest: React.FC<PartnerRequestProps> = (props) => {
    const { mode } = props;

    const createPartnerRequest = useMutation({
        mutationKey: CreatePartnerRequest().key,
        mutationFn: CreatePartnerRequest().service,
        onSuccess: () => {
            props.history.push('/');
        },
    });

    React.useEffect(() => {
        MapStore.getOption().then((opts) => {
            setAzureSubscriptionKey(opts.subscriptionKey);
        });
    }, []);

    const form = useForm<FormInputs>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            partnerTypeId: PartnerType.GOVERNMENT,
        },
    });

    const onSubmit: SubmitHandler<FormInputs> = (data) => {
        const body = new PartnerRequestData();
        body.name = data.name ?? '';
        body.email = data.email ?? '';
        body.phone = data.phone ?? '';
        body.website = data.website ?? '';
        body.partnerRequestStatusId = Constants.PartnerRequestStatusSent;
        body.notes = data.notes ?? '';
        body.streetAddress = data.streetAddress ?? '';
        body.city = data.city ?? '';
        body.region = data.region ?? '';
        body.country = data.country ?? '';
        body.latitude = data.location.lat ?? 0;
        body.longitude = data.location.lng ?? 0;
        body.createdByUserId = props.currentUser.id;
        body.partnerTypeId = Number(data.partnerTypeId);
        body.isBecomeAPartnerRequest = mode !== PartnerRequestMode.SEND;

        createPartnerRequest.mutateAsync(body);
    };

    const location = form.watch('location');

    const title = mode === 'send' ? 'Send invite to join TrashMob as a partner' : 'Apply to become a partner';

    const [azureSubscriptionKey, setAzureSubscriptionKey] = React.useState<string>();

    const { refetch: refetchAddressReverse } = useAzureMapSearchAddressReverse(
        {
            lat: location?.lat,
            long: location?.lng,
            azureKey: azureSubscriptionKey || '',
        },
        { enabled: false },
    );

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

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push('/partnerships');
    }

    return (
        <div className='tailwind'>
            <div className='container !py-12'>
                <Card className=''>
                    <CardHeader>
                        <CardTitle className='text-4xl text-primary'>{title}</CardTitle>
                    </CardHeader>
                    <CardContent>
                        {mode === PartnerRequestMode.SEND && (
                            <p>
                                Use this form to send an informational note to a potential TrashMob.eco partner in your
                                community. Fill out as much detail as you can and TrashMob.eco will reach out to the
                                email address provided with an information packet to see if they would like to become a
                                TrashMob.eco Partner!
                            </p>
                        )}
                        {mode === PartnerRequestMode.REQUEST && (
                            <p>
                                Use this form to request to become a TrashMob.eco partner. TrashMob.eco site
                                adminsitrators will review your request, and either approve it, or reach out to you for
                                more information. If approved, you will be sent a Welcome email with instructions on how
                                to complete setup of your partnership.
                            </p>
                        )}
                        <p>
                            If connecting with a government partner, the department responsible for managing waste and
                            maintaining cleanliness in a community is often a part of the public works department,
                            environmental services division, or a similar agency. You can find contact information for
                            these organizations by searching online or by calling the city's main government phone
                            number and asking for the appropriate department.
                        </p>
                    </CardContent>
                    <CardContent>
                        <TooltipProvider>
                            <ShadcnForm {...form}>
                                <form onSubmit={form.handleSubmit(onSubmit)} className='grid grid-cols-12 gap-4'>
                                    <FormField
                                        control={form.control}
                                        name='name'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <ShadcnTooltip>
                                                    <TooltipTrigger>
                                                        <FormLabel>Partner Name</FormLabel>
                                                    </TooltipTrigger>
                                                    <TooltipContent className='max-w-64'>
                                                        {ToolTips.PartnerRequestName}
                                                    </TooltipContent>
                                                </ShadcnTooltip>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='partnerTypeId'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <ShadcnTooltip>
                                                    <TooltipTrigger>
                                                        <FormLabel>Type</FormLabel>
                                                    </TooltipTrigger>
                                                    <TooltipContent className='max-w-64'>
                                                        {ToolTips.PartnerType}
                                                    </TooltipContent>
                                                </ShadcnTooltip>
                                                <FormControl>
                                                    <RadioGroup
                                                        onValueChange={field.onChange}
                                                        defaultValue={field.value}
                                                        className='flex flex-row space-y-1 h-9 items-center'
                                                    >
                                                        <FormItem className='flex items-center space-x-3 space-y-0'>
                                                            <FormControl>
                                                                <RadioGroupItem value={PartnerType.GOVERNMENT} />
                                                            </FormControl>
                                                            <FormLabel className='font-normal'>Government</FormLabel>
                                                        </FormItem>
                                                        <FormItem className='flex items-center space-x-3 space-y-0'>
                                                            <FormControl>
                                                                <RadioGroupItem value={PartnerType.BUSINESS} />
                                                            </FormControl>
                                                            <FormLabel className='font-normal'>Business</FormLabel>
                                                        </FormItem>
                                                    </RadioGroup>
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='email'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <ShadcnTooltip>
                                                    <TooltipTrigger>
                                                        <FormLabel>Email</FormLabel>
                                                    </TooltipTrigger>
                                                    <TooltipContent className='max-w-64'>
                                                        {mode === 'send'
                                                            ? ToolTips.PartnerRequestInviteEmail
                                                            : ToolTips.PartnerRequestEmail}
                                                    </TooltipContent>
                                                </ShadcnTooltip>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='website'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <ShadcnTooltip>
                                                    <TooltipTrigger>
                                                        <FormLabel>Website</FormLabel>
                                                    </TooltipTrigger>
                                                    <TooltipContent className='max-w-64'>
                                                        {ToolTips.PartnerRequestWebsite}
                                                    </TooltipContent>
                                                </ShadcnTooltip>
                                                <FormControl>
                                                    <Input {...field} />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='phone'
                                        render={({ field }) => (
                                            <FormItem className='col-span-6'>
                                                <ShadcnTooltip>
                                                    <TooltipTrigger>
                                                        <FormLabel>Phone</FormLabel>
                                                    </TooltipTrigger>
                                                    <TooltipContent className='max-w-64'>
                                                        {mode === 'send'
                                                            ? ToolTips.PartnerRequestInvitePhone
                                                            : ToolTips.PartnerRequestPhone}
                                                    </TooltipContent>
                                                </ShadcnTooltip>
                                                <FormControl>
                                                    <PhoneInput
                                                        country='us'
                                                        value={field.value}
                                                        onChange={field.onChange}
                                                        inputClass='flex h-9 !w-full rounded-md border border-input bg-transparent pr-3 py-1 text-base shadow-sm '
                                                    />
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
                                                <ShadcnTooltip>
                                                    <TooltipTrigger>
                                                        <FormLabel>Notes</FormLabel>
                                                    </TooltipTrigger>
                                                    <TooltipContent className='max-w-64'>
                                                        {mode === 'send'
                                                            ? ToolTips.PartnerRequestInviteNotes
                                                            : ToolTips.PartnerRequestNotes}
                                                    </TooltipContent>
                                                </ShadcnTooltip>
                                                <FormControl>
                                                    <Textarea {...field} maxLength={2048} className='h-24' />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='location'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12'>
                                                <FormLabel>Choose a location!</FormLabel>
                                                <FormControl>
                                                    <div className='relative w-full'>
                                                        <GoogleMap onClick={handleClickMap}>
                                                            <Marker
                                                                position={field.value}
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
                                                </FormControl>
                                            </FormItem>
                                        )}
                                    />
                                    <FormField
                                        control={form.control}
                                        name='streetAddress'
                                        render={({ field }) => (
                                            <FormItem className='col-span-12'>
                                                <FormLabel>Street Address</FormLabel>
                                                <FormControl>
                                                    <Input {...field} disabled />
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
                                                <FormLabel>City</FormLabel>
                                                <FormControl>
                                                    <Input {...field} disabled />
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
                                                <FormLabel>Postal Code</FormLabel>
                                                <FormControl>
                                                    <Input {...field} disabled />
                                                </FormControl>
                                                <FormMessage />
                                            </FormItem>
                                        )}
                                    />
                                    <div className='col-span-12 flex justify-end gap-2'>
                                        <Button variant='secondary' onClick={handleCancel}>
                                            Cancel
                                        </Button>
                                        <Button type='submit'>Submit</Button>
                                    </div>
                                </form>
                            </ShadcnForm>
                        </TooltipProvider>
                    </CardContent>
                </Card>
            </div>
        </div>
    );
};

const PartnerRequestWrapper = (props: PartnerRequestProps) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <PartnerRequest {...props} />
        </APIProvider>
    );
};

export default withRouter(PartnerRequestWrapper);
