import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { GetAllPartnerDocuments } from '@/services/documents';
import { columns } from './columns';

export const SiteAdminDocuments = () => {
    const { data: documents } = useQuery({
        queryKey: GetAllPartnerDocuments().key,
        queryFn: GetAllPartnerDocuments().service,
        select: (res) => res.data,
    });

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
