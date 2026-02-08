import { useQuery } from '@tanstack/react-query';
import { useParams, Link } from 'react-router';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { ArrowLeft, CheckCircle, XCircle, Clock, Mail } from 'lucide-react';
import { GetCommunityInviteBatchDetails } from '@/services/email-invites';

export const PartnerCommunityInviteDetails = () => {
    const { partnerId, batchId } = useParams<{ partnerId: string; batchId: string }>();

    const { data: batch, isLoading } = useQuery({
        queryKey: GetCommunityInviteBatchDetails({ communityId: partnerId!, id: batchId! }).key,
        queryFn: GetCommunityInviteBatchDetails({ communityId: partnerId!, id: batchId! }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!batchId,
    });

    if (isLoading) {
        return <div className='py-4'>Loading...</div>;
    }

    if (!batch) {
        return <div className='py-4'>Batch not found</div>;
    }

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'Sent':
                return (
                    <Badge variant='success' className='gap-1'>
                        <CheckCircle className='h-3 w-3' />
                        Sent
                    </Badge>
                );
            case 'Failed':
                return (
                    <Badge variant='destructive' className='gap-1'>
                        <XCircle className='h-3 w-3' />
                        Failed
                    </Badge>
                );
            case 'Pending':
            default:
                return (
                    <Badge variant='outline' className='gap-1'>
                        <Clock className='h-3 w-3' />
                        Pending
                    </Badge>
                );
        }
    };

    return (
        <div className='py-6 space-y-6'>
            <div className='flex items-center gap-4'>
                <Button variant='ghost' size='icon' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/invites`}>
                        <ArrowLeft className='h-4 w-4' />
                    </Link>
                </Button>
                <h1 className='text-2xl font-bold'>Batch Details</h1>
            </div>

            {/* Summary Card */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Mail className='h-5 w-5' />
                        Batch Summary
                    </CardTitle>
                    <CardDescription>Created on {new Date(batch.createdDate).toLocaleString()}</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid gap-4 md:grid-cols-5'>
                        <div>
                            <p className='text-sm text-muted-foreground'>Total</p>
                            <p className='text-2xl font-bold'>{batch.totalCount}</p>
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Sent</p>
                            <p className='text-2xl font-bold text-green-600'>{batch.sentCount}</p>
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Failed</p>
                            <p className='text-2xl font-bold text-red-600'>{batch.failedCount}</p>
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Status</p>
                            <p className='mt-1'>{getStatusBadge(batch.status)}</p>
                        </div>
                        <div>
                            <p className='text-sm text-muted-foreground'>Sent By</p>
                            <p className='text-sm font-medium'>{batch.senderUser?.userName || 'Unknown'}</p>
                        </div>
                    </div>
                </CardContent>
            </Card>

            {/* Individual Invites */}
            <Card>
                <CardHeader>
                    <CardTitle>Individual Invites</CardTitle>
                    <CardDescription>{batch.invites?.length || 0} email addresses in this batch</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Email</TableHead>
                                <TableHead>Status</TableHead>
                                <TableHead>Sent Date</TableHead>
                                <TableHead>Error</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {(batch.invites || []).map((invite) => (
                                <TableRow key={invite.id}>
                                    <TableCell className='font-mono text-sm'>{invite.email}</TableCell>
                                    <TableCell>{getStatusBadge(invite.status)}</TableCell>
                                    <TableCell>
                                        {invite.sentDate ? new Date(invite.sentDate).toLocaleString() : '-'}
                                    </TableCell>
                                    <TableCell className='max-w-xs truncate text-sm text-red-600'>
                                        {invite.errorMessage || '-'}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunityInviteDetails;
