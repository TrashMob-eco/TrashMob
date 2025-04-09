import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, SquareX } from 'lucide-react';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Button } from '@/components/ui/button';
import PartnerData from '@/components/Models/PartnerData';

interface GetColumnsProps {
    onDelete: (id: string, name: string) => void;
}

export const getColumns = ({ onDelete }: GetColumnsProps): ColumnDef<PartnerData>[] => [
    {
        accessorKey: 'name',
        header: 'Name',
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
