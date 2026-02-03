import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Progress } from '@/components/ui/progress';
import { Skeleton } from '@/components/ui/skeleton';
import { GetMyAchievements, GetUnreadAchievements, MarkAchievementsRead } from '@/services/achievements';
import { AchievementCategoryLabels, AchievementDto } from '@/components/Models/AchievementData';
import { useLogin } from '@/hooks/useLogin';
import { Award, Trophy, Star, Target, Lock, CheckCircle } from 'lucide-react';
import { useEffect } from 'react';

// Category to icon mapping
const getCategoryIcon = (category: string) => {
    switch (category) {
        case 'Participation':
            return <Target className='h-5 w-5' />;
        case 'Impact':
            return <Star className='h-5 w-5' />;
        case 'Special':
            return <Award className='h-5 w-5' />;
        default:
            return <Trophy className='h-5 w-5' />;
    }
};

const AchievementCard = ({ achievement }: { achievement: AchievementDto }) => {
    const formatDate = (dateStr?: string) => {
        if (!dateStr) return '';
        const date = new Date(dateStr);
        return date.toLocaleDateString('en-US', {
            month: 'short',
            day: 'numeric',
            year: 'numeric',
        });
    };

    return (
        <Card className={achievement.isEarned ? 'border-primary/50 bg-primary/5' : 'opacity-60'}>
            <CardContent className='pt-4'>
                <div className='flex items-start gap-3'>
                    <div
                        className={`p-2 rounded-full ${
                            achievement.isEarned ? 'bg-primary/20 text-primary' : 'bg-muted text-muted-foreground'
                        }`}
                    >
                        {achievement.isEarned ? <CheckCircle className='h-6 w-6' /> : <Lock className='h-6 w-6' />}
                    </div>
                    <div className='flex-1'>
                        <div className='flex items-center gap-2'>
                            <h3 className='font-semibold'>{achievement.displayName}</h3>
                            <Badge variant='outline' className='text-xs'>
                                {achievement.points} pts
                            </Badge>
                        </div>
                        <p className='text-sm text-muted-foreground mt-1'>{achievement.description}</p>
                        {achievement.isEarned && achievement.earnedDate ? (
                            <p className='text-xs text-primary mt-2'>Earned on {formatDate(achievement.earnedDate)}</p>
                        ) : null}
                    </div>
                </div>
            </CardContent>
        </Card>
    );
};

const AchievementsSkeleton = () => (
    <div className='space-y-6'>
        <div className='grid gap-4 md:grid-cols-3'>
            {[1, 2, 3].map((i) => (
                <Card key={i}>
                    <CardContent className='pt-4'>
                        <Skeleton className='h-20 w-full' />
                    </CardContent>
                </Card>
            ))}
        </div>
        <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-3'>
            {[1, 2, 3, 4, 5, 6].map((i) => (
                <Card key={i}>
                    <CardContent className='pt-4'>
                        <Skeleton className='h-24 w-full' />
                    </CardContent>
                </Card>
            ))}
        </div>
    </div>
);

export const AchievementsPage = () => {
    const { isUserLoaded } = useLogin();
    const queryClient = useQueryClient();

    // Fetch user's achievements
    const { data: achievementsResponse, isLoading } = useQuery({
        queryKey: GetMyAchievements().key,
        queryFn: GetMyAchievements().service,
        enabled: isUserLoaded,
    });
    const achievements = achievementsResponse?.data;

    // Fetch unread achievements
    const { data: unreadResponse } = useQuery({
        queryKey: GetUnreadAchievements().key,
        queryFn: GetUnreadAchievements().service,
        enabled: isUserLoaded,
    });
    const unreadAchievements = unreadResponse?.data || [];

    // Mark achievements as read
    const markReadMutation = useMutation({
        mutationFn: async (ids: number[]) => {
            return MarkAchievementsRead({ achievementTypeIds: ids }).service();
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: GetUnreadAchievements().key });
        },
    });

    // Auto-mark achievements as read when page loads
    useEffect(() => {
        if (unreadAchievements.length > 0) {
            const ids = unreadAchievements.map((n) => n.achievement.id);
            markReadMutation.mutate(ids);
        }
    }, [unreadAchievements]);

    // Group achievements by category
    const groupedAchievements = achievements?.achievements.reduce(
        (acc, achievement) => {
            const category = achievement.category || 'Other';
            if (!acc[category]) {
                acc[category] = [];
            }
            acc[category].push(achievement);
            return acc;
        },
        {} as Record<string, AchievementDto[]>,
    );

    const progressPercent = achievements ? (achievements.earnedCount / achievements.totalCount) * 100 : 0;

    if (!isUserLoaded) {
        return (
            <div className='container py-8'>
                <div className='text-center py-12'>
                    <Trophy className='h-12 w-12 text-muted-foreground mx-auto mb-4' />
                    <h2 className='text-xl font-semibold mb-2'>Sign In to View Achievements</h2>
                    <p className='text-muted-foreground'>
                        Create an account or sign in to track your volunteer achievements.
                    </p>
                </div>
            </div>
        );
    }

    return (
        <div className='container py-8'>
            <div className='mb-8'>
                <h1 className='text-3xl font-bold flex items-center gap-2'>
                    <Trophy className='h-8 w-8 text-yellow-500' />
                    Achievements
                </h1>
                <p className='text-muted-foreground mt-2'>
                    Track your volunteer milestones and earn badges for your contributions.
                </p>
            </div>

            {isLoading ? (
                <AchievementsSkeleton />
            ) : achievements ? (
                <div className='space-y-8'>
                    {/* Summary Cards */}
                    <div className='grid gap-4 md:grid-cols-3'>
                        <Card>
                            <CardHeader className='pb-2'>
                                <CardDescription>Total Points</CardDescription>
                                <CardTitle className='text-3xl'>{achievements.totalPoints}</CardTitle>
                            </CardHeader>
                        </Card>
                        <Card>
                            <CardHeader className='pb-2'>
                                <CardDescription>Achievements Earned</CardDescription>
                                <CardTitle className='text-3xl'>
                                    {achievements.earnedCount} / {achievements.totalCount}
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                <Progress value={progressPercent} className='h-2' />
                            </CardContent>
                        </Card>
                        <Card>
                            <CardHeader className='pb-2'>
                                <CardDescription>Progress</CardDescription>
                                <CardTitle className='text-3xl'>{Math.round(progressPercent)}%</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <p className='text-xs text-muted-foreground'>
                                    {achievements.totalCount - achievements.earnedCount} achievements remaining
                                </p>
                            </CardContent>
                        </Card>
                    </div>

                    {/* Achievement Categories */}
                    {groupedAchievements
                        ? Object.entries(groupedAchievements).map(([category, categoryAchievements]) => (
                              <div key={category}>
                                  <div className='flex items-center gap-2 mb-4'>
                                      {getCategoryIcon(category)}
                                      <h2 className='text-xl font-semibold'>
                                          {AchievementCategoryLabels[category] || category}
                                      </h2>
                                      <Badge variant='secondary' className='ml-2'>
                                          {categoryAchievements.filter((a) => a.isEarned).length} /{' '}
                                          {categoryAchievements.length}
                                      </Badge>
                                  </div>
                                  <div className='grid gap-4 md:grid-cols-2 lg:grid-cols-3'>
                                      {categoryAchievements.map((achievement) => (
                                          <AchievementCard key={achievement.id} achievement={achievement} />
                                      ))}
                                  </div>
                              </div>
                          ))
                        : null}
                </div>
            ) : (
                <div className='text-center py-12'>
                    <p className='text-muted-foreground'>Unable to load achievements. Please try again later.</p>
                </div>
            )}
        </div>
    );
};
