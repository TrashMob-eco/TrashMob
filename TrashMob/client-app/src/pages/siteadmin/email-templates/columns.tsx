import { Link } from 'react-router';
import { ColumnDef } from '@tanstack/react-table';
import EmailTemplateData from '@/components/Models/EmailTemplateData';

export const columns: ColumnDef<EmailTemplateData>[] = [
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
