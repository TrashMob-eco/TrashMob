import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/ui/data-table';
import { Plus } from 'lucide-react';
import { Link, Outlet, useMatch, useNavigate } from 'react-router';

import { getColumns } from './columns';
import { DeactivateWaiverVersion, GetAllWaiverVersions } from '@/services/waiver-admin';

export const SiteAdminWaivers = () => {
    const queryClient = useQueryClient();
    const navigate = useNavigate();
    const isEdit = useMatch(`/siteadmin/waivers/:waiverId/edit`);
    const isCreate = useMatch(`/siteadmin/waivers/create`);

    const { data: waivers } = useQuery({
        queryKey: GetAllWaiverVersions().key,
        queryFn: GetAllWaiverVersions().service,
        select: (res) => res.data,
    });

    const deactivateWaiver = useMutation({
        mutationKey: DeactivateWaiverVersion().key,
        mutationFn: DeactivateWaiverVersion().service,
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: GetAllWaiverVersions().key,
                refetchType: 'all',
            });
        },
    });

    const handleDeactivate = (waiverId: string, name: string) => {
        if (!window.confirm(`Are you sure you want to deactivate waiver: ${name}?`)) return;
        deactivateWaiver.mutateAsync({ id: waiverId });
    };

    const columns = getColumns({ onDeactivate: handleDeactivate });
    const len = (waivers || []).length;

    return (
        <Card>
            <CardHeader>
                <CardTitle>Waivers ({len})</CardTitle>
            </CardHeader>
            <CardContent>
                <DataTable columns={columns} data={waivers || []} />
                <Button variant='ghost' className='w-full' asChild>
                    <Link to='create'>
                        <Plus /> Add Waiver Version
                    </Link>
                </Button>
            </CardContent>
            <Dialog open={!!isEdit} onOpenChange={() => navigate(`/siteadmin/waivers`)}>
                <DialogContent
                    className='sm:max-w-[800px] overflow-y-scroll max-h-screen'
                    onOpenAutoFocus={(e) => e.preventDefault()}
                >
                    <DialogHeader>
                        <DialogTitle>Edit Waiver</DialogTitle>
                    </DialogHeader>
                    <div>
                        <Outlet />
                    </div>
                </DialogContent>
            </Dialog>
            <Dialog open={!!isCreate} onOpenChange={() => navigate(`/siteadmin/waivers`)}>
                <DialogContent
                    className='sm:max-w-[800px] overflow-y-scroll max-h-screen'
                    onOpenAutoFocus={(e) => e.preventDefault()}
                >
                    <DialogHeader>
                        <DialogTitle>Create Waiver</DialogTitle>
                    </DialogHeader>
                    <div>
                        <Outlet />
                    </div>
                </DialogContent>
            </Dialog>
        </Card>
    );
};
