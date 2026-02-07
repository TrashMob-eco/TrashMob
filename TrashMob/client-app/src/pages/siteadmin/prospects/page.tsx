import { useState } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Plus, Sparkles, Upload } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { DataTable } from '@/components/ui/data-table';
import { useToast } from '@/hooks/use-toast';
import { getColumns } from './columns';
import { GetCommunityProspects, DeleteCommunityProspect } from '@/services/community-prospects';
import { PIPELINE_STAGES } from '@/components/prospects/pipeline-stage-badge';

export const SiteAdminProspects = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [stageFilter, setStageFilter] = useState<number | undefined>(undefined);

    const queryParams = stageFilter !== undefined ? { stage: stageFilter } : undefined;

    const { data: prospects } = useQuery({
        queryKey: GetCommunityProspects(queryParams).key,
        queryFn: GetCommunityProspects(queryParams).service,
        select: (res) => res.data,
    });

    const deleteProspect = useMutation({
        mutationKey: DeleteCommunityProspect().key,
        mutationFn: DeleteCommunityProspect().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/communityprospects'], refetchType: 'all' });
            toast({ variant: 'default', title: 'Prospect deleted' });
        },
    });

    function handleDelete(id: string, name: string) {
        if (!window.confirm(`Are you sure you want to delete prospect: ${name}?`)) return;
        deleteProspect.mutate({ id });
    }

    const columns = getColumns({ onDelete: handleDelete });
    const len = (prospects || []).length;

    return (
        <Card>
            <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle>Prospects ({len})</CardTitle>
                <div className='flex gap-2'>
                    <Button variant='outline' asChild>
                        <Link to='/siteadmin/prospects/discovery'>
                            <Sparkles className='mr-2 h-4 w-4' /> AI Discovery
                        </Link>
                    </Button>
                    <Button variant='outline' asChild>
                        <Link to='/siteadmin/prospects/import'>
                            <Upload className='mr-2 h-4 w-4' /> Import CSV
                        </Link>
                    </Button>
                    <Button asChild>
                        <Link to='/siteadmin/prospects/create'>
                            <Plus className='mr-2 h-4 w-4' /> Add Prospect
                        </Link>
                    </Button>
                </div>
            </CardHeader>
            <CardContent>
                <Tabs
                    value={stageFilter !== undefined ? String(stageFilter) : 'all'}
                    onValueChange={(val) => setStageFilter(val === 'all' ? undefined : Number(val))}
                    className='mb-4'
                >
                    <TabsList className='flex-wrap h-auto'>
                        <TabsTrigger value='all'>All</TabsTrigger>
                        {PIPELINE_STAGES.map((s) => (
                            <TabsTrigger key={s.value} value={String(s.value)}>
                                {s.label}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                </Tabs>
                <DataTable
                    columns={columns}
                    data={prospects || []}
                    enableSearch
                    searchPlaceholder='Search prospects...'
                    searchColumns={['name', 'city', 'contactEmail']}
                />
            </CardContent>
        </Card>
    );
};
