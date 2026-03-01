import { useState } from 'react';
import { Link } from 'react-router';
import { useMutation } from '@tanstack/react-query';
import { Sparkles, Loader2 } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Checkbox } from '@/components/ui/checkbox';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { DataTable } from '@/components/ui/data-table';
import { useToast } from '@/hooks/use-toast';
import { DiscoverGrants, type DiscoverGrants_Body, type DiscoveredGrantData } from '@/services/grants';
import type { ColumnDef } from '@tanstack/react-table';

const FOCUS_AREAS = [
    'Environmental Cleanup',
    'Conservation',
    'Community Development',
    'Youth Engagement',
    'Environmental Justice',
    'Climate Action',
];

const formatAmount = (min: number | null, max: number | null) => {
    if (min != null && max != null) return `$${min.toLocaleString()} – $${max.toLocaleString()}`;
    if (min != null) return `$${min.toLocaleString()}+`;
    if (max != null) return `Up to $${max.toLocaleString()}`;
    return '—';
};

const discoveredColumns: ColumnDef<DiscoveredGrantData>[] = [
    { accessorKey: 'funderName', header: 'Funder' },
    { accessorKey: 'programName', header: 'Program' },
    {
        id: 'amount',
        header: 'Amount Range',
        cell: ({ row }) => formatAmount(row.original.amountMin, row.original.amountMax),
    },
    {
        accessorKey: 'deadline',
        header: 'Deadline',
        cell: ({ getValue }) => {
            const val = getValue<string | null>();
            return val ? new Date(val).toLocaleDateString() : '—';
        },
    },
    {
        accessorKey: 'url',
        header: 'URL',
        cell: ({ getValue }) => {
            const url = getValue<string | null>();
            return url ? (
                <a href={url} target='_blank' rel='noopener noreferrer' className='text-primary hover:underline'>
                    Link
                </a>
            ) : (
                '—'
            );
        },
    },
    { accessorKey: 'rationale', header: 'Rationale' },
    {
        id: 'actions',
        header: '',
        cell: ({ row }) => {
            const g = row.original;
            const params = new URLSearchParams();
            if (g.funderName) params.set('funderName', g.funderName);
            if (g.programName) params.set('programName', g.programName);
            if (g.description) params.set('description', g.description);
            if (g.amountMin != null) params.set('amountMin', String(g.amountMin));
            if (g.amountMax != null) params.set('amountMax', String(g.amountMax));
            if (g.deadline) params.set('submissionDeadline', g.deadline.split('T')[0]);
            if (g.url) params.set('grantUrl', g.url);
            if (g.eligibilityNotes) params.set('notes', g.eligibilityNotes);
            return (
                <Button variant='outline' size='sm' asChild>
                    <Link to={`/siteadmin/grants/create?${params.toString()}`}>Add to Pipeline</Link>
                </Button>
            );
        },
    },
];

export const SiteAdminGrantDiscovery = () => {
    const { toast } = useToast();

    const [queryTab, setQueryTab] = useState('custom');
    const [prompt, setPrompt] = useState('');
    const [selectedAreas, setSelectedAreas] = useState<string[]>([]);
    const [maxResults, setMaxResults] = useState(10);

    const discover = useMutation({
        mutationKey: DiscoverGrants().key,
        mutationFn: DiscoverGrants().service,
        onError: () => toast({ variant: 'destructive', title: 'Discovery failed' }),
    });

    const toggleArea = (area: string) => {
        setSelectedAreas((prev) => (prev.includes(area) ? prev.filter((a) => a !== area) : [...prev, area]));
    };

    function handleDiscover() {
        const body: DiscoverGrants_Body =
            queryTab === 'custom' ? { prompt, maxResults } : { focusAreas: selectedAreas.join(', '), maxResults };
        discover.mutate(body);
    }

    const discoveryResult = discover.data?.data;

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Sparkles className='h-5 w-5' /> AI Grant Discovery
                    </CardTitle>
                </CardHeader>
                <CardContent className='space-y-4'>
                    <Tabs value={queryTab} onValueChange={setQueryTab}>
                        <TabsList>
                            <TabsTrigger value='custom'>Custom Query</TabsTrigger>
                            <TabsTrigger value='focus'>By Focus Area</TabsTrigger>
                        </TabsList>
                        <TabsContent value='custom' className='space-y-3 pt-2'>
                            <div>
                                <Label htmlFor='prompt'>Research Query</Label>
                                <Textarea
                                    id='prompt'
                                    value={prompt}
                                    onChange={(e) => setPrompt(e.target.value)}
                                    rows={3}
                                    placeholder='e.g. "Find EPA grants for community cleanup programs in the Pacific Northwest"'
                                />
                            </div>
                        </TabsContent>
                        <TabsContent value='focus' className='space-y-3 pt-2'>
                            <div>
                                <Label>Focus Areas</Label>
                                <div className='grid grid-cols-2 md:grid-cols-3 gap-3 mt-2'>
                                    {FOCUS_AREAS.map((area) => (
                                        <label key={area} className='flex items-center gap-2 text-sm cursor-pointer'>
                                            <Checkbox
                                                checked={selectedAreas.includes(area)}
                                                onCheckedChange={() => toggleArea(area)}
                                            />
                                            {area}
                                        </label>
                                    ))}
                                </div>
                            </div>
                        </TabsContent>
                    </Tabs>
                    <div className='flex items-center gap-4'>
                        <div className='w-32'>
                            <Label htmlFor='maxResults'>Max Results</Label>
                            <Input
                                id='maxResults'
                                type='number'
                                min={1}
                                max={25}
                                value={maxResults}
                                onChange={(e) => setMaxResults(Number(e.target.value))}
                            />
                        </div>
                        <Button
                            onClick={handleDiscover}
                            disabled={
                                discover.isPending ||
                                (queryTab === 'custom' && !prompt.trim()) ||
                                (queryTab === 'focus' && selectedAreas.length === 0)
                            }
                            className='mt-5'
                        >
                            {discover.isPending ? (
                                <>
                                    <Loader2 className='mr-2 h-4 w-4 animate-spin' /> Discovering...
                                </>
                            ) : (
                                <>
                                    <Sparkles className='mr-2 h-4 w-4' /> Discover
                                </>
                            )}
                        </Button>
                    </div>
                    {discoveryResult?.message ? (
                        <p className='text-sm text-muted-foreground'>{discoveryResult.message}</p>
                    ) : null}
                    {discoveryResult && discoveryResult.grants.length > 0 ? (
                        <div className='space-y-2'>
                            <p className='text-sm text-muted-foreground'>
                                Found {discoveryResult.grants.length} grant opportunities ({discoveryResult.tokensUsed}{' '}
                                tokens used)
                            </p>
                            <DataTable columns={discoveredColumns} data={discoveryResult.grants} />
                        </div>
                    ) : null}
                </CardContent>
            </Card>
        </div>
    );
};
