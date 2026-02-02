import { FC, useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { format, differenceInDays, parseISO } from 'date-fns';
import { Download, AlertTriangle, CheckCircle, FileText } from 'lucide-react';
import { AxiosResponse } from 'axios';

import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { UserWaiverData, WaiverVersionData } from '@/components/Models/WaiverVersionData';
import { GetMyWaivers, GetRequiredWaivers, getWaiverDownloadUrl } from '@/services/user-waivers';
import { WaiverSigningFlow } from '@/components/Waivers';

interface MyWaiversCardProps {
    userId: string;
}

export const MyWaiversCard: FC<MyWaiversCardProps> = ({ userId }) => {
    const queryClient = useQueryClient();
    const [showWaiverFlow, setShowWaiverFlow] = useState(false);

    // Get user's signed waivers
    const { data: myWaivers } = useQuery<AxiosResponse<UserWaiverData[]>, unknown, UserWaiverData[]>({
        queryKey: GetMyWaivers().key,
        queryFn: GetMyWaivers().service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    // Get pending waivers
    const { data: pendingWaivers, refetch: refetchPending } = useQuery<
        AxiosResponse<WaiverVersionData[]>,
        unknown,
        WaiverVersionData[]
    >({
        queryKey: GetRequiredWaivers().key,
        queryFn: GetRequiredWaivers().service,
        select: (res) => res.data,
        enabled: !!userId,
    });

    const handleDownloadPdf = (userWaiverId: string) => {
        const url = getWaiverDownloadUrl(userWaiverId);
        window.open(url, '_blank');
    };

    const handleWaiverFlowComplete = (allSigned: boolean) => {
        setShowWaiverFlow(false);
        if (allSigned) {
            // Refresh both lists
            queryClient.invalidateQueries({ queryKey: GetMyWaivers().key });
            refetchPending();
        }
    };

    const getExpiryStatus = (expiryDate: string) => {
        const days = differenceInDays(parseISO(expiryDate), new Date());
        if (days < 0) return { status: 'expired', label: 'Expired', variant: 'destructive' as const };
        if (days <= 30) return { status: 'expiring', label: `Expires in ${days} days`, variant: 'warning' as const };
        return { status: 'valid', label: 'Valid', variant: 'success' as const };
    };

    const hasPendingWaivers = pendingWaivers && pendingWaivers.length > 0;
    const waiverCount = (myWaivers || []).length;

    return (
        <>
            <Card className='mb-4'>
                <CardHeader>
                    <div className='flex flex-row items-center'>
                        <FileText className='inline-block h-5 w-5 mr-2 text-primary' />
                        <CardTitle className='grow text-primary'>My Waivers ({waiverCount})</CardTitle>
                        {hasPendingWaivers ? <Button variant='outline' size='sm' onClick={() => setShowWaiverFlow(true)}>
                                <AlertTriangle className='h-4 w-4 mr-1 text-amber-500' />
                                Sign Required Waivers ({pendingWaivers.length})
                            </Button> : null}
                    </div>
                </CardHeader>
                <CardContent>
                    {waiverCount === 0 ? (
                        <p className='text-muted-foreground text-center py-4'>
                            No signed waivers yet. Waivers will appear here after you register for an event.
                        </p>
                    ) : (
                        <div className='overflow-auto'>
                            <table className='w-full'>
                                <thead>
                                    <tr className='border-b'>
                                        <th className='text-left py-2 px-2 font-medium'>Waiver</th>
                                        <th className='text-left py-2 px-2 font-medium'>Signed</th>
                                        <th className='text-left py-2 px-2 font-medium'>Expires</th>
                                        <th className='text-left py-2 px-2 font-medium'>Status</th>
                                        <th className='text-right py-2 px-2 font-medium'>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {(myWaivers || []).map((waiver) => {
                                        const expiry = getExpiryStatus(waiver.expiryDate);
                                        return (
                                            <tr key={waiver.id} className='border-b last:border-0'>
                                                <td className='py-3 px-2'>
                                                    <div className='font-medium'>
                                                        {waiver.waiverVersion?.name || 'Waiver'}
                                                    </div>
                                                    <div className='text-sm text-muted-foreground'>
                                                        v{waiver.waiverVersion?.version || '1.0'}
                                                    </div>
                                                </td>
                                                <td className='py-3 px-2 text-sm'>
                                                    {format(parseISO(waiver.acceptedDate), 'MMM d, yyyy')}
                                                </td>
                                                <td className='py-3 px-2 text-sm'>
                                                    {format(parseISO(waiver.expiryDate), 'MMM d, yyyy')}
                                                </td>
                                                <td className='py-3 px-2'>
                                                    <Badge
                                                        variant={
                                                            expiry.status === 'valid'
                                                                ? 'default'
                                                                : expiry.status === 'expiring'
                                                                  ? 'secondary'
                                                                  : 'destructive'
                                                        }
                                                        className='flex items-center w-fit gap-1'
                                                    >
                                                        {expiry.status === 'valid' ? (
                                                            <CheckCircle className='h-3 w-3' />
                                                        ) : (
                                                            <AlertTriangle className='h-3 w-3' />
                                                        )}
                                                        {expiry.label}
                                                    </Badge>
                                                </td>
                                                <td className='py-3 px-2 text-right'>
                                                    <Button
                                                        variant='ghost'
                                                        size='sm'
                                                        onClick={() => handleDownloadPdf(waiver.id)}
                                                        title='Download PDF'
                                                    >
                                                        <Download className='h-4 w-4' />
                                                    </Button>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    )}
                </CardContent>
            </Card>

            {pendingWaivers ? <WaiverSigningFlow
                    waivers={pendingWaivers}
                    open={showWaiverFlow}
                    onComplete={handleWaiverFlowComplete}
                /> : null}
        </>
    );
};
