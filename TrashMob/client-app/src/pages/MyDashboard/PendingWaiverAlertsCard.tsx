import { FC } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link } from 'react-router';
import { AlertTriangle, FileText } from 'lucide-react';
import { format, parseISO } from 'date-fns';

import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { GetPendingWaiverRequests, PendingDependentWaiver } from '@/services/dependents';

interface PendingWaiverAlertsCardProps {
    userId: string;
}

export const PendingWaiverAlertsCard: FC<PendingWaiverAlertsCardProps> = ({ userId }) => {
    const { data: pendingWaivers } = useQuery({
        queryKey: GetPendingWaiverRequests({ userId }).key,
        queryFn: GetPendingWaiverRequests({ userId }).service,
        select: (res) => res.data,
        enabled: !!userId && userId !== '00000000-0000-0000-0000-000000000000',
    });

    if (!pendingWaivers || pendingWaivers.length === 0) return null;

    return (
        <Alert variant='destructive' className='mb-4'>
            <AlertTriangle className='h-4 w-4' />
            <AlertTitle>Waiver Signatures Required</AlertTitle>
            <AlertDescription>
                <p className='mb-3'>
                    Your child{pendingWaivers.length > 1 ? 'ren have' : ' has'} registered for event
                    {pendingWaivers.length > 1 ? 's' : ''} that require your waiver signature:
                </p>
                <div className='space-y-3'>
                    {pendingWaivers.map((pw: PendingDependentWaiver) => (
                        <div key={`${pw.dependentId}-${pw.eventId}`} className='rounded border p-3 bg-background'>
                            <div className='font-medium'>
                                {pw.dependentFirstName} — {pw.eventName}
                            </div>
                            <div className='text-sm text-muted-foreground'>
                                {pw.eventDate ? format(parseISO(pw.eventDate), 'MMM d, yyyy') : 'TBD'}
                            </div>
                            <div className='text-sm mt-1'>
                                {pw.requiredWaivers.length} waiver{pw.requiredWaivers.length > 1 ? 's' : ''} needed:
                                {pw.requiredWaivers.map((w) => (
                                    <span key={w.id} className='inline-flex items-center gap-1 ml-2'>
                                        <FileText className='h-3 w-3' />
                                        {w.name || w.scope}
                                    </span>
                                ))}
                            </div>
                            <Button variant='outline' size='sm' className='mt-2' asChild>
                                <Link to={`/mydashboard#dependents`}>Sign Waivers</Link>
                            </Button>
                        </div>
                    ))}
                </div>
            </AlertDescription>
        </Alert>
    );
};
