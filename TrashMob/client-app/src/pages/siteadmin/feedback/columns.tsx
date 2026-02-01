import { ColumnDef } from '@tanstack/react-table';
import { Ellipsis, Trash2, ExternalLink, CheckCircle, Clock, AlertCircle } from 'lucide-react';
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
import { UserFeedbackData } from '@/services/feedback';

interface GetColumnsProps {
    onUpdateStatus: (id: string, status: string) => void;
    onDelete: (id: string) => void;
}

const statusColors: Record<string, string> = {
    New: 'bg-blue-500',
    'In Review': 'bg-yellow-500',
    'In Progress': 'bg-purple-500',
    Resolved: 'bg-green-500',
    Closed: 'bg-gray-500',
};

const categoryColors: Record<string, string> = {
    Bug: 'bg-red-500',
    FeatureRequest: 'bg-blue-500',
    General: 'bg-gray-500',
    Praise: 'bg-green-500',
};

const categoryLabels: Record<string, string> = {
    Bug: 'Bug',
    FeatureRequest: 'Feature Request',
    General: 'General',
    Praise: 'Praise',
};

export const getColumns = ({ onUpdateStatus, onDelete }: GetColumnsProps): ColumnDef<UserFeedbackData>[] => [
    {
        accessorKey: 'category',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Category' />,
        cell: ({ row }) => {
            const category = row.getValue('category') as string;
            return (
                <Badge className={categoryColors[category] || 'bg-gray-500'}>
                    {categoryLabels[category] || category}
                </Badge>
            );
        },
    },
    {
        accessorKey: 'description',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Description' />,
        cell: ({ row }) => {
            const description = row.getValue('description') as string;
            return (
                <div className='max-w-md truncate' title={description}>
                    {description}
                </div>
            );
        },
    },
    {
        accessorKey: 'email',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Email' />,
        cell: ({ row }) => {
            const email = row.getValue('email') as string;
            return email || <span className='text-muted-foreground'>Anonymous</span>;
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
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Submitted' />,
        cell: ({ row }) => {
            const date = row.getValue('createdDate') as string;
            return date ? new Date(date).toLocaleDateString() : '-';
        },
    },
    {
        accessorKey: 'pageUrl',
        header: 'Page',
        cell: ({ row }) => {
            const url = row.getValue('pageUrl') as string;
            if (!url) return '-';
            try {
                const urlObj = new URL(url);
                return (
                    <a
                        href={url}
                        target='_blank'
                        rel='noopener noreferrer'
                        className='text-blue-500 hover:underline flex items-center gap-1'
                    >
                        {urlObj.pathname}
                        <ExternalLink className='h-3 w-3' />
                    </a>
                );
            } catch {
                return url;
            }
        },
    },
    {
        accessorKey: 'actions',
        header: 'Actions',
        cell: ({ row }) => {
            const feedback = row.original;
            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        <DropdownMenuItem onClick={() => onUpdateStatus(feedback.id!, 'In Review')}>
                            <Clock className='mr-2 h-4 w-4' />
                            Mark In Review
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => onUpdateStatus(feedback.id!, 'In Progress')}>
                            <AlertCircle className='mr-2 h-4 w-4' />
                            Mark In Progress
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => onUpdateStatus(feedback.id!, 'Resolved')}>
                            <CheckCircle className='mr-2 h-4 w-4' />
                            Mark Resolved
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem
                            onClick={() => onDelete(feedback.id!)}
                            className='text-destructive focus:text-destructive'
                        >
                            <Trash2 className='mr-2 h-4 w-4' />
                            Delete
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
