import { useState } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Loader2, Calendar, ClipboardCheck } from 'lucide-react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import { useLogin } from '@/hooks/useLogin';
import { useToast } from '@/hooks/use-toast';
import { AgeGateDialog } from '@/components/AgeGate/AgeGateDialog';
import { getApiConfig, getMsalClientInstance } from '@/store/AuthStore';
import { GetMyTeams } from '@/services/teams';
import { SubmitAdoption } from '@/services/team-adoptions';
import { GetAvailableAreas } from '@/services/adoptable-areas';
import AdoptableAreaData from '@/components/Models/AdoptableAreaData';

interface AdoptAreaDialogProps {
    area: AdoptableAreaData;
    communityId: string;
    open: boolean;
    onOpenChange: (open: boolean) => void;
}

export const AdoptAreaDialog = ({ area, communityId, open, onOpenChange }: AdoptAreaDialogProps) => {
    const { currentUser, isUserLoaded } = useLogin();
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [selectedTeamId, setSelectedTeamId] = useState('');
    const [applicationNotes, setApplicationNotes] = useState('');
    const [showAgeGate, setShowAgeGate] = useState(false);

    const isLoggedIn = isUserLoaded && !!currentUser.id;

    const { data: teams = [], isLoading: teamsLoading } = useQuery({
        queryKey: GetMyTeams().key,
        queryFn: GetMyTeams().service,
        select: (res) => res.data,
        enabled: open && isLoggedIn,
    });

    const submitAdoption = useMutation({
        mutationKey: SubmitAdoption().key,
        mutationFn: (vars: { teamId: string; adoptableAreaId: string; applicationNotes?: string }) =>
            SubmitAdoption().service(
                { teamId: vars.teamId },
                {
                    adoptableAreaId: vars.adoptableAreaId,
                    applicationNotes: vars.applicationNotes,
                },
            ),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetAvailableAreas({ partnerId: communityId }).key });
            toast({
                variant: 'primary',
                title: 'Application submitted!',
                description: 'The community will review your request.',
            });
            setSelectedTeamId('');
            setApplicationNotes('');
            onOpenChange(false);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Error', description: 'Failed to submit adoption application.' });
        },
    });

    function handleAgeGateConfirm() {
        setShowAgeGate(false);
        const apiConfig = getApiConfig();
        getMsalClientInstance().loginRedirect({ scopes: apiConfig.scopes });
    }

    function handleSignIn() {
        const accounts = getMsalClientInstance().getAllAccounts();
        if (!accounts || accounts.length === 0) {
            setShowAgeGate(true);
        }
    }

    function handleSubmit() {
        if (!selectedTeamId) return;
        submitAdoption.mutate({
            teamId: selectedTeamId,
            adoptableAreaId: area.id,
            applicationNotes: applicationNotes || undefined,
        });
    }

    // Auto-select team if user has exactly one
    if (teams.length === 1 && !selectedTeamId) {
        setSelectedTeamId(teams[0].id);
    }

    return (
        <>
            <Dialog open={open} onOpenChange={onOpenChange}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Apply to Adopt</DialogTitle>
                        <DialogDescription>Submit an application to adopt {area.name}.</DialogDescription>
                    </DialogHeader>

                    {/* Area summary */}
                    <div className='rounded-lg border p-3 space-y-2'>
                        <div className='flex items-center gap-2'>
                            <span className='font-medium text-sm'>{area.name}</span>
                            <Badge variant='outline'>{area.areaType}</Badge>
                        </div>
                        <div className='flex gap-4 text-xs text-muted-foreground'>
                            <div className='flex items-center gap-1'>
                                <Calendar className='h-3 w-3' />
                                <span>Every {area.cleanupFrequencyDays} days</span>
                            </div>
                            <div className='flex items-center gap-1'>
                                <ClipboardCheck className='h-3 w-3' />
                                <span>Min {area.minEventsPerYear} events/year</span>
                            </div>
                        </div>
                    </div>

                    {!isLoggedIn ? (
                        <div className='py-4 text-center space-y-3'>
                            <p className='text-sm text-muted-foreground'>You need to sign in to apply for adoption.</p>
                            <Button onClick={handleSignIn}>Sign In</Button>
                        </div>
                    ) : teamsLoading ? (
                        <div className='py-4 text-center'>
                            <Loader2 className='h-5 w-5 animate-spin mx-auto' />
                        </div>
                    ) : teams.length === 0 ? (
                        <div className='py-4 text-center space-y-3'>
                            <p className='text-sm text-muted-foreground'>
                                You need to be part of a team to adopt an area.
                            </p>
                            <Button asChild variant='outline'>
                                <Link to='/teams'>Find or Create a Team</Link>
                            </Button>
                        </div>
                    ) : (
                        <div className='space-y-4 py-2'>
                            <div className='space-y-2'>
                                <Label htmlFor='team-select'>Team</Label>
                                <Select value={selectedTeamId} onValueChange={setSelectedTeamId}>
                                    <SelectTrigger id='team-select'>
                                        <SelectValue placeholder='Select a team' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {teams.map((team) => (
                                            <SelectItem key={team.id} value={team.id}>
                                                {team.name}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                            <div className='space-y-2'>
                                <Label htmlFor='application-notes'>Application Notes (optional)</Label>
                                <Textarea
                                    id='application-notes'
                                    value={applicationNotes}
                                    onChange={(e) => setApplicationNotes(e.target.value)}
                                    placeholder='Tell the community why your team wants to adopt this area...'
                                    rows={3}
                                />
                            </div>
                        </div>
                    )}

                    <DialogFooter>
                        <Button variant='outline' onClick={() => onOpenChange(false)}>
                            Cancel
                        </Button>
                        {isLoggedIn && teams.length > 0 ? (
                            <Button onClick={handleSubmit} disabled={!selectedTeamId || submitAdoption.isPending}>
                                {submitAdoption.isPending ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                                Submit Application
                            </Button>
                        ) : null}
                    </DialogFooter>
                </DialogContent>
            </Dialog>

            <AgeGateDialog open={showAgeGate} onOpenChange={setShowAgeGate} onConfirm={handleAgeGateConfirm} />
        </>
    );
};
