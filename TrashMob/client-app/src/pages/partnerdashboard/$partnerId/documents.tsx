import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, Outlet, useMatch, useNavigate, useParams } from 'react-router';
import { AxiosResponse } from 'axios';
import { Plus } from 'lucide-react';

import { SidebarLayout } from '../../layouts/_layout.sidebar';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { DataTable } from '@/components/ui/data-table';
import { Progress } from '@/components/ui/progress';
import { useToast } from '@/hooks/use-toast';
import { useFeatureMetrics } from '@/hooks/useFeatureMetrics';

import {
    DeletePartnerDocumentByDocuemntId,
    DownloadPartnerDocument,
    GetPartnerDocumentsByPartnerId,
    GetPartnerStorageUsage,
} from '@/services/documents';
import PartnerDocumentData from '@/components/Models/PartnerDocumentData';
import { getColumns, documentTypeLabels, formatFileSize } from './documents-columns';

export const PartnerDocuments = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const { toast } = useToast();
    const { track } = useFeatureMetrics();
    const isEdit = useMatch(`/partnerdashboard/:partnerId/documents/:documentId/edit`);
    const isCreate = useMatch(`/partnerdashboard/:partnerId/documents/create`);

    const [typeFilter, setTypeFilter] = useState<string>('all');

    const { data: documents } = useQuery<AxiosResponse<PartnerDocumentData[]>, unknown, PartnerDocumentData[]>({
        queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
        queryFn: GetPartnerDocumentsByPartnerId({ partnerId }).service,
        select: (res) => res.data,
    });

    const { data: storageUsage } = useQuery({
        queryKey: GetPartnerStorageUsage({ partnerId }).key,
        queryFn: GetPartnerStorageUsage({ partnerId }).service,
        select: (res) => res.data,
    });

    const deletePartnerDocumentByDocuemntId = useMutation({
        mutationKey: DeletePartnerDocumentByDocuemntId().key,
        mutationFn: DeletePartnerDocumentByDocuemntId().service,
    });

    const removeDocument = (documentId: string, documentName: string) => {
        if (
            !window.confirm(
                `Please confirm that you want to remove document with name: '${documentName}' as a document from this Partner?`,
            )
        )
            return;

        deletePartnerDocumentByDocuemntId
            .mutateAsync({ documentId })
            .then(async () => {
                track({ category: 'Partner', action: 'Delete', target: 'Document' });
                await queryClient.invalidateQueries({
                    queryKey: GetPartnerDocumentsByPartnerId({ partnerId }).key,
                    refetchType: 'all',
                });
                await queryClient.invalidateQueries({
                    queryKey: GetPartnerStorageUsage({ partnerId }).key,
                    refetchType: 'all',
                });
            });
    };

    const handleDownload = async (doc: PartnerDocumentData) => {
        track({ category: 'Partner', action: 'Click', target: 'DocumentDownload' });
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

    const filteredDocuments = useMemo(() => {
        if (!documents) return [];
        if (typeFilter === 'all') return documents;
        return documents.filter((d) => d.documentTypeId === Number(typeFilter));
    }, [documents, typeFilter]);

    const columns = useMemo(
        () => getColumns({ onDownload: handleDownload, onDelete: removeDocument }),
        [partnerId],
    );

    const storagePercent =
        storageUsage && storageUsage.limitBytes > 0
            ? Math.round((storageUsage.usageBytes / storageUsage.limitBytes) * 100)
            : 0;

    return (
        <SidebarLayout
            title='Partner Documents'
            description='Manage documents relevant to the partnership. Upload files directly or link to external URLs. Supported formats: PDF, Word, Excel, PNG, JPEG (max 25 MB).'
        >
            <div className='space-y-4'>
                {/* Toolbar: type filter + add button */}
                <div className='flex items-center justify-between gap-4'>
                    <Select value={typeFilter} onValueChange={setTypeFilter}>
                        <SelectTrigger className='w-[180px]'>
                            <SelectValue placeholder='Filter by type' />
                        </SelectTrigger>
                        <SelectContent>
                            <SelectItem value='all'>All Types</SelectItem>
                            {Object.entries(documentTypeLabels).map(([key, label]) => (
                                <SelectItem key={key} value={key}>
                                    {label}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                    <Button asChild>
                        <Link to='create'>
                            <Plus /> Add Document
                        </Link>
                    </Button>
                </div>

                {/* Storage usage */}
                {storageUsage ? (
                    <div className='space-y-1'>
                        <div className='flex items-center justify-between text-sm text-muted-foreground'>
                            <span>
                                Storage: {formatFileSize(storageUsage.usageBytes)} of{' '}
                                {formatFileSize(storageUsage.limitBytes)}
                            </span>
                            <span>{storagePercent}%</span>
                        </div>
                        <Progress value={storagePercent} />
                    </div>
                ) : null}

                {/* DataTable */}
                <DataTable
                    columns={columns}
                    data={filteredDocuments}
                    enableSearch
                    searchPlaceholder='Search documents...'
                    searchColumns={['name']}
                />

                {/* Edit dialog */}
                <Dialog
                    open={!!isEdit}
                    onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/documents`)}
                >
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

                {/* Create dialog */}
                <Dialog
                    open={!!isCreate}
                    onOpenChange={() => navigate(`/partnerdashboard/${partnerId}/documents`)}
                >
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
