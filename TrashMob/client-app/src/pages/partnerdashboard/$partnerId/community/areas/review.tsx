import { useState, useCallback, useMemo } from 'react';
import { useParams, useNavigate, useSearchParams } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, ArrowLeft, Check, X, CheckCircle2, XCircle, AlertTriangle, Pencil, Save } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useToast } from '@/hooks/use-toast';
import StagedAdoptableAreaData, { ReviewStatus, ConfidenceLevel } from '@/components/Models/StagedAdoptableAreaData';
import AreaGenerationBatchData from '@/components/Models/AreaGenerationBatchData';
import {
    GetStagedAreas,
    GetGenerationBatch,
    ApproveStagedArea,
    RejectStagedArea,
    BulkApproveStagedAreas,
    BulkRejectStagedAreas,
    UpdateStagedAreaName,
    CreateApprovedAreas,
    GetAdoptableAreas,
} from '@/services/adoptable-areas';

const reviewStatusColors: Record<ReviewStatus, string> = {
    Pending: 'bg-yellow-100 text-yellow-800',
    Approved: 'bg-green-100 text-green-800',
    Rejected: 'bg-red-100 text-red-800',
};

const confidenceColors: Record<ConfidenceLevel, string> = {
    High: 'bg-green-100 text-green-800',
    Medium: 'bg-yellow-100 text-yellow-800',
    Low: 'bg-red-100 text-red-800',
};

export const PartnerCommunityAreasReview = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const [searchParams] = useSearchParams();
    const batchId = searchParams.get('batchId') || '';
    const { toast } = useToast();

    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
    const [editingId, setEditingId] = useState<string | null>(null);
    const [editName, setEditName] = useState('');

    // Fetch staged areas
    const {
        data: areas,
        isLoading,
        refetch: refetchAreas,
    } = useQuery<AxiosResponse<StagedAdoptableAreaData[]>, unknown, StagedAdoptableAreaData[]>({
        queryKey: GetStagedAreas({ partnerId, batchId }).key,
        queryFn: GetStagedAreas({ partnerId, batchId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!batchId,
    });

    // Fetch batch details
    const { data: batch } = useQuery<AxiosResponse<AreaGenerationBatchData>, unknown, AreaGenerationBatchData>({
        queryKey: GetGenerationBatch({ partnerId, batchId }).key,
        queryFn: GetGenerationBatch({ partnerId, batchId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && !!batchId,
    });

    // Approve single
    const { mutate: approveSingle } = useMutation({
        mutationKey: ApproveStagedArea().key,
        mutationFn: ApproveStagedArea().service,
        onSuccess: () => {
            refetchAreas();
        },
    });

    // Reject single
    const { mutate: rejectSingle } = useMutation({
        mutationKey: RejectStagedArea().key,
        mutationFn: RejectStagedArea().service,
        onSuccess: () => {
            refetchAreas();
        },
    });

    // Bulk approve
    const { mutate: bulkApprove, isPending: isBulkApproving } = useMutation({
        mutationKey: BulkApproveStagedAreas().key,
        mutationFn: (body: { batchId: string; ids?: string[] }) =>
            BulkApproveStagedAreas().service({ partnerId }, body),
        onSuccess: (res) => {
            refetchAreas();
            setSelectedIds(new Set());
            toast({ variant: 'primary', title: `${res.data} areas approved` });
        },
    });

    // Bulk reject
    const { mutate: bulkReject, isPending: isBulkRejecting } = useMutation({
        mutationKey: BulkRejectStagedAreas().key,
        mutationFn: (body: { batchId: string; ids?: string[] }) => BulkRejectStagedAreas().service({ partnerId }, body),
        onSuccess: (res) => {
            refetchAreas();
            setSelectedIds(new Set());
            toast({ variant: 'primary', title: `${res.data} areas rejected` });
        },
    });

    // Update name
    const { mutate: updateName } = useMutation({
        mutationKey: UpdateStagedAreaName().key,
        mutationFn: ({ id, name }: { id: string; name: string }) =>
            UpdateStagedAreaName().service({ partnerId, id }, { name }),
        onSuccess: () => {
            setEditingId(null);
            refetchAreas();
        },
    });

    // Create approved areas
    const { mutate: createApproved, isPending: isCreating } = useMutation({
        mutationKey: CreateApprovedAreas().key,
        mutationFn: (body: { batchId: string }) => CreateApprovedAreas().service({ partnerId }, body),
        onSuccess: (res) => {
            queryClient.invalidateQueries({ queryKey: GetAdoptableAreas({ partnerId }).key });
            toast({
                variant: 'primary',
                title: 'Areas created',
                description: `${res.data.createdCount} adoptable areas created, ${res.data.skippedDuplicateCount} duplicates skipped.`,
            });
            navigate(`/partnerdashboard/${partnerId}/community/areas`);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Error', description: 'Failed to create areas.' });
        },
    });

    const pendingAreas = useMemo(() => areas?.filter((a) => a.reviewStatus === 'Pending') ?? [], [areas]);
    const approvedAreas = useMemo(() => areas?.filter((a) => a.reviewStatus === 'Approved') ?? [], [areas]);
    const rejectedAreas = useMemo(() => areas?.filter((a) => a.reviewStatus === 'Rejected') ?? [], [areas]);

    const toggleSelect = useCallback((id: string) => {
        setSelectedIds((prev) => {
            const next = new Set(prev);
            if (next.has(id)) next.delete(id);
            else next.add(id);
            return next;
        });
    }, []);

    const toggleSelectAll = useCallback(() => {
        if (!pendingAreas) return;
        if (selectedIds.size === pendingAreas.length) {
            setSelectedIds(new Set());
        } else {
            setSelectedIds(new Set(pendingAreas.map((a) => a.id)));
        }
    }, [pendingAreas, selectedIds.size]);

    const handleBulkApprove = useCallback(() => {
        const ids = selectedIds.size > 0 ? Array.from(selectedIds) : undefined;
        bulkApprove({ batchId, ids });
    }, [selectedIds, batchId, bulkApprove]);

    const handleBulkReject = useCallback(() => {
        const ids = selectedIds.size > 0 ? Array.from(selectedIds) : undefined;
        bulkReject({ batchId, ids });
    }, [selectedIds, batchId, bulkReject]);

    const handleStartEdit = useCallback((area: StagedAdoptableAreaData) => {
        setEditingId(area.id);
        setEditName(area.name);
    }, []);

    const handleSaveName = useCallback(
        (id: string) => {
            updateName({ id, name: editName });
        },
        [editName, updateName],
    );

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8 space-y-6'>
            {/* Header */}
            <div className='flex items-center gap-4'>
                <Button
                    variant='ghost'
                    size='sm'
                    onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas/generate`)}
                >
                    <ArrowLeft className='h-4 w-4 mr-2' />
                    Back to Generate
                </Button>
            </div>

            {/* Summary */}
            <Card>
                <CardHeader>
                    <CardTitle>Review Staged Areas</CardTitle>
                    <CardDescription>
                        {batch
                            ? `Review ${batch.stagedCount} ${batch.category.toLowerCase()} areas discovered from OpenStreetMap.`
                            : 'Review staged areas before creating adoptable areas.'}
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='flex items-center gap-6 mb-4'>
                        <div className='flex items-center gap-2'>
                            <Badge className='bg-yellow-100 text-yellow-800'>{pendingAreas.length} Pending</Badge>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Badge className='bg-green-100 text-green-800'>{approvedAreas.length} Approved</Badge>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Badge className='bg-red-100 text-red-800'>{rejectedAreas.length} Rejected</Badge>
                        </div>
                    </div>

                    {/* Bulk Actions */}
                    <div className='flex items-center gap-2 mb-4'>
                        <Button
                            variant='outline'
                            size='sm'
                            onClick={handleBulkApprove}
                            disabled={isBulkApproving || pendingAreas.length === 0}
                        >
                            <Check className='h-4 w-4 mr-1' />
                            {selectedIds.size > 0 ? `Approve Selected (${selectedIds.size})` : 'Approve All Pending'}
                        </Button>
                        <Button
                            variant='outline'
                            size='sm'
                            onClick={handleBulkReject}
                            disabled={isBulkRejecting || pendingAreas.length === 0}
                        >
                            <X className='h-4 w-4 mr-1' />
                            {selectedIds.size > 0 ? `Reject Selected (${selectedIds.size})` : 'Reject All Pending'}
                        </Button>
                        {approvedAreas.length > 0 && (
                            <Button
                                className='ml-auto'
                                onClick={() => createApproved({ batchId })}
                                disabled={isCreating}
                            >
                                {isCreating ? (
                                    <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                ) : (
                                    <CheckCircle2 className='h-4 w-4 mr-2' />
                                )}
                                Create {approvedAreas.length} Approved Areas
                            </Button>
                        )}
                    </div>

                    {/* Areas Table */}
                    {areas && areas.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead className='w-10'>
                                        <Checkbox
                                            checked={
                                                pendingAreas.length > 0 && selectedIds.size === pendingAreas.length
                                            }
                                            onCheckedChange={toggleSelectAll}
                                        />
                                    </TableHead>
                                    <TableHead>Name</TableHead>
                                    <TableHead>Type</TableHead>
                                    <TableHead>Confidence</TableHead>
                                    <TableHead>Duplicate?</TableHead>
                                    <TableHead>Status</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {areas.map((area) => (
                                    <TableRow key={area.id} className={area.isPotentialDuplicate ? 'bg-yellow-50' : ''}>
                                        <TableCell>
                                            {area.reviewStatus === 'Pending' && (
                                                <Checkbox
                                                    checked={selectedIds.has(area.id)}
                                                    onCheckedChange={() => toggleSelect(area.id)}
                                                />
                                            )}
                                        </TableCell>
                                        <TableCell className='font-medium'>
                                            {editingId === area.id ? (
                                                <div className='flex items-center gap-1'>
                                                    <Input
                                                        value={editName}
                                                        onChange={(e) => setEditName(e.target.value)}
                                                        className='h-8 w-60'
                                                    />
                                                    <Button
                                                        variant='ghost'
                                                        size='sm'
                                                        onClick={() => handleSaveName(area.id)}
                                                    >
                                                        <Save className='h-3 w-3' />
                                                    </Button>
                                                    <Button
                                                        variant='ghost'
                                                        size='sm'
                                                        onClick={() => setEditingId(null)}
                                                    >
                                                        <X className='h-3 w-3' />
                                                    </Button>
                                                </div>
                                            ) : (
                                                <div className='flex items-center gap-1'>
                                                    {area.name}
                                                    {area.reviewStatus === 'Pending' && (
                                                        <Button
                                                            variant='ghost'
                                                            size='sm'
                                                            onClick={() => handleStartEdit(area)}
                                                        >
                                                            <Pencil className='h-3 w-3' />
                                                        </Button>
                                                    )}
                                                </div>
                                            )}
                                        </TableCell>
                                        <TableCell>{area.areaType}</TableCell>
                                        <TableCell>
                                            <Badge className={confidenceColors[area.confidence]}>
                                                {area.confidence}
                                            </Badge>
                                        </TableCell>
                                        <TableCell>
                                            {area.isPotentialDuplicate ? (
                                                <div className='flex items-center gap-1 text-yellow-600'>
                                                    <AlertTriangle className='h-3 w-3' />
                                                    <span className='text-xs'>{area.duplicateOfName}</span>
                                                </div>
                                            ) : null}
                                        </TableCell>
                                        <TableCell>
                                            <Badge className={reviewStatusColors[area.reviewStatus]}>
                                                {area.reviewStatus}
                                            </Badge>
                                        </TableCell>
                                        <TableCell className='text-right'>
                                            {area.reviewStatus === 'Pending' && (
                                                <div className='flex justify-end gap-1'>
                                                    <Button
                                                        variant='outline'
                                                        size='sm'
                                                        onClick={() => approveSingle({ partnerId, id: area.id })}
                                                    >
                                                        <Check className='h-3 w-3' />
                                                    </Button>
                                                    <Button
                                                        variant='outline'
                                                        size='sm'
                                                        onClick={() => rejectSingle({ partnerId, id: area.id })}
                                                    >
                                                        <XCircle className='h-3 w-3' />
                                                    </Button>
                                                </div>
                                            )}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-8 text-muted-foreground'>
                            No staged areas found for this batch.
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunityAreasReview;
