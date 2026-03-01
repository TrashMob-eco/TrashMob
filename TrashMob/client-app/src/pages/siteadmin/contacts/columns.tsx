import { ColumnDef } from '@tanstack/react-table';
import { Link } from 'react-router';
import { Edit, Ellipsis, Eye, SquareX } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import ContactData from '@/components/Models/ContactData';
import { ContactTypeBadge } from '@/components/contacts/contact-constants';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<ContactData>[] => [
    {
        accessorKey: 'lastName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => {
            const contact = row.original;
            const name = [contact.firstName, contact.lastName].filter(Boolean).join(' ');
            return (
                <Link to={`/siteadmin/contacts/${contact.id}`} className='font-medium hover:underline'>
                    {name || '—'}
                </Link>
            );
        },
    },
    {
        accessorKey: 'email',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Email' />,
        cell: ({ row }) => row.original.email || '—',
    },
    {
        accessorKey: 'organizationName',
        header: 'Organization',
        cell: ({ row }) => row.original.organizationName || '—',
    },
    {
        accessorKey: 'contactType',
        header: 'Type',
        cell: ({ row }) => <ContactTypeBadge type={row.original.contactType} />,
    },
    {
        accessorKey: 'isActive',
        header: 'Status',
        cell: ({ row }) =>
            row.original.isActive ? (
                <Badge variant='success'>Active</Badge>
            ) : (
                <Badge variant='secondary'>Inactive</Badge>
            ),
    },
    {
        id: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const contact = row.original;
            const name = [contact.firstName, contact.lastName].filter(Boolean).join(' ');
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
                                <Link to={`/siteadmin/contacts/${contact.id}`}>
                                    <Eye /> View Details
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuItem asChild>
                                <Link to={`/siteadmin/contacts/${contact.id}/edit`}>
                                    <Edit /> Edit
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem
                                onClick={() => onDelete(contact.id, name)}
                                className='text-destructive focus:text-destructive'
                            >
                                <SquareX /> Delete
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            );
        },
    },
];
