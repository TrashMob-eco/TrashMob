import { useCallback } from 'react';
import { Outlet, useLocation, useNavigate, useParams } from 'react-router';
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

export const PartnerLayout = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const pathPrefix = `/partnerdashboard/${partnerId}`;
    const navs = [
        { name: 'Manage Partner', value: `${pathPrefix}/edit` },
        { name: 'Manage Partner Locations', value: `${pathPrefix}/locations` },
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
            <div className='container mx-auto'>
                <div className='grid grid-cols-12 !gap-8'>
                    <div className='col-span-4'>
                        <Card>
                            <CardHeader>
                                <CardTitle className='font-semibold tracking-tight text-primary text-2xl'>
                                    Edit Partner Information
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                This page allows you to add basic details about your organization. Public notes may be
                                shown to TrashMob.eco users on the partnership page. Think of this as a blurb or a tag
                                line you may want to add to let users know more about your organization in general.
                            </CardContent>
                        </Card>
                    </div>
                    <div className='col-span-8'>
                        <Card>
                            <CardHeader>
                                <CardTitle className='font-semibold tracking-tight text-primary text-2xl'>
                                    Edit Partner
                                </CardTitle>
                            </CardHeader>
                            <CardContent>
                                <Outlet />
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        </div>
    );
};
