import { Badge } from '@/components/ui/badge';
import { useGetPartnerRequestStatuses } from '@/hooks/useGetPartnerRequestStatuses';
import { useMemo } from 'react';

interface PartnerRequestStatusBadgeProps {
    readonly statusId: number;
}

export const PartnerRequestStatusBadge = ({ statusId }: PartnerRequestStatusBadgeProps) => {
    const { data: statuses } = useGetPartnerRequestStatuses();
    const status = useMemo(() => statuses?.find((st) => st.id === statusId), [statuses]);
    if (!status) return null;

    switch (status.name) {
        case 'Approved':
            return <Badge variant='success'>{status.name}</Badge>;
        case 'Denied':
            return <Badge variant='destructive'>{status.name}</Badge>;
        default:
            return <Badge variant='secondary'>{status.name}</Badge>;
    }
};
