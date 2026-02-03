import { ColumnDef } from '@tanstack/react-table';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { EmailInviteBatch } from '@/services/email-invites';
import { Badge } from '@/components/ui/badge';
import { Eye } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Link } from 'react-router';

export const getColumns = (): ColumnDef<EmailInviteBatch>[] => [
    {
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Date' />,
        cell: ({ row }) => {
            const date = row.getValue('createdDate') as string;
            return new Date(date).toLocaleString();
        },
    },
    {
        accessorKey: 'senderUser',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Sent By' />,
        cell: ({ row }) => {
            const user = row.original.senderUser;
            return user?.userName || 'Unknown';
        },
    },
    {
        accessorKey: 'totalCount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Total' />,
    },
    {
        accessorKey: 'sentCount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Sent' />,
        cell: ({ row }) => {
            const sent = row.getValue('sentCount') as number;
            const total = row.original.totalCount;
            return (
                <span className={sent === total ? 'text-green-600' : 'text-amber-600'}>
                    {sent} / {total}
                </span>
            );
        },
    },
    {
        accessorKey: 'failedCount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Failed' />,
        cell: ({ row }) => {
            const failed = row.getValue('failedCount') as number;
            return failed > 0 ? <span className='text-red-600'>{failed}</span> : <span>0</span>;
        },
    },
    {
        accessorKey: 'status',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) => {
            const status = row.getValue('status') as string;
            switch (status) {
                case 'Complete':
                    return <Badge variant='success'>Complete</Badge>;
                case 'Processing':
                    return <Badge variant='secondary'>Processing</Badge>;
                case 'Failed':
                    return <Badge variant='destructive'>Failed</Badge>;
                default:
                    return <Badge variant='outline'>Pending</Badge>;
            }
        },
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const batch = row.original;
            return (
                <Button variant='ghost' size='icon' asChild>
                    <Link to={batch.id}>
                        <Eye className='h-4 w-4' />
                    </Link>
                </Button>
            );
        },
    },
];
