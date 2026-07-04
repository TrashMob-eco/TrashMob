import { useEffect, useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import moment, { type Moment } from 'moment';
import { CalendarClock, ChevronLeft, ChevronRight, Check, Save } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { useToast } from '@/hooks/use-toast';
import { getErrorMessage } from '@/lib/api-errors';
import {
    GetMonthlySalesReport,
    UpdateMonthlyTargets,
    UpsertMonthlyNarrative,
    type MonthlySalesMetricDto,
    type MarketIntelligenceCountDto,
} from '@/services/sales-reports';
import { SalesReportNarrativePanel } from '@/components/prospects/sales-report-narrative-panel';

/**
 * Monthly Municipal Sales Pipeline Report (Project 63 Phase 3).
 *
 * Renders the 7 tracked metrics with per-month targets, target-vs-actual
 * status pills, an inline "edit targets" mode, and a Market Intelligence
 * Notes section below (best-responding departments, common objections,
 * pricing feedback). NextMonthPriority persistence lands in Phase 4.
 *
 * Route: /siteadmin/prospects/reports/monthly
 * SalesRep or SiteAdmin only.
 */

function toMonthString(m: Moment): string {
    return m.format('YYYY-MM-01');
}

function StatusBadge({ status }: { status: MonthlySalesMetricDto['status'] }) {
    switch (status) {
        case 'Exceeded':
            return <Badge variant='success'>Exceeded</Badge>;
        case 'OnTrack':
            return <Badge>On track</Badge>;
        case 'Behind':
            return <Badge variant='destructive'>Behind</Badge>;
        default:
            return <Badge variant='outline'>No target</Badge>;
    }
}

function MarketIntelligenceCard({
    title,
    description,
    rows,
    emptyMessage,
}: {
    title: string;
    description: string;
    rows: MarketIntelligenceCountDto[];
    emptyMessage: string;
}) {
    return (
        <Card>
            <CardHeader>
                <CardTitle>{title}</CardTitle>
                <CardDescription>{description}</CardDescription>
            </CardHeader>
            <CardContent>
                {rows.length === 0 ? (
                    <p className='text-sm text-muted-foreground'>{emptyMessage}</p>
                ) : (
                    <ul className='divide-y'>
                        {rows.map((row) => (
                            <li key={row.label} className='flex items-center justify-between py-2 text-sm'>
                                <span>{row.label}</span>
                                <Badge variant='outline'>{row.count}</Badge>
                            </li>
                        ))}
                    </ul>
                )}
            </CardContent>
        </Card>
    );
}

export const SiteAdminProspectMonthlyReport = () => {
    const { toast } = useToast();
    const queryClient = useQueryClient();
    const [monthAnchor, setMonthAnchor] = useState<Moment>(() => moment().startOf('month'));
    const [editing, setEditing] = useState(false);
    const [draftTargets, setDraftTargets] = useState<Record<number, string>>({});

    const monthStr = useMemo(() => toMonthString(monthAnchor), [monthAnchor]);
    const isThisMonth = monthAnchor.isSame(moment().startOf('month'), 'day');

    const { data: report, isLoading } = useQuery({
        queryKey: GetMonthlySalesReport({ month: monthStr }).key,
        queryFn: GetMonthlySalesReport({ month: monthStr }).service,
        select: (res) => res.data,
    });

    useEffect(() => {
        // Reset the draft whenever the loaded report changes (new month or refetch).
        if (report) {
            const initial: Record<number, string> = {};
            for (const m of report.metrics) {
                initial[m.metric] = String(m.target);
            }
            setDraftTargets(initial);
            setEditing(false);
        }
    }, [report]);

    const saveTargets = useMutation({
        mutationKey: UpdateMonthlyTargets({ month: monthStr }).key,
        mutationFn: UpdateMonthlyTargets({ month: monthStr }).service,
        onSuccess: () => {
            toast({ variant: 'primary', title: 'Targets saved' });
            queryClient.invalidateQueries({ queryKey: GetMonthlySalesReport({ month: monthStr }).key });
            setEditing(false);
        },
        onError: (error: Error) => {
            toast({ variant: 'destructive', title: 'Save failed', description: getErrorMessage(error) });
        },
    });

    const handleSave = () => {
        if (!report) return;
        const updates = report.metrics
            .map((m) => {
                const draft = draftTargets[m.metric];
                const parsed = parseInt(draft ?? '', 10);
                if (Number.isNaN(parsed) || parsed < 0) return null;
                if (parsed === m.target) return null;
                return { metric: m.metric, target: parsed };
            })
            .filter((u): u is { metric: number; target: number } => u !== null);

        if (updates.length === 0) {
            toast({ title: 'No changes to save' });
            setEditing(false);
            return;
        }

        saveTargets.mutate({ targets: updates });
    };

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-col gap-3 md:flex-row md:items-center md:justify-between'>
                    <div>
                        <CardTitle>Monthly Sales Report</CardTitle>
                        <CardDescription>{monthAnchor.format('MMMM YYYY')}</CardDescription>
                    </div>
                    <div className='flex flex-wrap items-center gap-2'>
                        <Button
                            variant='outline'
                            size='icon'
                            onClick={() => setMonthAnchor((m) => m.clone().subtract(1, 'month'))}
                            aria-label='Previous month'
                        >
                            <ChevronLeft className='h-4 w-4' />
                        </Button>
                        <Input
                            type='month'
                            value={monthAnchor.format('YYYY-MM')}
                            onChange={(e) => {
                                const v = e.target.value;
                                if (!v) return;
                                setMonthAnchor(moment(`${v}-01`, 'YYYY-MM-DD'));
                            }}
                            className='w-40'
                        />
                        <Button
                            variant='outline'
                            size='icon'
                            onClick={() => setMonthAnchor((m) => m.clone().add(1, 'month'))}
                            disabled={isThisMonth}
                            aria-label='Next month'
                        >
                            <ChevronRight className='h-4 w-4' />
                        </Button>
                        <Button
                            variant='outline'
                            size='sm'
                            onClick={() => setMonthAnchor(moment().startOf('month'))}
                            disabled={isThisMonth}
                        >
                            <CalendarClock className='mr-1 h-4 w-4' />
                            This month
                        </Button>
                    </div>
                </CardHeader>
            </Card>

            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle>Goals</CardTitle>
                        <CardDescription>
                            Defaults come from Cynthia's baseline (20 / 20 / 15 / 10 / 3 / 2 / 1). Edit any row to
                            override for this month; other months are unaffected.
                        </CardDescription>
                    </div>
                    {editing ? (
                        <div className='flex items-center gap-2'>
                            <Button variant='ghost' size='sm' onClick={() => setEditing(false)}>
                                Cancel
                            </Button>
                            <Button size='sm' onClick={handleSave} disabled={saveTargets.isPending}>
                                <Save className='mr-1 h-4 w-4' />
                                Save targets
                            </Button>
                        </div>
                    ) : (
                        <Button variant='outline' size='sm' onClick={() => setEditing(true)} disabled={!report}>
                            <Check className='mr-1 h-4 w-4' />
                            Edit targets
                        </Button>
                    )}
                </CardHeader>
                <CardContent>
                    {isLoading && !report ? (
                        <p className='text-sm text-muted-foreground'>Loading report…</p>
                    ) : (
                        <div className='overflow-x-auto'>
                            <table className='min-w-full text-sm'>
                                <thead>
                                    <tr className='border-b text-left text-muted-foreground'>
                                        <th className='py-2 pr-4 font-medium'>Metric</th>
                                        <th className='py-2 pr-4 font-medium'>Target</th>
                                        <th className='py-2 pr-4 font-medium'>Actual</th>
                                        <th className='py-2 pr-4 font-medium'>Status</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {report?.metrics.map((m) => (
                                        <tr key={m.metric} className='border-b last:border-b-0'>
                                            <td className='py-2 pr-4 font-medium'>{m.label}</td>
                                            <td className='py-2 pr-4 tabular-nums'>
                                                {editing ? (
                                                    <Input
                                                        type='number'
                                                        min={0}
                                                        value={draftTargets[m.metric] ?? ''}
                                                        onChange={(e) =>
                                                            setDraftTargets((prev) => ({
                                                                ...prev,
                                                                [m.metric]: e.target.value,
                                                            }))
                                                        }
                                                        className='h-8 w-20'
                                                    />
                                                ) : (
                                                    m.target
                                                )}
                                            </td>
                                            <td className='py-2 pr-4 tabular-nums'>{m.actual}</td>
                                            <td className='py-2 pr-4'>
                                                <StatusBadge status={m.status} />
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </CardContent>
            </Card>

            <div className='grid gap-4 md:grid-cols-3'>
                <MarketIntelligenceCard
                    title='Best responding departments'
                    description='Departments whose prospects responded this month.'
                    rows={report?.bestRespondingDepartments ?? []}
                    emptyMessage='No responses recorded this month.'
                />
                <MarketIntelligenceCard
                    title='Common objections'
                    description='Top objections captured across touched prospects.'
                    rows={report?.commonObjections ?? []}
                    emptyMessage='No objections captured yet.'
                />
                <MarketIntelligenceCard
                    title='Pricing feedback'
                    description='Budget, pricing, and procurement signals.'
                    rows={report?.pricingFeedback ?? []}
                    emptyMessage='No pricing feedback captured yet.'
                />
            </div>

            <SalesReportNarrativePanel
                title='Recommended next-month priority'
                description='What the salesperson recommends focusing on next month. Auto-saves when you click away.'
                placeholder='e.g. Move all Bay Area meetings into scheduled; open a NorCal region tier.'
                persistedValue={report?.nextMonthPriority ?? null}
                mutationKey={UpsertMonthlyNarrative({ month: monthStr }).key}
                save={(value) => UpsertMonthlyNarrative({ month: monthStr }).service({ nextMonthPriority: value })}
                invalidateQueryKey={GetMonthlySalesReport({ month: monthStr }).key}
            />
        </div>
    );
};
