import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Eye, Pencil, Trash2 } from 'lucide-react';
import { LitterReportStatusBadge } from '@/components/litter-reports/litter-report-status-badge';
import LitterReportData from '@/components/Models/LitterReportData';
import moment from 'moment';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';

export const columns: ColumnDef<LitterReportData>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => {
            const name = row.getValue('name') as string;
            return <span className='font-medium'>{name || 'Unnamed Report'}</span>;
        },
    },
    {
        accessorKey: 'litterReportStatusId',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) => {
            return <LitterReportStatusBadge statusId={row.getValue('litterReportStatusId')} />;
        },
    },
    {
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Created' />,
        cell: ({ row }) => {
            const date = row.getValue('createdDate');
            return date ? moment(date as Date).format('lll') : '-';
        },
    },
    {
        accessorKey: 'createdByUserName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Created By' />,
        cell: ({ row }) => {
            return row.getValue('createdByUserName') || '-';
        },
    },
    {
        accessorKey: 'litterImages',
        header: 'Images',
        cell: ({ row }) => {
            const images = row.original.litterImages || [];
            return images.length;
        },
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const litterReport = row.original;

            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem asChild>
                            <Link to={`/litterreports/${litterReport.id}`}>
                                <Eye />
                                View Report
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to={`/litterreports/${litterReport.id}/edit`}>
                                <Pencil />
                                Edit Report
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to={`/litterreports/${litterReport.id}/delete`}>
                                <Trash2 />
                                Delete Report
                            </Link>
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
