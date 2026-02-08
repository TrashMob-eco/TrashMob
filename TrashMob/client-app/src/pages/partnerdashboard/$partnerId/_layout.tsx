import { useMemo } from 'react';
import { Outlet, useParams } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import {
    ClipboardList,
    Building2,
    MapPin,
    Wrench,
    Users,
    ShieldCheck,
    FileText,
    Share2,
    LayoutDashboard,
    Edit,
    Map,
    ClipboardCheck,
    Mail,
    Globe,
} from 'lucide-react';
import { SidebarNav, NavGroup } from '@/components/ui/sidebar-nav';
import { GetPartnerById } from '@/services/partners';

export const PartnerLayout = () => {
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const { data: partner } = useQuery({
        queryKey: GetPartnerById({ partnerId }).key,
        queryFn: GetPartnerById({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    const pathPrefix = `/partnerdashboard/${partnerId}`;

    const navGroups = useMemo(() => {
        const groups: NavGroup[] = [
            {
                title: 'Partner Management',
                items: [
                    { name: 'Service Requests', href: `${pathPrefix}`, icon: ClipboardList },
                    { name: 'Edit Partner', href: `${pathPrefix}/edit`, icon: Building2 },
                    { name: 'Locations', href: `${pathPrefix}/locations`, icon: MapPin },
                    { name: 'Services', href: `${pathPrefix}/services`, icon: Wrench },
                    { name: 'Contacts', href: `${pathPrefix}/contacts`, icon: Users },
                    { name: 'Admins', href: `${pathPrefix}/admins`, icon: ShieldCheck },
                    { name: 'Documents', href: `${pathPrefix}/documents`, icon: FileText },
                    { name: 'Social Media', href: `${pathPrefix}/socials`, icon: Share2 },
                ],
            },
        ];

        if (partner && partner.partnerTypeId === 1) {
            groups.push({
                title: 'Community',
                items: [
                    { name: 'Dashboard', href: `${pathPrefix}/community`, icon: LayoutDashboard },
                    { name: 'Edit Content', href: `${pathPrefix}/community/content`, icon: Edit },
                    { name: 'Regional Settings', href: `${pathPrefix}/community/regional-settings`, icon: Globe },
                    { name: 'Adoptable Areas', href: `${pathPrefix}/community/areas`, icon: Map },
                    { name: 'Adoptions', href: `${pathPrefix}/community/adoptions`, icon: ClipboardCheck },
                    { name: 'Invites', href: `${pathPrefix}/community/invites`, icon: Mail },
                ],
            });
        }

        return groups;
    }, [pathPrefix, partner]);

    return (
        <div className='container mx-auto py-6'>
            <h1 className='scroll-m-20 pb-6 text-3xl font-light tracking-tight'>
                {partner?.name || 'Partner Dashboard'}
            </h1>
            <div className='flex flex-col gap-6 lg:flex-row'>
                <aside className='w-full shrink-0 lg:w-64'>
                    <div className='sticky top-4 rounded-lg border bg-card p-4'>
                        <SidebarNav groups={navGroups} />
                    </div>
                </aside>
                <main className='min-w-0 flex-1'>
                    <Outlet context={{ partner }} />
                </main>
            </div>
        </div>
    );
};
