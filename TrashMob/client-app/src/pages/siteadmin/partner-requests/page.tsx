import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { columns } from './columns';
import { GetPartnerRequests } from '@/services/partners';

export const SiteAdminPartnerRequests = () => {
    const { data: partnerRequests } = useQuery({
        queryKey: GetPartnerRequests().key,
        queryFn: GetPartnerRequests().service,
        select: (res) => res.data,
    });

    const len = (partnerRequests || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Partner Requests ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable
                    columns={columns}
                    data={partnerRequests || []}
                    enableSearch
                    searchPlaceholder='Search partner requests...'
                    searchColumns={['name', 'email']}
                />
            </CardContent>
        </Card>
    );
};
