import { ColumnDef } from '@tanstack/react-table';
import { Link } from 'react-router';
import { DataTableColumnHeader } from '@/components/ui/data-table';
import { DonorLifecycleBadge, EngagementScoreBadge } from '@/components/contacts/contact-constants';
import { type ContactEngagementScoreData } from '@/services/contacts';

function formatDate(dateStr: string | null): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString();
}

function getLastActivity(score: ContactEngagementScoreData): string {
    const dates = [score.lastDonationDate, score.lastInteractionDate].filter(Boolean) as string[];
    if (dates.length === 0) return '—';
    const latest = dates.sort().reverse()[0];
    return formatDate(latest);
}

export const engagementColumns: ColumnDef<ContactEngagementScoreData>[] = [
    {
        accessorKey: 'contactName',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
        cell: ({ row }) => (
            <Link
                to={`/siteadmin/contacts/${row.original.contactId}`}
                className='font-medium hover:underline'
            >
                {row.original.contactName || '—'}
            </Link>
        ),
    },
    {
        accessorKey: 'engagementScore',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Score' />,
        cell: ({ row }) => <EngagementScoreBadge score={row.original.engagementScore} />,
    },
    {
        accessorKey: 'donorLifecycleStage',
        header: 'Lifecycle',
        cell: ({ row }) => <DonorLifecycleBadge stage={row.original.donorLifecycleStage} />,
    },
    {
        accessorKey: 'totalDonations',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Total Donations' />,
        cell: ({ row }) =>
            row.original.totalDonations > 0
                ? `$${row.original.totalDonations.toLocaleString(undefined, { minimumFractionDigits: 2 })}`
                : '—',
    },
    {
        accessorKey: 'eventsAttended',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Events' />,
        cell: ({ row }) => (row.original.eventsAttended > 0 ? row.original.eventsAttended : '—'),
    },
    {
        id: 'lastActivity',
        header: 'Last Activity',
        cell: ({ row }) => getLastActivity(row.original),
    },
];
