import { useCallback } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';

export const SiteAdminLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();

    const pathPrefix = `/siteadmin`;
    const navs = [
        { name: 'Manage Users', value: `${pathPrefix}/users` },
        { name: 'Manage Events', value: `${pathPrefix}/events` },
        { name: 'Manage Partners', value: `${pathPrefix}/partners` },
        { name: 'Manage Partner Requests', value: `${pathPrefix}/partner-requests` },
        // { name: 'View Executive Summary', value: `${pathPrefix}/executive-summary` },
        { name: 'Send Notifications', value: `${pathPrefix}/send-notifications` },
        { name: 'View Email Templates', value: `${pathPrefix}/email-templates` },
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
                <h1 className='scroll-m-20 pb-4 text-3xl font-light tracking-tight first:mt-0'>Site Administration</h1>
                <Tabs onValueChange={handleValueChange} value={location.pathname}>
                    <TabsList className='w-full'>
                        {navs.map((nav, idx) => (
                            <TabsTrigger className='whitespace-normal' key={`tab-${idx}`} value={nav.value}>
                                {nav.name}
                            </TabsTrigger>
                        ))}
                    </TabsList>
                </Tabs>
            </div>
            <div className='container mx-auto pb-4'>
                <Outlet />
            </div>
        </div>
    );
};
