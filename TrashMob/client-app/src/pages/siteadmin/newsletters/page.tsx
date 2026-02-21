import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/ui/data-table';
import { Plus } from 'lucide-react';
import { GetNewsletters, DeleteNewsletter, SendNewsletter, Newsletter } from '@/services/newsletters';
import { getColumns } from './columns';
import { NewsletterEditorDialog } from './newsletter-editor-dialog';
import { ScheduleDialog } from './schedule-dialog';
import { TestSendDialog } from './test-send-dialog';

const statusFilters = ['All', 'Draft', 'Scheduled', 'Sending', 'Sent'] as const;

export const SiteAdminNewsletters = () => {
    const queryClient = useQueryClient();
    const [statusFilter, setStatusFilter] = useState<string>('All');
    const [editorOpen, setEditorOpen] = useState(false);
    const [editingNewsletter, setEditingNewsletter] = useState<Newsletter | null>(null);
    const [scheduleOpen, setScheduleOpen] = useState(false);
    const [schedulingId, setSchedulingId] = useState<string | null>(null);
    const [testSendOpen, setTestSendOpen] = useState(false);
    const [testSendId, setTestSendId] = useState<string | null>(null);

    const queryParams = statusFilter === 'All' ? undefined : { status: statusFilter };

    const { data: newsletters, isLoading } = useQuery({
        queryKey: GetNewsletters(queryParams).key,
        queryFn: GetNewsletters(queryParams).service,
        select: (res) => res.data,
    });

    const deleteNewsletter = useMutation({
        mutationKey: DeleteNewsletter().key,
        mutationFn: DeleteNewsletter().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/admin/newsletters'],
                refetchType: 'all',
            });
        },
    });

    const sendNewsletter = useMutation({
        mutationKey: SendNewsletter().key,
        mutationFn: SendNewsletter().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ['/admin/newsletters'],
                refetchType: 'all',
            });
        },
    });

    const handleCreate = () => {
        setEditingNewsletter(null);
        setEditorOpen(true);
    };

    const handleEdit = (newsletter: Newsletter) => {
        setEditingNewsletter(newsletter);
        setEditorOpen(true);
    };

    const handleSend = (id: string) => {
        if (!window.confirm('Are you sure you want to send this newsletter immediately?')) return;
        sendNewsletter.mutateAsync({ id });
    };

    const handleSchedule = (id: string) => {
        setSchedulingId(id);
        setScheduleOpen(true);
    };

    const handleDelete = (id: string) => {
        if (!window.confirm('Are you sure you want to delete this newsletter?')) return;
        deleteNewsletter.mutateAsync({ id });
    };

    const handleTestSend = (id: string) => {
        setTestSendId(id);
        setTestSendOpen(true);
    };

    const columns = getColumns({
        onEdit: handleEdit,
        onSend: handleSend,
        onSchedule: handleSchedule,
        onTestSend: handleTestSend,
        onDelete: handleDelete,
    });

    const len = (newsletters || []).length;

    return (
        <>
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <CardTitle>Newsletters ({len})</CardTitle>
                        <div className='flex items-center gap-4'>
                            <Tabs value={statusFilter} onValueChange={setStatusFilter}>
                                <TabsList>
                                    {statusFilters.map((status) => (
                                        <TabsTrigger key={status} value={status}>
                                            {status}
                                        </TabsTrigger>
                                    ))}
                                </TabsList>
                            </Tabs>
                            <Button onClick={handleCreate}>
                                <Plus className='mr-2 h-4 w-4' />
                                Create Newsletter
                            </Button>
                        </div>
                    </div>
                </CardHeader>
                <CardContent>
                    {isLoading ? (
                        <div className='text-center py-8'>Loading newsletters...</div>
                    ) : (
                        <DataTable columns={columns} data={newsletters || []} />
                    )}
                </CardContent>
            </Card>

            <NewsletterEditorDialog open={editorOpen} onOpenChange={setEditorOpen} newsletter={editingNewsletter} />

            <ScheduleDialog open={scheduleOpen} onOpenChange={setScheduleOpen} newsletterId={schedulingId} />

            <TestSendDialog open={testSendOpen} onOpenChange={setTestSendOpen} newsletterId={testSendId} />
        </>
    );
};
