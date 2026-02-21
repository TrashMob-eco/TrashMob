import { Outlet, useParams, Link } from 'react-router';
import { LayoutDashboard, ClipboardPlus, History, ArrowLeft } from 'lucide-react';
import { SidebarNav, NavGroup } from '@/components/ui/sidebar-nav';
import { Button } from '@/components/ui/button';

export const CompanyDashboardLayout = () => {
    const { companyId } = useParams<{ companyId: string }>() as { companyId: string };

    const pathPrefix = `/companydashboard/${companyId}`;

    const navGroups: NavGroup[] = [
        {
            title: 'Company Portal',
            items: [
                { name: 'Dashboard', href: pathPrefix, icon: LayoutDashboard },
                { name: 'Log Cleanup', href: `${pathPrefix}/log-cleanup`, icon: ClipboardPlus },
                { name: 'Cleanup History', href: `${pathPrefix}/history`, icon: History },
            ],
        },
    ];

    return (
        <div className='container mx-auto py-6'>
            <div className='mb-4'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to='/mydashboard#my-companies'>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to My Dashboard
                    </Link>
                </Button>
            </div>
            <h1 className='scroll-m-20 pb-6 text-3xl font-light tracking-tight'>Company Dashboard</h1>
            <div className='flex flex-col gap-6 lg:flex-row'>
                <aside className='w-full shrink-0 lg:w-64'>
                    <div className='sticky top-4 rounded-lg border bg-card p-4'>
                        <SidebarNav groups={navGroups} />
                    </div>
                </aside>
                <main className='min-w-0 flex-1'>
                    <Outlet />
                </main>
            </div>
        </div>
    );
};
