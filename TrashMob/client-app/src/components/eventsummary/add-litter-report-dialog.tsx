import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Loader2, MapPin, Plus, Search } from 'lucide-react';

import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent } from '@/components/ui/card';
import { ScrollArea } from '@/components/ui/scroll-area';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';
import LitterReportData from '@/components/Models/LitterReportData';
import { GetNewLitterReports } from '@/services/litter-report';
import { AddEventLitterReport, GetEventLitterReports } from '@/services/event-litter-reports';
import { useToast } from '@/hooks/use-toast';

interface AddLitterReportDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    eventId: string;
    existingLitterReportIds: string[];
}

export const AddLitterReportDialog = ({
    open,
    onOpenChange,
    eventId,
    existingLitterReportIds,
}: AddLitterReportDialogProps) => {
    const queryClient = useQueryClient();
    const { toast } = useToast();
    const [searchTerm, setSearchTerm] = useState('');
    const [addingId, setAddingId] = useState<string | null>(null);

    const { data: litterReports, isLoading } = useQuery({
        queryKey: GetNewLitterReports().key,
        queryFn: GetNewLitterReports().service,
        select: (res) => res.data,
        enabled: open,
    });

    const addMutation = useMutation({
        mutationKey: AddEventLitterReport().key,
        mutationFn: AddEventLitterReport().service,
    });

    const handleAdd = async (report: LitterReportData) => {
        setAddingId(report.id);
        try {
            await addMutation.mutateAsync({
                eventId,
                litterReportId: report.id,
            });
            await queryClient.invalidateQueries({
                queryKey: GetEventLitterReports({ eventId }).key,
            });
            toast({
                title: 'Litter report added',
                description: `"${report.name}" has been associated with this event.`,
            });
            onOpenChange(false);
        } catch (error) {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to add the litter report to this event.',
            });
        } finally {
            setAddingId(null);
        }
    };

    const getLocation = (report: LitterReportData) => {
        const firstImage = report.litterImages?.[0];
        if (!firstImage) return 'No location';
        const parts = [firstImage.city, firstImage.region].filter(Boolean);
        return parts.join(', ') || 'No location';
    };

    // Filter out already-associated reports and apply search
    const availableReports = (litterReports || []).filter((report) => {
        // Exclude already associated reports
        if (existingLitterReportIds.includes(report.id)) return false;

        // Apply search filter
        if (searchTerm) {
            const term = searchTerm.toLowerCase();
            const matchesName = report.name?.toLowerCase().includes(term);
            const matchesLocation = getLocation(report).toLowerCase().includes(term);
            return matchesName || matchesLocation;
        }

        return true;
    });

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='max-w-lg'>
                <DialogHeader>
                    <DialogTitle>Add Litter Report</DialogTitle>
                    <DialogDescription>
                        Select a litter report that was cleaned during this event.
                    </DialogDescription>
                </DialogHeader>

                <div className='relative'>
                    <Search className='absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground' />
                    <Input
                        placeholder='Search by name or location...'
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className='pl-9'
                    />
                </div>

                <ScrollArea className='h-[300px] pr-4'>
                    {isLoading ? (
                        <div className='flex items-center justify-center py-8'>
                            <Loader2 className='h-6 w-6 animate-spin text-muted-foreground' />
                        </div>
                    ) : availableReports.length === 0 ? (
                        <div className='text-center text-muted-foreground py-8'>
                            {searchTerm
                                ? 'No matching litter reports found.'
                                : 'No available litter reports to add.'}
                        </div>
                    ) : (
                        <div className='space-y-2'>
                            {availableReports.map((report) => {
                                const statusId = report.litterReportStatusId as LitterReportStatusEnum;
                                const isAdding = addingId === report.id;

                                return (
                                    <Card key={report.id} className={isAdding ? 'opacity-50' : ''}>
                                        <CardContent className='p-3'>
                                            <div className='flex items-start justify-between gap-2'>
                                                <div className='flex-1 min-w-0'>
                                                    <div className='font-medium truncate'>
                                                        {report.name || 'Untitled Report'}
                                                    </div>
                                                    <div className='flex items-center gap-1 text-sm text-muted-foreground'>
                                                        <MapPin className='h-3 w-3' />
                                                        <span className='truncate'>{getLocation(report)}</span>
                                                    </div>
                                                    <div className='flex items-center gap-2 mt-1'>
                                                        <Badge
                                                            variant='outline'
                                                            className={`${LitterReportStatusColors[statusId] || 'bg-gray-500'} text-white border-0 text-xs`}
                                                        >
                                                            {LitterReportStatusLabels[statusId] || 'Unknown'}
                                                        </Badge>
                                                        <span className='text-xs text-muted-foreground'>
                                                            {report.litterImages?.length || 0} photo(s)
                                                        </span>
                                                    </div>
                                                </div>
                                                <Button
                                                    size='sm'
                                                    onClick={() => handleAdd(report)}
                                                    disabled={isAdding}
                                                >
                                                    {isAdding ? (
                                                        <Loader2 className='h-4 w-4 animate-spin' />
                                                    ) : (
                                                        <Plus className='h-4 w-4' />
                                                    )}
                                                </Button>
                                            </div>
                                        </CardContent>
                                    </Card>
                                );
                            })}
                        </div>
                    )}
                </ScrollArea>
            </DialogContent>
        </Dialog>
    );
};
