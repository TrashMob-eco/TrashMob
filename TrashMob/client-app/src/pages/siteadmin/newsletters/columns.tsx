import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Send, Calendar, Trash2, Eye, Edit } from 'lucide-react';
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
import { Newsletter } from '@/services/newsletters';

interface GetColumnsProps {
    onEdit: (newsletter: Newsletter) => void;
    onSend: (id: string) => void;
    onSchedule: (id: string) => void;
    onDelete: (id: string) => void;
}

const statusColors: Record<string, string> = {
    Draft: 'bg-gray-500',
    Scheduled: 'bg-blue-500',
    Sending: 'bg-yellow-500',
    Sent: 'bg-green-500',
};

export const getColumns = ({ onEdit, onSend, onSchedule, onDelete }: GetColumnsProps): ColumnDef<Newsletter>[] => [
    {
        accessorKey: 'subject',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Subject' />,
        cell: ({ row }) => {
            const subject = row.getValue('subject') as string;
            return <div className='max-w-md font-medium'>{subject}</div>;
        },
    },
    {
        accessorKey: 'categoryName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Category' />,
        cell: ({ row }) => {
            const categoryName = row.getValue('categoryName') as string;
            return <span>{categoryName || '-'}</span>;
        },
    },
    {
        accessorKey: 'status',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Status' />,
        cell: ({ row }) => {
            const status = row.getValue('status') as string;
            return <Badge className={statusColors[status] || 'bg-gray-500'}>{status}</Badge>;
        },
    },
    {
        accessorKey: 'targetType',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Target' />,
        cell: ({ row }) => {
            const targetType = row.getValue('targetType') as string;
            return <span>{targetType}</span>;
        },
    },
    {
        accessorKey: 'recipientCount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Recipients' />,
        cell: ({ row }) => {
            const count = row.getValue('recipientCount') as number;
            return <span>{count.toLocaleString()}</span>;
        },
    },
    {
        accessorKey: 'sentCount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Sent' />,
        cell: ({ row }) => {
            const newsletter = row.original;
            if (newsletter.status === 'Draft' || newsletter.status === 'Scheduled') {
                return <span className='text-muted-foreground'>-</span>;
            }
            return <span>{newsletter.sentCount.toLocaleString()}</span>;
        },
    },
    {
        accessorKey: 'openCount',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Opens' />,
        cell: ({ row }) => {
            const newsletter = row.original;
            if (newsletter.status === 'Draft' || newsletter.status === 'Scheduled') {
                return <span className='text-muted-foreground'>-</span>;
            }
            const openRate =
                newsletter.sentCount > 0 ? ((newsletter.openCount / newsletter.sentCount) * 100).toFixed(1) : '0';
            return (
                <span>
                    {newsletter.openCount.toLocaleString()} ({openRate}%)
                </span>
            );
        },
    },
    {
        accessorKey: 'scheduledDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Scheduled/Sent' />,
        cell: ({ row }) => {
            const newsletter = row.original;
            if (newsletter.sentDate) {
                return new Date(newsletter.sentDate).toLocaleDateString();
            }
            if (newsletter.scheduledDate) {
                return new Date(newsletter.scheduledDate).toLocaleDateString();
            }
            return <span className='text-muted-foreground'>-</span>;
        },
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const newsletter = row.original;
            const isDraft = newsletter.status === 'Draft';

            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem onClick={() => onEdit(newsletter)}>
                            {isDraft ? (
                                <>
                                    <Edit className='mr-2 h-4 w-4' />
                                    Edit
                                </>
                            ) : (
                                <>
                                    <Eye className='mr-2 h-4 w-4' />
                                    View
                                </>
                            )}
                        </DropdownMenuItem>
                        {isDraft ? (
                            <>
                                <DropdownMenuItem onClick={() => onSchedule(newsletter.id)}>
                                    <Calendar className='mr-2 h-4 w-4' />
                                    Schedule
                                </DropdownMenuItem>
                                <DropdownMenuItem onClick={() => onSend(newsletter.id)}>
                                    <Send className='mr-2 h-4 w-4' />
                                    Send Now
                                </DropdownMenuItem>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem
                                    onClick={() => onDelete(newsletter.id)}
                                    className='text-destructive focus:text-destructive'
                                >
                                    <Trash2 className='mr-2 h-4 w-4' />
                                    Delete
                                </DropdownMenuItem>
                            </>
                        ) : null}
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
