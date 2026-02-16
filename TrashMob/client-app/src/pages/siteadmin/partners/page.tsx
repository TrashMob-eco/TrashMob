import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { DataTable } from '@/components/ui/data-table';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
    DialogDescription,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { Loader2 } from 'lucide-react';
import { getColumns } from './columns';
import { DeletePartnerById, GetPartners, UpdatePartner } from '@/services/partners';
import { useToast } from '@/hooks/use-toast';
import PartnerData from '@/components/Models/PartnerData';

function generateSlug(partner: PartnerData): string {
    const parts: string[] = [];
    if (partner.city) {
        parts.push(partner.city);
    } else if (partner.name) {
        parts.push(partner.name);
    }
    if (partner.region) {
        parts.push(partner.region);
    }
    return parts
        .join('-')
        .toLowerCase()
        .replace(/[^a-z0-9-]/g, '-')
        .replace(/-+/g, '-')
        .replace(/^-|-$/g, '');
}

export const SiteAdminPartners = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [editingPartner, setEditingPartner] = useState<PartnerData | null>(null);
    const [editSlug, setEditSlug] = useState('');
    const [editHomePageEnabled, setEditHomePageEnabled] = useState(false);

    const { data: partners } = useQuery({
        queryKey: GetPartners().key,
        queryFn: GetPartners().service,
        select: (res) => res.data,
    });

    const deletePartnerById = useMutation({
        mutationKey: DeletePartnerById().key,
        mutationFn: DeletePartnerById().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetPartners().key });
            toast({ variant: 'default', title: 'Partner deleted' });
        },
    });

    const updatePartner = useMutation({
        mutationKey: UpdatePartner().key,
        mutationFn: UpdatePartner().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetPartners().key });
            toast({ variant: 'primary', title: 'Community settings updated' });
            setEditingPartner(null);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Error', description: 'Failed to update partner.' });
        },
    });

    function handleDelete(id: string, name: string) {
        if (!window.confirm(`Are you sure you want to delete partner with name: ${name}`)) return;
        deletePartnerById.mutate({ id });
    }

    function handleToggleHomePage(partner: PartnerData) {
        setEditingPartner(partner);
        setEditSlug(partner.slug || generateSlug(partner));
        setEditHomePageEnabled(!partner.homePageEnabled);
    }

    function handleSaveCommunitySettings() {
        if (!editingPartner) return;
        updatePartner.mutate({ ...editingPartner, slug: editSlug, homePageEnabled: editHomePageEnabled });
    }

    const columns = getColumns({ onDelete: handleDelete, onToggleHomePage: handleToggleHomePage });

    const len = (partners || []).length;

    return (
        <>
            <Card>
                <CardHeader>
                    <CardTitle>Partners ({len})</CardTitle>
                </CardHeader>
                <CardContent>
                    <DataTable
                        columns={columns}
                        data={partners || []}
                        enableSearch
                        searchPlaceholder='Search partners...'
                        searchColumns={['name', 'website']}
                    />
                </CardContent>
            </Card>

            <Dialog open={!!editingPartner} onOpenChange={(open) => !open && setEditingPartner(null)}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Community Page Settings</DialogTitle>
                        <DialogDescription>Configure the community page for {editingPartner?.name}.</DialogDescription>
                    </DialogHeader>
                    <div className='space-y-4 py-4'>
                        <div className='space-y-2'>
                            <Label htmlFor='slug'>URL Slug</Label>
                            <Input
                                id='slug'
                                value={editSlug}
                                onChange={(e) => setEditSlug(e.target.value)}
                                placeholder='e.g. seattle-wa'
                            />
                            <p className='text-xs text-muted-foreground'>
                                The community page will be at /communities/{editSlug || '...'}
                            </p>
                        </div>
                        <div className='flex items-center space-x-2'>
                            <Checkbox
                                id='homePageEnabled'
                                checked={editHomePageEnabled}
                                onCheckedChange={(checked) => setEditHomePageEnabled(!!checked)}
                            />
                            <Label htmlFor='homePageEnabled'>Enable Community Page</Label>
                        </div>
                        {editHomePageEnabled && !editSlug ? (
                            <p className='text-sm text-destructive'>A slug is required to enable the community page.</p>
                        ) : null}
                    </div>
                    <DialogFooter>
                        <Button variant='outline' onClick={() => setEditingPartner(null)}>
                            Cancel
                        </Button>
                        <Button
                            onClick={handleSaveCommunitySettings}
                            disabled={updatePartner.isPending || (editHomePageEnabled && !editSlug)}
                        >
                            {updatePartner.isPending ? <Loader2 className='h-4 w-4 animate-spin mr-2' /> : null}
                            Save
                        </Button>
                    </DialogFooter>
                </DialogContent>
            </Dialog>
        </>
    );
};
