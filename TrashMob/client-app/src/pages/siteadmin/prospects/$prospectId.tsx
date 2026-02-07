import { useState } from 'react';
import { Link, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import moment from 'moment';
import { Edit, Plus, Globe, Mail, User, MapPin } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Progress } from '@/components/ui/progress';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { useToast } from '@/hooks/use-toast';
import {
    GetCommunityProspectById,
    GetProspectActivities,
    CreateProspectActivity,
    GetProspectScoreBreakdown,
} from '@/services/community-prospects';
import { PipelineStageBadge, ACTIVITY_TYPES } from '@/components/prospects/pipeline-stage-badge';
import ProspectActivityData from '@/components/Models/ProspectActivityData';

export const SiteAdminProspectDetail = () => {
    const { prospectId } = useParams<{ prospectId: string }>() as { prospectId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [activityDialogOpen, setActivityDialogOpen] = useState(false);
    const [activityType, setActivityType] = useState('Note');
    const [activitySubject, setActivitySubject] = useState('');
    const [activityDetails, setActivityDetails] = useState('');

    const { data: prospect } = useQuery({
        queryKey: GetCommunityProspectById({ id: prospectId }).key,
        queryFn: GetCommunityProspectById({ id: prospectId }).service,
        select: (res) => res.data,
        enabled: !!prospectId,
    });

    const { data: activities } = useQuery({
        queryKey: GetProspectActivities({ id: prospectId }).key,
        queryFn: GetProspectActivities({ id: prospectId }).service,
        select: (res) => res.data,
        enabled: !!prospectId,
    });

    const { data: scoreBreakdown } = useQuery({
        queryKey: GetProspectScoreBreakdown({ id: prospectId }).key,
        queryFn: GetProspectScoreBreakdown({ id: prospectId }).service,
        select: (res) => res.data,
        enabled: !!prospectId,
    });

    const createActivity = useMutation({
        mutationKey: CreateProspectActivity({ id: prospectId }).key,
        mutationFn: CreateProspectActivity({ id: prospectId }).service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Activity logged' });
            queryClient.invalidateQueries({
                queryKey: GetProspectActivities({ id: prospectId }).key,
                refetchType: 'all',
            });
            setActivityDialogOpen(false);
            setActivityType('Note');
            setActivitySubject('');
            setActivityDetails('');
        },
    });

    function handleLogActivity() {
        const body = new ProspectActivityData();
        body.prospectId = prospectId;
        body.activityType = activityType;
        body.subject = activitySubject;
        body.details = activityDetails;
        createActivity.mutate(body);
    }

    if (!prospect) {
        return <p>Loading...</p>;
    }

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <CardTitle>{prospect.name}</CardTitle>
                    <Button variant='outline' asChild>
                        <Link to={`/siteadmin/prospects/${prospectId}/edit`}>
                            <Edit className='mr-2 h-4 w-4' /> Edit
                        </Link>
                    </Button>
                </CardHeader>
                <CardContent>
                    <div className='grid grid-cols-12 gap-4'>
                        <div className='col-span-12 md:col-span-6 space-y-3'>
                            <div className='flex items-center gap-2'>
                                <Badge variant='outline'>{prospect.type}</Badge>
                                <PipelineStageBadge stage={prospect.pipelineStage} />
                                {prospect.fitScore > 0 && <Badge variant='secondary'>Fit: {prospect.fitScore}</Badge>}
                            </div>
                            {prospect.city || prospect.region ? (
                                <div className='flex items-center gap-2 text-muted-foreground'>
                                    <MapPin className='h-4 w-4' />
                                    {[prospect.city, prospect.region, prospect.country].filter(Boolean).join(', ')}
                                </div>
                            ) : null}
                            {prospect.population ? (
                                <p className='text-sm text-muted-foreground'>
                                    Population: {prospect.population.toLocaleString()}
                                </p>
                            ) : null}
                        </div>
                        <div className='col-span-12 md:col-span-6 space-y-3'>
                            {prospect.contactName ? (
                                <div className='flex items-center gap-2 text-sm'>
                                    <User className='h-4 w-4 text-muted-foreground' />
                                    {prospect.contactName}
                                    {prospect.contactTitle ? (
                                        <span className='text-muted-foreground'>({prospect.contactTitle})</span>
                                    ) : null}
                                </div>
                            ) : null}
                            {prospect.contactEmail ? (
                                <div className='flex items-center gap-2 text-sm'>
                                    <Mail className='h-4 w-4 text-muted-foreground' />
                                    <a href={`mailto:${prospect.contactEmail}`} className='hover:underline'>
                                        {prospect.contactEmail}
                                    </a>
                                </div>
                            ) : null}
                            {prospect.website ? (
                                <div className='flex items-center gap-2 text-sm'>
                                    <Globe className='h-4 w-4 text-muted-foreground' />
                                    <a
                                        href={prospect.website}
                                        target='_blank'
                                        rel='noopener noreferrer'
                                        className='hover:underline'
                                    >
                                        {prospect.website}
                                    </a>
                                </div>
                            ) : null}
                        </div>
                        {prospect.notes ? (
                            <div className='col-span-12'>
                                <h4 className='text-sm font-medium mb-1'>Notes</h4>
                                <p className='text-sm text-muted-foreground whitespace-pre-wrap'>{prospect.notes}</p>
                            </div>
                        ) : null}
                    </div>
                </CardContent>
            </Card>

            {scoreBreakdown ? (
                <Card>
                    <CardHeader>
                        <CardTitle>Score Breakdown (FitScore: {scoreBreakdown.totalScore})</CardTitle>
                    </CardHeader>
                    <CardContent className='space-y-4'>
                        <ScoreBar label='Event Density' value={scoreBreakdown.eventDensityScore} max={30} />
                        <ScoreBar label='Population' value={scoreBreakdown.populationScore} max={25} />
                        <ScoreBar label='Geographic Gap' value={scoreBreakdown.geographicGapScore} max={30} />
                        <ScoreBar label='Community Type' value={scoreBreakdown.communityTypeFitScore} max={15} />
                        <div className='grid grid-cols-2 gap-4 pt-2 text-sm text-muted-foreground'>
                            <p>Nearby Events: {scoreBreakdown.nearbyEventCount}</p>
                            <p>
                                Nearest Partner:{' '}
                                {scoreBreakdown.nearestPartnerDistanceMiles !== null
                                    ? `${scoreBreakdown.nearestPartnerDistanceMiles} mi`
                                    : 'None'}
                            </p>
                        </div>
                    </CardContent>
                </Card>
            ) : null}

            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <CardTitle>Activity ({(activities || []).length})</CardTitle>
                    <Dialog open={activityDialogOpen} onOpenChange={setActivityDialogOpen}>
                        <DialogTrigger asChild>
                            <Button variant='outline'>
                                <Plus className='mr-2 h-4 w-4' /> Log Activity
                            </Button>
                        </DialogTrigger>
                        <DialogContent>
                            <DialogHeader>
                                <DialogTitle>Log Activity</DialogTitle>
                            </DialogHeader>
                            <div className='space-y-4'>
                                <div>
                                    <label className='text-sm font-medium'>Type</label>
                                    <Select value={activityType} onValueChange={setActivityType}>
                                        <SelectTrigger>
                                            <SelectValue />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {ACTIVITY_TYPES.map((t) => (
                                                <SelectItem key={t} value={t}>
                                                    {t}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                </div>
                                <div>
                                    <label className='text-sm font-medium'>Subject</label>
                                    <Input
                                        value={activitySubject}
                                        onChange={(e) => setActivitySubject(e.target.value)}
                                        placeholder='Brief subject'
                                    />
                                </div>
                                <div>
                                    <label className='text-sm font-medium'>Details</label>
                                    <Textarea
                                        value={activityDetails}
                                        onChange={(e) => setActivityDetails(e.target.value)}
                                        rows={3}
                                        placeholder='Activity details...'
                                    />
                                </div>
                            </div>
                            <DialogFooter>
                                <Button variant='outline' onClick={() => setActivityDialogOpen(false)}>
                                    Cancel
                                </Button>
                                <Button onClick={handleLogActivity} disabled={createActivity.isPending}>
                                    Save
                                </Button>
                            </DialogFooter>
                        </DialogContent>
                    </Dialog>
                </CardHeader>
                <CardContent>
                    {(activities || []).length === 0 ? (
                        <p className='text-muted-foreground text-sm'>No activities recorded yet.</p>
                    ) : (
                        <div className='space-y-4'>
                            {(activities || []).map((activity) => (
                                <div key={activity.id} className='border-l-2 border-muted pl-4 pb-2'>
                                    <div className='flex items-center gap-2'>
                                        <Badge variant='outline'>{activity.activityType}</Badge>
                                        <span className='text-xs text-muted-foreground'>
                                            {moment(activity.createdDate).format('MMM D, YYYY h:mm A')}
                                        </span>
                                    </div>
                                    {activity.subject ? (
                                        <p className='text-sm font-medium mt-1'>{activity.subject}</p>
                                    ) : null}
                                    {activity.details ? (
                                        <p className='text-sm text-muted-foreground mt-1'>{activity.details}</p>
                                    ) : null}
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

function ScoreBar({ label, value, max }: { label: string; value: number; max: number }) {
    const pct = max > 0 ? (value / max) * 100 : 0;
    return (
        <div className='space-y-1'>
            <div className='flex justify-between text-sm'>
                <span>{label}</span>
                <span className='text-muted-foreground'>
                    {value}/{max}
                </span>
            </div>
            <Progress value={pct} className='h-2' />
        </div>
    );
}
