import { useState } from 'react';
import { Link } from 'react-router';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Sparkles, MapPin, RefreshCcw, Loader2 } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { DataTable } from '@/components/ui/data-table';
import { useToast } from '@/hooks/use-toast';
import {
    DiscoverProspects,
    GetGeographicGaps,
    RescoreAllProspects,
    type DiscoverProspects_Body,
} from '@/services/community-prospects';
import type DiscoveredProspectData from '@/components/Models/DiscoveredProspectData';
import type GeographicGapData from '@/components/Models/GeographicGapData';
import type { ColumnDef } from '@tanstack/react-table';

const discoveredColumns: ColumnDef<DiscoveredProspectData>[] = [
    { accessorKey: 'name', header: 'Name' },
    { accessorKey: 'type', header: 'Type' },
    {
        accessorKey: 'estimatedPopulation',
        header: 'Population',
        cell: ({ getValue }) => {
            const val = getValue<number | null>();
            return val ? val.toLocaleString() : '-';
        },
    },
    {
        accessorKey: 'website',
        header: 'Website',
        cell: ({ getValue }) => {
            const url = getValue<string | null>();
            return url ? (
                <a href={url} target='_blank' rel='noopener noreferrer' className='text-primary hover:underline'>
                    Link
                </a>
            ) : (
                '-'
            );
        },
    },
    { accessorKey: 'contactSuggestion', header: 'Contact Suggestion' },
    { accessorKey: 'rationale', header: 'Rationale' },
    {
        id: 'actions',
        header: '',
        cell: ({ row }) => {
            const prospect = row.original;
            const params = new URLSearchParams();
            if (prospect.name) params.set('name', prospect.name);
            if (prospect.type) params.set('type', prospect.type);
            if (prospect.estimatedPopulation) params.set('population', String(prospect.estimatedPopulation));
            if (prospect.website) params.set('website', prospect.website);
            return (
                <Button variant='outline' size='sm' asChild>
                    <Link to={`/siteadmin/prospects/create?${params.toString()}`}>Add to Pipeline</Link>
                </Button>
            );
        },
    },
];

const gapColumns: ColumnDef<GeographicGapData>[] = [
    { accessorKey: 'city', header: 'City' },
    { accessorKey: 'region', header: 'Region' },
    { accessorKey: 'country', header: 'Country' },
    { accessorKey: 'eventCount', header: 'Events' },
    {
        accessorKey: 'nearestPartnerDistanceMiles',
        header: 'Nearest Partner (mi)',
        cell: ({ getValue }) => {
            const val = getValue<number | null>();
            return val !== null ? `${val} mi` : 'No partners';
        },
    },
];

export const SiteAdminProspectDiscovery = () => {
    const { toast } = useToast();

    // AI Discovery state
    const [queryTab, setQueryTab] = useState('custom');
    const [prompt, setPrompt] = useState('');
    const [city, setCity] = useState('');
    const [region, setRegion] = useState('');
    const [country, setCountry] = useState('');
    const [maxResults, setMaxResults] = useState(10);

    const discover = useMutation({
        mutationKey: DiscoverProspects().key,
        mutationFn: DiscoverProspects().service,
        onError: () => toast({ variant: 'destructive', title: 'Discovery failed' }),
    });

    const rescore = useMutation({
        mutationKey: RescoreAllProspects().key,
        mutationFn: RescoreAllProspects().service,
        onSuccess: (res) => toast({ variant: 'default', title: `Re-scored ${res.data} prospects` }),
        onError: () => toast({ variant: 'destructive', title: 'Re-score failed' }),
    });

    const { data: gaps, isLoading: gapsLoading } = useQuery({
        queryKey: GetGeographicGaps().key,
        queryFn: GetGeographicGaps().service,
        select: (res) => res.data,
    });

    function handleDiscover() {
        const body: DiscoverProspects_Body =
            queryTab === 'custom' ? { prompt, maxResults } : { city, region, country, maxResults };
        discover.mutate(body);
    }

    const discoveryResult = discover.data?.data;

    return (
        <div className='space-y-6'>
            {/* AI Discovery */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Sparkles className='h-5 w-5' /> AI Discovery
                    </CardTitle>
                </CardHeader>
                <CardContent className='space-y-4'>
                    <Tabs value={queryTab} onValueChange={setQueryTab}>
                        <TabsList>
                            <TabsTrigger value='custom'>Custom Query</TabsTrigger>
                            <TabsTrigger value='location'>By Location</TabsTrigger>
                        </TabsList>
                        <TabsContent value='custom' className='space-y-3 pt-2'>
                            <div>
                                <Label htmlFor='prompt'>Research Query</Label>
                                <Textarea
                                    id='prompt'
                                    value={prompt}
                                    onChange={(e) => setPrompt(e.target.value)}
                                    rows={3}
                                    placeholder='e.g. "Give me a list of cities of more than 100,000 people that have adopt-a-street programs on their website"'
                                />
                            </div>
                        </TabsContent>
                        <TabsContent value='location' className='space-y-3 pt-2'>
                            <div className='grid grid-cols-3 gap-3'>
                                <div>
                                    <Label htmlFor='city'>City</Label>
                                    <Input
                                        id='city'
                                        value={city}
                                        onChange={(e) => setCity(e.target.value)}
                                        placeholder='e.g. Seattle'
                                    />
                                </div>
                                <div>
                                    <Label htmlFor='region'>Region / State</Label>
                                    <Input
                                        id='region'
                                        value={region}
                                        onChange={(e) => setRegion(e.target.value)}
                                        placeholder='e.g. WA'
                                    />
                                </div>
                                <div>
                                    <Label htmlFor='country'>Country</Label>
                                    <Input
                                        id='country'
                                        value={country}
                                        onChange={(e) => setCountry(e.target.value)}
                                        placeholder='e.g. United States'
                                    />
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
                            disabled={discover.isPending || (queryTab === 'custom' && !prompt.trim())}
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
                    {discoveryResult && discoveryResult.prospects.length > 0 ? (
                        <div className='space-y-2'>
                            <p className='text-sm text-muted-foreground'>
                                Found {discoveryResult.prospects.length} prospects ({discoveryResult.tokensUsed} tokens
                                used)
                            </p>
                            <DataTable columns={discoveredColumns} data={discoveryResult.prospects} />
                        </div>
                    ) : null}
                </CardContent>
            </Card>

            {/* Geographic Gaps */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <MapPin className='h-5 w-5' /> Geographic Gaps
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    {gapsLoading ? (
                        <p className='text-sm text-muted-foreground'>Loading gaps...</p>
                    ) : (gaps || []).length === 0 ? (
                        <p className='text-sm text-muted-foreground'>No geographic gaps found.</p>
                    ) : (
                        <DataTable columns={gapColumns} data={gaps || []} />
                    )}
                </CardContent>
            </Card>

            {/* Re-score */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <RefreshCcw className='h-5 w-5' /> Scoring
                    </CardTitle>
                </CardHeader>
                <CardContent>
                    <p className='text-sm text-muted-foreground mb-3'>
                        Recalculate FitScores for all prospects based on current event and partner data.
                    </p>
                    <Button variant='outline' onClick={() => rescore.mutate()} disabled={rescore.isPending}>
                        {rescore.isPending ? (
                            <>
                                <Loader2 className='mr-2 h-4 w-4 animate-spin' /> Scoring...
                            </>
                        ) : (
                            'Re-score All Prospects'
                        )}
                    </Button>
                </CardContent>
            </Card>
        </div>
    );
};
