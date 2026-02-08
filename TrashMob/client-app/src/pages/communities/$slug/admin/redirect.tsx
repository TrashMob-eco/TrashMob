import { useParams, Navigate } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2 } from 'lucide-react';
import CommunityData from '@/components/Models/CommunityData';
import { GetCommunityBySlug } from '@/services/communities';

export const CommunityAdminRedirect = () => {
    const { slug } = useParams<{ slug: string }>() as { slug: string };

    const { data: community, isLoading } = useQuery<AxiosResponse<CommunityData>, unknown, CommunityData>({
        queryKey: GetCommunityBySlug({ slug }).key,
        queryFn: GetCommunityBySlug({ slug }).service,
        select: (res) => res.data,
        enabled: !!slug,
    });

    if (isLoading) {
        return (
            <div className='flex justify-center items-center py-16'>
                <Loader2 className='animate-spin mr-2' /> Redirecting...
            </div>
        );
    }

    if (community?.id) {
        return <Navigate to={`/partnerdashboard/${community.id}/community`} replace />;
    }

    return <Navigate to='/' replace />;
};

export default CommunityAdminRedirect;
