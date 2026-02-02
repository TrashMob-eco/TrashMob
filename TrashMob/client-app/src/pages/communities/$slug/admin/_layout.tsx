import { useCallback } from 'react';
import { Outlet, useLocation, useNavigate, useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { ArrowLeft, Loader2 } from 'lucide-react';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Button } from '@/components/ui/button';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunityBySlug } from '@/services/communities';

export const CommunityAdminLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { slug } = useParams<{ slug: string }>() as { slug: string };

    const { data: community, isLoading } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityBySlug({ slug }).key,
        queryFn: GetCommunityBySlug({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    const pathPrefix = `/communities/${slug}/admin`;
    const navs = [
        { name: 'Dashboard', value: `${pathPrefix}` },
        { name: 'Edit Content', value: `${pathPrefix}/content` },
    ];

    const handleValueChange = useCallback(
        (value: string) => {
            navigate(value);
        },
        [navigate],
    );

    if (isLoading) {
        return (
            <div className='container py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    if (!community) {
        return (
            <div className='container py-8 text-center'>
                <p>Community not found.</p>
            </div>
        );
    }

    return (
        <div>
            <div className='bg-muted border-b'>
                <div className='container py-4'>
                    <div className='flex items-center gap-4 mb-4'>
                        <Button variant='ghost' size='sm' asChild>
                            <Link to={`/communities/${slug}`}>
                                <ArrowLeft className='h-4 w-4 mr-2' />
                                Back to Community
                            </Link>
                        </Button>
                    </div>
                    <h1 className='text-2xl font-bold'>{community.name} - Admin</h1>
                    <p className='text-muted-foreground'>Manage your community page content and view metrics</p>
                </div>
            </div>
            <div className='container mx-auto my-4'>
                <Tabs value={location.pathname} onValueChange={handleValueChange}>
                    <TabsList className='w-full h-14'>
                        {navs.map((nav, idx) => (
                            <TabsTrigger className='whitespace-normal' value={nav.value} key={`tab-${idx}`}>
                                {nav.name}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                </Tabs>
            </div>
            <Outlet context={{ community }} />
        </div>
    );
};
