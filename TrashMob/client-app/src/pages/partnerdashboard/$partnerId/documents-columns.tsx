import { ColumnDef } from '@tanstack/react-table';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { Badge } from '@/components/ui/badge';
import { DocumentExpirationBadge } from '@/components/documents/document-expiration-badge';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Download, Ellipsis, ExternalLink, Pencil, SquareX } from 'lucide-react';
import { Link } from 'react-router';
import PartnerDocumentData from '@/components/Models/PartnerDocumentData';

export const documentTypeLabels: Record<number, string> = {
    0: 'Other',
    1: 'Agreement',
    2: 'Contract',
    3: 'Report',
    4: 'Insurance',
    5: 'Certificate',
};

export function formatFileSize(bytes: number | null): string {
    if (bytes == null || bytes === 0) return '';
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

function formatDate(date: Date | string | null): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString();
}

interface GetColumnsProps {
    onDownload: (doc: PartnerDocumentData) => void;
    onDelete: (documentId: string, documentName: string) => void;
}

export const getColumns = ({ onDownload, onDelete }: GetColumnsProps): ColumnDef<PartnerDocumentData>[] => [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
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
        id: 'actions',
        header: 'Actions',
        enableSorting: false,
        cell: ({ row }) => {
            const doc = row.original;
            return (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='ghost' size='icon'>
                            <Ellipsis />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent className='w-56'>
                        {doc.blobStoragePath || doc.url ? (
                            <DropdownMenuItem onClick={() => onDownload(doc)}>
                                {doc.blobStoragePath ? (
                                    <>
                                        <Download />
                                        Download
                                    </>
                                ) : (
                                    <>
                                        <ExternalLink />
                                        Open Link
                                    </>
                                )}
                            </DropdownMenuItem>
                        ) : null}
                        <DropdownMenuItem asChild>
                            <Link to={`${doc.id}/edit`}>
                                <Pencil />
                                Edit Document
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={() => onDelete(doc.id, doc.name)}>
                            <SquareX />
                            Remove Document
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            );
        },
    },
];
