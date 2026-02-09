import { useCallback } from 'react';
import { useParams, Link } from 'react-router';
import { ArrowLeft, Download, FileSpreadsheet } from 'lucide-react';

import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { ExportSponsorCleanupLogs } from '@/services/sponsor-portal';

export const SponsorReports = () => {
    const { sponsorId } = useParams<{ sponsorId: string }>() as { sponsorId: string };
    const { toast } = useToast();

    const handleExport = useCallback(async () => {
        if (!sponsorId) return;
        try {
            const response = await ExportSponsorCleanupLogs({ sponsorId }).service();
            const blob = response.data;
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `CleanupLogs_${new Date().toISOString().split('T')[0]}.csv`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
            toast({
                variant: 'primary',
                title: 'Export successful',
                description: 'Cleanup log data has been downloaded.',
            });
        } catch {
            toast({
                variant: 'destructive',
                title: 'Export failed',
                description: 'Failed to download cleanup log data. Please try again.',
            });
        }
    }, [sponsorId, toast]);

    return (
        <div className='space-y-6'>
            <div>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/sponsordashboard/${sponsorId}`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Dashboard
                    </Link>
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <FileSpreadsheet className='h-5 w-5' />
                        Reports
                    </CardTitle>
                    <CardDescription>
                        Download cleanup log data for your records or marketing materials.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='space-y-4'>
                        <div className='rounded-lg border p-4'>
                            <div className='flex items-center justify-between'>
                                <div>
                                    <h3 className='font-medium'>Cleanup Logs (CSV)</h3>
                                    <p className='text-sm text-muted-foreground'>
                                        All cleanup dates, areas, bags collected, and weights.
                                    </p>
                                </div>
                                <Button onClick={handleExport} size='lg' className='h-12'>
                                    <Download className='h-4 w-4 mr-2' />
                                    Download
                                </Button>
                            </div>
                        </div>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};

export default SponsorReports;
