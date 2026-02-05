import { useState } from 'react';
import { useParams, Link } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { MapPin, Users, ArrowLeft, Crown, Globe, Lock, Calendar, UserPlus, LogOut, Loader2, Image, Share2 } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import TeamData from '@/components/Models/TeamData';
import TeamMemberData from '@/components/Models/TeamMemberData';
import TeamPhotoData from '@/components/Models/TeamPhotoData';
import EventData from '@/components/Models/EventData';
import {
    GetTeamById,
    GetTeamMembers,
    GetTeamPhotos,
    GetTeamUpcomingEvents,
    JoinTeam,
    RemoveTeamMember,
} from '@/services/teams';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { ReportPhotoButton } from '@/components/ReportPhotoButton';
import { ShareDialog } from '@/components/sharing';
import { getTeamShareableContent, getTeamShareMessage } from '@/lib/sharing-messages';
import { ColumnDef } from '@tanstack/react-table';
import moment from 'moment';

const getLocation = (team: TeamData) => {
    const parts = [team.city, team.region, team.country].filter(Boolean);
    return parts.join(', ') || 'Location not specified';
};

const formatDate = (date: Date) => {
    return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
};

export const TeamDetailPage = () => {
    const { teamId } = useParams<{ teamId: string }>() as { teamId: string };
    const queryClient = useQueryClient();
    const { currentUser, isUserLoaded } = useLogin();
    const { toast } = useToast();
    const [showShareDialog, setShowShareDialog] = useState(false);

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

    const { data: upcomingEvents } = useQuery<AxiosResponse<EventData[]>, unknown, EventData[]>({
        queryKey: GetTeamUpcomingEvents({ teamId }).key,
        queryFn: GetTeamUpcomingEvents({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const { data: photos } = useQuery<AxiosResponse<TeamPhotoData[]>, unknown, TeamPhotoData[]>({
        queryKey: GetTeamPhotos({ teamId }).key,
        queryFn: GetTeamPhotos({ teamId }).service,
        select: (res) => res.data,
        enabled: !!teamId,
    });

    const joinMutation = useMutation({
        mutationKey: JoinTeam().key,
        mutationFn: JoinTeam().service,
        onSuccess: async () => {
            toast({
                title: 'Joined team!',
                description: 'You are now a member of this team.',
            });
            await queryClient.invalidateQueries({ queryKey: GetTeamMembers({ teamId }).key });
        },
        onError: () => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to join the team. Please try again.',
            });
        },
    });

    const leaveMutation = useMutation({
        mutationKey: RemoveTeamMember().key,
        mutationFn: RemoveTeamMember().service,
        onSuccess: async () => {
            toast({
                title: 'Left team',
                description: 'You have left the team.',
            });
            await queryClient.invalidateQueries({ queryKey: GetTeamMembers({ teamId }).key });
        },
        onError: (error: Error) => {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: error.message || 'Failed to leave the team. Please try again.',
            });
        },
    });

    const isMember = members?.some((m) => m.userId === currentUser.id);
    const isLead = members?.some((m) => m.userId === currentUser.id && m.isTeamLead);
    const leadCount = members?.filter((m) => m.isTeamLead).length || 0;

    const handleJoin = () => {
        joinMutation.mutate({ teamId });
    };

    const handleLeave = () => {
        if (isLead && leadCount <= 1) {
            toast({
                variant: 'destructive',
                title: 'Cannot leave',
                description: 'You are the last team lead. Promote another member before leaving.',
            });
            return;
        }
        if (window.confirm('Are you sure you want to leave this team?')) {
            leaveMutation.mutate({ teamId, userId: currentUser.id });
        }
    };

    const memberColumns: ColumnDef<TeamMemberData>[] = [
        {
            accessorKey: 'userName',
            header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
            cell: ({ row }) => (
                <div className='flex items-center gap-2'>
                    <span>{row.original.userName || 'Unknown'}</span>
                    {row.original.isTeamLead ? (
                        <Badge variant='outline' className='bg-primary text-white border-0'>
                            <Crown className='h-3 w-3 mr-1' /> Lead
                        </Badge>
                    ) : null}
                </div>
            ),
        },
        {
            accessorKey: 'joinedDate',
            header: ({ column }) => <DataTableColumnHeader column={column} title='Joined' />,
            cell: ({ row }) => formatDate(row.original.joinedDate),
        },
    ];

    const eventColumns: ColumnDef<EventData>[] = [
        {
            accessorKey: 'name',
            header: ({ column }) => <DataTableColumnHeader column={column} title='Event' />,
            cell: ({ row }) => (
                <Link to={`/eventdetails/${row.original.id}`} className='text-primary hover:underline font-medium'>
                    {row.original.name}
                </Link>
            ),
        },
        {
            accessorKey: 'eventDate',
            header: ({ column }) => <DataTableColumnHeader column={column} title='Date' />,
            cell: ({ row }) => moment(row.original.eventDate).format('MMM D, YYYY'),
        },
        {
            accessorKey: 'city',
            header: 'Location',
            cell: ({ row }) => row.original.city || '-',
        },
    ];

    if (isLoadingTeam) {
        return (
            <div>
                <HeroSection Title='Team' Description='Loading...' />
                <div className='container py-8 text-center'>
                    <Loader2 className='h-8 w-8 animate-spin mx-auto' />
                </div>
            </div>
        );
    }

    if (!team) {
        return (
            <div>
                <HeroSection Title='Team' Description='Not Found' />
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

    return (
        <div>
            <HeroSection Title={team.name} Description={team.description || 'Team details'} />
            <div className='container py-8'>
                <div className='mb-4'>
                    <Button variant='outline' asChild>
                        <Link to='/teams'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Teams
                        </Link>
                    </Button>
                </div>

                <div className='grid grid-cols-1 lg:grid-cols-3 gap-6'>
                    {/* Main content */}
                    <div className='lg:col-span-2 space-y-6'>
                        {/* Team Info */}
                        <Card>
                            <CardHeader>
                                <div className='flex items-center justify-between'>
                                    <div className='flex items-center gap-4'>
                                        {team.logoUrl ? (
                                            <img
                                                src={team.logoUrl}
                                                alt={`${team.name} logo`}
                                                className='w-16 h-16 rounded-lg object-cover border'
                                            />
                                        ) : (
                                            <div className='w-16 h-16 rounded-lg bg-muted flex items-center justify-center'>
                                                <Users className='h-8 w-8 text-muted-foreground' />
                                            </div>
                                        )}
                                        <CardTitle className='text-2xl'>{team.name}</CardTitle>
                                    </div>
                                    <div className='flex items-center gap-2'>
                                        {team.isPublic ? (
                                            <Badge
                                                variant='outline'
                                                className='bg-green-100 text-green-800 border-green-300'
                                            >
                                                <Globe className='h-3 w-3 mr-1' /> Public
                                            </Badge>
                                        ) : (
                                            <Badge
                                                variant='outline'
                                                className='bg-gray-100 text-gray-800 border-gray-300'
                                            >
                                                <Lock className='h-3 w-3 mr-1' /> Private
                                            </Badge>
                                        )}
                                    </div>
                                </div>
                            </CardHeader>
                            <CardContent>
                                <div className='space-y-4'>
                                    {team.description ? (
                                        <div>
                                            <h3 className='font-semibold mb-2'>About</h3>
                                            <p className='text-muted-foreground'>{team.description}</p>
                                        </div>
                                    ) : null}
                                    <div className='flex items-center gap-2 text-muted-foreground'>
                                        <MapPin className='h-4 w-4' />
                                        <span>{getLocation(team)}</span>
                                    </div>
                                </div>
                            </CardContent>
                        </Card>

                        {/* Members */}
                        <Card>
                            <CardHeader>
                                <CardTitle className='flex items-center gap-2'>
                                    <Users className='h-5 w-5' />
                                    Members ({members?.length || 0})
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                {isLoadingMembers ? (
                                    <div className='text-center py-4'>Loading members...</div>
                                ) : members && members.length > 0 ? (
                                    <DataTable columns={memberColumns} data={members} />
                                ) : (
                                    <p className='text-muted-foreground text-center py-4'>No members yet.</p>
                                )}
                            </CardContent>
                        </Card>

                        {/* Upcoming Events */}
                        <Card>
                            <CardHeader>
                                <CardTitle className='flex items-center gap-2'>
                                    <Calendar className='h-5 w-5' />
                                    Upcoming Events ({upcomingEvents?.length || 0})
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                {upcomingEvents && upcomingEvents.length > 0 ? (
                                    <DataTable columns={eventColumns} data={upcomingEvents} />
                                ) : (
                                    <p className='text-muted-foreground text-center py-4'>No upcoming events.</p>
                                )}
                            </CardContent>
                        </Card>

                        {/* Team Photos */}
                        {photos && photos.filter((p) => !p.inReview || currentUser.isSiteAdmin).length > 0 ? (
                            <Card>
                                <CardHeader>
                                    <CardTitle className='flex items-center gap-2'>
                                        <Image className='h-5 w-5' />
                                        Team Photos (
                                        {photos.filter((p) => !p.inReview || currentUser.isSiteAdmin).length})
                                    </CardTitle>
                                </CardHeader>
                                <CardContent>
                                    <div className='grid grid-cols-2 md:grid-cols-3 gap-4'>
                                        {photos
                                            .filter((p) => !p.inReview || currentUser.isSiteAdmin)
                                            .map((photo) => (
                                                <div
                                                    key={photo.id}
                                                    className='relative rounded-lg overflow-hidden border aspect-square group'
                                                >
                                                    <img
                                                        src={photo.imageUrl}
                                                        alt={photo.caption || 'Team activity'}
                                                        className='w-full h-full object-cover'
                                                    />
                                                    {/* Report button - show for logged in users who aren't the uploader */}
                                                    {isUserLoaded && photo.uploadedByUserId !== currentUser.id ? (
                                                        <div className='absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity'>
                                                            <ReportPhotoButton
                                                                photoId={photo.id}
                                                                photoType='TeamPhoto'
                                                                variant='secondary'
                                                                size='icon'
                                                                className='h-8 w-8 bg-white/80 hover:bg-white'
                                                            />
                                                        </div>
                                                    ) : null}
                                                    {/* In review badge for admins */}
                                                    {photo.inReview && currentUser.isSiteAdmin ? (
                                                        <Badge className='absolute bottom-2 left-2 bg-yellow-500'>
                                                            Flagged
                                                        </Badge>
                                                    ) : null}
                                                </div>
                                            ))}
                                    </div>
                                </CardContent>
                            </Card>
                        ) : null}
                    </div>

                    {/* Sidebar */}
                    <div className='space-y-6'>
                        {/* Share Card */}
                        <Card>
                            <CardContent className='pt-6'>
                                <Button
                                    variant='outline'
                                    className='w-full'
                                    onClick={() => setShowShareDialog(true)}
                                >
                                    <Share2 className='h-4 w-4 mr-2' />
                                    Share Team
                                </Button>
                            </CardContent>
                        </Card>

                        {/* Actions */}
                        {isUserLoaded ? (
                            <Card>
                                <CardHeader>
                                    <CardTitle>Actions</CardTitle>
                                </CardHeader>
                                <CardContent className='space-y-3'>
                                    {!isMember ? (
                                        <Button
                                            className='w-full'
                                            onClick={handleJoin}
                                            disabled={joinMutation.isPending || !team.isPublic}
                                        >
                                            {joinMutation.isPending ? (
                                                <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                            ) : (
                                                <UserPlus className='h-4 w-4 mr-2' />
                                            )}
                                            {team.requiresApproval ? 'Request to Join' : 'Join Team'}
                                        </Button>
                                    ) : (
                                        <>
                                            {isLead ? (
                                                <Button variant='outline' className='w-full' asChild>
                                                    <Link to={`/teams/${teamId}/edit`}>Manage Team</Link>
                                                </Button>
                                            ) : null}
                                            <Button
                                                variant='outline'
                                                className='w-full'
                                                onClick={handleLeave}
                                                disabled={leaveMutation.isPending}
                                            >
                                                {leaveMutation.isPending ? (
                                                    <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                                ) : (
                                                    <LogOut className='h-4 w-4 mr-2' />
                                                )}
                                                Leave Team
                                            </Button>
                                        </>
                                    )}
                                    {!team.isPublic && !isMember && (
                                        <p className='text-sm text-muted-foreground text-center'>
                                            This is a private team. You must be invited to join.
                                        </p>
                                    )}
                                </CardContent>
                            </Card>
                        ) : null}

                        {/* Team Stats */}
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
                                <div className='flex justify-between'>
                                    <span className='text-muted-foreground'>Upcoming Events</span>
                                    <span className='font-medium'>{upcomingEvents?.length || 0}</span>
                                </div>
                            </CardContent>
                        </Card>

                        {/* Member Status */}
                        {isMember ? (
                            <Card>
                                <CardContent className='pt-6'>
                                    <div className='flex items-center gap-2 text-green-600'>
                                        <Users className='h-5 w-5' />
                                        <span className='font-medium'>You are a member</span>
                                        {isLead ? (
                                            <Badge variant='outline' className='bg-primary text-white border-0 ml-2'>
                                                <Crown className='h-3 w-3 mr-1' /> Lead
                                            </Badge>
                                        ) : null}
                                    </div>
                                </CardContent>
                            </Card>
                        ) : null}
                    </div>
                </div>
            </div>

            {/* Share Dialog */}
            <ShareDialog
                content={getTeamShareableContent(team)}
                open={showShareDialog}
                onOpenChange={setShowShareDialog}
                message={getTeamShareMessage(team)}
            />
        </div>
    );
};

export default TeamDetailPage;
