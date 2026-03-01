import { useCallback } from 'react';
import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AlertTriangle, DollarSign, Download, Landmark, TrendingUp, Users } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Progress } from '@/components/ui/progress';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { useToast } from '@/hooks/use-toast';
import { ExportDonorReport, ExportFundraisingSummary, GetFundraisingDashboard } from '@/services/contacts';

export const SiteAdminFundraisingDashboard = () => {
    const { toast } = useToast();

    const { data: dashboard } = useQuery({
        queryKey: GetFundraisingDashboard().key,
        queryFn: GetFundraisingDashboard().service,
        select: (res) => res.data,
    });

    const maxLifecycleCount = Math.max(...(dashboard?.lifecycleBreakdown?.map((s) => s.count) ?? [1]));
    const maxGrantCount = Math.max(...(dashboard?.grantPipeline?.map((g) => g.count) ?? [1]));

    const handleExport = useCallback(
        async (type: 'donors' | 'summary') => {
            try {
                const service = type === 'donors' ? ExportDonorReport() : ExportFundraisingSummary();
                const response = await service.service();
                const blob = response.data;
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = `${type === 'donors' ? 'DonorReport' : 'FundraisingSummary'}_${new Date().toISOString().split('T')[0]}.csv`;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
                toast({ variant: 'primary', title: 'Export successful' });
            } catch {
                toast({ variant: 'destructive', title: 'Export failed' });
            }
        },
        [toast],
    );

    return (
        <div className='space-y-6'>
            {/* Summary Cards */}
            <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Total Raised YTD</CardTitle>
                        <DollarSign className='h-4 w-4 text-green-600' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>
                            ${(dashboard?.totalRaisedYtd ?? 0).toLocaleString(undefined, { minimumFractionDigits: 2 })}
                        </div>
                        <p className='text-xs text-muted-foreground'>
                            Last year: $
                            {(dashboard?.totalRaisedLastYear ?? 0).toLocaleString(undefined, {
                                minimumFractionDigits: 2,
                            })}
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Donor Count</CardTitle>
                        <Users className='h-4 w-4 text-blue-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{dashboard?.donorCountYtd ?? 0}</div>
                        <p className='text-xs text-muted-foreground'>
                            {dashboard?.newDonorsYtd ?? 0} new, {dashboard?.repeatDonorsYtd ?? 0} repeat
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Average Gift</CardTitle>
                        <DollarSign className='h-4 w-4 text-amber-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>
                            $
                            {(dashboard?.averageGiftSizeYtd ?? 0).toLocaleString(undefined, {
                                minimumFractionDigits: 2,
                            })}
                        </div>
                        <p className='text-xs text-muted-foreground'>
                            {dashboard?.donationCountYtd ?? 0} donations YTD
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Retention Rate</CardTitle>
                        <TrendingUp className='h-4 w-4 text-purple-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{dashboard?.retentionRate?.toFixed(1) ?? '0'}%</div>
                        <p className='text-xs text-muted-foreground'>
                            {dashboard?.lapsedDonors ?? 0} lapsed donors
                        </p>
                    </CardContent>
                </Card>
            </div>

            {/* LYBUNT Alert */}
            {(dashboard?.lybuntCount ?? 0) > 0 ? (
                <Card className='border-amber-200 bg-amber-50 dark:border-amber-800 dark:bg-amber-950'>
                    <CardContent className='flex items-center gap-3 pt-6'>
                        <AlertTriangle className='h-5 w-5 text-amber-600' />
                        <div className='flex-1'>
                            <p className='font-medium'>
                                {dashboard?.lybuntCount} donor{dashboard?.lybuntCount === 1 ? '' : 's'} gave last year
                                but not this year
                            </p>
                            <p className='text-sm text-muted-foreground'>
                                Review LYBUNT contacts to re-engage lapsed donors
                            </p>
                        </div>
                        <Button variant='outline' size='sm' asChild>
                            <Link to='/siteadmin/fundraising/engagement?tab=lybunt'>View LYBUNT</Link>
                        </Button>
                    </CardContent>
                </Card>
            ) : null}

            {/* Donor Lifecycle Funnel */}
            <Card>
                <CardHeader>
                    <CardTitle>Donor Lifecycle</CardTitle>
                    <CardDescription>Contact distribution across lifecycle stages</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='space-y-4'>
                        {(dashboard?.lifecycleBreakdown ?? []).map((stage) => (
                            <div key={stage.stage} className='space-y-1'>
                                <div className='flex items-center justify-between text-sm'>
                                    <span className='font-medium'>{stage.stage}</span>
                                    <span className='text-muted-foreground'>{stage.count}</span>
                                </div>
                                <Progress
                                    value={maxLifecycleCount > 0 ? (stage.count / maxLifecycleCount) * 100 : 0}
                                />
                            </div>
                        ))}
                        {(dashboard?.lifecycleBreakdown?.length ?? 0) === 0 ? (
                            <p className='text-sm text-muted-foreground'>No lifecycle data yet</p>
                        ) : null}
                    </div>
                </CardContent>
            </Card>

            {/* Monthly Giving */}
            <Card>
                <CardHeader>
                    <CardTitle>Monthly Giving</CardTitle>
                    <CardDescription>Donation totals over the last 12 months</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Month</TableHead>
                                <TableHead className='text-right'>Amount</TableHead>
                                <TableHead className='text-right'>Donations</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {(dashboard?.monthlyGiving ?? []).map((m) => (
                                <TableRow key={m.month}>
                                    <TableCell className='font-medium'>{m.month}</TableCell>
                                    <TableCell className='text-right'>
                                        ${m.amount.toLocaleString(undefined, { minimumFractionDigits: 2 })}
                                    </TableCell>
                                    <TableCell className='text-right'>{m.donationCount}</TableCell>
                                </TableRow>
                            ))}
                            {(dashboard?.monthlyGiving?.length ?? 0) === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={3} className='text-center text-muted-foreground'>
                                        No donation data yet
                                    </TableCell>
                                </TableRow>
                            ) : null}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>

            {/* Campaign Performance */}
            <Card>
                <CardHeader>
                    <CardTitle>Campaign Performance</CardTitle>
                    <CardDescription>Fundraising breakdown by campaign</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Campaign</TableHead>
                                <TableHead className='text-right'>Total Raised</TableHead>
                                <TableHead className='text-right'>Donors</TableHead>
                                <TableHead className='text-right'>Donations</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {(dashboard?.campaignBreakdown ?? []).map((c) => (
                                <TableRow key={c.campaign}>
                                    <TableCell className='font-medium'>{c.campaign}</TableCell>
                                    <TableCell className='text-right'>
                                        ${c.totalRaised.toLocaleString(undefined, { minimumFractionDigits: 2 })}
                                    </TableCell>
                                    <TableCell className='text-right'>{c.donorCount}</TableCell>
                                    <TableCell className='text-right'>{c.donationCount}</TableCell>
                                </TableRow>
                            ))}
                            {(dashboard?.campaignBreakdown?.length ?? 0) === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={4} className='text-center text-muted-foreground'>
                                        No campaign data yet
                                    </TableCell>
                                </TableRow>
                            ) : null}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>

            {/* Grant Pipeline */}
            <Card>
                <CardHeader>
                    <CardTitle>Grant Pipeline</CardTitle>
                    <CardDescription>Grant applications by status</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid gap-4 md:grid-cols-3 mb-6'>
                        <div className='flex items-center gap-2'>
                            <Landmark className='h-5 w-5 text-green-500' />
                            <div>
                                <p className='text-sm font-medium'>Awarded</p>
                                <p className='text-2xl font-bold'>
                                    $
                                    {(dashboard?.totalGrantsAwarded ?? 0).toLocaleString(undefined, {
                                        minimumFractionDigits: 2,
                                    })}
                                </p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Landmark className='h-5 w-5 text-amber-500' />
                            <div>
                                <p className='text-sm font-medium'>Pending</p>
                                <p className='text-2xl font-bold'>
                                    $
                                    {(dashboard?.totalGrantsPending ?? 0).toLocaleString(undefined, {
                                        minimumFractionDigits: 2,
                                    })}
                                </p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Landmark className='h-5 w-5 text-blue-500' />
                            <div>
                                <p className='text-sm font-medium'>Upcoming Deadlines</p>
                                <p className='text-2xl font-bold'>{dashboard?.upcomingDeadlineCount ?? 0}</p>
                            </div>
                        </div>
                    </div>
                    <div className='space-y-4'>
                        {(dashboard?.grantPipeline ?? []).map((g) => (
                            <div key={g.status} className='space-y-1'>
                                <div className='flex items-center justify-between text-sm'>
                                    <span className='font-medium'>{g.label}</span>
                                    <span className='text-muted-foreground'>
                                        {g.count} — $
                                        {g.totalAmount.toLocaleString(undefined, { minimumFractionDigits: 2 })}
                                    </span>
                                </div>
                                <Progress value={maxGrantCount > 0 ? (g.count / maxGrantCount) * 100 : 0} />
                            </div>
                        ))}
                        {(dashboard?.grantPipeline?.length ?? 0) === 0 ? (
                            <p className='text-sm text-muted-foreground'>No grant data yet</p>
                        ) : null}
                    </div>
                </CardContent>
            </Card>

            {/* Export */}
            <Card>
                <CardHeader>
                    <CardTitle>Export Reports</CardTitle>
                    <CardDescription>Download reports as CSV for board presentations</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='flex gap-3'>
                        <Button variant='outline' onClick={() => handleExport('donors')}>
                            <Download className='mr-2 h-4 w-4' />
                            Donor Report
                        </Button>
                        <Button variant='outline' onClick={() => handleExport('summary')}>
                            <Download className='mr-2 h-4 w-4' />
                            Fundraising Summary
                        </Button>
                    </div>
                </CardContent>
            </Card>
        </div>
    );
};
