import { Link } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { Divider } from './Divider';

export const MainNav = ({
    className,
    isUserLoaded,
    ...props
}: React.HTMLAttributes<HTMLElement> & { isUserLoaded: boolean }) => {
    const mainNavItems = [
        { name: 'Getting Started', url: '/gettingstarted' },
        ...(isUserLoaded ? [{ name: 'My Dashboard', url: '/mydashboard' }] : []),
        { name: 'Events', url: '/#events' },
        { name: 'Donate', url: 'https://donate.stripe.com/14k9DN2EnfAog9O3cc' },
        { name: 'Shop', url: '/shop' },
    ];

    return (
        <nav
            className={cn('flex flex-col flex-1 items-start lg:flex-row lg:items-center lg:justify-evenly', className)}
            {...props}
        >
            {mainNavItems.map((nav) => (
                <Link
                    to={nav.url}
                    key={nav.name}
                    className='text-lg lg:text-base xl:text-lg font-medium text-foreground no-underline transition-colors hover:text-[--primary] !p-2 !pl-0 !lg:pl-2'
                >
                    {nav.name}
                </Link>
            ))}
            <Divider />
        </nav>
    );
};
