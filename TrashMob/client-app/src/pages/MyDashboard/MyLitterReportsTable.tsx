import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { MapPin, Calendar, Eye } from 'lucide-react';

import { DataTable, DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import LitterReportData from '@/components/Models/LitterReportData';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';

const formatDate = (date: Date | null) => {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
};

const getLocation = (report: LitterReportData) => {
    const firstImage = report.litterImages?.[0];
    if (!firstImage) return '-';
    const parts = [firstImage.city, firstImage.region].filter(Boolean);
    return parts.join(', ') || '-';
};

const columns: ColumnDef<LitterReportData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => (
            <Link to={`/litterreports/${row.original.id}`} className='text-primary hover:underline font-medium'>
                {row.original.name || 'Untitled Report'}
            </Link>
        ),
    },
    {
        id: 'location',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Location' />,
        cell: ({ row }) => (
            <div className='flex items-center gap-1'>
                <MapPin className='h-4 w-4 text-muted-foreground' />
                <span>{getLocation(row.original)}</span>
            </div>
        ),
    },
    {
        accessorKey: 'litterReportStatusId',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) => {
            const statusId = row.original.litterReportStatusId as LitterReportStatusEnum;
            const label = LitterReportStatusLabels[statusId] || 'Unknown';
            const colorClass = LitterReportStatusColors[statusId] || 'bg-gray-500';
            return (
                <Badge variant='outline' className={`${colorClass} text-white border-0`}>
                    {label}
                </Badge>
            );
        },
    },
    {
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Reported' />,
        cell: ({ row }) => (
            <div className='flex items-center gap-1'>
                <Calendar className='h-4 w-4 text-muted-foreground' />
                <span>{formatDate(row.original.createdDate)}</span>
            </div>
        ),
    },
    {
        id: 'actions',
        header: '',
        cell: ({ row }) => (
            <Button variant='ghost' size='sm' asChild>
                <Link to={`/litterreports/${row.original.id}`}>
                    <Eye className='h-4 w-4 mr-1' /> View
                </Link>
            </Button>
        ),
    },
];

interface MyLitterReportsTableProps {
    items: LitterReportData[];
}

export const MyLitterReportsTable = ({ items }: MyLitterReportsTableProps) => {
    if (items.length === 0) {
        return <p className='text-muted-foreground text-center py-4'>You haven't submitted any litter reports yet.</p>;
    }

    return <DataTable columns={columns} data={items} />;
};
