import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import { GetFeaturedCommunities } from '@/services/communities';

export const FeaturedCommunitiesSection = () => {
    const { ref: viewportRef, isInViewPort } = useIsInViewport<HTMLDivElement>();

    const { data: communities } = useQuery({
        queryKey: GetFeaturedCommunities().key,
        queryFn: GetFeaturedCommunities().service,
    });

    const featuredCommunities = communities && communities.length > 0 ? communities.slice(0, 3) : null;

    // Don't render the section if there are no featured communities
    if (!featuredCommunities) return null;

    return (
        <section className='bg-card py-16' ref={viewportRef}>
            <div className='container'>
                <div
                    className={cn('transition-all duration-700 ease-out', {
                        'opacity-100 translate-y-0': isInViewPort,
                        'opacity-0 translate-y-10': !isInViewPort,
                    })}
                >
                    <h2 className='text-3xl font-bold text-center mb-2'>Featured Communities</h2>
                    <p className='text-center text-muted-foreground mb-10 max-w-2xl mx-auto'>
                        See how communities across the country are using TrashMob to make a difference.
                    </p>
                    <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6'>
                        {featuredCommunities.map((community) => (
                            <Card key={community.id}>
                                <CardHeader className='text-center'>
                                    {community.logoUrl ? (
                                        <img
                                            src={community.logoUrl}
                                            alt={community.name}
                                            className='h-16 w-16 mx-auto rounded-full object-cover mt-0'
                                        />
                                    ) : null}
                                    <CardTitle>{community.name}</CardTitle>
                                    {community.city && community.region ? (
                                        <p className='text-sm text-muted-foreground'>
                                            {community.city}, {community.region}
                                        </p>
                                    ) : null}
                                </CardHeader>
                                <CardContent className='text-center'>
                                    {community.tagline ? (
                                        <p className='text-sm italic mb-4'>"{community.tagline}"</p>
                                    ) : null}
                                    <Button variant='outline' size='sm' asChild>
                                        <Link to={`/communities/${community.slug}`}>View Community</Link>
                                    </Button>
                                </CardContent>
                            </Card>
                        ))}
                    </div>
                    <div className='flex flex-col sm:flex-row justify-center gap-4 mt-10'>
                        <Button variant='outline' asChild>
                            <Link to='/communities'>View All Communities</Link>
                        </Button>
                        <Button asChild>
                            <Link to='/for-communities'>Start Your Community</Link>
                        </Button>
                    </div>
                </div>
            </div>
        </section>
    );
};
