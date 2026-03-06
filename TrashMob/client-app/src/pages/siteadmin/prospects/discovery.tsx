import { useState } from 'react';
import { Link, useNavigate } from 'react-router';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Sparkles, MapPin, RefreshCcw, Loader2, Search } from 'lucide-react';
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
            if (prospect.contactName) params.set('contactName', prospect.contactName);
            if (prospect.contactEmail) params.set('contactEmail', prospect.contactEmail);
            if (prospect.contactTitle) params.set('contactTitle', prospect.contactTitle);
            return (
                <Button variant='outline' size='sm' asChild>
                    <Link to={`/siteadmin/prospects/create?${params.toString()}`}>Add to Pipeline</Link>
                </Button>
            );
        },
    },
];

const GapResearchButton = ({ gap }: { gap: GeographicGapData }) => {
    const navigate = useNavigate();
    const { toast } = useToast();

    const research = useMutation({
        mutationKey: ['/communityprospects', 'gap-research', gap.city, gap.region],
        mutationFn: DiscoverProspects().service,
        onSuccess: (res) => {
            const params = new URLSearchParams();
            params.set('city', gap.city);
            if (gap.region) params.set('region', gap.region);
            if (gap.country) params.set('country', gap.country);
            if (gap.averageLatitude != null) params.set('latitude', String(gap.averageLatitude));
            if (gap.averageLongitude != null) params.set('longitude', String(gap.averageLongitude));

            const prospect = res.data?.prospects?.[0];
            if (prospect) {
                params.set('name', prospect.name || gap.city);
                if (prospect.type) params.set('type', prospect.type);
                if (prospect.estimatedPopulation) params.set('population', String(prospect.estimatedPopulation));
                if (prospect.website) params.set('website', prospect.website);
                if (prospect.contactName) params.set('contactName', prospect.contactName);
                if (prospect.contactEmail) params.set('contactEmail', prospect.contactEmail);
                if (prospect.contactTitle) params.set('contactTitle', prospect.contactTitle);

                const notes: string[] = [];
                if (prospect.rationale) notes.push(`AI Research: ${prospect.rationale}`);
                if (prospect.contactSuggestion) notes.push(`Suggested Contact: ${prospect.contactSuggestion}`);
                if (prospect.contactName || prospect.contactEmail) {
                    const contactParts = [prospect.contactName, prospect.contactTitle, prospect.contactEmail]
                        .filter(Boolean)
                        .join(' | ');
                    notes.push(`AI-Discovered Contact: ${contactParts} — verify before sending outreach`);
                }
                notes.push(
                    `Source: Geographic gap with ${gap.eventCount} events, nearest partner ${gap.nearestPartnerDistanceMiles != null ? `${gap.nearestPartnerDistanceMiles} mi` : 'none'}`,
                );
                params.set('notes', notes.join('\n\n'));
            } else {
                params.set('name', gap.city);
                params.set('type', 'Municipality');
                params.set(
                    'notes',
                    `Geographic gap with ${gap.eventCount} events, nearest partner ${gap.nearestPartnerDistanceMiles != null ? `${gap.nearestPartnerDistanceMiles} mi` : 'none'}. AI research returned no results.`,
                );
            }

            navigate(`/siteadmin/prospects/create?${params.toString()}`);
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Research failed. Adding with basic info.' });
            const params = new URLSearchParams();
            params.set('name', gap.city);
            params.set('city', gap.city);
            if (gap.region) params.set('region', gap.region);
            if (gap.country) params.set('country', gap.country);
            if (gap.averageLatitude != null) params.set('latitude', String(gap.averageLatitude));
            if (gap.averageLongitude != null) params.set('longitude', String(gap.averageLongitude));
            params.set('type', 'Municipality');
            navigate(`/siteadmin/prospects/create?${params.toString()}`);
        },
    });

    return (
        <Button
            variant='outline'
            size='sm'
            disabled={research.isPending}
            onClick={() =>
                research.mutate({
                    city: gap.city,
                    region: gap.region,
                    country: gap.country || 'United States',
                    maxResults: 1,
                })
            }
        >
            {research.isPending ? (
                <>
                    <Loader2 className='mr-1 h-3 w-3 animate-spin' /> Researching...
                </>
            ) : (
                <>
                    <Search className='mr-1 h-3 w-3' /> Research & Add
                </>
            )}
        </Button>
    );
};

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
    {
        id: 'status',
        header: 'Status',
        cell: ({ row }) => {
            const gap = row.original;
            if (!gap.existingProspectId) return null;
            return (
                <Link
                    to={`/siteadmin/prospects/${gap.existingProspectId}`}
                    className='inline-flex items-center rounded-full bg-teal-100 px-2.5 py-0.5 text-xs font-medium text-teal-800 hover:bg-teal-200'
                >
                    In Pipeline
                </Link>
            );
        },
    },
    {
        id: 'actions',
        header: '',
        cell: ({ row }) => <GapResearchButton gap={row.original} />,
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
