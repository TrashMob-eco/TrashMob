import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { AxiosResponse } from 'axios';
import { Download, Ellipsis, ExternalLink, Pencil, Plus, SquareX } from 'lucide-react';

import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuTrigger,
    DropdownMenuContent,
    DropdownMenuItem,
} from '@/components/ui/dropdown-menu';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/hooks/use-toast';

import {
    DeletePartnerDocumentByDocuemntId,
    DownloadPartnerDocument,
    GetPartnerDocumentsByPartnerId,
} from '@/services/documents';
import PartnerDocumentData from '@/components/Models/PartnerDocumentData';

const documentTypeLabels: Record<number, string> = {
    0: 'Other',
    1: 'Agreement',
    2: 'Contract',
    3: 'Report',
    4: 'Insurance',
    5: 'Certificate',
};

function formatFileSize(bytes: number | null): string {
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

export const PartnerDocuments = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const { toast } = useToast();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/documents/:documentId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/documents/create`);

    const { data: documents } = useQuery<AxiosResponse<PartnerDocumentData[]>, unknown, PartnerDocumentData[]>({
        queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerDocumentsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });

    const deletePartnerDocumentByDocuemntId = useMutation({
        mutationKey: DeletePartnerDocumentByDocuemntId().key,
        mutationFn: DeletePartnerDocumentByDocuemntId().service,
    });

    const [isDeletingId, setIsDeletingId] = useState<string | null>(null);
    const removeDocument = (documentId: string, documentName: string) => {
        setIsDeletingId(documentId);
        if (
            !window.confirm(
                `Please confirm that you want to remove document with name: '${documentName}' as a document from this Partner?`,
            )
        )
            return;

        deletePartnerDocumentByDocuemntId
            .mutateAsync({ documentId })
            .then(async () => {
                return queryClient.invalidateQueries({
                    queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
                    refetchType: 'all',
                });
            })
            .then(() => {
                setIsDeletingId(null);
            });
    };

    const handleDownload = async (doc: PartnerDocumentData) => {
        if (doc.blobStoragePath) {
            try {
                const response = await DownloadPartnerDocument({ documentId: doc.id }).service();
                const { downloadUrl } = response.data;
                window.open(downloadUrl, '_blank');
            } catch {
                toast({
                    variant: 'destructive',
                    title: 'Download failed',
                    description: 'Could not generate download link.',
                });
            }
        } else if (doc.url) {
            window.open(doc.url, '_blank');
        }
    };

    return (
        <SidebarLayout
            title='Partner Documents'
            description='Manage documents relevant to the partnership. Upload files directly or link to external URLs. Supported formats: PDF, Word, Excel, PNG, JPEG (max 25 MB).'
        >
            <div>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Type</TableHead>
                            <TableHead>Size</TableHead>
                            <TableHead>Expiration</TableHead>
                            <TableHead>Actions</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(documents || []).map((row) => (
                            <TableRow key={row.id} className={isDeletingId === row.id ? 'opacity-20' : ''}>
                                <TableCell>{row.name}</TableCell>
                                <TableCell>
                                    <Badge variant='outline'>{documentTypeLabels[row.documentTypeId] ?? 'Other'}</Badge>
                                </TableCell>
                                <TableCell>{formatFileSize(row.fileSizeBytes)}</TableCell>
                                <TableCell>{formatDate(row.expirationDate)}</TableCell>
                                <TableCell>
                                    <DropdownMenu>
                                        <DropdownMenuTrigger asChild>
                                            <Button variant='ghost' size='icon'>
                                                <Ellipsis />
                                            </Button>
                                        </DropdownMenuTrigger>
                                        <DropdownMenuContent className='w-56'>
                                            {row.blobStoragePath || row.url ? (
                                                <DropdownMenuItem onClick={() => handleDownload(row)}>
                                                    {row.blobStoragePath ? (
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
                                                <Link to={`${row.id}/edit`}>
                                                    <Pencil />
                                                    Edit Document
                                                </Link>
                                            </DropdownMenuItem>
                                            <DropdownMenuItem onClick={() => removeDocument(row.id, row.name)}>
                                                <SquareX />
                                                Remove Document
                                            </DropdownMenuItem>
                                        </DropdownMenuContent>
                                    </DropdownMenu>
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell colSpan={5}>
                                <Button variant='ghost' className='w-full' asChild>
                                    <Link to='create'>
                                        <Plus /> Add Document
                                    </Link>
                                </Button>
                            </TableCell>
                        </TableRow>
                    </TableBody>
                </Table>
                <Dialog open={!!isEdit} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/documents`)}>
                    <DialogContent
                        className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                        onOpenAutoFocus={(e) => e.preventDefault()}
                    >
                        <DialogHeader>
                            <DialogTitle>Edit Document</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
                <Dialog open={!!isCreate} onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/documents`)}>
                    <DialogContent
                        className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                        onOpenAutoFocus={(e) => e.preventDefault()}
                    >
                        <DialogHeader>
                            <DialogTitle>Add Document</DialogTitle>
                        </DialogHeader>
                        <div>
                            <Outlet />
                        </div>
                    </DialogContent>
                </Dialog>
            </div>
        </SidebarLayout>
    );
};
