import { Link } from 'react-router';
import { cn } from '@/lib/utils';

export const MainNav = ({
    className,
    isUserLoaded,
    ...props
}: React.HTMLAttributes<HTMLElement> & { readonly isUserLoaded: boolean }) => {
    const mainNavItems = [
        { name: 'Getting Started', to: '/gettingstarted' },
        ...(isUserLoaded ? [{ name: 'My Dashboard', to: '/mydashboard' }] : []),
        { name: 'Events', to: '/#events', component: 'a' },
        { name: 'Donate', to: 'https://donate.stripe.com/14k9DN2EnfAog9O3cc' },
        { name: 'Shop', to: '/shop' },
    ];

    return (
        <nav className={cn('flex flex-col flex-1 gap-3 items-start lg:flex-row', className)} {...props}>
            {mainNavItems.map((nav) => {
                if (nav.component === 'a') {
                    return (
                        <a
                            className='text-sm font-medium text-foreground no-underline transition-colors hover:text-[--primary] !p-2 !pl-0 !lg:pl-2'
                            href={nav.to}
                            key={nav.name}
                        >
                            {nav.name}
                        </a>
                    );
                } else {
                    return (
                        <Link
                            className='text-sm font-medium text-foreground no-underline transition-colors hover:text-[--primary] !p-2 !pl-0 !lg:pl-2'
                            key={nav.name}
                            to={nav.to}
                        >
                            {nav.name}
                        </Link>
                    );
                }
            })}
        </nav>
    );
};
