import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { DataTable } from '@/components/ui/data-table';
import { GetAllFeedback, UpdateFeedback, DeleteFeedback } from '@/services/feedback';
import { getColumns } from './columns';

const statusFilters = ['All', 'New', 'In Review', 'In Progress', 'Resolved', 'Closed'] as const;

export const SiteAdminFeedback = () => {
    const queryClient = useQueryClient();
    const [statusFilter, setStatusFilter] = useState<string>('All');

    const queryParams = statusFilter === 'All' ? undefined : { status: statusFilter };

    const { data: feedbackList, isLoading } = useQuery({
        queryKey: GetAllFeedback(queryParams).key,
        queryFn: GetAllFeedback(queryParams).service,
        select: (res) => res.data,
    });

    const updateFeedback = useMutation({
        mutationKey: UpdateFeedback().key,
        mutationFn: ({ id, status }: { id: string; status: string }) =>
            UpdateFeedback().service({ id }, { status }),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/feedback'],
                refetchType: 'all',
            });
        },
    });

    const deleteFeedback = useMutation({
        mutationKey: DeleteFeedback().key,
        mutationFn: DeleteFeedback().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/feedback'],
                refetchType: 'all',
            });
        },
    });

    const handleUpdateStatus = (id: string, status: string) => {
        updateFeedback.mutateAsync({ id, status });
    };

    const handleDelete = (id: string) => {
        if (!window.confirm('Are you sure you want to delete this feedback?')) return;
        deleteFeedback.mutateAsync({ id });
    };

    const columns = getColumns({
        onUpdateStatus: handleUpdateStatus,
        onDelete: handleDelete,
    });

    const len = (feedbackList || []).length;

    return (
        <Card>
            <CardHeader>
                <div className='flex items-center justify-between'>
                    <CardTitle>User Feedback ({len})</CardTitle>
                    <Tabs value={statusFilter} onValueChange={setStatusFilter}>
                        <TabsList>
                            {statusFilters.map((status) => (
                                <TabsTrigger key={status} value={status}>
                                    {status}
                                </TabsTrigger>
                            ))}
                        </TabsList>
                    </Tabs>
                </div>
            </CardHeader>
            <CardContent>
                {isLoading ? (
                    <div className='text-center py-8'>Loading feedback...</div>
                ) : (
                    <DataTable columns={columns} data={feedbackList || []} />
                )}
            </CardContent>
        </Card>
    );
};
