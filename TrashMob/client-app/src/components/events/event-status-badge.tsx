import { Badge } from '@/components/ui/badge';
import { EventStatus } from '@/enums/EventStatus';

interface EventStatusBadgeProps {
    readonly statusId: number;
}

export const EventStatusBadge = ({ statusId }: EventStatusBadgeProps) => {
    switch (statusId) {
        case EventStatus.Active:
            return <Badge variant='default'>Active</Badge>;
        case EventStatus.Full:
            return <Badge variant='success'>Full</Badge>;
        case EventStatus.Completed:
            return <Badge variant='success'>Completed</Badge>;
        case EventStatus.Canceled:
            return (
                <Badge className='bg-destructive/75' variant='destructive'>
                    Canceled
                </Badge>
            );
    }
};
