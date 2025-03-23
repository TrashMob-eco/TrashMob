import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { getColumns } from './columns';
import {
    ApprovePartnerRequest,
    DenyPartnerRequest,
    GetPartnerRequests,
    GetPartnerRequestStatuses,
} from '@/services/partners';
import { useToast } from '@/hooks/use-toast';

export const SiteAdminPartnerRequests = () => {
    const queryClient = useQueryClient();
    const { toast } = useToast();

    const { data: statuses } = useQuery({
        queryKey: GetPartnerRequestStatuses().key,
        queryFn: GetPartnerRequestStatuses().service,
    });

    const { data: partnerRequests } = useQuery({
        queryKey: GetPartnerRequests().key,
        queryFn: GetPartnerRequests().service,
        select: (res) => res.data,
    });

    const approvePartnerRequest = useMutation({
        mutationKey: ApprovePartnerRequest().key,
        mutationFn: ApprovePartnerRequest().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetPartnerRequests().key,
                refetchType: 'all',
            });
            toast({
                variant: 'primary',
                title: 'Partner request approved',
            });
        },
    });

    const denyPartnerRequest = useMutation({
        mutationKey: DenyPartnerRequest().key,
        mutationFn: DenyPartnerRequest().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetPartnerRequests().key,
                refetchType: 'all',
            });
            toast({
                variant: 'primary',
                title: 'Partner request denied',
            });
        },
    });

    // Handle approve request for a partner
    function handleApprove(id: string, name: string) {
        if (!window.confirm(`Do you want to approve partner with name: ${name}`)) return;
        approvePartnerRequest.mutateAsync({ id });
    }

    // Handle approve request for a partner
    function handleDeny(id: string, name: string) {
        if (!window.confirm(`Do you want to deny partner with name: ${name}`)) return;
        denyPartnerRequest.mutateAsync({ id });
    }

    const columns = getColumns({ onApprove: handleApprove, onDeny: handleDeny });

    const len = (partnerRequests || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Partner Requests ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={partnerRequests || []} />
            </CardContent>
        </Card>
    );
};
