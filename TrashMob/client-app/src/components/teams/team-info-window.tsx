import { Link } from 'react-router';
import { MapPin, Users, Globe, Lock } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import TeamData from '@/components/Models/TeamData';

interface TeamInfoWindowHeaderProps {
    name: string;
    logoUrl?: string;
}

export const TeamInfoWindowHeader = ({ name, logoUrl }: TeamInfoWindowHeaderProps) => {
    return (
        <div className='flex items-center gap-2'>
            {logoUrl ? (
                <img src={logoUrl} alt={`${name} logo`} className='w-6 h-6 rounded object-cover' />
            ) : (
                <Users className='h-4 w-4' />
            )}
            <span className='font-semibold'>{name}</span>
        </div>
    );
};

interface TeamInfoWindowContentProps {
    team: TeamData;
}

const getLocation = (team: TeamData) => {
    const parts = [team.city, team.region, team.country].filter(Boolean);
    return parts.join(', ') || 'Location not specified';
};

export const TeamInfoWindowContent = ({ team }: TeamInfoWindowContentProps) => {
    return (
        <div className='p-2 min-w-[200px] max-w-[280px]'>
            <div className='space-y-2'>
                {team.description ? (
                    <p className='text-sm text-muted-foreground line-clamp-2'>{team.description}</p>
                ) : null}

                <div className='flex items-center gap-1 text-sm text-muted-foreground'>
                    <MapPin className='h-3 w-3' />
                    <span>{getLocation(team)}</span>
                </div>

                <div className='flex items-center gap-2'>
                    {team.isPublic ? (
                        <Badge variant='outline' className='bg-green-100 text-green-800 border-green-300 text-xs'>
                            <Globe className='h-3 w-3 mr-1' /> Public
                        </Badge>
                    ) : (
                        <Badge variant='outline' className='bg-gray-100 text-gray-800 border-gray-300 text-xs'>
                            <Lock className='h-3 w-3 mr-1' /> Private
                        </Badge>
                    )}
                    {team.requiresApproval ? (
                        <span className='text-xs text-muted-foreground'>Requires approval</span>
                    ) : (
                        <span className='text-xs text-green-600'>Open to join</span>
                    )}
                </div>

                <Button size='sm' className='w-full mt-2' asChild>
                    <Link to={`/teams/${team.id}`}>View Team</Link>
                </Button>
            </div>
        </div>
    );
};
