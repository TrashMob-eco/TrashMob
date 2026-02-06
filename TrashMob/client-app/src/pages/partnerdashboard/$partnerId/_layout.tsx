import { useCallback, useMemo } from 'react';
import { Outlet, useLocation, useNavigate, useParams, Link } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { Globe } from 'lucide-react';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Button } from '@/components/ui/button';
import { GetPartnerById } from '@/services/partners';

export const PartnerLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const { data: partner } = useQuery({
        queryKey: GetPartnerById({ partnerId }).key,
        queryFn: GetPartnerById({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const pathPrefix = `/partnerdashboard/${partnerId}`;
    const navs = useMemo(
        () => [
            { name: 'Service Requests', value: `${pathPrefix}` },
            { name: 'Manage Partner', value: `${pathPrefix}/edit` },
            { name: 'Manage Partner Locations', value: `${pathPrefix}/locations` },
            { name: 'Manage Services', value: `${pathPrefix}/services` },
            { name: 'Manage Partner Contacts', value: `${pathPrefix}/contacts` },
            { name: 'Manage Partner Admins', value: `${pathPrefix}/admins` },
            { name: 'Manage Partner Documents', value: `${pathPrefix}/documents` },
            { name: 'Manage Partner Social Media Accounts', value: `${pathPrefix}/socials` },
        ],
        [pathPrefix],
    );

    const handleValueChange = useCallback(
        (value: string) => {
            navigate(value);
        },
        [navigate],
    );

    // Check if this partner has a community page enabled
    const hasCommunityPage = partner?.homePageEnabled && partner?.slug;

    return (
        <div>
            <div className='container mx-auto my-4'>
                <div className='flex items-center gap-4 flex-wrap'>
                    <Tabs value={location.pathname} onValueChange={handleValueChange} className='flex-1'>
                        <TabsList className='w-full h-14'>
                            {navs.map((nav, idx) => (
                                <TabsTrigger className='whitespace-normal' value={nav.value} key={`tab-${idx}`}>
                                    {nav.name}
                                </TabsTrigger>
                            ))}
                        </TabsList>
                    </Tabs>
                    {hasCommunityPage ? (
                        <Button variant='outline' asChild className='h-14'>
                            <Link to={`/communities/${partner.slug}/admin`}>
                                <Globe className='h-4 w-4 mr-2' />
                                Manage Community Page
                            </Link>
                        </Button>
                    ) : null}
                </div>
            </div>
            <Outlet />
        </div>
    );
};
