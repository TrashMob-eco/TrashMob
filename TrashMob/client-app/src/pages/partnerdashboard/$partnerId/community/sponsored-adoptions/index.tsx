import { useMemo } from 'react';
import { useParams, Link, useNavigate } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, Plus, Pencil, ClipboardCheck, TrendingUp, AlertTriangle, Users } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import SponsoredAdoptionData, { SponsoredAdoptionStatus } from '@/components/Models/SponsoredAdoptionData';
import { SponsoredAdoptionComplianceStats } from '@/components/Models/SponsoredAdoptionComplianceStats';
import { GetSponsoredAdoptions, GetSponsoredAdoptionCompliance } from '@/services/sponsored-adoptions';

const statusColors: Record<SponsoredAdoptionStatus, string> = {
    Active: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    Expired: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-200',
    Terminated: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
};

export const PartnerCommunitySponsoredAdoptions = () => {
    const navigate = useNavigate();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const { data: adoptions, isLoading: adoptionsLoading } = useQuery<
        AxiosResponse<SponsoredAdoptionData[]>,
        unknown,
        SponsoredAdoptionData[]
    >({
        queryKey: GetSponsoredAdoptions({ partnerId }).key,
        queryFn: GetSponsoredAdoptions({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const { data: complianceStats, isLoading: statsLoading } = useQuery<
        AxiosResponse<SponsoredAdoptionComplianceStats>,
        unknown,
        SponsoredAdoptionComplianceStats
    >({
        queryKey: GetSponsoredAdoptionCompliance({ partnerId }).key,
        queryFn: GetSponsoredAdoptionCompliance({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const activeAdoptions = useMemo(() => adoptions?.filter((a) => a.status === 'Active') ?? [], [adoptions]);

    const overdueCount = complianceStats?.adoptionsOverdue ?? 0;

    const formatDate = (dateStr: string | null | undefined) => {
        if (!dateStr) return '-';
        return new Date(dateStr).toLocaleDateString();
    };

    if (adoptionsLoading || statsLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8 space-y-6'>
            {complianceStats ? (
                <div className='grid gap-4 md:grid-cols-4'>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Total Adoptions</CardTitle>
                            <Users className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{complianceStats.totalSponsoredAdoptions}</div>
                            <p className='text-xs text-muted-foreground'>{complianceStats.activeAdoptions} active</p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Active</CardTitle>
                            <ClipboardCheck className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{complianceStats.activeAdoptions}</div>
                            <p className='text-xs text-muted-foreground'>
                                {complianceStats.adoptionsOnSchedule} on schedule
                            </p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Compliance Rate</CardTitle>
                            <TrendingUp className='h-4 w-4 text-muted-foreground' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold'>{complianceStats.complianceRate.toFixed(0)}%</div>
                            <p className='text-xs text-muted-foreground'>
                                {complianceStats.totalCleanupLogs} cleanup logs
                            </p>
                        </CardContent>
                    </Card>
                    <Card>
                        <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                            <CardTitle className='text-sm font-medium'>Overdue</CardTitle>
                            <AlertTriangle className='h-4 w-4 text-red-500' />
                        </CardHeader>
                        <CardContent>
                            <div className='text-2xl font-bold text-red-600'>{complianceStats.adoptionsOverdue}</div>
                            <p className='text-xs text-muted-foreground'>Need follow-up</p>
                        </CardContent>
                    </Card>
                </div>
            ) : null}

            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <div>
                            <CardTitle className='flex items-center gap-2'>
                                <ClipboardCheck className='h-5 w-5' />
                                Sponsored Adoptions
                            </CardTitle>
                            <CardDescription>
                                Manage sponsor-funded professional cleanup programs for adopted areas.
                            </CardDescription>
                        </div>
                        <Button
                            onClick={() =>
                                navigate(`/partnerdashboard/${partnerId}/community/sponsored-adoptions/create`)
                            }
                        >
                            <Plus className='h-4 w-4 mr-2' />
                            Create Sponsored Adoption
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    <Tabs defaultValue='active'>
                        <TabsList className='mb-4'>
                            <TabsTrigger value='active'>Active ({activeAdoptions.length})</TabsTrigger>
                            <TabsTrigger value='overdue' className={overdueCount > 0 ? 'text-red-600' : ''}>
                                Overdue ({overdueCount})
                            </TabsTrigger>
                            <TabsTrigger value='all'>All ({adoptions?.length || 0})</TabsTrigger>
                        </TabsList>

                        <TabsContent value='active'>
                            <AdoptionTable
                                adoptions={activeAdoptions}
                                partnerId={partnerId}
                                formatDate={formatDate}
                                emptyMessage='No active sponsored adoptions.'
                                emptyDescription='Create a sponsored adoption to start tracking professional cleanup programs.'
                            />
                        </TabsContent>

                        <TabsContent value='overdue'>
                            <AdoptionTable
                                adoptions={activeAdoptions.filter(() => overdueCount > 0)}
                                partnerId={partnerId}
                                formatDate={formatDate}
                                emptyMessage='No overdue adoptions!'
                                emptyDescription='All sponsored adoptions are on schedule.'
                            />
                        </TabsContent>

                        <TabsContent value='all'>
                            <AdoptionTable
                                adoptions={adoptions ?? []}
                                partnerId={partnerId}
                                formatDate={formatDate}
                                emptyMessage='No sponsored adoptions yet.'
                                emptyDescription='Create your first sponsored adoption to start tracking professional cleanup programs.'
                            />
                        </TabsContent>
                    </Tabs>
                </CardContent>
            </Card>
        </div>
    );
};

const AdoptionTable = ({
    adoptions,
    partnerId,
    formatDate,
    emptyMessage,
    emptyDescription,
}: {
    adoptions: SponsoredAdoptionData[];
    partnerId: string;
    formatDate: (d: string | null | undefined) => string;
    emptyMessage: string;
    emptyDescription: string;
}) => {
    const navigate = useNavigate();

    if (adoptions.length === 0) {
        return (
            <div className='text-center py-12'>
                <ClipboardCheck className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                <h3 className='text-lg font-medium mb-2'>{emptyMessage}</h3>
                <p className='text-muted-foreground mb-4'>{emptyDescription}</p>
                <Button onClick={() => navigate(`/partnerdashboard/${partnerId}/community/sponsored-adoptions/create`)}>
                    <Plus className='h-4 w-4 mr-2' />
                    Create Sponsored Adoption
                </Button>
            </div>
        );
    }

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead>Area</TableHead>
                    <TableHead>Sponsor</TableHead>
                    <TableHead>Company</TableHead>
                    <TableHead>Start Date</TableHead>
                    <TableHead>Frequency</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead className='text-right'>Actions</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {adoptions.map((adoption) => (
                    <TableRow key={adoption.id}>
                        <TableCell className='font-medium'>{adoption.adoptableArea?.name || 'Unknown Area'}</TableCell>
                        <TableCell>{adoption.sponsor?.name || 'Unknown Sponsor'}</TableCell>
                        <TableCell>{adoption.professionalCompany?.name || 'Unknown Company'}</TableCell>
                        <TableCell>{formatDate(adoption.startDate)}</TableCell>
                        <TableCell>{adoption.cleanupFrequencyDays} days</TableCell>
                        <TableCell>
                            <Badge className={statusColors[adoption.status]}>{adoption.status}</Badge>
                        </TableCell>
                        <TableCell className='text-right'>
                            <Button variant='outline' size='sm' asChild>
                                <Link
                                    to={`/partnerdashboard/${partnerId}/community/sponsored-adoptions/${adoption.id}/edit`}
                                >
                                    <Pencil className='h-4 w-4' />
                                </Link>
                            </Button>
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    );
};

export default PartnerCommunitySponsoredAdoptions;
