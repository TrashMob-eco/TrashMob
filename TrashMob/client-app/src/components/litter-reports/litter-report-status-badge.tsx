import { Badge } from '@/components/ui/badge';
import { LitterReportStatusEnum } from '@/components/Models/LitterReportStatus';

interface LitterReportStatusBadgeProps {
    statusId: number;
}

export const LitterReportStatusBadge = ({ statusId }: LitterReportStatusBadgeProps) => {
    switch (statusId) {
        case LitterReportStatusEnum.New:
            return <Badge variant='destructive'>New</Badge>;
        case LitterReportStatusEnum.Assigned:
            return (
                <Badge variant='default' className='bg-yellow-500'>
                    Assigned
                </Badge>
            );
        case LitterReportStatusEnum.Cleaned:
            return <Badge variant='success'>Cleaned</Badge>;
        case LitterReportStatusEnum.Cancelled:
            return (
                <Badge variant='secondary' className='bg-gray-500 text-white'>
                    Cancelled
                </Badge>
            );
        default:
            return <Badge variant='outline'>Unknown</Badge>;
    }
};
