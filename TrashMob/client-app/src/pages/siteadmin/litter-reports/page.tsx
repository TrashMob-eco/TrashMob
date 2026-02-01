import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { columns } from './columns';
import { GetLitterReports } from '@/services/litter-report';

export const SiteAdminLitterReports = () => {
    const { data: litterReports, isLoading } = useQuery({
        queryKey: GetLitterReports().key,
        queryFn: GetLitterReports().service,
        select: (res) => res.data,
    });

    const len = litterReports?.length ?? 0;

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className='text-primary'>Litter Reports</CardTitle>
                </CardHeader>
                <CardContent>Loading...</CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle className='text-primary'>Litter Reports ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={litterReports || []} />
            </CardContent>
        </Card>
    );
};
