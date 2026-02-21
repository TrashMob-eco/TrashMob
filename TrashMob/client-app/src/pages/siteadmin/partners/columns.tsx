import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, ExternalLink, SquareX, Check, X } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import PartnerData from '@/components/Models/PartnerData';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
    onToggleHomePage: (partner: PartnerData) => void;
}

const partnerTypeLabels: Record<number, string> = {
    1: 'Government',
    2: 'Business',
    3: 'Community',
};

const partnerStatusLabels: Record<
    number,
    { label: string; variant: 'default' | 'secondary' | 'destructive' | 'outline' | 'success' }
> = {
    1: { label: 'Active', variant: 'success' },
    2: { label: 'Inactive', variant: 'secondary' },
    3: { label: 'Pending', variant: 'outline' },
};

export const getColumns = ({ onDelete, onToggleHomePage }: GetColumnsProps): ColumnDef<PartnerData>[] => [
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
        accessorKey: 'slug',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Slug' />,
        cell: ({ row }) => {
            const slug = row.original.slug;
            return slug ? (
                <code className='text-xs bg-muted px-1.5 py-0.5 rounded'>{slug}</code>
            ) : (
                <span className='text-xs text-muted-foreground'>—</span>
            );
        },
    },
    {
        accessorKey: 'homePageEnabled',
        header: 'Community Page',
        cell: ({ row }) => {
            const enabled = row.original.homePageEnabled;
            return enabled ? (
                <Badge variant='success'>
                    <Check className='h-3 w-3 mr-1' /> Enabled
                </Badge>
            ) : (
                <Badge variant='secondary'>
                    <X className='h-3 w-3 mr-1' /> Disabled
                </Badge>
            );
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
                            <DropdownMenuSeparator />
                            <DropdownMenuItem onClick={() => onToggleHomePage(partner)}>
                                {partner.homePageEnabled ? <X /> : <Check />}
                                {partner.homePageEnabled ? 'Disable Community Page' : 'Enable Community Page'}
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
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
