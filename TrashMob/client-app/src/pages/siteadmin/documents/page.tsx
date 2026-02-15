import { useMutation, useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { DownloadPartnerDocument, GetAllPartnerDocuments } from '@/services/documents';
import { getColumns } from './columns';

export const SiteAdminDocuments = () => {
    const { data: documents } = useQuery({
        queryKey: GetAllPartnerDocuments().key,
        queryFn: GetAllPartnerDocuments().service,
        select: (res) => res.data,
    });

    const downloadMutation = useMutation({
        mutationFn: async (documentId: string) => {
            const res = await DownloadPartnerDocument({ documentId }).service();
            return res.data;
        },
        onSuccess: (data) => {
            if (data.downloadUrl) {
                window.open(data.downloadUrl, '_blank');
            }
        },
    });

    function handleDownload(documentId: string) {
        downloadMutation.mutate(documentId);
    }

    const columns = getColumns({ onDownload: handleDownload });
    const len = (documents || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Partner Documents ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable
                    columns={columns}
                    data={documents || []}
                    enableSearch
                    searchPlaceholder='Search documents...'
                    searchColumns={['name', 'partnerName']}
                />
            </CardContent>
        </Card>
    );
};
