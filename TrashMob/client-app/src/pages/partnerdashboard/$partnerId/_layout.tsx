import { useCallback } from 'react';
import { Outlet, useLocation, useNavigate, useParams, useRouteLoaderData, useMatches } from 'react-router';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
// import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

export const PartnerLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const pathPrefix = `/partnerdashboard/${partnerId}`;
    const navs = [
        { name: 'Manage Partner', value: `${pathPrefix}/edit` },
        { name: 'Manage Partner Locations', value: `${pathPrefix}/locations` },
        { name: 'Manage Services', value: `${pathPrefix}/services` },
        { name: 'Manage Partner Contacts', value: `${pathPrefix}/contacts` },
        { name: 'Manage Partner Admins', value: `${pathPrefix}/admins` },
        { name: 'Manage Partner Documents', value: `${pathPrefix}/documents` },
        { name: 'Manage Partner Social Media Accounts', value: `${pathPrefix}/socials` },
    ];

    const handleValueChange = useCallback(
        (value: string) => {
            navigate(value);
        },
        [navigate],
    );

    return (
        <div className='tailwind'>
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
            <Outlet />
        </div>
    );
};
