import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Eye } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import PartnerRequestData from '@/components/Models/PartnerRequestData';
import { PartnerRequestStatusBadge } from '@/components/partner-requests/partner-request-status-badge';

export const columns: ColumnDef<PartnerRequestData>[] = [
    {
        accessorKey: 'name',
        header: 'Name',
    },
    {
        accessorKey: 'email',
        header: 'Email',
    },
    {
        accessorKey: 'partnerRequestStatusId',
        header: 'Status',
        cell: ({ row }) => <PartnerRequestStatusBadge statusId={row.getValue('partnerRequestStatusId')} />,
    },
    {
        id: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const partnerRequest = row.original;
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
                                <Link to={`${partnerRequest.id}`}>
                                    <Eye />
                                    View Details
                                </Link>
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
