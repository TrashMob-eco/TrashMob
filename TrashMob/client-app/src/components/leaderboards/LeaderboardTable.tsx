import { Trophy, Medal, Award, CircleUserRound } from 'lucide-react';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { LeaderboardEntry } from '@/components/Models/LeaderboardData';

interface LeaderboardTableProps {
    entries: LeaderboardEntry[];
    isLoading?: boolean;
    entityType?: 'users' | 'teams';
}

const getRankIcon = (rank: number) => {
    switch (rank) {
        case 1:
            return <Trophy className='h-5 w-5 text-yellow-500' />;
        case 2:
            return <Medal className='h-5 w-5 text-gray-400' />;
        case 3:
            return <Award className='h-5 w-5 text-amber-600' />;
        default:
            return null;
    }
};

const getRankBadgeClass = (rank: number): string => {
    switch (rank) {
        case 1:
            return 'bg-yellow-100 text-yellow-800 border-yellow-300';
        case 2:
            return 'bg-gray-100 text-gray-700 border-gray-300';
        case 3:
            return 'bg-amber-100 text-amber-800 border-amber-300';
        default:
            return 'bg-muted text-muted-foreground';
    }
};

export const LeaderboardTable = ({ entries, isLoading, entityType = 'users' }: LeaderboardTableProps) => {
    if (isLoading) {
        return (
            <div className='flex justify-center items-center py-12'>
                <div className='animate-spin rounded-full h-8 w-8 border-b-2 border-primary' />
            </div>
        );
    }

    if (!entries || entries.length === 0) {
        return (
            <div className='text-center py-12 text-muted-foreground'>
                <p>No leaderboard data available for the selected criteria.</p>
                <p className='text-sm mt-2'>Try selecting a different time range or check back later.</p>
            </div>
        );
    }

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead className='w-20'>Rank</TableHead>
                    <TableHead>{entityType === 'teams' ? 'Team' : 'Volunteer'}</TableHead>
                    <TableHead className='hidden sm:table-cell'>Location</TableHead>
                    <TableHead className='text-right'>Score</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {entries.map((entry) => (
                    <TableRow key={entry.entityId} className={entry.rank <= 3 ? 'bg-muted/30' : ''}>
                        <TableCell>
                            <div className='flex items-center gap-2'>
                                {getRankIcon(entry.rank)}
                                <span
                                    className={`inline-flex items-center justify-center w-8 h-8 rounded-full text-sm font-semibold border ${getRankBadgeClass(entry.rank)}`}
                                >
                                    {entry.rank}
                                </span>
                            </div>
                        </TableCell>
                        <TableCell className='font-medium'>
                            <span className='flex items-center gap-2'>
                                {entry.profilePhotoUrl ? (
                                    <img
                                        src={entry.profilePhotoUrl}
                                        alt={entry.entityName}
                                        className='h-6 w-6 rounded-full object-cover'
                                        referrerPolicy='no-referrer'
                                    />
                                ) : (
                                    <CircleUserRound className='h-6 w-6 text-muted-foreground' />
                                )}
                                {entry.entityName}
                            </span>
                        </TableCell>
                        <TableCell className='hidden sm:table-cell text-muted-foreground'>
                            {entry.city && entry.region
                                ? `${entry.city}, ${entry.region}`
                                : entry.region || entry.city || '-'}
                        </TableCell>
                        <TableCell className='text-right font-semibold'>{entry.formattedScore}</TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    );
};
