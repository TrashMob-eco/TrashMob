import { ColumnDef } from '@tanstack/react-table';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { DocumentExpirationBadge } from '@/components/documents/document-expiration-badge';
import { AdminPartnerDocumentData } from '@/services/documents';
import { documentTypeLabels, formatFileSize } from '@/pages/partnerdashboard/$partnerId/documents-columns';

function formatDate(date: Date | string | null): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString();
}

export const columns: ColumnDef<AdminPartnerDocumentData>[] = [
    {
        id: 'partnerName',
        accessorFn: (row) => row.partner?.name ?? '',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Partner' />,
    },
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Document Name' />,
    },
    {
        accessorKey: 'documentTypeId',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Type' />,
        cell: ({ row }) => (
            <Badge variant='outline'>
                {documentTypeLabels[row.getValue('documentTypeId') as number] ?? 'Other'}
            </Badge>
        ),
    },
    {
        accessorKey: 'fileSizeBytes',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Size' />,
        cell: ({ row }) => formatFileSize(row.getValue('fileSizeBytes') as number | null),
    },
    {
        accessorKey: 'expirationDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Expiration' />,
        cell: ({ row }) => {
            const date = row.getValue('expirationDate') as Date | null;
            return (
                <div className='flex items-center gap-2'>
                    <DocumentExpirationBadge expirationDate={date} />
                    <span className='text-sm text-muted-foreground'>{formatDate(date)}</span>
                </div>
            );
        },
    },
    {
        accessorKey: 'createdDate',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Created' />,
        cell: ({ row }) => formatDate(row.getValue('createdDate') as Date | null),
    },
];
