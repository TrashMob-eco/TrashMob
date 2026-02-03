import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { LeaderboardTable } from '@/components/leaderboards/LeaderboardTable';
import { GetLeaderboard, GetLeaderboardOptions, GetMyRank, GetTeamLeaderboard } from '@/services/leaderboards';
import { LeaderboardTypeLabels, TimeRangeLabels } from '@/components/Models/LeaderboardData';
import { useLogin } from '@/hooks/useLogin';
import { Trophy, TrendingUp, Users, Info, Users2 } from 'lucide-react';

export const LeaderboardsPage = () => {
    const { isUserLoaded } = useLogin();
    const [entityType, setEntityType] = useState<'users' | 'teams'>('users');
    const [leaderboardType, setLeaderboardType] = useState('Events');
    const [timeRange, setTimeRange] = useState('Month');

    // Fetch available options
    const { data: optionsResponse } = useQuery({
        queryKey: GetLeaderboardOptions().key,
        queryFn: GetLeaderboardOptions().service,
    });
    const options = optionsResponse?.data;

    // Fetch user leaderboard data
    const { data: userLeaderboardResponse, isLoading: isLoadingUsers } = useQuery({
        queryKey: GetLeaderboard({ type: leaderboardType, timeRange, limit: 50 }).key,
        queryFn: GetLeaderboard({ type: leaderboardType, timeRange, limit: 50 }).service,
        enabled: entityType === 'users',
    });
    const userLeaderboard = userLeaderboardResponse?.data;

    // Fetch team leaderboard data
    const { data: teamLeaderboardResponse, isLoading: isLoadingTeams } = useQuery({
        queryKey: GetTeamLeaderboard({ type: leaderboardType, timeRange, limit: 50 }).key,
        queryFn: GetTeamLeaderboard({ type: leaderboardType, timeRange, limit: 50 }).service,
        enabled: entityType === 'teams',
    });
    const teamLeaderboard = teamLeaderboardResponse?.data;

    // Fetch current user's rank (only when logged in and viewing users)
    const { data: myRankResponse } = useQuery({
        queryKey: GetMyRank({ type: leaderboardType, timeRange }).key,
        queryFn: GetMyRank({ type: leaderboardType, timeRange }).service,
        enabled: isUserLoaded && entityType === 'users',
    });
    const myRank = myRankResponse?.data;

    const currentLeaderboard = entityType === 'users' ? userLeaderboard : teamLeaderboard;
    const isLoading = entityType === 'users' ? isLoadingUsers : isLoadingTeams;

    const formatLastUpdated = (dateStr?: string) => {
        if (!dateStr) return '';
        const date = new Date(dateStr);
        return date.toLocaleDateString('en-US', {
            month: 'short',
            day: 'numeric',
            year: 'numeric',
            hour: 'numeric',
            minute: '2-digit',
        });
    };

    return (
        <div className='container py-8'>
            <div className='mb-8'>
                <h1 className='text-3xl font-bold flex items-center gap-2'>
                    <Trophy className='h-8 w-8 text-yellow-500' />
                    Leaderboards
                </h1>
                <p className='text-muted-foreground mt-2'>
                    See how TrashMob volunteers and teams are making an impact in their communities.
                </p>
            </div>

            {/* Entity Type Tabs */}
            <Tabs value={entityType} onValueChange={(v) => setEntityType(v as 'users' | 'teams')} className='mb-6'>
                <TabsList>
                    <TabsTrigger value='users' className='gap-2'>
                        <Users className='h-4 w-4' />
                        Volunteers
                    </TabsTrigger>
                    <TabsTrigger value='teams' className='gap-2'>
                        <Users2 className='h-4 w-4' />
                        Teams
                    </TabsTrigger>
                </TabsList>
            </Tabs>

            {/* Filters */}
            <div className='flex flex-wrap gap-4 mb-6'>
                <div className='w-48'>
                    <label className='text-sm font-medium mb-1 block'>Leaderboard Type</label>
                    <Select value={leaderboardType} onValueChange={setLeaderboardType}>
                        <SelectTrigger>
                            <SelectValue placeholder='Select type' />
                        </SelectTrigger>
                        <SelectContent>
                            {(options?.types || ['Events', 'Bags', 'Weight', 'Hours']).map((type) => (
                                <SelectItem key={type} value={type}>
                                    {LeaderboardTypeLabels[type] || type}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>

                <div className='w-40'>
                    <label className='text-sm font-medium mb-1 block'>Time Range</label>
                    <Select value={timeRange} onValueChange={setTimeRange}>
                        <SelectTrigger>
                            <SelectValue placeholder='Select range' />
                        </SelectTrigger>
                        <SelectContent>
                            {(options?.timeRanges || ['Week', 'Month', 'Year', 'AllTime']).map((range) => (
                                <SelectItem key={range} value={range}>
                                    {TimeRangeLabels[range] || range}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>
            </div>

            <div className='grid gap-6 lg:grid-cols-4'>
                {/* Sidebar with user rank and stats */}
                <div className='lg:col-span-1 space-y-4'>
                    {/* User's Rank Card (only for volunteers tab) */}
                    {entityType === 'users' && isUserLoaded && myRank ? (
                        <Card>
                            <CardHeader className='pb-3'>
                                <CardTitle className='text-lg flex items-center gap-2'>
                                    <TrendingUp className='h-5 w-5 text-primary' />
                                    Your Ranking
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                {myRank.isEligible ? (
                                    <div className='space-y-2'>
                                        <div className='flex justify-between items-center'>
                                            <span className='text-muted-foreground'>Rank</span>
                                            <span className='text-2xl font-bold'>#{myRank.rank}</span>
                                        </div>
                                        <div className='flex justify-between items-center'>
                                            <span className='text-muted-foreground'>Score</span>
                                            <span className='font-semibold'>{myRank.formattedScore}</span>
                                        </div>
                                        <div className='text-xs text-muted-foreground mt-2'>
                                            Out of {myRank.totalRanked} ranked volunteers
                                        </div>
                                    </div>
                                ) : (
                                    <div className='text-sm text-muted-foreground'>
                                        <p>
                                            {myRank.ineligibleReason || 'Complete 3+ events to appear on leaderboards.'}
                                        </p>
                                    </div>
                                )}
                            </CardContent>
                        </Card>
                    ) : null}

                    {/* Stats Card */}
                    <Card>
                        <CardHeader className='pb-3'>
                            <CardTitle className='text-lg flex items-center gap-2'>
                                {entityType === 'users' ? (
                                    <Users className='h-5 w-5 text-primary' />
                                ) : (
                                    <Users2 className='h-5 w-5 text-primary' />
                                )}
                                Stats
                            </CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className='space-y-2'>
                                <div className='flex justify-between items-center'>
                                    <span className='text-muted-foreground'>
                                        {entityType === 'users' ? 'Ranked Volunteers' : 'Ranked Teams'}
                                    </span>
                                    <span className='font-semibold'>{currentLeaderboard?.totalEntries || 0}</span>
                                </div>
                                {currentLeaderboard?.computedDate ? (
                                    <div className='text-xs text-muted-foreground mt-2'>
                                        Last updated: {formatLastUpdated(currentLeaderboard.computedDate)}
                                    </div>
                                ) : null}
                            </div>
                        </CardContent>
                    </Card>

                    {/* Info Card */}
                    <Card className='bg-muted/50'>
                        <CardContent className='pt-4'>
                            <div className='flex gap-2 text-sm text-muted-foreground'>
                                <Info className='h-4 w-4 mt-0.5 shrink-0' />
                                <div>
                                    <p className='font-medium text-foreground'>How Rankings Work</p>
                                    <ul className='mt-1 space-y-1 text-xs'>
                                        {entityType === 'users' ? (
                                            <>
                                                <li>Attend at least 3 events to qualify</li>
                                                <li>Rankings update daily</li>
                                                <li>Opt-out anytime in your profile settings</li>
                                            </>
                                        ) : (
                                            <>
                                                <li>Team members must attend 3+ events combined</li>
                                                <li>Scores aggregate all member contributions</li>
                                                <li>Rankings update daily</li>
                                            </>
                                        )}
                                    </ul>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                </div>

                {/* Main Leaderboard */}
                <div className='lg:col-span-3'>
                    <Card>
                        <CardHeader>
                            <CardTitle>
                                {LeaderboardTypeLabels[leaderboardType] || leaderboardType} -{' '}
                                {TimeRangeLabels[timeRange] || timeRange}
                            </CardTitle>
                            <CardDescription>
                                Top {entityType === 'users' ? 'volunteers' : 'teams'} ranked by{' '}
                                {(LeaderboardTypeLabels[leaderboardType] || leaderboardType).toLowerCase()}
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <LeaderboardTable
                                entries={currentLeaderboard?.entries || []}
                                isLoading={isLoading}
                                entityType={entityType}
                            />
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};
