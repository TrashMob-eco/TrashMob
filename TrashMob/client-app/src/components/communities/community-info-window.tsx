import { Link } from 'react-router';
import { MapPin, Building2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import CommunityData from '@/components/Models/CommunityData';
import { getLocation } from '@/lib/community-utils';

interface CommunityInfoWindowHeaderProps {
    name: string;
    bannerUrl?: string;
}

export const CommunityInfoWindowHeader = ({ name, bannerUrl }: CommunityInfoWindowHeaderProps) => {
    return (
        <div className='flex items-center gap-2'>
            {bannerUrl ? (
                <img src={bannerUrl} alt={`${name} banner`} className='w-6 h-6 rounded object-cover' />
            ) : (
                <Building2 className='h-4 w-4' />
            )}
            <span className='font-semibold'>{name}</span>
        </div>
    );
};

interface CommunityInfoWindowContentProps {
    community: CommunityData;
}

export const CommunityInfoWindowContent = ({ community }: CommunityInfoWindowContentProps) => {
    return (
        <div className='p-2 min-w-[200px] max-w-[280px]'>
            <div className='space-y-2'>
                {community.tagline ? (
                    <p className='text-sm text-muted-foreground line-clamp-2'>{community.tagline}</p>
                ) : null}

                <div className='flex items-center gap-1 text-sm text-muted-foreground'>
                    <MapPin className='h-3 w-3' />
                    <span>{getLocation(community)}</span>
                </div>

                <Button size='sm' className='w-full mt-2' asChild>
                    <Link to={`/communities/${community.slug}`}>View Community</Link>
                </Button>
            </div>
        </div>
    );
};
