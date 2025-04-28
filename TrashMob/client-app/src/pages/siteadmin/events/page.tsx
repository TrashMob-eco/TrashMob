import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import { GetAllEvents } from '@/services/events';
import { columns } from './columns';

export const SiteAdminEvents = () => {
    const { data: events } = useQuery({
        queryKey: GetAllEvents().key,
        queryFn: GetAllEvents().service,
        select: (res) => res.data,
    });

    const len = (events || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Events ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={events || []} />
            </CardContent>
        </Card>
    );
};
