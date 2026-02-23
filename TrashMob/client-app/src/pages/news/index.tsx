import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Link, useSearchParams } from 'react-router';
import { Calendar, ChevronLeft, ChevronRight, Clock, Loader2, Newspaper, User } from 'lucide-react';

import { HeroSection } from '@/components/Customization/HeroSection';
import { PageHead } from '@/components/SEO/PageHead';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { GetNewsCategories, GetNewsPosts, NewsPostData } from '@/services/cms';
import { Services } from '@/config/services.config';

const PAGE_SIZE = 9;

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
    });
}

function NewsCard({ post }: { post: { id: number; attributes: NewsPostData } }) {
    const { attributes } = post;
    const coverUrl = attributes.coverImage?.data?.attributes?.url;
    const categoryName = attributes.category?.data?.attributes?.name;

    return (
        <Link to={`/news/${attributes.slug}`} className='group'>
            <Card className='h-full overflow-hidden transition-shadow hover:shadow-lg'>
                {/* Cover image */}
                <div className='aspect-video overflow-hidden bg-muted'>
                    {coverUrl ? (
                        <img
                            src={coverUrl}
                            alt={attributes.coverImage?.data?.attributes?.alternativeText || attributes.title}
                            className='h-full w-full object-cover transition-transform group-hover:scale-105'
                        />
                    ) : (
                        <div className='flex h-full w-full items-center justify-center bg-gradient-to-br from-primary/10 to-primary/5'>
                            <Newspaper className='h-12 w-12 text-muted-foreground/50' />
                        </div>
                    )}
                </div>

                <CardContent className='p-4'>
                    {/* Category badge */}
                    {categoryName ? <Badge variant='outline' className='mb-2'>
                            {categoryName}
                        </Badge> : null}

                    {/* Title */}
                    <h3 className='mb-2 text-lg font-semibold leading-tight line-clamp-2 group-hover:text-primary'>
                        {attributes.title}
                    </h3>

                    {/* Excerpt */}
                    <p className='mb-3 text-sm text-muted-foreground line-clamp-3'>{attributes.excerpt}</p>

                    {/* Meta row */}
                    <div className='flex flex-wrap items-center gap-3 text-xs text-muted-foreground'>
                        <span className='flex items-center gap-1'>
                            <User className='h-3 w-3' />
                            {attributes.author}
                        </span>
                        <span className='flex items-center gap-1'>
                            <Calendar className='h-3 w-3' />
                            {formatDate(attributes.publishedAt)}
                        </span>
                        {attributes.estimatedReadTime ? <span className='flex items-center gap-1'>
                                <Clock className='h-3 w-3' />
                                {attributes.estimatedReadTime} min read
                            </span> : null}
                    </div>
                </CardContent>
            </Card>
        </Link>
    );
}

function LoadingSkeleton() {
    return (
        <div className='grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3'>
            {Array.from({ length: 6 }).map((_, i) => (
                <Card key={i} className='overflow-hidden'>
                    <Skeleton className='aspect-video w-full' />
                    <CardContent className='p-4'>
                        <Skeleton className='mb-2 h-5 w-20' />
                        <Skeleton className='mb-2 h-6 w-full' />
                        <Skeleton className='mb-2 h-6 w-3/4' />
                        <Skeleton className='mb-3 h-4 w-full' />
                        <Skeleton className='h-4 w-2/3' />
                        <Skeleton className='mt-3 h-3 w-1/2' />
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}

export const NewsPage = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const currentPage = Number(searchParams.get('page')) || 1;
    const currentCategory = searchParams.get('category') || '';

    // Track category selection locally for optimistic UI
    const [selectedCategory, setSelectedCategory] = useState(currentCategory);

    // Fetch categories for filter pills
    const { data: categories } = useQuery({
        queryKey: GetNewsCategories().key,
        queryFn: GetNewsCategories().service,
        staleTime: Services.CACHE.FOR_ONE_MINUTE * 5,
    });

    // Fetch posts with server-side pagination
    const {
        data: postsResponse,
        isLoading,
        isFetching,
    } = useQuery({
        queryKey: GetNewsPosts({ page: currentPage, pageSize: PAGE_SIZE, category: currentCategory || undefined }).key,
        queryFn: GetNewsPosts({ page: currentPage, pageSize: PAGE_SIZE, category: currentCategory || undefined })
            .service,
        staleTime: Services.CACHE.FOR_ONE_MINUTE * 2,
    });

    const posts = postsResponse?.data ?? [];
    const pagination = postsResponse?.meta?.pagination ?? { page: 1, pageSize: PAGE_SIZE, pageCount: 0, total: 0 };

    function updateParams(page: number, category: string) {
        const params = new URLSearchParams();
        if (page > 1) params.set('page', String(page));
        if (category) params.set('category', category);
        setSearchParams(params, { replace: true });
    }

    function handleCategoryChange(value: string) {
        const cat = value || '';
        setSelectedCategory(cat);
        updateParams(1, cat); // Reset to page 1 on category change
    }

    function handlePageChange(page: number) {
        updateParams(page, currentCategory);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    return (
        <div>
            <PageHead title='News' description='Latest news, updates, and community stories from TrashMob.eco.' />
            <HeroSection
                Title='News'
                Description='Stories, updates, and highlights from the TrashMob community.'
            />

            <div className='container py-8'>
                {/* Category filter pills */}
                {categories && categories.length > 0 ? <div className='mb-6'>
                        <ToggleGroup
                            type='single'
                            value={selectedCategory}
                            onValueChange={handleCategoryChange}
                            className='flex-wrap justify-start'
                        >
                            <ToggleGroupItem value='' className='rounded-full px-4'>
                                All
                            </ToggleGroupItem>
                            {categories.map((cat) => (
                                <ToggleGroupItem key={cat.slug} value={cat.slug} className='rounded-full px-4'>
                                    {cat.name}
                                </ToggleGroupItem>
                            ))}
                        </ToggleGroup>
                    </div> : null}

                {/* Content area */}
                {isLoading ? (
                    <LoadingSkeleton />
                ) : posts.length > 0 ? (
                    <>
                        {/* Post count */}
                        <p className='mb-4 text-sm text-muted-foreground'>
                            {pagination.total} post{pagination.total !== 1 ? 's' : ''}
                            {currentCategory ? ` in "${categories?.find((c) => c.slug === currentCategory)?.name || currentCategory}"` : ''}
                        </p>

                        {/* Card grid */}
                        <div className='grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3'>
                            {posts.map((post) => (
                                <NewsCard key={post.id} post={post} />
                            ))}
                        </div>

                        {/* Pagination */}
                        {pagination.pageCount > 1 && (
                            <div className='mt-8 flex items-center justify-center gap-2'>
                                <Button
                                    variant='outline'
                                    size='sm'
                                    onClick={() => handlePageChange(currentPage - 1)}
                                    disabled={currentPage <= 1 || isFetching}
                                >
                                    <ChevronLeft className='mr-1 h-4 w-4' />
                                    Previous
                                </Button>
                                <span className='px-4 text-sm text-muted-foreground'>
                                    Page {pagination.page} of {pagination.pageCount}
                                </span>
                                <Button
                                    variant='outline'
                                    size='sm'
                                    onClick={() => handlePageChange(currentPage + 1)}
                                    disabled={currentPage >= pagination.pageCount || isFetching}
                                >
                                    Next
                                    <ChevronRight className='ml-1 h-4 w-4' />
                                </Button>
                            </div>
                        )}

                        {/* Fetching overlay */}
                        {isFetching && !isLoading ? <div className='mt-4 flex justify-center'>
                                <Loader2 className='h-5 w-5 animate-spin text-muted-foreground' />
                            </div> : null}
                    </>
                ) : (
                    /* Empty state */
                    <Card>
                        <CardContent className='py-12'>
                            <div className='text-center'>
                                <Newspaper className='mx-auto mb-4 h-12 w-12 text-muted-foreground' />
                                <h3 className='mb-2 text-lg font-medium'>No news posts yet</h3>
                                <p className='text-muted-foreground'>
                                    {currentCategory
                                        ? 'No posts found in this category. Try selecting a different category.'
                                        : 'Check back soon for news and updates from the TrashMob community.'}
                                </p>
                                {currentCategory ? <Button
                                        variant='outline'
                                        className='mt-4'
                                        onClick={() => handleCategoryChange('')}
                                    >
                                        View all posts
                                    </Button> : null}
                            </div>
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default NewsPage;
