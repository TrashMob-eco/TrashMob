import { useState } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Badge } from '@/components/ui/badge';
import { GetComplianceSummary, GetUserWaivers, GetActiveWaiverVersions, ExportWaivers } from '@/services/waiver-admin';
import { UserWaiverFilter } from '@/components/Models/WaiverVersionData';
import { Download, FileText, Search, Users, AlertTriangle, CheckCircle, Clock } from 'lucide-react';

export const WaiverComplianceDashboard = () => {
    const [filter, setFilter] = useState<UserWaiverFilter>({
        page: 1,
        pageSize: 25,
    });
    const [searchTerm, setSearchTerm] = useState('');

    // Fetch compliance summary
    const { data: summary } = useQuery({
        queryKey: GetComplianceSummary().key,
        queryFn: GetComplianceSummary().service,
        select: (res) => res.data,
    });

    // Fetch active waivers for filter dropdown
    const { data: waiverVersions } = useQuery({
        queryKey: GetActiveWaiverVersions().key,
        queryFn: GetActiveWaiverVersions().service,
        select: (res) => res.data,
    });

    // Fetch user waivers with filtering
    const { data: waiversResult, refetch: refetchWaivers } = useQuery({
        queryKey: [...GetUserWaivers().key, filter],
        queryFn: () => GetUserWaivers().service(filter),
        select: (res) => res.data,
    });

    // Export mutation
    const exportMutation = useMutation({
        mutationKey: ExportWaivers().key,
        mutationFn: ExportWaivers().service,
        onSuccess: (response) => {
            // Create download link for the blob
            const blob = response.data;
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `waiver-export-${new Date().toISOString().slice(0, 10)}.csv`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        },
    });

    const handleSearch = () => {
        setFilter((prev) => ({ ...prev, searchTerm, page: 1 }));
    };

    const handleFilterChange = (key: keyof UserWaiverFilter, value: string | boolean | undefined) => {
        setFilter((prev) => ({ ...prev, [key]: value, page: 1 }));
    };

    const handlePageChange = (newPage: number) => {
        setFilter((prev) => ({ ...prev, page: newPage }));
    };

    const handleExport = () => {
        exportMutation.mutate(filter);
    };

    return (
        <div className='space-y-6'>
            {/* Summary Cards */}
            <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Total Active Users</CardTitle>
                        <Users className='h-4 w-4 text-muted-foreground' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{summary?.totalActiveUsers ?? '-'}</div>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Compliance Rate</CardTitle>
                        <CheckCircle className='h-4 w-4 text-green-600' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{summary?.compliancePercentage ?? 0}%</div>
                        <p className='text-xs text-muted-foreground'>
                            {summary?.usersWithValidWaivers ?? 0} users with valid waivers
                        </p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>Expiring Soon</CardTitle>
                        <Clock className='h-4 w-4 text-amber-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{summary?.usersWithExpiringWaivers ?? 0}</div>
                        <p className='text-xs text-muted-foreground'>Within 30 days</p>
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader className='flex flex-row items-center justify-between space-y-0 pb-2'>
                        <CardTitle className='text-sm font-medium'>No Waiver</CardTitle>
                        <AlertTriangle className='h-4 w-4 text-red-500' />
                    </CardHeader>
                    <CardContent>
                        <div className='text-2xl font-bold'>{summary?.usersWithoutWaivers ?? 0}</div>
                        <p className='text-xs text-muted-foreground'>Users without valid waivers</p>
                    </CardContent>
                </Card>
            </div>

            {/* Signing Method Stats */}
            <Card>
                <CardHeader>
                    <CardTitle>Signing Methods</CardTitle>
                    <CardDescription>Breakdown of how waivers were signed</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className='grid gap-4 md:grid-cols-3'>
                        <div className='flex items-center gap-2'>
                            <FileText className='h-5 w-5 text-blue-500' />
                            <div>
                                <p className='text-sm font-medium'>E-Signature</p>
                                <p className='text-2xl font-bold'>{summary?.eSignatureCount ?? 0}</p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <FileText className='h-5 w-5 text-orange-500' />
                            <div>
                                <p className='text-sm font-medium'>Paper Upload</p>
                                <p className='text-2xl font-bold'>{summary?.paperUploadCount ?? 0}</p>
                            </div>
                        </div>
                        <div className='flex items-center gap-2'>
                            <Users className='h-5 w-5 text-purple-500' />
                            <div>
                                <p className='text-sm font-medium'>Minor Waivers</p>
                                <p className='text-2xl font-bold'>{summary?.minorWaiversCount ?? 0}</p>
                            </div>
                        </div>
                    </div>
                </CardContent>
            </Card>

            {/* Waiver Records */}
            <Card>
                <CardHeader>
                    <div className='flex items-center justify-between'>
                        <div>
                            <CardTitle>Signed Waivers</CardTitle>
                            <CardDescription>{waiversResult?.totalCount ?? 0} total records</CardDescription>
                        </div>
                        <Button onClick={handleExport} disabled={exportMutation.isPending}>
                            <Download className='mr-2 h-4 w-4' />
                            {exportMutation.isPending ? 'Exporting...' : 'Export CSV'}
                        </Button>
                    </div>
                </CardHeader>
                <CardContent>
                    {/* Filters */}
                    <div className='mb-4 grid gap-4 md:grid-cols-4'>
                        <div className='md:col-span-2'>
                            <Label htmlFor='search'>Search</Label>
                            <div className='flex gap-2'>
                                <Input
                                    id='search'
                                    placeholder='Search by name or email...'
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                                />
                                <Button variant='outline' onClick={handleSearch}>
                                    <Search className='h-4 w-4' />
                                </Button>
                            </div>
                        </div>
                        <div>
                            <Label>Waiver Version</Label>
                            <Select
                                value={filter.waiverVersionId || 'all'}
                                onValueChange={(v) =>
                                    handleFilterChange('waiverVersionId', v === 'all' ? undefined : v)
                                }
                            >
                                <SelectTrigger>
                                    <SelectValue placeholder='All versions' />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value='all'>All versions</SelectItem>
                                    {(waiverVersions || []).map((wv) => (
                                        <SelectItem key={wv.id} value={wv.id}>
                                            {wv.name} v{wv.version}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>
                        <div>
                            <Label>Status</Label>
                            <Select
                                value={filter.isValid === undefined ? 'all' : filter.isValid ? 'valid' : 'expired'}
                                onValueChange={(v) =>
                                    handleFilterChange('isValid', v === 'all' ? undefined : v === 'valid')
                                }
                            >
                                <SelectTrigger>
                                    <SelectValue placeholder='All' />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value='all'>All</SelectItem>
                                    <SelectItem value='valid'>Valid</SelectItem>
                                    <SelectItem value='expired'>Expired</SelectItem>
                                </SelectContent>
                            </Select>
                        </div>
                    </div>

                    {/* Table */}
                    <div className='overflow-auto'>
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>User</TableHead>
                                    <TableHead>Waiver</TableHead>
                                    <TableHead>Signed As</TableHead>
                                    <TableHead>Method</TableHead>
                                    <TableHead>Accepted</TableHead>
                                    <TableHead>Expires</TableHead>
                                    <TableHead>Status</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {(waiversResult?.items || []).map((waiver) => (
                                    <TableRow key={waiver.id}>
                                        <TableCell>
                                            <div>
                                                <p className='font-medium'>{waiver.userName}</p>
                                                <p className='text-sm text-muted-foreground'>{waiver.userEmail}</p>
                                            </div>
                                        </TableCell>
                                        <TableCell>
                                            {waiver.waiverName} v{waiver.waiverVersion}
                                        </TableCell>
                                        <TableCell>
                                            <div>
                                                <p>{waiver.typedLegalName}</p>
                                                {waiver.isMinor ? (
                                                    <Badge variant='outline' className='text-xs'>
                                                        Minor
                                                    </Badge>
                                                ) : null}
                                            </div>
                                        </TableCell>
                                        <TableCell>
                                            <Badge
                                                variant='outline'
                                                className={
                                                    waiver.signingMethod === 'PaperUpload'
                                                        ? 'bg-orange-50 text-orange-700'
                                                        : 'bg-blue-50 text-blue-700'
                                                }
                                            >
                                                {waiver.signingMethod === 'PaperUpload' ? 'Paper' : 'E-Sign'}
                                            </Badge>
                                        </TableCell>
                                        <TableCell>{new Date(waiver.acceptedDate).toLocaleDateString()}</TableCell>
                                        <TableCell>{new Date(waiver.expiryDate).toLocaleDateString()}</TableCell>
                                        <TableCell>
                                            {waiver.isValid ? (
                                                <Badge className='bg-green-100 text-green-700'>Valid</Badge>
                                            ) : (
                                                <Badge className='bg-red-100 text-red-700'>Expired</Badge>
                                            )}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </div>

                    {/* Pagination */}
                    {waiversResult && waiversResult.totalPages > 1 ? (
                        <div className='mt-4 flex items-center justify-between'>
                            <p className='text-sm text-muted-foreground'>
                                Page {waiversResult.page} of {waiversResult.totalPages}
                            </p>
                            <div className='flex gap-2'>
                                <Button
                                    variant='outline'
                                    size='sm'
                                    onClick={() => handlePageChange(waiversResult.page - 1)}
                                    disabled={waiversResult.page <= 1}
                                >
                                    Previous
                                </Button>
                                <Button
                                    variant='outline'
                                    size='sm'
                                    onClick={() => handlePageChange(waiversResult.page + 1)}
                                    disabled={waiversResult.page >= waiversResult.totalPages}
                                >
                                    Next
                                </Button>
                            </div>
                        </div>
                    ) : null}
                </CardContent>
            </Card>
        </div>
    );
};
