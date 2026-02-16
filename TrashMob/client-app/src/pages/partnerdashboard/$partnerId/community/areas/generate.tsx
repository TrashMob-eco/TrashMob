import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, Sparkles, ArrowLeft, X, Clock, CheckCircle2, AlertCircle, XCircle } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Progress } from '@/components/ui/progress';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useToast } from '@/hooks/use-toast';
import AreaGenerationBatchData, { BatchStatus } from '@/components/Models/AreaGenerationBatchData';
import {
    StartAreaGeneration,
    GetGenerationStatus,
    GetGenerationBatches,
    CancelGeneration,
} from '@/services/adoptable-areas';

const CATEGORIES = [
    { value: 'School', label: 'Schools' },
    { value: 'Park', label: 'Parks' },
    { value: 'Trail', label: 'Trails' },
];

const statusIcons: Record<BatchStatus, React.ReactNode> = {
    Queued: <Clock className='h-4 w-4 text-muted-foreground' />,
    Discovering: <Loader2 className='h-4 w-4 animate-spin text-blue-500' />,
    Processing: <Loader2 className='h-4 w-4 animate-spin text-blue-500' />,
    Complete: <CheckCircle2 className='h-4 w-4 text-green-500' />,
    Failed: <AlertCircle className='h-4 w-4 text-red-500' />,
    Cancelled: <XCircle className='h-4 w-4 text-gray-500' />,
};

const statusColors: Record<BatchStatus, string> = {
    Queued: 'bg-gray-100 text-gray-800',
    Discovering: 'bg-blue-100 text-blue-800',
    Processing: 'bg-blue-100 text-blue-800',
    Complete: 'bg-green-100 text-green-800',
    Failed: 'bg-red-100 text-red-800',
    Cancelled: 'bg-gray-100 text-gray-800',
};

export const PartnerCommunityAreasGenerate = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();

    const [category, setCategory] = useState('');
    const [isRunning, setIsRunning] = useState(false);
    const [activeBatchId, setActiveBatchId] = useState<string | null>(null);

    // Poll for active batch status
    const { data: activeStatus } = useQuery<AxiosResponse<AreaGenerationBatchData>, unknown, AreaGenerationBatchData>({
        queryKey: GetGenerationStatus({ partnerId }).key,
        queryFn: GetGenerationStatus({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId && isRunning,
        refetchInterval: isRunning ? 3000 : false,
        retry: false,
    });

    // Fetch batch history
    const { data: batches } = useQuery<AxiosResponse<AreaGenerationBatchData[]>, unknown, AreaGenerationBatchData[]>({
        queryKey: GetGenerationBatches({ partnerId }).key,
        queryFn: GetGenerationBatches({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    // Start generation
    const { mutate: startGeneration, isPending: isStarting } = useMutation({
        mutationKey: StartAreaGeneration().key,
        mutationFn: StartAreaGeneration().service,
        onSuccess: (res) => {
            setActiveBatchId(res.data.id);
            setIsRunning(true);
            toast({
                variant: 'primary',
                title: 'Generation started',
                description: `Searching for ${category.toLowerCase()}s in your community area...`,
            });
        },
        onError: (error: any) => {
            const message = error?.response?.data || 'Failed to start generation.';
            toast({ variant: 'destructive', title: 'Error', description: message });
        },
    });

    // Cancel generation
    const { mutate: cancelGeneration } = useMutation({
        mutationKey: CancelGeneration().key,
        mutationFn: CancelGeneration().service,
        onSuccess: () => {
            setIsRunning(false);
            queryClient.invalidateQueries({ queryKey: GetGenerationBatches({ partnerId }).key });
            toast({ variant: 'primary', title: 'Generation cancelled' });
        },
    });

    // Monitor active status for completion
    useEffect(() => {
        if (activeStatus && ['Complete', 'Failed', 'Cancelled'].includes(activeStatus.status)) {
            setIsRunning(false);
            queryClient.invalidateQueries({ queryKey: GetGenerationBatches({ partnerId }).key });
        }
    }, [activeStatus, partnerId, queryClient]);

    const handleStart = useCallback(() => {
        if (!category) return;
        startGeneration({ partnerId }, { category });
    }, [category, partnerId, startGeneration]);

    const handleCancel = useCallback(() => {
        if (!activeBatchId) return;
        cancelGeneration({ partnerId, batchId: activeBatchId });
    }, [activeBatchId, partnerId, cancelGeneration]);

    const progressPercent =
        activeStatus && activeStatus.discoveredCount > 0
            ? Math.round((activeStatus.processedCount / activeStatus.discoveredCount) * 100)
            : 0;

    return (
        <div className='py-8 space-y-6'>
            {/* Header */}
            <div className='flex items-center gap-4'>
                <Button
                    variant='ghost'
                    size='sm'
                    onClick={() => navigate(`/partnerdashboard/${partnerId}/community/areas`)}
                >
                    <ArrowLeft className='h-4 w-4 mr-2' />
                    Back to Areas
                </Button>
            </div>

            {/* Configuration / Progress Card */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Sparkles className='h-5 w-5' />
                        AI Area Generation
                    </CardTitle>
                    <CardDescription>
                        Automatically discover and create adoptable areas from OpenStreetMap data. Select a category to
                        search for all matching locations within your community boundaries.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {isRunning && activeStatus ? (
                        // Progress view
                        <div className='space-y-6'>
                            <div className='flex items-center justify-between'>
                                <div className='flex items-center gap-2'>
                                    {statusIcons[activeStatus.status]}
                                    <span className='font-medium capitalize'>{activeStatus.status}</span>
                                    <Badge className={statusColors[activeStatus.status]}>{activeStatus.category}</Badge>
                                </div>
                                <Button variant='outline' size='sm' onClick={handleCancel}>
                                    <X className='h-4 w-4 mr-2' />
                                    Cancel
                                </Button>
                            </div>

                            <div className='space-y-2'>
                                <div className='flex justify-between text-sm text-muted-foreground'>
                                    <span>
                                        {activeStatus.processedCount} of {activeStatus.discoveredCount} processed
                                    </span>
                                    <span>{progressPercent}%</span>
                                </div>
                                <Progress value={progressPercent} />
                            </div>

                            <div className='grid grid-cols-3 gap-4 text-center'>
                                <div>
                                    <div className='text-2xl font-bold'>{activeStatus.discoveredCount}</div>
                                    <div className='text-sm text-muted-foreground'>Discovered</div>
                                </div>
                                <div>
                                    <div className='text-2xl font-bold'>{activeStatus.stagedCount}</div>
                                    <div className='text-sm text-muted-foreground'>Staged</div>
                                </div>
                                <div>
                                    <div className='text-2xl font-bold'>{activeStatus.skippedCount}</div>
                                    <div className='text-sm text-muted-foreground'>Skipped</div>
                                </div>
                            </div>
                        </div>
                    ) : activeStatus && activeStatus.status === 'Complete' ? (
                        // Completed view
                        <div className='space-y-6'>
                            <div className='flex items-center gap-2 text-green-600'>
                                <CheckCircle2 className='h-5 w-5' />
                                <span className='font-medium'>Generation Complete</span>
                            </div>
                            <div className='grid grid-cols-3 gap-4 text-center'>
                                <div>
                                    <div className='text-2xl font-bold'>{activeStatus.discoveredCount}</div>
                                    <div className='text-sm text-muted-foreground'>Discovered</div>
                                </div>
                                <div>
                                    <div className='text-2xl font-bold'>{activeStatus.stagedCount}</div>
                                    <div className='text-sm text-muted-foreground'>Staged for Review</div>
                                </div>
                                <div>
                                    <div className='text-2xl font-bold'>{activeStatus.skippedCount}</div>
                                    <div className='text-sm text-muted-foreground'>Skipped</div>
                                </div>
                            </div>
                            {activeStatus.stagedCount > 0 && (
                                <Button
                                    onClick={() =>
                                        navigate(
                                            `/partnerdashboard/${partnerId}/community/areas/review?batchId=${activeStatus.id}`,
                                        )
                                    }
                                >
                                    Review {activeStatus.stagedCount} Staged Areas
                                </Button>
                            )}
                        </div>
                    ) : (
                        // Configuration view
                        <div className='space-y-4'>
                            <div className='flex items-end gap-4'>
                                <div className='flex-1'>
                                    <label className='text-sm font-medium mb-2 block'>Category</label>
                                    <Select value={category} onValueChange={setCategory}>
                                        <SelectTrigger>
                                            <SelectValue placeholder='Select a category...' />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {CATEGORIES.map((cat) => (
                                                <SelectItem key={cat.value} value={cat.value}>
                                                    {cat.label}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                </div>
                                <Button onClick={handleStart} disabled={!category || isStarting}>
                                    {isStarting ? (
                                        <Loader2 className='h-4 w-4 mr-2 animate-spin' />
                                    ) : (
                                        <Sparkles className='h-4 w-4 mr-2' />
                                    )}
                                    Start Generation
                                </Button>
                            </div>
                            <p className='text-sm text-muted-foreground'>
                                This will search OpenStreetMap for all{' '}
                                {category ? category.toLowerCase() + 's' : 'locations'} within your community boundaries
                                and stage them for review before creating adoptable areas.
                            </p>
                        </div>
                    )}
                </CardContent>
            </Card>

            {/* Batch History */}
            {batches && batches.length > 0 ? (
                <Card>
                    <CardHeader>
                        <CardTitle>Generation History</CardTitle>
                    </CardHeader>
                    <CardContent>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Category</TableHead>
                                    <TableHead>Status</TableHead>
                                    <TableHead>Discovered</TableHead>
                                    <TableHead>Staged</TableHead>
                                    <TableHead>Created</TableHead>
                                    <TableHead>Date</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {batches.map((batch) => (
                                    <TableRow key={batch.id}>
                                        <TableCell className='font-medium'>{batch.category}</TableCell>
                                        <TableCell>
                                            <div className='flex items-center gap-1'>
                                                {statusIcons[batch.status]}
                                                <Badge className={statusColors[batch.status]}>{batch.status}</Badge>
                                            </div>
                                        </TableCell>
                                        <TableCell>{batch.discoveredCount}</TableCell>
                                        <TableCell>{batch.stagedCount}</TableCell>
                                        <TableCell>{batch.createdCount}</TableCell>
                                        <TableCell>{new Date(batch.createdDate).toLocaleDateString()}</TableCell>
                                        <TableCell className='text-right'>
                                            {batch.status === 'Complete' && batch.stagedCount > 0 && (
                                                <Button
                                                    variant='outline'
                                                    size='sm'
                                                    onClick={() =>
                                                        navigate(
                                                            `/partnerdashboard/${partnerId}/community/areas/review?batchId=${batch.id}`,
                                                        )
                                                    }
                                                >
                                                    Review
                                                </Button>
                                            )}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </CardContent>
                </Card>
            ) : null}
        </div>
    );
};

export default PartnerCommunityAreasGenerate;
