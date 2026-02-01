import { useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import { ArrowLeft, Globe, Loader2, Lock, MapPin, Users } from 'lucide-react';
import { Link } from 'react-router';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Switch } from '@/components/ui/switch';
import { Button } from '@/components/ui/button';
import { GoogleMap } from '@/components/Map/GoogleMap';
import {
    AzureSearchLocationInputWithKey as AzureSearchLocationInput,
    SearchLocationOption,
} from '@/components/Map/AzureSearchLocationInput';
import TeamData from '@/components/Models/TeamData';
import { CreateTeam, GetMyTeams, GetTeamsILead } from '@/services/teams';
import { AzureMapSearchAddressReverse, AzureMapSearchAddressReverse_Params } from '@/services/maps';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { useGetAzureKey } from '@/hooks/useGetAzureKey';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { cn } from '@/lib/utils';

const MAX_TEAM_NAME_LENGTH = 200;
const MAX_TEAM_DESC_LENGTH = 2000;

const createTeamSchema = z.object({
    name: z
        .string()
        .min(3, { message: 'Team name must be at least 3 characters.' })
        .max(MAX_TEAM_NAME_LENGTH, { message: `Team name must be shorter than ${MAX_TEAM_NAME_LENGTH} characters.` }),
    description: z
        .string()
        .max(MAX_TEAM_DESC_LENGTH, { message: `Description must be shorter than ${MAX_TEAM_DESC_LENGTH} characters.` })
        .optional(),
    isPublic: z.boolean(),
    requiresApproval: z.boolean(),
    latitude: z.number().nullable(),
    longitude: z.number().nullable(),
    city: z.string().optional(),
    region: z.string().optional(),
    country: z.string().optional(),
    postalCode: z.string().optional(),
});

type CreateTeamFormValues = z.infer<typeof createTeamSchema>;

const CreateTeamForm = () => {
    const azureKey = useGetAzureKey();
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const createTeam = useMutation({
        mutationKey: CreateTeam().key,
        mutationFn: CreateTeam().service,
        onSuccess: async (response) => {
            toast({
                title: 'Team created!',
                description: `Your team "${response.data.name}" has been created successfully.`,
            });
            await queryClient.invalidateQueries({ queryKey: GetMyTeams().key });
            await queryClient.invalidateQueries({ queryKey: GetTeamsILead().key });
            navigate(`/teams/${response.data.id}`);
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: error.message || 'Failed to create team. Please try again.',
            });
        },
    });

    const form = useForm<CreateTeamFormValues>({
        resolver: zodResolver(createTeamSchema),
        defaultValues: {
            name: '',
            description: '',
            isPublic: true,
            requiresApproval: true,
            latitude: null,
            longitude: null,
            city: '',
            region: '',
            country: '',
            postalCode: '',
        },
    });

    const map = useMap('teamLocationPicker');

    const handleSelectSearchLocation = useCallback(
        (location: SearchLocationOption) => {
            const { lat, lon } = location.position;
            form.setValue('latitude', lat);
            form.setValue('longitude', lon);
            if (map) map.panTo({ lat, lng: lon });
        },
        [map, form],
    );

    const handleClickMap = useCallback(
        (e: MapMouseEvent) => {
            if (e.detail.latLng) {
                const lat = e.detail.latLng.lat;
                const lng = e.detail.latLng.lng;
                form.setValue('latitude', lat);
                form.setValue('longitude', lng);
            }
        },
        [form],
    );

    const handleMarkerDragEnd = useCallback(
        (e: google.maps.MapMouseEvent) => {
            if (e.latLng) {
                const lat = e.latLng.lat();
                const lng = e.latLng.lng();
                form.setValue('latitude', lat);
                form.setValue('longitude', lng);
            }
        },
        [form],
    );

    const latitude = form.watch('latitude');
    const longitude = form.watch('longitude');
    const teamName = form.watch('name');
    const description = form.watch('description');
    const isPublic = form.watch('isPublic');

    const searchAddressReverse = async (params: AzureMapSearchAddressReverse_Params) => {
        try {
            const { data } = await AzureMapSearchAddressReverse().service(params);
            const firstResult = data?.addresses[0];
            if (firstResult) {
                form.setValue('city', firstResult.address.municipality || '');
                form.setValue('region', firstResult.address.countrySubdivisionName || '');
                form.setValue('country', firstResult.address.country || '');
                form.setValue('postalCode', firstResult.address.postalCode || '');
            }
        } catch {
            // Ignore reverse geocoding errors
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

    const onSubmit = (formValues: CreateTeamFormValues) => {
        const team = new TeamData();
        team.name = formValues.name;
        team.description = formValues.description || '';
        team.isPublic = formValues.isPublic;
        team.requiresApproval = formValues.requiresApproval;
        team.latitude = formValues.latitude;
        team.longitude = formValues.longitude;
        team.city = formValues.city || '';
        team.region = formValues.region || '';
        team.country = formValues.country || '';
        team.postalCode = formValues.postalCode || '';
        team.createdByUserId = currentUser.id;
        team.isActive = true;

        createTeam.mutate(team);
    };

    const defaultCenter = latitude && longitude ? { lat: latitude, lng: longitude } : { lat: 47.6062, lng: -122.3321 };

    return (
        <div>
            <HeroSection Title='Create a Team' Description='Start a team to organize cleanup events together' />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to='/teams'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Teams
                        </Link>
                    </Button>
                </div>

                <div className='grid grid-cols-1 lg:grid-cols-3 gap-6'>
                    <div className='lg:col-span-2'>
                        <Card>
                            <CardHeader>
                                <CardTitle className='flex items-center gap-2'>
                                    <Users className='h-5 w-5' />
                                    Team Details
                                </CardTitle>
                                <CardDescription>
                                    Fill in the details below to create your team. You'll be added as the team lead.
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                <Form {...form}>
                                    <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-6'>
                                        <FormField
                                            control={form.control}
                                            name='name'
                                            render={({ field }) => (
                                                <FormItem>
                                                    <FormLabel>Team Name *</FormLabel>
                                                    <FormControl>
                                                        <Input placeholder='My Awesome Cleanup Crew' {...field} />
                                                    </FormControl>
                                                    <div className='flex justify-between'>
                                                        <FormMessage />
                                                        <FormDescription
                                                            className={cn('text-right', {
                                                                'text-destructive':
                                                                    teamName.length > MAX_TEAM_NAME_LENGTH,
                                                            })}
                                                        >
                                                            {teamName.length}/{MAX_TEAM_NAME_LENGTH}
                                                        </FormDescription>
                                                    </div>
                                                </FormItem>
                                            )}
                                        />

                                        <FormField
                                            control={form.control}
                                            name='description'
                                            render={({ field }) => (
                                                <FormItem>
                                                    <FormLabel>Description</FormLabel>
                                                    <FormControl>
                                                        <Textarea
                                                            placeholder='Tell people about your team and what areas you focus on...'
                                                            className='resize-none min-h-[100px]'
                                                            {...field}
                                                        />
                                                    </FormControl>
                                                    <div className='flex justify-between'>
                                                        <FormMessage />
                                                        <FormDescription
                                                            className={cn('text-right', {
                                                                'text-destructive':
                                                                    (description || '').length > MAX_TEAM_DESC_LENGTH,
                                                            })}
                                                        >
                                                            {(description || '').length}/{MAX_TEAM_DESC_LENGTH}
                                                        </FormDescription>
                                                    </div>
                                                </FormItem>
                                            )}
                                        />

                                        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                                            <FormField
                                                control={form.control}
                                                name='isPublic'
                                                render={({ field }) => (
                                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                                        <div className='space-y-0.5'>
                                                            <FormLabel className='flex items-center gap-2'>
                                                                {field.value ? (
                                                                    <Globe className='h-4 w-4' />
                                                                ) : (
                                                                    <Lock className='h-4 w-4' />
                                                                )}
                                                                {field.value ? 'Public Team' : 'Private Team'}
                                                            </FormLabel>
                                                            <FormDescription>
                                                                {field.value
                                                                    ? 'Visible on map and searchable'
                                                                    : 'Invite only, hidden from search'}
                                                            </FormDescription>
                                                        </div>
                                                        <FormControl>
                                                            <Switch
                                                                checked={field.value}
                                                                onCheckedChange={field.onChange}
                                                            />
                                                        </FormControl>
                                                    </FormItem>
                                                )}
                                            />

                                            <FormField
                                                control={form.control}
                                                name='requiresApproval'
                                                render={({ field }) => (
                                                    <FormItem className='flex flex-row items-center justify-between rounded-lg border p-4'>
                                                        <div className='space-y-0.5'>
                                                            <FormLabel>Require Approval</FormLabel>
                                                            <FormDescription>
                                                                {field.value
                                                                    ? 'Lead must approve join requests'
                                                                    : 'Anyone can join immediately'}
                                                            </FormDescription>
                                                        </div>
                                                        <FormControl>
                                                            <Switch
                                                                checked={field.value}
                                                                onCheckedChange={field.onChange}
                                                            />
                                                        </FormControl>
                                                    </FormItem>
                                                )}
                                            />
                                        </div>

                                        <div className='space-y-4'>
                                            <FormLabel className='flex items-center gap-2'>
                                                <MapPin className='h-4 w-4' />
                                                Team Location
                                            </FormLabel>
                                            <FormDescription>
                                                Set your team's primary location. This helps people find teams near
                                                them.
                                            </FormDescription>

                                            <AzureSearchLocationInput
                                                className='w-full'
                                                placeholder='Search for your location...'
                                                onSelectLocation={handleSelectSearchLocation}
                                            />

                                            <div className='relative w-full h-[300px] rounded-lg overflow-hidden border'>
                                                <GoogleMap
                                                    id='teamLocationPicker'
                                                    defaultCenter={defaultCenter}
                                                    defaultZoom={latitude && longitude ? 12 : 4}
                                                    style={{ width: '100%', height: '100%' }}
                                                    onClick={handleClickMap}
                                                    gestureHandling='greedy'
                                                >
                                                    {latitude && longitude ? (
                                                        <Marker
                                                            position={{ lat: latitude, lng: longitude }}
                                                            draggable
                                                            onDragEnd={handleMarkerDragEnd}
                                                        />
                                                    ) : null}
                                                </GoogleMap>
                                            </div>

                                            {latitude && longitude ? (
                                                <p className='text-sm text-muted-foreground'>
                                                    {[form.watch('city'), form.watch('region'), form.watch('country')]
                                                        .filter(Boolean)
                                                        .join(', ') || 'Location set'}
                                                </p>
                                            ) : (
                                                <p className='text-sm text-muted-foreground'>
                                                    Click on the map or search to set your team's location
                                                </p>
                                            )}
                                        </div>

                                        <div className='flex gap-4 justify-end pt-4'>
                                            <Button type='button' variant='outline' asChild>
                                                <Link to='/teams'>Cancel</Link>
                                            </Button>
                                            <Button type='submit' disabled={createTeam.isPending}>
                                                {createTeam.isPending ? (
                                                    <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                                ) : (
                                                    <Users className='h-4 w-4 mr-2' />
                                                )}
                                                Create Team
                                            </Button>
                                        </div>
                                    </form>
                                </Form>
                            </CardContent>
                        </Card>
                    </div>

                    <div className='space-y-6'>
                        <Card>
                            <CardHeader>
                                <CardTitle>About Teams</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-4 text-sm text-muted-foreground'>
                                <p>
                                    Teams allow you to organize regular cleanup efforts with a consistent group of
                                    volunteers.
                                </p>
                                <p>
                                    <strong>As a team lead, you can:</strong>
                                </p>
                                <ul className='list-disc list-inside space-y-1'>
                                    <li>Invite new members</li>
                                    <li>Approve join requests</li>
                                    <li>Promote members to co-leads</li>
                                    <li>Create team events</li>
                                    <li>Track your team's collective impact</li>
                                </ul>
                            </CardContent>
                        </Card>

                        <Card>
                            <CardHeader>
                                <CardTitle>Team Visibility</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-4 text-sm text-muted-foreground'>
                                <div className='flex items-start gap-2'>
                                    <Globe className='h-4 w-4 mt-0.5 text-green-600' />
                                    <div>
                                        <p className='font-medium text-foreground'>Public Teams</p>
                                        <p>
                                            Appear on the teams map and in search results. Anyone can request to join.
                                        </p>
                                    </div>
                                </div>
                                <div className='flex items-start gap-2'>
                                    <Lock className='h-4 w-4 mt-0.5 text-gray-600' />
                                    <div>
                                        <p className='font-medium text-foreground'>Private Teams</p>
                                        <p>
                                            Hidden from map and search. Members can only join through direct invitation.
                                        </p>
                                    </div>
                                </div>
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
};

export const CreateTeamPage = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) {
        return (
            <div>
                <HeroSection Title='Create a Team' Description='Start a team to organize cleanup events together' />
                <div className='container py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </div>
            </div>
        );
    }

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <CreateTeamForm />
        </APIProvider>
    );
};

export default CreateTeamPage;
