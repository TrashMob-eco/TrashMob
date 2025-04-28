import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Check, Ellipsis, SquareCheck, SquareX, X } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import PartnerRequestData from '@/components/Models/PartnerRequestData';
import { PartnerRequestStatusBadge } from '@/components/partner-requests/partner-request-status-badge';

interface GetColumnsProps {
    onApprove: (id: string, name: string) => void;
    onDeny: (id: string, name: string) => void;
}

export const getColumns = ({ onApprove, onDeny }: GetColumnsProps): ColumnDef<PartnerRequestData>[] => [
    {
        accessorKey: 'name',
        header: 'Name',
    },
    {
        accessorKey: 'email',
        header: 'Email',
    },
    {
        accessorKey: 'phone',
        header: 'Phone',
    },
    {
        accessorKey: 'website',
        header: 'Website',
        cell: ({ row }) => (
            <a href={row.getValue('website')} rel='noreferrer' target='_blank'>
                {row.getValue('website')}
            </a>
        ),
    },
    {
        accessorKey: 'city',
        header: 'City',
    },
    {
        accessorKey: 'region',
        header: 'Region',
    },
    {
        accessorKey: 'country',
        header: 'Country',
    },
    {
        accessorKey: 'partnerRequestStatusId',
        header: 'Request Status',
        cell: ({ row }) => <PartnerRequestStatusBadge statusId={row.getValue('partnerRequestStatusId')} />,
    },
    {
        accessorKey: 'isBecomeAPartnerRequest',
        header: 'Is Become Partner Request',
        cell: ({ row }) => {
            return row.getValue('isBecomeAPartnerRequest') ? (
                <Check className='text-primary' />
            ) : (
                <X className='text-destructive' />
            );
        },
    },
    {
        accessorKey: 'notes',
        header: 'Notes',
    },
    {
        accessorKey: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const partnerRequest = row.original;
            return (
                <div className='text-right'>
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button size='icon' variant='ghost'>
                                <Ellipsis />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent className='w-56'>
                            <DropdownMenuItem onClick={() => onApprove(partnerRequest.id, partnerRequest.name)}>
                                <SquareCheck />
                                Approve request
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={() => onDeny(partnerRequest.id, partnerRequest.name)}>
                                <SquareX />
                                Deny request
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
