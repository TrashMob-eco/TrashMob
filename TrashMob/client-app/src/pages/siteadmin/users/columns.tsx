import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, SquareX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import UserData from '@/components/Models/UserData';

interface GetColumnsProps {
  onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<UserData>[] => [
    {
        accessorKey: 'userName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='UserName' />,
    },
    {
        accessorKey: 'email',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Email' />,
    },
    {
        accessorKey: 'city',
        header: ({ column }) => <DataTableColumnHeader column={column} title='City' />,
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
        accessorKey: 'postalCode',
        header: 'Postal Code',
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const user = row.original;
            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                      <DropdownMenuItem onClick={() => onDelete(user.id, user.userName)}>
                          <SquareX />
                          Delete User
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
