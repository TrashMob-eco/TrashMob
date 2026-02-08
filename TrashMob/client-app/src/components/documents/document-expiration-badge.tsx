import { Badge } from '@/components/ui/badge';

interface DocumentExpirationBadgeProps {
    expirationDate: Date | string | null;
}

export const DocumentExpirationBadge = ({ expirationDate }: DocumentExpirationBadgeProps) => {
    if (!expirationDate) return null;

    const expDate = new Date(expirationDate);
    const now = new Date();
    const daysUntil = Math.ceil((expDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));

    if (daysUntil < 0) {
        return <Badge variant='destructive'>Expired</Badge>;
    }
    if (daysUntil <= 30) {
        return (
            <Badge variant='default' className='bg-yellow-500'>
                Expiring Soon
            </Badge>
        );
    }
    return <Badge variant='success'>Active</Badge>;
};
