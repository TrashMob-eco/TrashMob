import { Outlet } from 'react-router';
import {
    Users,
    Calendar,
    Building2,
    UsersRound,
    Trash2,
    Handshake,
    Briefcase,
    Bell,
    Mail,
    FileText,
    MessageSquare,
    ImageIcon,
    ScrollText,
    Send,
    Newspaper,
    Search,
    Target,
    Upload,
} from 'lucide-react';
import { SidebarNav, NavGroup } from '@/components/ui/sidebar-nav';

const pathPrefix = '/siteadmin';

const navGroups: NavGroup[] = [
    {
        title: 'Data Management',
        items: [
            { name: 'Users', href: `${pathPrefix}/users`, icon: Users },
            { name: 'Events', href: `${pathPrefix}/events`, icon: Calendar },
            { name: 'Partners', href: `${pathPrefix}/partners`, icon: Building2 },
            { name: 'Teams', href: `${pathPrefix}/teams`, icon: UsersRound },
            { name: 'Litter Reports', href: `${pathPrefix}/litter-reports`, icon: Trash2 },
            { name: 'Partner Requests', href: `${pathPrefix}/partner-requests`, icon: Handshake },
            { name: 'Job Opportunities', href: `${pathPrefix}/job-opportunities`, icon: Briefcase },
            { name: 'Prospects', href: `${pathPrefix}/prospects`, icon: Target },
            { name: 'Discovery', href: `${pathPrefix}/prospects/discovery`, icon: Search },
            { name: 'Import CSV', href: `${pathPrefix}/prospects/import`, icon: Upload },
        ],
    },
    {
        title: 'Communications',
        items: [
            { name: 'Send Notifications', href: `${pathPrefix}/send-notifications`, icon: Bell },
            { name: 'Email Templates', href: `${pathPrefix}/email-templates`, icon: Mail },
            { name: 'Newsletters', href: `${pathPrefix}/newsletters`, icon: Newspaper },
            { name: 'Bulk Invites', href: `${pathPrefix}/invites`, icon: Send },
        ],
    },
    {
        title: 'Moderation',
        items: [
            { name: 'User Feedback', href: `${pathPrefix}/feedback`, icon: MessageSquare },
            { name: 'Photo Moderation', href: `${pathPrefix}/photo-moderation`, icon: ImageIcon },
            { name: 'Manage Content', href: `${pathPrefix}/content`, icon: FileText },
            { name: 'Waivers', href: `${pathPrefix}/waivers`, icon: ScrollText },
        ],
    },
];

export const SiteAdminLayout = () => {
    return (
        <div className='container mx-auto py-6'>
            <h1 className='scroll-m-20 pb-6 text-3xl font-light tracking-tight'>Site Administration</h1>
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
