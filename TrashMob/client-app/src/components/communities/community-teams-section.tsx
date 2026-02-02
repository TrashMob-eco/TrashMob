import { Link } from 'react-router';
import { Users, MapPin, ExternalLink } from 'lucide-react';
import TeamData from '@/components/Models/TeamData';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';

interface CommunityTeamsSectionProps {
    teams: TeamData[];
    isLoading?: boolean;
}

export const CommunityTeamsSection = ({ teams, isLoading }: CommunityTeamsSectionProps) => {
    const getInitials = (name: string) => {
        return name
            .split(' ')
            .map((word) => word[0])
            .join('')
            .toUpperCase()
            .slice(0, 2);
    };

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <CardTitle className='text-lg'>Teams Nearby</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className='space-y-3'>
                        {[1, 2, 3].map((i) => (
                            <div key={i} className='animate-pulse flex items-center gap-3'>
                                <div className='h-10 w-10 bg-muted rounded-full' />
                                <div className='flex-1'>
                                    <div className='h-4 bg-muted rounded w-3/4 mb-2' />
                                    <div className='h-3 bg-muted rounded w-1/2' />
                                </div>
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardHeader className='flex flex-row items-center justify-between'>
                <CardTitle className='text-lg'>Teams Nearby</CardTitle>
                <Link to='/teams'>
                    <Button variant='ghost' size='sm'>
                        View All
                        <ExternalLink className='ml-1 h-3 w-3' />
                    </Button>
                </Link>
            </CardHeader>
            <CardContent>
                {teams.length === 0 ? (
                    <p className='text-sm text-muted-foreground'>No teams found near this community.</p>
                ) : (
                    <div className='space-y-3'>
                        {teams.slice(0, 5).map((team) => (
                            <Link
                                key={team.id}
                                to={`/teams/${team.id}`}
                                className='flex items-center gap-3 p-2 rounded-lg hover:bg-muted transition-colors'
                            >
                                <Avatar className='h-10 w-10'>
                                    <AvatarImage src={team.logoUrl} alt={team.name} />
                                    <AvatarFallback className='bg-primary/10 text-primary text-sm'>
                                        {getInitials(team.name)}
                                    </AvatarFallback>
                                </Avatar>
                                <div className='flex-1 min-w-0'>
                                    <h4 className='font-medium text-sm truncate'>{team.name}</h4>
                                    <div className='flex items-center gap-2 text-xs text-muted-foreground'>
                                        <div className='flex items-center gap-1'>
                                            <Users className='h-3 w-3' />
                                            <span>{team.memberCount || 0} members</span>
                                        </div>
                                        {team.city ? (
                                            <div className='flex items-center gap-1'>
                                                <MapPin className='h-3 w-3' />
                                                <span className='truncate'>{team.city}</span>
                                            </div>
                                        ) : null}
                                    </div>
                                </div>
                            </Link>
                        ))}
                        {teams.length > 5 && (
                            <p className='text-xs text-muted-foreground text-center'>+ {teams.length - 5} more teams</p>
                        )}
                    </div>
                )}
            </CardContent>
        </Card>
    );
};
