import { useState } from 'react';
import { useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { Helmet } from 'react-helmet-async';
import { ArrowLeft, Calendar, Clock, Loader2, Newspaper, Share2, User } from 'lucide-react';
import { BlocksRenderer, type BlocksContent } from '@strapi/blocks-react-renderer';

import { PageHead } from '@/components/SEO/PageHead';
import { ShareDialog } from '@/components/sharing/ShareDialog';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { getNewsPostShareableContent, getNewsPostShareMessage } from '@/lib/sharing-messages';
import { GetNewsPostBySlug, GetNewsPosts, type NewsPostItem } from '@/services/cms';
import { Services } from '@/config/services.config';

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
    });
}

function RelatedPostCard({ post }: { post: NewsPostItem }) {
    const coverUrl = post.coverImage?.url;

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
                            <Newspaper className='h-8 w-8 text-muted-foreground/50' />
                        </div>
                    )}
                </div>
                <CardContent className='p-4'>
                    <h4 className='font-semibold leading-tight line-clamp-2 group-hover:text-primary'>
                        {post.title}
                    </h4>
                    <p className='mt-1 text-sm text-muted-foreground line-clamp-2'>{post.excerpt}</p>
                </CardContent>
            </Card>
        </Link>
    );
}

const BASE_URL = 'https://www.trashmob.eco';

export const NewsPostDetailPage = () => {
    const { slug } = useParams<{ slug: string }>() as { slug: string };
    const [shareOpen, setShareOpen] = useState(false);

    // Fetch the post
    const { data: post, isLoading } = useQuery({
        queryKey: GetNewsPostBySlug(slug).key,
        queryFn: GetNewsPostBySlug(slug).service,
        staleTime: Services.CACHE.FOR_ONE_MINUTE * 5,
        enabled: !!slug,
    });

    const categorySlug = post?.category?.slug;

    // Fetch related posts (same category, exclude current)
    const { data: relatedResponse } = useQuery({
        queryKey: GetNewsPosts({ category: categorySlug, pageSize: 4 }).key,
        queryFn: GetNewsPosts({ category: categorySlug, pageSize: 4 }).service,
        staleTime: Services.CACHE.FOR_ONE_MINUTE * 5,
        enabled: !!categorySlug,
    });

    const relatedPosts = (relatedResponse?.data ?? []).filter((p) => p.slug !== slug).slice(0, 3);

    const coverUrl = post?.coverImage?.url;
    const categoryName = post?.category?.name;

    // Loading state
    if (isLoading) {
        return (
            <div className='container py-16'>
                <div className='flex flex-col items-center justify-center'>
                    <Loader2 className='h-8 w-8 animate-spin text-muted-foreground mb-4' />
                    <p className='text-muted-foreground'>Loading article...</p>
                </div>
            </div>
        );
    }

    // Not found state
    if (!post) {
        return (
            <div className='container py-16'>
                <Card>
                    <CardContent className='py-12'>
                        <div className='text-center'>
                            <Newspaper className='mx-auto mb-4 h-12 w-12 text-muted-foreground' />
                            <h2 className='mb-2 text-xl font-semibold'>Post not found</h2>
                            <p className='mb-4 text-muted-foreground'>
                                The article you're looking for doesn't exist or has been removed.
                            </p>
                            <Button variant='outline' asChild>
                                <Link to='/news'>
                                    <ArrowLeft className='h-4 w-4 mr-2' /> Back to News
                                </Link>
                            </Button>
                        </div>
                    </CardContent>
                </Card>
            </div>
        );
    }

    const canonicalUrl = `${BASE_URL}/news/${slug}`;
    const jsonLd = {
        '@context': 'https://schema.org',
        '@type': 'Article',
        headline: post.title,
        description: post.excerpt,
        image: coverUrl || undefined,
        author: { '@type': 'Person', name: post.author },
        publisher: { '@type': 'Organization', name: 'TrashMob.eco', url: BASE_URL },
        datePublished: post.publishedAt,
        mainEntityOfPage: canonicalUrl,
    };

    return (
        <div>
            <PageHead title={post.title} description={post.excerpt} image={coverUrl} type='article' />
            <Helmet>
                <script type='application/ld+json'>{JSON.stringify(jsonLd)}</script>
            </Helmet>

            <ShareDialog
                content={getNewsPostShareableContent(post, slug)}
                open={shareOpen}
                onOpenChange={setShareOpen}
                message={getNewsPostShareMessage(post)}
            />

            <div className='container py-8'>
                {/* Back navigation */}
                <div className='mb-6 flex items-center justify-between'>
                    <Button variant='outline' asChild>
                        <Link to='/news'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to News
                        </Link>
                    </Button>
                    <Button variant='outline' onClick={() => setShareOpen(true)}>
                        <Share2 className='h-4 w-4 mr-2' /> Share
                    </Button>
                </div>

                {/* Article */}
                <article className='mx-auto max-w-3xl'>
                    {/* Cover image */}
                    {coverUrl ? (
                        <div className='mb-6 overflow-hidden rounded-lg'>
                            <img
                                src={coverUrl}
                                alt={post.coverImage?.alternativeText || post.title}
                                className='w-full object-cover'
                            />
                        </div>
                    ) : null}

                    {/* Header */}
                    <header className='mb-8'>
                        {categoryName ? (
                            <Badge variant='outline' className='mb-3'>
                                {categoryName}
                            </Badge>
                        ) : null}

                        <h1 className='mb-4 text-3xl font-bold leading-tight md:text-4xl'>{post.title}</h1>

                        <div className='flex flex-wrap items-center gap-4 text-sm text-muted-foreground'>
                            <span className='flex items-center gap-1'>
                                <User className='h-4 w-4' />
                                {post.author}
                            </span>
                            <span className='flex items-center gap-1'>
                                <Calendar className='h-4 w-4' />
                                {formatDate(post.publishedAt)}
                            </span>
                            {post.estimatedReadTime ? (
                                <span className='flex items-center gap-1'>
                                    <Clock className='h-4 w-4' />
                                    {post.estimatedReadTime} min read
                                </span>
                            ) : null}
                        </div>
                    </header>

                    {/* Separator */}
                    <hr className='mb-8' />

                    {/* Body */}
                    <div className='prose prose-lg max-w-none dark:prose-invert'>
                        {post.body ? <BlocksRenderer content={post.body as BlocksContent} /> : null}
                    </div>

                    {/* Tags */}
                    {post.tags && post.tags.length > 0 ? (
                        <div className='mt-8 flex flex-wrap gap-2'>
                            {post.tags.map((tag) => (
                                <Badge key={tag} variant='secondary'>
                                    {tag}
                                </Badge>
                            ))}
                        </div>
                    ) : null}
                </article>

                {/* Related posts */}
                {relatedPosts.length > 0 ? (
                    <div className='mx-auto mt-12 max-w-3xl'>
                        <hr className='mb-8' />
                        <h2 className='mb-6 text-2xl font-semibold'>Related Posts</h2>
                        <div className='grid grid-cols-1 gap-6 md:grid-cols-3'>
                            {relatedPosts.map((rp) => (
                                <RelatedPostCard key={rp.id} post={rp} />
                            ))}
                        </div>
                    </div>
                ) : null}
            </div>
        </div>
    );
};

export default NewsPostDetailPage;
