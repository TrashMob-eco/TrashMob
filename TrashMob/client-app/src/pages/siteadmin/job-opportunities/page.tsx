import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/ui/data-table';
import { Plus } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate } from 'react-router';

import { getColumns } from './columns';
import { DeleteJobOpportunityById, GetAllJobOpportunities } from '@/services/opportunities';

export const SiteAdminJobOpportunities = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isEdit = useMatch(`/siteadmin/job-opportunities/:jobId/edit`);
    const isCreate = useMatch(`/siteadmin/job-opportunities/create`);

    const { data: opportunities } = useQuery({
        queryKey: GetAllJobOpportunities().key,
        queryFn: GetAllJobOpportunities().service,
        select: (res) => res.data,
    });

    const deleteJobOpportunityById = useMutation({
        mutationKey: DeleteJobOpportunityById().key,
        mutationFn: DeleteJobOpportunityById().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetAllJobOpportunities().key,
                refetchType: 'all',
            });
        },
    });

    const removeOpportunity = (jobId: string, title: string) => {
        if (!window.confirm(`Are you sure you want to delete job: ${title}?`)) return;
        deleteJobOpportunityById.mutateAsync({ id: jobId });
    };

    const columns = getColumns({ onDelete: removeOpportunity });
    const len = (opportunities || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Opportunities ({len}) </CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable
                    columns={columns}
                    data={opportunities || []}
                    enableSearch
                    searchPlaceholder='Search opportunities...'
                    searchColumns={['name', 'description', 'city', 'region']}
                />
                <Button variant='ghost' className='w-full' asChild>
                    <Link to='create'>
                        <Plus /> Add Job Opportunity
                    </Link>
                </Button>
            </CardContent>
            <Dialog open={!!isEdit} onOpenChange={() => navigate(`/siteadmin/job-opportunities`)}>
                <DialogContent
                    className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                    onOpenAutoFocus={(e) => e.preventDefault()}
                >
                    <DialogHeader>
                        <DialogTitle>Edit Job</DialogTitle>
                    </DialogHeader>
                    <div>
                        <Outlet />
                    </div>
                </DialogContent>
            </Dialog>
            <Dialog open={!!isCreate} onOpenChange={() => navigate(`/siteadmin/job-opportunities`)}>
                <DialogContent
                    className='sm:max-w-[600px] overflow-y-scroll max-h-screen'
                    onOpenAutoFocus={(e) => e.preventDefault()}
                >
                    <DialogHeader>
                        <DialogTitle>Create Job</DialogTitle>
                    </DialogHeader>
                    <div>
                        <Outlet />
                    </div>
                </DialogContent>
            </Dialog>
        </Card>
    );
};
