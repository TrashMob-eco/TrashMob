import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, ExternalLink, SquareX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import PartnerData from '@/components/Models/PartnerData';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
}

const partnerTypeLabels: Record<number, string> = {
    1: 'Government',
    2: 'Business',
    3: 'Community',
};

const partnerStatusLabels: Record<number, { label: string; variant: 'default' | 'secondary' | 'destructive' | 'outline' | 'success' }> = {
    1: { label: 'Active', variant: 'success' },
    2: { label: 'Inactive', variant: 'secondary' },
    3: { label: 'Pending', variant: 'outline' },
};

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<PartnerData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
    },
    {
        accessorKey: 'partnerTypeId',
        header: 'Type',
        cell: ({ row }) => (
            <Badge variant='outline'>{partnerTypeLabels[row.getValue('partnerTypeId') as number] ?? 'Unknown'}</Badge>
        ),
    },
    {
        accessorKey: 'partnerStatusId',
        header: 'Status',
        cell: ({ row }) => {
            const status = partnerStatusLabels[row.getValue('partnerStatusId') as number];
            return <Badge variant={status?.variant ?? 'outline'}>{status?.label ?? 'Unknown'}</Badge>;
        },
    },
    {
        accessorKey: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const partner = row.original;
            return (
                <div className='text-right'>
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant='ghost' size='icon'>
                                <Ellipsis />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent className='w-56'>
                            <DropdownMenuItem asChild>
                                <Link to={`/partnerdashboard/${partner.id}`}>
                                    <ExternalLink />
                                    View Dashboard
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => onDelete(partner.id, partner.name)}>
                                <SquareX />
                                Delete partner
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
