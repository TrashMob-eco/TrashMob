import { Link } from 'react-router';
import { cn } from '@/lib/utils';
import {
    NavigationMenu,
    NavigationMenuContent,
    NavigationMenuItem,
    NavigationMenuLink,
    NavigationMenuList,
    NavigationMenuTrigger,
    navigationMenuTriggerStyle,
} from '@/components/ui/navigation-menu';
import {
    MapPin,
    CalendarPlus,
    Gauge,
    Map,
    BookOpen,
    HelpCircle,
    ShoppingBag,
    List,
    Users,
    Building2,
    Trophy,
    Award,
    Sparkles,
} from 'lucide-react';
import React from 'react';

interface ListItemProps extends React.ComponentPropsWithoutRef<'a'> {
    title: string;
    icon?: React.ReactNode;
    to: string;
}

const ListItem = React.forwardRef<HTMLAnchorElement, ListItemProps>(
    ({ className, title, children, icon, to, ...props }, ref) => {
        return (
            <li>
                <NavigationMenuLink asChild>
                    <Link
                        ref={ref}
                        to={to}
                        className={cn(
                            'block select-none space-y-1 rounded-md p-3 leading-none no-underline outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground',
                            className,
                        )}
                        {...props}
                    >
                        <div className='flex items-center gap-2 text-sm font-medium leading-none'>
                            {icon}
                            {title}
                        </div>
                        {children ? (
                            <p className='line-clamp-2 text-sm leading-snug text-muted-foreground'>{children}</p>
                        ) : null}
                    </Link>
                </NavigationMenuLink>
            </li>
        );
    },
);
ListItem.displayName = 'ListItem';

// Mobile nav item component for collapsed menu
const MobileNavItem = ({ to, children, external }: { to: string; children: React.ReactNode; external?: boolean }) => {
    if (external) {
        return (
            <a
                href={to}
                className='block text-sm font-medium text-foreground no-underline transition-colors hover:text-primary py-2'
                target='_blank'
                rel='noopener noreferrer'
            >
                {children}
            </a>
        );
    }
    return (
        <Link
            to={to}
            className='block text-sm font-medium text-foreground no-underline transition-colors hover:text-primary py-2'
        >
            {children}
        </Link>
    );
};

interface MainNavProps extends React.HTMLAttributes<HTMLElement> {
    isUserLoaded: boolean;
}

export const MainNav = ({ className, isUserLoaded, ...props }: MainNavProps) => {
    return (
        <nav className={cn('flex flex-col flex-1 gap-3 items-start lg:flex-row lg:items-center', className)} {...props}>
            {/* Desktop Navigation */}
            <div className='hidden lg:block'>
                <NavigationMenu delayDuration={100}>
                    <NavigationMenuList>
                        {/* Explore Dropdown - Events, Teams, Communities, Leaderboards */}
                        <NavigationMenuItem>
                            <NavigationMenuTrigger>Explore</NavigationMenuTrigger>
                            <NavigationMenuContent>
                                <ul className='grid w-[320px] gap-1 p-2'>
                                    <ListItem to='/#events' title='Events' icon={<CalendarPlus className='h-4 w-4' />}>
                                        Find cleanup events happening near you.
                                    </ListItem>
                                    <ListItem to='/teams' title='Teams' icon={<Users className='h-4 w-4' />}>
                                        Join or create a team to volunteer together.
                                    </ListItem>
                                    <ListItem
                                        to='/communities'
                                        title='Communities'
                                        icon={<Building2 className='h-4 w-4' />}
                                    >
                                        Discover community programs and partnerships.
                                    </ListItem>
                                    <ListItem
                                        to='/leaderboards'
                                        title='Leaderboards'
                                        icon={<Trophy className='h-4 w-4' />}
                                    >
                                        See top volunteers and teams.
                                    </ListItem>
                                </ul>
                            </NavigationMenuContent>
                        </NavigationMenuItem>

                        {/* Take Action Dropdown */}
                        <NavigationMenuItem>
                            <NavigationMenuTrigger>Take Action</NavigationMenuTrigger>
                            <NavigationMenuContent>
                                <ul className='grid w-[280px] gap-1 p-2'>
                                    <ListItem
                                        to='/litterreports/create'
                                        title='Report Litter'
                                        icon={<MapPin className='h-4 w-4' />}
                                    >
                                        Spotted litter? Report it so others can help clean it up.
                                    </ListItem>
                                    <ListItem
                                        to='/events/create'
                                        title='Create Event'
                                        icon={<CalendarPlus className='h-4 w-4' />}
                                    >
                                        Organize a cleanup event in your community.
                                    </ListItem>
                                    <ListItem
                                        to='/litterreports'
                                        title='View Litter Reports'
                                        icon={<List className='h-4 w-4' />}
                                    >
                                        See reported litter locations near you.
                                    </ListItem>
                                </ul>
                            </NavigationMenuContent>
                        </NavigationMenuItem>

                        {/* My TrashMob Dropdown (only when logged in) */}
                        {isUserLoaded ? (
                            <NavigationMenuItem>
                                <NavigationMenuTrigger>My TrashMob</NavigationMenuTrigger>
                                <NavigationMenuContent>
                                    <ul className='grid w-[280px] gap-1 p-2'>
                                        <ListItem
                                            to='/mydashboard'
                                            title='My Dashboard'
                                            icon={<Gauge className='h-4 w-4' />}
                                        >
                                            View your events, stats, and activity.
                                        </ListItem>
                                        <ListItem
                                            to='/locationpreference'
                                            title='Location Preference'
                                            icon={<Map className='h-4 w-4' />}
                                        >
                                            Set your preferred location for nearby events.
                                        </ListItem>
                                        <ListItem
                                            to='/achievements'
                                            title='Achievements'
                                            icon={<Award className='h-4 w-4' />}
                                        >
                                            Track your volunteer milestones and badges.
                                        </ListItem>
                                    </ul>
                                </NavigationMenuContent>
                            </NavigationMenuItem>
                        ) : null}

                        {/* About Dropdown */}
                        <NavigationMenuItem>
                            <NavigationMenuTrigger>About</NavigationMenuTrigger>
                            <NavigationMenuContent>
                                <ul className='grid w-[280px] gap-1 p-2'>
                                    <ListItem to='/whatsnew' title="What's New" icon={<Sparkles className='h-4 w-4' />}>
                                        See the latest features and updates.
                                    </ListItem>
                                    <ListItem
                                        to='/gettingstarted'
                                        title='Getting Started'
                                        icon={<BookOpen className='h-4 w-4' />}
                                    >
                                        New to TrashMob? Learn how to get involved.
                                    </ListItem>
                                    <ListItem to='/help' title='Help & FAQ' icon={<HelpCircle className='h-4 w-4' />}>
                                        Find answers to common questions.
                                    </ListItem>
                                    <ListItem to='/shop' title='Shop' icon={<ShoppingBag className='h-4 w-4' />}>
                                        Get TrashMob gear and supplies.
                                    </ListItem>
                                </ul>
                            </NavigationMenuContent>
                        </NavigationMenuItem>

                        {/* Donate - Direct Link with emphasis */}
                        <NavigationMenuItem>
                            <NavigationMenuLink asChild className={navigationMenuTriggerStyle()}>
                                <a
                                    href='https://donate.stripe.com/14k9DN2EnfAog9O3cc'
                                    target='_blank'
                                    rel='noopener noreferrer'
                                    className='text-primary font-semibold'
                                >
                                    Donate
                                </a>
                            </NavigationMenuLink>
                        </NavigationMenuItem>
                    </NavigationMenuList>
                </NavigationMenu>
            </div>

            {/* Mobile Navigation - Flat list */}
            <div className='flex flex-col gap-1 lg:hidden w-full'>
                <div className='border-l-2 border-muted pl-3 ml-2 space-y-1'>
                    <p className='text-xs text-muted-foreground uppercase tracking-wide pt-1'>Explore</p>
                    <MobileNavItem to='/#events'>Events</MobileNavItem>
                    <MobileNavItem to='/teams'>Teams</MobileNavItem>
                    <MobileNavItem to='/communities'>Communities</MobileNavItem>
                    <MobileNavItem to='/leaderboards'>Leaderboards</MobileNavItem>
                </div>
                <div className='border-l-2 border-muted pl-3 ml-2 space-y-1'>
                    <p className='text-xs text-muted-foreground uppercase tracking-wide pt-1'>Take Action</p>
                    <MobileNavItem to='/litterreports/create'>Report Litter</MobileNavItem>
                    <MobileNavItem to='/events/create'>Create Event</MobileNavItem>
                    <MobileNavItem to='/litterreports'>View Litter Reports</MobileNavItem>
                </div>
                {isUserLoaded ? (
                    <div className='border-l-2 border-muted pl-3 ml-2 space-y-1'>
                        <p className='text-xs text-muted-foreground uppercase tracking-wide pt-1'>My TrashMob</p>
                        <MobileNavItem to='/mydashboard'>My Dashboard</MobileNavItem>
                        <MobileNavItem to='/locationpreference'>Location Preference</MobileNavItem>
                        <MobileNavItem to='/achievements'>Achievements</MobileNavItem>
                    </div>
                ) : null}
                <div className='border-l-2 border-muted pl-3 ml-2 space-y-1'>
                    <p className='text-xs text-muted-foreground uppercase tracking-wide pt-1'>About</p>
                    <MobileNavItem to='/whatsnew'>What's New</MobileNavItem>
                    <MobileNavItem to='/gettingstarted'>Getting Started</MobileNavItem>
                    <MobileNavItem to='/help'>Help & FAQ</MobileNavItem>
                    <MobileNavItem to='/shop'>Shop</MobileNavItem>
                </div>
                <MobileNavItem to='https://donate.stripe.com/14k9DN2EnfAog9O3cc' external>
                    Donate
                </MobileNavItem>
            </div>
        </nav>
    );
};
