import { Link } from 'react-router';
import { MapPin, Calendar, Eye } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import LitterReportData from '@/components/Models/LitterReportData';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';

interface LitterReportInfoWindowProps {
    report: LitterReportData;
}

const formatDate = (date: Date | null) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
};

export const LitterReportInfoWindowHeader = ({ name }: { name: string }) => {
    return <span className='font-semibold text-sm'>{name || 'Litter Report'}</span>;
};

export const LitterReportInfoWindowContent = ({ report }: LitterReportInfoWindowProps) => {
    const statusId = report.litterReportStatusId as LitterReportStatusEnum;
    const statusLabel = LitterReportStatusLabels[statusId] || 'Unknown';
    const statusColor = LitterReportStatusColors[statusId] || 'bg-gray-500';
    const firstImage = report.litterImages?.[0];

    const location = firstImage ? [firstImage.city, firstImage.region].filter(Boolean).join(', ') : null;

    return (
        <div className='flex flex-col gap-2 min-w-[200px] max-w-[280px]'>
            <div className='flex items-center justify-between'>
                <Badge variant='outline' className={`${statusColor} text-white border-0 text-xs`}>
                    {statusLabel}
                </Badge>
                {report.litterImages?.length ? (
                    <span className='text-xs text-muted-foreground'>{report.litterImages.length} photo(s)</span>
                ) : null}
            </div>

            {report.description ? (
                <p className='text-sm text-muted-foreground line-clamp-2'>{report.description}</p>
            ) : null}

            {location ? (
                <div className='flex items-center gap-1 text-xs text-muted-foreground'>
                    <MapPin className='h-3 w-3' />
                    <span>{location}</span>
                </div>
            ) : null}

            <div className='flex items-center gap-1 text-xs text-muted-foreground'>
                <Calendar className='h-3 w-3' />
                <span>Reported {formatDate(report.createdDate)}</span>
            </div>

            <Button variant='outline' size='sm' className='w-full mt-1' asChild>
                <Link to={`/litterreports/${report.id}`}>
                    <Eye className='h-3 w-3 mr-1' /> View Details
                </Link>
            </Button>
        </div>
    );
};
