import { useCallback, useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { APIProvider, MapMouseEvent, Marker, useMap } from '@vis.gl/react-google-maps';
import { ArrowLeft, Crown, Globe, Loader2, Lock, MapPin, Save, Trash2, UserMinus, UserPlus, Users } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Switch } from '@/components/ui/switch';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { GoogleMap } from '@/components/Map/GoogleMap';
import {
    AzureSearchLocationInputWithKey as AzureSearchLocationInput,
    SearchLocationOption,
} from '@/components/Map/AzureSearchLocationInput';
import TeamData from '@/components/Models/TeamData';
import TeamMemberData from '@/components/Models/TeamMemberData';
import {
    GetTeamById,
    GetTeamMembers,
    UpdateTeam,
    DeactivateTeam,
    RemoveTeamMember,
    PromoteToTeamLead,
    DemoteFromTeamLead,
    GetMyTeams,
    GetTeamsILead,
} from '@/services/teams';
import { AzureMapSearchAddressReverse, AzureMapSearchAddressReverse_Params } from '@/services/maps';
import { useGetGoogleMapApiKey } from '@/hooks/useGetGoogleMapApiKey';
import { useGetAzureKey } from '@/hooks/useGetAzureKey';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { cn } from '@/lib/utils';

const MAX_TEAM_NAME_LENGTH = 200;
const MAX_TEAM_DESC_LENGTH = 2000;

const editTeamSchema = z.object({
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

type EditTeamFormValues = z.infer<typeof editTeamSchema>;

const formatDate = (date: Date) => {
    return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
};

const EditTeamForm = () => {
    const { teamId } = useParams<{ teamId: string }>() as { teamId: string };
    const azureKey = useGetAzureKey();
    const { currentUser } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const [processingMemberId, setProcessingMemberId] = useState<string | null>(null);

    const { data: team, isLoading: isLoadingTeam } = useQuery<AxiosResponse<TeamData>, unknown, TeamData>({
        queryKey: GetTeamById({ teamId }).key,
        queryFn: GetTeamById({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const { data: members, isLoading: isLoadingMembers } = useQuery<
        AxiosResponse<TeamMemberData[]>,
        unknown,
        TeamMemberData[]
    >({
        queryKey: GetTeamMembers({ teamId }).key,
        queryFn: GetTeamMembers({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const updateTeam = useMutation({
        mutationKey: UpdateTeam().key,
        mutationFn: UpdateTeam().service,
        onSuccess: async () => {
            toast({ title: 'Team updated!', description: 'Your changes have been saved.' });
            await queryClient.invalidateQueries({ queryKey: GetTeamById({ teamId }).key });
            await queryClient.invalidateQueries({ queryKey: GetMyTeams().key });
            await queryClient.invalidateQueries({ queryKey: GetTeamsILead().key });
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: error.message || 'Failed to update team.',
            });
        },
    });

    const deactivateTeam = useMutation({
        mutationKey: DeactivateTeam().key,
        mutationFn: DeactivateTeam().service,
        onSuccess: async () => {
            toast({ title: 'Team deactivated', description: 'The team has been deactivated.' });
            await queryClient.invalidateQueries({ queryKey: GetMyTeams().key });
            await queryClient.invalidateQueries({ queryKey: GetTeamsILead().key });
            navigate('/mydashboard');
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: error.message || 'Failed to deactivate team.',
            });
        },
    });

    const removeMember = useMutation({
        mutationKey: RemoveTeamMember().key,
        mutationFn: RemoveTeamMember().service,
        onSuccess: async () => {
            toast({ title: 'Member removed' });
            await queryClient.invalidateQueries({ queryKey: GetTeamMembers({ teamId }).key });
            setProcessingMemberId(null);
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Error', description: error.message || 'Failed to remove member.' });
            setProcessingMemberId(null);
        },
    });

    const promoteMember = useMutation({
        mutationKey: PromoteToTeamLead().key,
        mutationFn: PromoteToTeamLead().service,
        onSuccess: async () => {
            toast({ title: 'Member promoted to lead' });
            await queryClient.invalidateQueries({ queryKey: GetTeamMembers({ teamId }).key });
            setProcessingMemberId(null);
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: error.message || 'Failed to promote member.',
            });
            setProcessingMemberId(null);
        },
    });

    const demoteMember = useMutation({
        mutationKey: DemoteFromTeamLead().key,
        mutationFn: DemoteFromTeamLead().service,
        onSuccess: async () => {
            toast({ title: 'Lead role removed' });
            await queryClient.invalidateQueries({ queryKey: GetTeamMembers({ teamId }).key });
            setProcessingMemberId(null);
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Error', description: error.message || 'Failed to demote member.' });
            setProcessingMemberId(null);
        },
    });

    const form = useForm<EditTeamFormValues>({
        resolver: zodResolver(editTeamSchema),
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

    // Populate form when team data loads
    useEffect(() => {
        if (team) {
            form.reset({
                name: team.name,
                description: team.description,
                isPublic: team.isPublic,
                requiresApproval: team.requiresApproval,
                latitude: team.latitude,
                longitude: team.longitude,
                city: team.city,
                region: team.region,
                country: team.country,
                postalCode: team.postalCode,
            });
        }
    }, [team, form]);

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

    const onSubmit = (formValues: EditTeamFormValues) => {
        if (!team) return;

        const updatedTeam = new TeamData();
        updatedTeam.id = team.id;
        updatedTeam.name = formValues.name;
        updatedTeam.description = formValues.description || '';
        updatedTeam.isPublic = formValues.isPublic;
        updatedTeam.requiresApproval = formValues.requiresApproval;
        updatedTeam.latitude = formValues.latitude;
        updatedTeam.longitude = formValues.longitude;
        updatedTeam.city = formValues.city || '';
        updatedTeam.region = formValues.region || '';
        updatedTeam.country = formValues.country || '';
        updatedTeam.postalCode = formValues.postalCode || '';
        updatedTeam.createdByUserId = team.createdByUserId;
        updatedTeam.createdDate = team.createdDate;
        updatedTeam.isActive = team.isActive;
        updatedTeam.logoUrl = team.logoUrl;

        updateTeam.mutate(updatedTeam);
    };

    const handleDeactivateTeam = () => {
        if (window.confirm('Are you sure you want to deactivate this team? This action can be undone by an admin.')) {
            deactivateTeam.mutate({ teamId });
        }
    };

    const handleRemoveMember = (userId: string, userName: string) => {
        if (window.confirm(`Remove "${userName}" from the team?`)) {
            setProcessingMemberId(userId);
            removeMember.mutate({ teamId, userId });
        }
    };

    const handlePromoteMember = (userId: string) => {
        setProcessingMemberId(userId);
        promoteMember.mutate({ teamId, userId });
    };

    const handleDemoteMember = (userId: string, userName: string) => {
        const leadCount = members?.filter((m) => m.isTeamLead).length || 0;
        if (leadCount <= 1) {
            toast({
                variant: 'destructive',
                title: 'Cannot remove last lead',
                description: 'A team must have at least one lead. Promote another member first.',
            });
            return;
        }
        if (window.confirm(`Remove lead role from "${userName}"?`)) {
            setProcessingMemberId(userId);
            demoteMember.mutate({ teamId, userId });
        }
    };

    // Check if current user is a team lead
    const isLead = members?.some((m) => m.userId === currentUser.id && m.isTeamLead);
    const leadCount = members?.filter((m) => m.isTeamLead).length || 0;
    const defaultCenter = latitude && longitude ? { lat: latitude, lng: longitude } : { lat: 47.6062, lng: -122.3321 };

    if (isLoadingTeam) {
        return (
            <div>
                <HeroSection Title='Manage Team' Description='Loading...' />
                <div className='container py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </div>
            </div>
        );
    }

    if (!team) {
        return (
            <div>
                <HeroSection Title='Team Not Found' Description='' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>This team could not be found.</p>
                    <Button asChild>
                        <Link to='/teams'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Teams
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    if (!isLead) {
        return (
            <div>
                <HeroSection Title='Access Denied' Description='' />
                <div className='container py-8 text-center'>
                    <p className='mb-4'>You must be a team lead to manage this team.</p>
                    <Button asChild>
                        <Link to={`/teams/${teamId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Team
                        </Link>
                    </Button>
                </div>
            </div>
        );
    }

    return (
        <div>
            <HeroSection Title={`Manage: ${team.name}`} Description='Edit team settings and manage members' />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to={`/teams/${teamId}`}>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Team
                        </Link>
                    </Button>
                </div>

                <div className='grid grid-cols-1 lg:grid-cols-3 gap-6'>
                    <div className='lg:col-span-2 space-y-6'>
                        {/* Team Settings */}
                        <Card>
                            <CardHeader>
                                <CardTitle className='flex items-center gap-2'>
                                    <Users className='h-5 w-5' />
                                    Team Settings
                                </CardTitle>
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
                                                        <Input {...field} />
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
                                                        <Textarea className='resize-none min-h-[100px]' {...field} />
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

                                            <AzureSearchLocationInput
                                                className='w-full'
                                                placeholder='Search for a new location...'
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
                                            ) : null}
                                        </div>

                                        <div className='flex gap-4 justify-end pt-4'>
                                            <Button type='submit' disabled={updateTeam.isPending}>
                                                {updateTeam.isPending ? (
                                                    <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                                ) : (
                                                    <Save className='h-4 w-4 mr-2' />
                                                )}
                                                Save Changes
                                            </Button>
                                        </div>
                                    </form>
                                </Form>
                            </CardContent>
                        </Card>

                        {/* Team Members */}
                        <Card>
                            <CardHeader>
                                <CardTitle className='flex items-center gap-2'>
                                    <Users className='h-5 w-5' />
                                    Team Members ({members?.length || 0})
                                </CardTitle>
                                <CardDescription>Manage your team members and leads</CardDescription>
                            </CardHeader>
                            <CardContent>
                                {isLoadingMembers ? (
                                    <div className='text-center py-4'>
                                        <Loader2 className='h-6 w-6 animate-spin mx-auto' />
                                    </div>
                                ) : (
                                    <Table>
                                        <TableHeader>
                                            <TableRow>
                                                <TableHead>Member</TableHead>
                                                <TableHead>Joined</TableHead>
                                                <TableHead>Role</TableHead>
                                                <TableHead className='text-right'>Actions</TableHead>
                                            </TableRow>
                                        </TableHeader>
                                        <TableBody>
                                            {(members || []).map((member) => {
                                                const isCurrentUser = member.userId === currentUser.id;
                                                const isProcessing = processingMemberId === member.userId;

                                                return (
                                                    <TableRow
                                                        key={member.userId}
                                                        className={isProcessing ? 'opacity-50' : ''}
                                                    >
                                                        <TableCell>
                                                            <span className='font-medium'>
                                                                {member.userName || 'Unknown'}
                                                                {isCurrentUser ? ' (You)' : ''}
                                                            </span>
                                                        </TableCell>
                                                        <TableCell>{formatDate(member.joinedDate)}</TableCell>
                                                        <TableCell>
                                                            {member.isTeamLead ? (
                                                                <Badge className='bg-primary'>
                                                                    <Crown className='h-3 w-3 mr-1' /> Lead
                                                                </Badge>
                                                            ) : (
                                                                <Badge variant='outline'>Member</Badge>
                                                            )}
                                                        </TableCell>
                                                        <TableCell className='text-right'>
                                                            <div className='flex gap-2 justify-end'>
                                                                {member.isTeamLead ? (
                                                                    <Button
                                                                        variant='outline'
                                                                        size='sm'
                                                                        onClick={() =>
                                                                            handleDemoteMember(
                                                                                member.userId,
                                                                                member.userName || 'Unknown',
                                                                            )
                                                                        }
                                                                        disabled={
                                                                            isProcessing ||
                                                                            (isCurrentUser && leadCount <= 1)
                                                                        }
                                                                        title={
                                                                            isCurrentUser && leadCount <= 1
                                                                                ? 'Cannot demote last lead'
                                                                                : 'Remove lead role'
                                                                        }
                                                                    >
                                                                        {isProcessing ? (
                                                                            <Loader2 className='h-4 w-4 animate-spin' />
                                                                        ) : (
                                                                            <Crown className='h-4 w-4' />
                                                                        )}
                                                                    </Button>
                                                                ) : (
                                                                    <Button
                                                                        variant='outline'
                                                                        size='sm'
                                                                        onClick={() =>
                                                                            handlePromoteMember(member.userId)
                                                                        }
                                                                        disabled={isProcessing}
                                                                        title='Promote to lead'
                                                                    >
                                                                        {isProcessing ? (
                                                                            <Loader2 className='h-4 w-4 animate-spin' />
                                                                        ) : (
                                                                            <UserPlus className='h-4 w-4' />
                                                                        )}
                                                                    </Button>
                                                                )}
                                                                {!isCurrentUser ? (
                                                                    <Button
                                                                        variant='outline'
                                                                        size='sm'
                                                                        onClick={() =>
                                                                            handleRemoveMember(
                                                                                member.userId,
                                                                                member.userName || 'Unknown',
                                                                            )
                                                                        }
                                                                        disabled={isProcessing}
                                                                        className='text-destructive hover:text-destructive'
                                                                        title='Remove from team'
                                                                    >
                                                                        {isProcessing ? (
                                                                            <Loader2 className='h-4 w-4 animate-spin' />
                                                                        ) : (
                                                                            <UserMinus className='h-4 w-4' />
                                                                        )}
                                                                    </Button>
                                                                ) : null}
                                                            </div>
                                                        </TableCell>
                                                    </TableRow>
                                                );
                                            })}
                                        </TableBody>
                                    </Table>
                                )}
                            </CardContent>
                        </Card>
                    </div>

                    {/* Sidebar */}
                    <div className='space-y-6'>
                        <Card>
                            <CardHeader>
                                <CardTitle>Team Stats</CardTitle>
                            </CardHeader>
                            <CardContent className='space-y-3'>
                                <div className='flex justify-between'>
                                    <span className='text-muted-foreground'>Members</span>
                                    <span className='font-medium'>{members?.length || 0}</span>
                                </div>
                                <div className='flex justify-between'>
                                    <span className='text-muted-foreground'>Team Leads</span>
                                    <span className='font-medium'>{leadCount}</span>
                                </div>
                            </CardContent>
                        </Card>

                        <Card className='border-destructive'>
                            <CardHeader>
                                <CardTitle className='text-destructive flex items-center gap-2'>
                                    <Trash2 className='h-5 w-5' />
                                    Danger Zone
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                <p className='text-sm text-muted-foreground mb-4'>
                                    Deactivating this team will hide it from search and prevent new members from
                                    joining. An admin can reactivate it later.
                                </p>
                                <Button
                                    variant='destructive'
                                    className='w-full'
                                    onClick={handleDeactivateTeam}
                                    disabled={deactivateTeam.isPending}
                                >
                                    {deactivateTeam.isPending ? (
                                        <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                    ) : (
                                        <Trash2 className='h-4 w-4 mr-2' />
                                    )}
                                    Deactivate Team
                                </Button>
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
};

export const TeamEditPage = () => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey();

    if (isLoading) {
        return (
            <div>
                <HeroSection Title='Manage Team' Description='Loading...' />
                <div className='container py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </div>
            </div>
        );
    }

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <EditTeamForm />
        </APIProvider>
    );
};

export default TeamEditPage;
