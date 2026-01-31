import { useState } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { CheckCircle, Ellipsis, ExternalLink, Loader2, Plus, Trash2 } from 'lucide-react';

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
    LitterReportStatusEnum,
    LitterReportStatusLabels,
    LitterReportStatusColors,
} from '@/components/Models/LitterReportStatus';
import { GetEventLitterReports, DeleteEventLitterReport } from '@/services/event-litter-reports';
import { UpdateLitterReport } from '@/services/litter-report';
import { useToast } from '@/hooks/use-toast';
import { AddLitterReportDialog } from './add-litter-report-dialog';

interface AssociatedLitterReportsProps {
    eventId: string;
    isOwner: boolean;
}

export const AssociatedLitterReports = ({ eventId, isOwner }: AssociatedLitterReportsProps) => {
    const queryClient = useQueryClient();
    const { toast } = useToast();
    const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
    const [processingId, setProcessingId] = useState<string | null>(null);

    const { data: eventLitterReports, isLoading } = useQuery({
        queryKey: GetEventLitterReports({ eventId }).key,
        queryFn: GetEventLitterReports({ eventId }).service,
        select: (res) => res.data,
    });

    const deleteMutation = useMutation({
        mutationKey: DeleteEventLitterReport({ eventId, litterReportId: '' }).key,
        mutationFn: DeleteEventLitterReport({ eventId, litterReportId: '' }).service,
    });

    const updateLitterReportMutation = useMutation({
        mutationKey: UpdateLitterReport({ litterReport: {} as any }).key,
        mutationFn: UpdateLitterReport({ litterReport: {} as any }).service,
    });

    const handleRemove = async (litterReportId: string, name: string) => {
        if (!window.confirm(`Remove "${name}" from this event?`)) return;

        setProcessingId(litterReportId);
        try {
            await DeleteEventLitterReport({ eventId, litterReportId }).service();
            await queryClient.invalidateQueries({
                queryKey: GetEventLitterReports({ eventId }).key,
            });
            toast({
                title: 'Litter report removed',
                description: 'The litter report has been removed from this event.',
            });
        } catch (error) {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to remove the litter report.',
            });
        } finally {
            setProcessingId(null);
        }
    };

    const handleMarkAsCleaned = async (litterReportId: string, name: string) => {
        const report = eventLitterReports?.find((r) => r.litterReportId === litterReportId)?.litterReport;
        if (!report) return;

        setProcessingId(litterReportId);
        try {
            await UpdateLitterReport({
                litterReport: {
                    ...report,
                    litterReportStatusId: LitterReportStatusEnum.Cleaned,
                },
            }).service();
            await queryClient.invalidateQueries({
                queryKey: GetEventLitterReports({ eventId }).key,
            });
            toast({
                title: 'Marked as cleaned',
                description: `"${name}" has been marked as cleaned.`,
            });
        } catch (error) {
            toast({
                variant: 'destructive',
                title: 'Error',
                description: 'Failed to update the litter report status.',
            });
        } finally {
            setProcessingId(null);
        }
    };

    const getLocation = (report: { litterImages?: Array<{ city?: string; region?: string }> }) => {
        const firstImage = report.litterImages?.[0];
        if (!firstImage) return '-';
        const parts = [firstImage.city, firstImage.region].filter(Boolean);
        return parts.join(', ') || '-';
    };

    if (isLoading) {
        return (
            <div className='flex items-center justify-center py-8'>
                <Loader2 className='h-6 w-6 animate-spin text-muted-foreground' />
            </div>
        );
    }

    return (
        <div>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Name</TableHead>
                        <TableHead>Location</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead>Photos</TableHead>
                        {isOwner ? <TableHead className='w-[50px]' /> : null}
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {(eventLitterReports || []).length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={isOwner ? 5 : 4} className='text-center text-muted-foreground py-8'>
                                No litter reports associated with this event yet.
                            </TableCell>
                        </TableRow>
                    ) : null}
                    {(eventLitterReports || []).map((elr) => {
                        const report = elr.litterReport;
                        const statusId = report.litterReportStatusId as LitterReportStatusEnum;
                        const isProcessing = processingId === elr.litterReportId;

                        return (
                            <TableRow key={elr.litterReportId} className={isProcessing ? 'opacity-50' : ''}>
                                <TableCell>
                                    <Link
                                        to={`/litterreports/${elr.litterReportId}`}
                                        className='text-primary hover:underline font-medium flex items-center gap-1'
                                    >
                                        {report.name || 'Untitled Report'}
                                        <ExternalLink className='h-3 w-3' />
                                    </Link>
                                </TableCell>
                                <TableCell>{getLocation(report)}</TableCell>
                                <TableCell>
                                    <Badge
                                        variant='outline'
                                        className={`${LitterReportStatusColors[statusId] || 'bg-gray-500'} text-white border-0`}
                                    >
                                        {LitterReportStatusLabels[statusId] || 'Unknown'}
                                    </Badge>
                                </TableCell>
                                <TableCell>{report.litterImages?.length || 0}</TableCell>
                                {isOwner ? (
                                    <TableCell>
                                        <DropdownMenu>
                                            <DropdownMenuTrigger asChild>
                                                <Button variant='ghost' size='icon' disabled={isProcessing}>
                                                    {isProcessing ? (
                                                        <Loader2 className='h-4 w-4 animate-spin' />
                                                    ) : (
                                                        <Ellipsis className='h-4 w-4' />
                                                    )}
                                                </Button>
                                            </DropdownMenuTrigger>
                                            <DropdownMenuContent align='end'>
                                                {statusId !== LitterReportStatusEnum.Cleaned ? (
                                                    <DropdownMenuItem
                                                        onClick={() =>
                                                            handleMarkAsCleaned(elr.litterReportId, report.name)
                                                        }
                                                    >
                                                        <CheckCircle className='h-4 w-4 mr-2' />
                                                        Mark as Cleaned
                                                    </DropdownMenuItem>
                                                ) : null}
                                                <DropdownMenuItem
                                                    onClick={() => handleRemove(elr.litterReportId, report.name)}
                                                    className='text-destructive'
                                                >
                                                    <Trash2 className='h-4 w-4 mr-2' />
                                                    Remove from Event
                                                </DropdownMenuItem>
                                            </DropdownMenuContent>
                                        </DropdownMenu>
                                    </TableCell>
                                ) : null}
                            </TableRow>
                        );
                    })}
                    {isOwner ? (
                        <TableRow>
                            <TableCell colSpan={5}>
                                <Button variant='ghost' onClick={() => setIsAddDialogOpen(true)}>
                                    <Plus className='h-4 w-4 mr-2' /> Add Litter Report
                                </Button>
                            </TableCell>
                        </TableRow>
                    ) : null}
                </TableBody>
            </Table>

            <AddLitterReportDialog
                open={isAddDialogOpen}
                onOpenChange={setIsAddDialogOpen}
                eventId={eventId}
                existingLitterReportIds={(eventLitterReports || []).map((r) => r.litterReportId)}
            />
        </div>
    );
};
