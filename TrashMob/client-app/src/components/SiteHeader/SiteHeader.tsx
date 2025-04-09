import React from 'react';
import { Link } from 'react-router';
import { Menu, Plus } from 'lucide-react';
import { cn } from '@/lib/utils';
import { MainNav } from './MainNav';
import { Button } from '../ui/button';
import { UserNav } from './UserNav';
import UserData from '../Models/UserData';
import { Logo } from '../Logo';
import { Divider } from './Divider';

interface SiteHeaderProps {
    currentUser: UserData;
    isUserLoaded: boolean;
}

export const SiteHeader = (props: SiteHeaderProps) => {
    const { currentUser, isUserLoaded } = props;
    const [show, setShow] = React.useState<boolean>(false);

    return (
        <div className='tailwind'>
            <div className='border-b shadow-md shadow-black/10 bg-white py-4'>
                <div className='container'>
                    <div className='flex items-center flex-wrap flex-row'>
                        <a className='-ml-2 grow lg:grow-0' href='/'>
                            <Logo className='w-[165px]' />
                        </a>
                        <Button
                            className='lg:hidden w-10 h-10 [&_svg]:size-6'
                            variant='outline'
                            aria-label='Toggle menu'
                            onClick={() => setShow(!show)}
                        >
                            <Menu strokeWidth={1} />
                        </Button>
                        <Divider className='!ml-4 hidden lg:block' />
                        <div
                            className={cn(
                                'flex flex-1 !px-4',
                                'overflow-hidden transition-all duration-300',
                                'items-start justify-between flex-col basis-full',
                                'lg:items-center lg:justify-between lg:flex-row lg:basis-0',
                                'lg:max-h-none',
                                {
                                    'max-h-84': show,
                                    'max-h-0': !show,
                                },
                            )}
                        >
                            <MainNav className='flex-1 !py-4 lg:!py-0' isUserLoaded={isUserLoaded} />
                            <div className={cn('flex flex-row gap-4')}>
                                <Button asChild className='flex md:hidden xl:flex'>
                                    <Link to='/events/create'>
                                        <Plus /> Create an Event
                                    </Link>
                                </Button>
                                <UserNav currentUser={currentUser} isUserLoaded={isUserLoaded} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
