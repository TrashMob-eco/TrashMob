import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { Progress } from '@/components/ui/progress';
import { GetPipelineAnalytics } from '@/services/community-prospects';
import { BarChart3, Mail, MousePointerClick, TrendingUp, Users } from 'lucide-react';

export const SiteAdminProspectAnalytics = () => {
    const { data: analytics } = useQuery({
        queryKey: GetPipelineAnalytics().key,
        queryFn: GetPipelineAnalytics().service,
        select: (res) => res.data,
    });

    const maxStageCount = Math.max(...(analytics?.stageCounts?.map((s) => s.count) ?? [1]));

    return (
        <div className='space-y-6'>
            {/* Summary Cards */}
            <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Total Prospects</CardTitle>
                        <Users className='h-4 w-4 text-muted-foreground' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{analytics?.totalProspects ?? '-'}</div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Conversion Rate</CardTitle>
                        <TrendingUp className='h-4 w-4 text-green-600' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{analytics?.conversionRate?.toFixed(1) ?? '0'}%</div>
                        <p className='text-xs text-muted-foreground'>
                            {analytics?.convertedCount ?? 0} converted to partners
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Open Rate</CardTitle>
                        <Mail className='h-4 w-4 text-blue-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{analytics?.openRate?.toFixed(1) ?? '0'}%</div>
                        <p className='text-xs text-muted-foreground'>
                            {analytics?.totalEmailsOpened ?? 0} of {analytics?.totalEmailsSent ?? 0} emails
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Avg. Days in Pipeline</CardTitle>
                        <BarChart3 className='h-4 w-4 text-amber-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{analytics?.averageDaysInPipeline?.toFixed(0) ?? '-'}</div>
                        <p className='text-xs text-muted-foreground'>From prospect to partner</p>
                    </CardContent>
                </Card>
            </div>

            {/* Pipeline Funnel */}
            <Card>
                <CardHeader>
                    <CardTitle>Pipeline Funnel</CardTitle>
                    <CardDescription>Prospect distribution across pipeline stages</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='space-y-4'>
                        {(analytics?.stageCounts ?? []).map((stage) => (
                            <div key={stage.stage} className='space-y-1'>
                                <div className='flex items-center justify-between text-sm'>
                                    <span className='font-medium'>{stage.label}</span>
                                    <span className='text-muted-foreground'>{stage.count}</span>
                                </div>
                                <Progress value={maxStageCount > 0 ? (stage.count / maxStageCount) * 100 : 0} />
                            </div>
                        ))}
                        {(analytics?.stageCounts?.length ?? 0) === 0 ? (
                            <p className='text-sm text-muted-foreground'>No pipeline data yet</p>
                        ) : null}
                    </div>
                </CardContent>
            </Card>

            {/* Outreach Metrics */}
            <Card>
                <CardHeader>
                    <CardTitle>Outreach Metrics</CardTitle>
                    <CardDescription>Email engagement statistics</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid gap-4 md:grid-cols-4'>
                        <div className='flex items-center gap-2'>
                            <Mail className='h-5 w-5 text-blue-500' />
                            <div>
                                <p className='text-sm font-medium'>Sent</p>
                                <p className='text-2xl font-bold'>{analytics?.totalEmailsSent ?? 0}</p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Mail className='h-5 w-5 text-green-500' />
                            <div>
                                <p className='text-sm font-medium'>Opened</p>
                                <p className='text-2xl font-bold'>{analytics?.totalEmailsOpened ?? 0}</p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <MousePointerClick className='h-5 w-5 text-purple-500' />
                            <div>
                                <p className='text-sm font-medium'>Clicked</p>
                                <p className='text-2xl font-bold'>{analytics?.totalEmailsClicked ?? 0}</p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Mail className='h-5 w-5 text-red-500' />
                            <div>
                                <p className='text-sm font-medium'>Bounced</p>
                                <p className='text-2xl font-bold'>{analytics?.totalEmailsBounced ?? 0}</p>
                            </div>
                        </div>
                    </div>
                </CardContent>
            </Card>

            {/* Type Breakdown */}
            <Card>
                <CardHeader>
                    <CardTitle>Prospect Types</CardTitle>
                    <CardDescription>Breakdown by organization type</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Type</TableHead>
                                <TableHead>Count</TableHead>
                                <TableHead>Converted</TableHead>
                                <TableHead>Rate</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {(analytics?.typeBreakdown ?? []).map((t) => (
                                <TableRow key={t.type}>
                                    <TableCell className='font-medium'>{t.type || 'Unknown'}</TableCell>
                                    <TableCell>{t.count}</TableCell>
                                    <TableCell>{t.convertedCount}</TableCell>
                                    <TableCell>
                                        <Badge variant='outline'>
                                            {t.count > 0 ? ((t.convertedCount / t.count) * 100).toFixed(1) : '0'}%
                                        </Badge>
                                    </TableCell>
                                </TableRow>
                            ))}
                            {(analytics?.typeBreakdown?.length ?? 0) === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={4} className='text-center text-muted-foreground'>
                                        No type data yet
                                    </TableCell>
                                </TableRow>
                            ) : null}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    );
};
