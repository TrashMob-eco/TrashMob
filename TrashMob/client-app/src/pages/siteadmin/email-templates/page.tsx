import orderBy from 'lodash/orderBy';
import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { GetAdminEmailTemplates } from '@/services/admin';
import { columns } from './columns';

export const SiteAdminEmailTemplates = () => {
    const { data: emailTemplates } = useQuery({
        queryKey: GetAdminEmailTemplates().key,
        queryFn: GetAdminEmailTemplates().service,
        select: (res) => orderBy(res.data, ['name'], ['asc']),
    });

    const len = (emailTemplates || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Email Templates ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable
                    columns={columns}
                    data={emailTemplates || []}
                    enableSearch
                    searchPlaceholder='Search templates...'
                    searchColumns={['name']}
                />
            </CardContent>
        </Card>
    );
};
