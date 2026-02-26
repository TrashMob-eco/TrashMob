import { Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { Calendar, Newspaper, User } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { useIsInViewport } from '@/hooks/useIsInViewport';
import { cn } from '@/lib/utils';
import { GetNewsPosts, type NewsPostItem } from '@/services/cms';
import { Services } from '@/config/services.config';

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
    });
}

function NewsCard({ post }: { post: NewsPostItem }) {
    const coverUrl = post.coverImage?.url;
    const categoryName = post.category?.name;

    return (
        <Link to={`/news/${post.slug}`} className='group'>
            <Card className='h-full overflow-hidden transition-shadow hover:shadow-lg'>
                <div className='aspect-video overflow-hidden bg-muted'>
                    {coverUrl ? (
                        <img
                            src={coverUrl}
                            alt={post.coverImage?.alternativeText || post.title}
                            className='h-full w-full object-cover transition-transform group-hover:scale-105'
                        />
                    ) : (
                        <div className='flex h-full w-full items-center justify-center bg-gradient-to-br from-primary/10 to-primary/5'>
                            <Newspaper className='h-12 w-12 text-muted-foreground/50' />
                        </div>
                    )}
                </div>
                <CardContent className='p-4'>
                    {categoryName ? (
                        <Badge variant='outline' className='mb-2'>
                            {categoryName}
                        </Badge>
                    ) : null}
                    <h3 className='mb-2 text-lg font-semibold leading-tight line-clamp-2 group-hover:text-primary'>
                        {post.title}
                    </h3>
                    <p className='mb-3 text-sm text-muted-foreground line-clamp-2'>{post.excerpt}</p>
                    <div className='flex flex-wrap items-center gap-3 text-xs text-muted-foreground'>
                        <span className='flex items-center gap-1'>
                            <User className='h-3 w-3' />
                            {post.author}
                        </span>
                        <span className='flex items-center gap-1'>
                            <Calendar className='h-3 w-3' />
                            {formatDate(post.publishedAt)}
                        </span>
                    </div>
                </CardContent>
            </Card>
        </Link>
    );
}

export const LatestNewsSection = () => {
    const { ref: viewportRef, isInViewPort } = useIsInViewport<HTMLDivElement>();

    const { data: postsResponse } = useQuery({
        queryKey: GetNewsPosts({ pageSize: 3 }).key,
        queryFn: GetNewsPosts({ pageSize: 3 }).service,
        staleTime: Services.CACHE.FOR_ONE_MINUTE * 5,
    });

    const posts = postsResponse?.data;

    // Don't render the section if there are no posts
    if (!posts || posts.length === 0) return null;

    return (
        <section className='bg-card py-16' ref={viewportRef}>
            <div className='container'>
                <div
                    className={cn('transition-all duration-700 ease-out', {
                        'opacity-100 translate-y-0': isInViewPort,
                        'opacity-0 translate-y-10': !isInViewPort,
                    })}
                >
                    <h2 className='text-3xl font-bold text-center mb-2'>Latest News</h2>
                    <p className='text-center text-muted-foreground mb-10 max-w-2xl mx-auto'>
                        Stories, updates, and highlights from the TrashMob community.
                    </p>
                    <div className='grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6'>
                        {posts.map((post) => (
                            <NewsCard key={post.id} post={post} />
                        ))}
                    </div>
                    <div className='flex justify-center mt-10'>
                        <Button variant='outline' asChild>
                            <Link to='/news'>View All News</Link>
                        </Button>
                    </div>
                </div>
            </div>
        </section>
    );
};
