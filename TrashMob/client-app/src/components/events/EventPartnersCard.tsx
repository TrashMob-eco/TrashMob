import { useQuery } from '@tanstack/react-query';
import { GetEventPartnerLocationServices } from '@/services/locations';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Building2, CheckCircle, Clock, XCircle } from 'lucide-react';

interface EventPartnersCardProps {
    eventId: string;
}

// Status enum values
const PartnerServiceStatus = {
    None: 0,
    Requested: 1,
    Accepted: 2,
    Declined: 3,
} as const;

function getStatusBadge(statusId: number) {
    switch (statusId) {
        case PartnerServiceStatus.Accepted:
            return (
                <Badge variant='default' className='bg-green-600'>
                    <CheckCircle className='h-3 w-3 mr-1' />
                    Confirmed
                </Badge>
            );
        case PartnerServiceStatus.Requested:
            return (
                <Badge variant='secondary'>
                    <Clock className='h-3 w-3 mr-1' />
                    Pending
                </Badge>
            );
        case PartnerServiceStatus.Declined:
            return (
                <Badge variant='destructive'>
                    <XCircle className='h-3 w-3 mr-1' />
                    Declined
                </Badge>
            );
        default:
            return null;
    }
}

export function EventPartnersCard({ eventId }: EventPartnersCardProps) {
    const { data: eventPartners, isLoading } = useQuery({
        queryKey: GetEventPartnerLocationServices({ eventId }).key,
        queryFn: GetEventPartnerLocationServices({ eventId }).service,
        select: (res) => res.data,
        enabled: !!eventId,
    });

    // Filter to only show accepted or requested partners (not declined)
    const visiblePartners = (eventPartners || []).filter(
        (p) =>
            p.eventPartnerLocationStatusId === PartnerServiceStatus.Accepted ||
            p.eventPartnerLocationStatusId === PartnerServiceStatus.Requested,
    );

    if (isLoading) {
        return null;
    }

    if (!visiblePartners || visiblePartners.length === 0) {
        return null;
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle className='flex items-center gap-2'>
                    <Building2 className='h-5 w-5' />
                    Event Partners
                </CardTitle>
            </CardHeader>
            <CardContent>
                <div className='space-y-4'>
                    {visiblePartners.map((partner) => (
                        <div key={`${partner.partnerLocationId}`} className='border rounded-lg p-4 bg-muted/50'>
                            <div className='flex items-start justify-between gap-4'>
                                <div className='flex-1'>
                                    <h4 className='font-semibold text-lg'>{partner.partnerName}</h4>
                                    {partner.partnerLocationName ? (
                                        <p className='text-sm text-muted-foreground'>{partner.partnerLocationName}</p>
                                    ) : null}
                                </div>
                                {getStatusBadge(partner.eventPartnerLocationStatusId)}
                            </div>

                            {partner.partnerServicesEngaged ? (
                                <div className='mt-3'>
                                    <p className='text-sm font-medium'>Services:</p>
                                    <p className='text-sm text-muted-foreground'>{partner.partnerServicesEngaged}</p>
                                </div>
                            ) : null}

                            {partner.partnerLocationNotes ? (
                                <div className='mt-3'>
                                    <p className='text-sm font-medium'>Notes:</p>
                                    <p className='text-sm text-muted-foreground'>{partner.partnerLocationNotes}</p>
                                </div>
                            ) : null}
                        </div>
                    ))}
                </div>
            </CardContent>
        </Card>
    );
}
