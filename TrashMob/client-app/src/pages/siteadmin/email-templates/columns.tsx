import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import { features } from '@/components/ui/data-table';
import EmailTemplateData from '@/components/Models/EmailTemplateData';

export const columns: ColumnDef<typeof features, EmailTemplateData>[] = [
    {
        accessorKey: 'name',
        header: 'Name',
        cell: ({ row }) => {
            const name = row.getValue('name') as string;
            return (
                <Link to={`${encodeURIComponent(name)}`} className='hover:underline text-primary font-medium'>
                    {name}
                </Link>
            );
        },
    },
];
