import { Link, useNavigate, useParams } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import moment from 'moment';
import { ArrowLeft, Check, Globe, Mail, MapPin, Phone, X } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/hooks/use-toast';
import { ApprovePartnerRequest, DenyPartnerRequest, GetPartnerRequestById, GetPartnerRequests } from '@/services/partners';
import { PartnerRequestStatusBadge } from '@/components/partner-requests/partner-request-status-badge';

export const SiteAdminPartnerRequestDetail = () => {
    const { requestId } = useParams<{ requestId: string }>() as { requestId: string };
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const { data: request } = useQuery({
        queryKey: GetPartnerRequestById({ id: requestId }).key,
        queryFn: GetPartnerRequestById({ id: requestId }).service,
        select: (res) => res.data,
        enabled: !!requestId,
    });

    const approveMutation = useMutation({
        mutationKey: ApprovePartnerRequest().key,
        mutationFn: ApprovePartnerRequest().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetPartnerRequests().key, refetchType: 'all' });
            queryClient.invalidateQueries({ queryKey: GetPartnerRequestById({ id: requestId }).key, refetchType: 'all' });
            toast({ variant: 'primary', title: 'Partner request approved' });
            navigate('/siteadmin/partner-requests');
        },
    });

    const denyMutation = useMutation({
        mutationKey: DenyPartnerRequest().key,
        mutationFn: DenyPartnerRequest().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetPartnerRequests().key, refetchType: 'all' });
            queryClient.invalidateQueries({ queryKey: GetPartnerRequestById({ id: requestId }).key, refetchType: 'all' });
            toast({ variant: 'primary', title: 'Partner request denied' });
            navigate('/siteadmin/partner-requests');
        },
    });

    function handleApprove() {
        if (!window.confirm(`Do you want to approve partner request: ${request?.name}?`)) return;
        approveMutation.mutate({ id: requestId });
    }

    function handleDeny() {
        if (!window.confirm(`Do you want to deny partner request: ${request?.name}?`)) return;
        denyMutation.mutate({ id: requestId });
    }

    if (!request) {
        return <p>Loading...</p>;
    }

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div className='flex items-center gap-3'>
                        <Button variant='ghost' size='icon' asChild>
                            <Link to='/siteadmin/partner-requests'>
                                <ArrowLeft className='h-4 w-4' />
                            </Link>
                        </Button>
                        <CardTitle>{request.name}</CardTitle>
                        <PartnerRequestStatusBadge statusId={request.partnerRequestStatusId} />
                    </div>
                    <div className='flex items-center gap-2'>
                        <Button variant='outline' onClick={handleApprove} disabled={approveMutation.isPending}>
                            <Check className='mr-1 h-4 w-4' /> Approve
                        </Button>
                        <Button variant='destructive' onClick={handleDeny} disabled={denyMutation.isPending}>
                            <X className='mr-1 h-4 w-4' /> Deny
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    <div className='grid grid-cols-12 gap-4'>
                        <div className='col-span-12 md:col-span-6 space-y-3'>
                            <div className='flex items-center gap-2'>
                                <Mail className='h-4 w-4 text-muted-foreground' />
                                <span>{request.email || '—'}</span>
                            </div>
                            {request.phone ? (
                                <div className='flex items-center gap-2'>
                                    <Phone className='h-4 w-4 text-muted-foreground' />
                                    <span>{request.phone}</span>
                                </div>
                            ) : null}
                            {request.website ? (
                                <div className='flex items-center gap-2'>
                                    <Globe className='h-4 w-4 text-muted-foreground' />
                                    <a href={request.website} target='_blank' rel='noopener noreferrer' className='hover:underline'>
                                        {request.website}
                                    </a>
                                </div>
                            ) : null}
                            <div className='flex items-center gap-2'>
                                <MapPin className='h-4 w-4 text-muted-foreground' />
                                <span>
                                    {[request.streetAddress, request.city, request.region, request.country]
                                        .filter(Boolean)
                                        .join(', ') || '—'}
                                </span>
                            </div>
                        </div>
                        <div className='col-span-12 md:col-span-6 space-y-3'>
                            <div>
                                <h4 className='text-sm font-medium text-muted-foreground'>Become a Partner Request</h4>
                                <p>{request.isBecomeAPartnerRequest ? 'Yes' : 'No'}</p>
                            </div>
                            <div>
                                <h4 className='text-sm font-medium text-muted-foreground'>Created</h4>
                                <p>{moment(request.createdDate).format('MMM D, YYYY h:mm A')}</p>
                            </div>
                        </div>
                        {request.notes ? (
                            <div className='col-span-12'>
                                <h4 className='text-sm font-medium text-muted-foreground'>Notes</h4>
                                <p className='text-sm whitespace-pre-wrap'>{request.notes}</p>
                            </div>
                        ) : null}
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};
