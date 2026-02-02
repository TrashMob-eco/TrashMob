import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Pencil, SquareX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { WaiverVersionData, WaiverScope } from '@/components/Models/WaiverVersionData';
import { Badge } from '@/components/ui/badge';
import { Link } from 'react-router';

interface GetColumnsProps {
    onDeactivate: (id: string, name: string) => void;
}

export const getColumns = ({ onDeactivate }: GetColumnsProps): ColumnDef<WaiverVersionData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
    },
    {
        accessorKey: 'version',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Version' />,
    },
    {
        accessorKey: 'scope',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Scope' />,
        cell: ({ row }) => {
            const scope = row.getValue('scope') as WaiverScope;
            return scope === WaiverScope.Global ? (
                <Badge variant='default'>Global</Badge>
            ) : (
                <Badge variant='secondary'>Community</Badge>
            );
        },
    },
    {
        accessorKey: 'effectiveDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Effective Date' />,
        cell: ({ row }) => {
            const date = row.getValue('effectiveDate') as string;
            return new Date(date).toLocaleDateString();
        },
    },
    {
        accessorKey: 'isActive',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) =>
            row.getValue('isActive') ? (
                <Badge variant='success'>Active</Badge>
            ) : (
                <Badge variant='destructive'>Inactive</Badge>
            ),
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const waiver = row.original;
            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem asChild>
                            <Link to={`${waiver.id}/edit`}>
                                <Pencil />
                                Edit Waiver
                            </Link>
                        </DropdownMenuItem>
                        {waiver.isActive ? (
                            <DropdownMenuItem onClick={() => onDeactivate(waiver.id, waiver.name)}>
                                <SquareX />
                                Deactivate Waiver
                            </DropdownMenuItem>
                        ) : null}
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
