import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import moment, { type Moment } from 'moment';
import { CalendarClock, ChevronLeft, ChevronRight, Download } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { GetWeeklySalesReport, type WeeklySalesReportDto } from '@/services/sales-reports';

/**
 * Weekly Municipal Sales Pipeline Report (Project 63 Phase 2).
 *
 * Renders the same numbers Cynthia tracks in her spreadsheet — auto-generated
 * from the CRM so the salesperson never double-enters. Free-text "Next Steps"
 * lands in Phase 4.
 *
 * Route: /siteadmin/prospects/reports/weekly
 * SalesRep or SiteAdmin only.
 */

/** Weeks end on Sunday per Cynthia's spreadsheet convention. */
function endOfSalesWeek(anchor: Moment): Moment {
    return anchor.clone().endOf('isoWeek'); // isoWeek ends on Sunday
}

function toDateOnlyString(m: Moment): string {
    return m.format('YYYY-MM-DD');
}

interface MetricCardProps {
    label: string;
    value: number | undefined;
    hint?: string;
}

const MetricCard = ({ label, value, hint }: MetricCardProps) => (
    <Card>
        <CardHeader className='pb-1'>
            <CardTitle className='text-sm font-medium text-muted-foreground'>{label}</CardTitle>
        </CardHeader>
        <CardContent>
            <div className='text-2xl font-bold tabular-nums'>{value ?? '—'}</div>
            {hint ? <p className='text-xs text-muted-foreground'>{hint}</p> : null}
        </CardContent>
    </Card>
);

const FEEDBACK_EMPTY = 'No feedback captured for this window.';

function buildCsv(report: WeeklySalesReportDto): string {
    const rows: string[][] = [
        ['Metric', 'Value'],
        ['Period start', report.periodStart],
        ['Period end', report.periodEnd],
        ['Prospects researched', String(report.prospectsResearched)],
        ['New contacts added', String(report.newContactsAdded)],
        ['Outreach touches', String(report.outreachTouches)],
        ['Follow-up touches', String(report.followUpTouches)],
        ['Responses', String(report.responses)],
        ['Meetings requested', String(report.meetingsRequested)],
        ['Meetings scheduled', String(report.meetingsScheduled)],
        ['Meetings held', String(report.meetingsHeld)],
        [],
        ['Key Municipal Feedback'],
        ...(report.keyMunicipalFeedback.length ? report.keyMunicipalFeedback.map((f) => [f]) : [[FEEDBACK_EMPTY]]),
        [],
        ['Pricing / Business Model Feedback'],
        ...(report.pricingFeedback.length ? report.pricingFeedback.map((f) => [f]) : [[FEEDBACK_EMPTY]]),
    ];

    const escape = (cell: string) => {
        // Wrap in quotes and double any embedded quotes so Excel round-trips cleanly.
        const needsQuoting = /[,"\r\n]/.test(cell);
        const escaped = cell.replace(/"/g, '""');
        return needsQuoting ? `"${escaped}"` : escaped;
    };

    return rows.map((r) => r.map(escape).join(',')).join('\r\n');
}

function downloadCsv(report: WeeklySalesReportDto) {
    const csv = buildCsv(report);
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `weekly-sales-report-${report.periodEnd.substring(0, 10)}.csv`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

export const SiteAdminProspectWeeklyReport = () => {
    const [weekEnding, setWeekEnding] = useState<Moment>(() => endOfSalesWeek(moment()));

    const weekEndingStr = useMemo(() => toDateOnlyString(weekEnding), [weekEnding]);

    const { data: report, isLoading } = useQuery({
        queryKey: GetWeeklySalesReport({ weekEnding: weekEndingStr }).key,
        queryFn: GetWeeklySalesReport({ weekEnding: weekEndingStr }).service,
        select: (res) => res.data,
    });

    const isThisWeek = weekEnding.isSame(endOfSalesWeek(moment()), 'day');

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-col gap-3 md:flex-row md:items-center md:justify-between'>
                    <div>
                        <CardTitle>Weekly Sales Report</CardTitle>
                        <CardDescription>
                            Week of {weekEnding.clone().startOf('isoWeek').format('MMM D')} –{' '}
                            {weekEnding.format('MMM D, YYYY')}
                        </CardDescription>
                    </div>
                    <div className='flex flex-wrap items-center gap-2'>
                        <Button
                            variant='outline'
                            size='icon'
                            onClick={() => setWeekEnding((w) => w.clone().subtract(7, 'days'))}
                            aria-label='Previous week'
                        >
                            <ChevronLeft className='h-4 w-4' />
                        </Button>
                        <Input
                            type='date'
                            value={weekEndingStr}
                            onChange={(e) => {
                                const v = e.target.value;
                                if (!v) return;
                                setWeekEnding(endOfSalesWeek(moment(v, 'YYYY-MM-DD')));
                            }}
                            className='w-40'
                        />
                        <Button
                            variant='outline'
                            size='icon'
                            onClick={() => setWeekEnding((w) => w.clone().add(7, 'days'))}
                            disabled={isThisWeek}
                            aria-label='Next week'
                        >
                            <ChevronRight className='h-4 w-4' />
                        </Button>
                        <Button
                            variant='outline'
                            size='sm'
                            onClick={() => setWeekEnding(endOfSalesWeek(moment()))}
                            disabled={isThisWeek}
                        >
                            <CalendarClock className='mr-1 h-4 w-4' />
                            This week
                        </Button>
                        <Button
                            variant='outline'
                            size='sm'
                            onClick={() => report && downloadCsv(report)}
                            disabled={!report}
                        >
                            <Download className='mr-1 h-4 w-4' />
                            Export CSV
                        </Button>
                    </div>
                </CardHeader>
            </Card>

            {isLoading && !report ? (
                <Card>
                    <CardContent className='py-8 text-sm text-muted-foreground'>Loading report…</CardContent>
                </Card>
            ) : (
                <>
                    <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-4'>
                        <MetricCard label='Prospects researched' value={report?.prospectsResearched} />
                        <MetricCard label='New contacts added' value={report?.newContactsAdded} />
                        <MetricCard label='Outreach touches' value={report?.outreachTouches} />
                        <MetricCard label='Follow-up touches' value={report?.followUpTouches} />
                        <MetricCard label='Responses' value={report?.responses} />
                        <MetricCard label='Meetings requested' value={report?.meetingsRequested} />
                        <MetricCard label='Meetings scheduled' value={report?.meetingsScheduled} />
                        <MetricCard label='Meetings held' value={report?.meetingsHeld} />
                    </div>

                    <div className='grid gap-4 md:grid-cols-2'>
                        <Card>
                            <CardHeader>
                                <CardTitle>Key Municipal Feedback</CardTitle>
                                <CardDescription>
                                    Objections and questions captured on prospects touched this week.
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                {(report?.keyMunicipalFeedback ?? []).length === 0 ? (
                                    <p className='text-sm text-muted-foreground'>{FEEDBACK_EMPTY}</p>
                                ) : (
                                    <ul className='list-disc space-y-1 pl-5 text-sm'>
                                        {report!.keyMunicipalFeedback.map((f) => (
                                            <li key={f}>{f}</li>
                                        ))}
                                    </ul>
                                )}
                            </CardContent>
                        </Card>
                        <Card>
                            <CardHeader>
                                <CardTitle>Pricing / Business Model Feedback</CardTitle>
                                <CardDescription>
                                    What the salesperson heard about budget, pricing, or procurement.
                                </CardDescription>
                            </CardHeader>
                            <CardContent>
                                {(report?.pricingFeedback ?? []).length === 0 ? (
                                    <p className='text-sm text-muted-foreground'>{FEEDBACK_EMPTY}</p>
                                ) : (
                                    <ul className='list-disc space-y-1 pl-5 text-sm'>
                                        {report!.pricingFeedback.map((f) => (
                                            <li key={f}>{f}</li>
                                        ))}
                                    </ul>
                                )}
                            </CardContent>
                        </Card>
                    </div>
                </>
            )}
        </div>
    );
};
