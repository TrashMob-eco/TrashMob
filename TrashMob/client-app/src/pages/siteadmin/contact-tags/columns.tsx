import { ColumnDef } from '@tanstack/react-table';
import { Edit, Ellipsis, SquareX } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import ContactTagData from '@/components/Models/ContactTagData';

interface GetColumnsProps {
    onEdit: (tag: ContactTagData) => void;
    onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onEdit, onDelete }: GetColumnsProps): ColumnDef<ContactTagData>[] => [
    {
        accessorKey: 'color',
        header: '',
        cell: ({ row }) => {
            const color = row.original.color;
            return color ? (
                <div className='h-4 w-4 rounded-full border' style={{ backgroundColor: color }} />
            ) : (
                <div className='h-4 w-4 rounded-full border bg-muted' />
            );
        },
        size: 40,
    },
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
    },
    {
        id: 'actions',
        header: () => <div className='text-right'>Actions</div>,
        cell: ({ row }) => {
            const tag = row.original;
            return (
                <div className='text-right'>
                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant='ghost' size='icon'>
                                <Ellipsis />
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent className='w-56'>
                            <DropdownMenuItem onClick={() => onEdit(tag)}>
                                <Edit /> Edit
                            </DropdownMenuItem>
                            <DropdownMenuItem
                                onClick={() => onDelete(tag.id, tag.name)}
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
