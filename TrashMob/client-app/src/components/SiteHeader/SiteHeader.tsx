import React from 'react';
import { Menu } from 'lucide-react';
import logo from '@/logo.svg';
import { cn } from '@/lib/utils';
import { MainNav } from './MainNav';
import { Button } from '../ui/button';
import { UserNav } from './UserNav';
import UserData from '../Models/UserData';

interface SiteHeaderProps {
    currentUser: UserData;
    isUserLoaded: boolean;
}

export const SiteHeader = (props: SiteHeaderProps) => {
    const { currentUser, isUserLoaded } = props;
    const [show, setShow] = React.useState<boolean>(false);

    return (
        <div className='tailwind'>
            <div className='border-b shadow-md shadow-black/10 bg-white py-3'>
                <div className='container'>
                    <div className='flex items-center flex-wrap flex-row'>
                        <a className='-ml-2 xl:w-[230px] grow lg:grow-0' href='/'>
                            <img src={logo} className='w-48' alt='Trashmob logo' />
                        </a>
                        <Button
                            className='lg:hidden w-10 h-10 [&_svg]:size-6 bg-primary'
                            variant='outline'
                            aria-label='Toggle menu'
                            onClick={() => setShow(!show)}
                        >
                            <Menu strokeWidth={1} />
                        </Button>
                        <div
                            className={cn(
                                'flex flex-1',
                                'overflow-hidden transition-all duration-300',
                                'items-start justify-between flex-col basis-full',
                                'lg:items-center lg:justify-between lg:flex-row lg:basis-0',
                                'lg:max-h-none',
                                {
                                    'max-h-64': show,
                                    'max-h-0': !show,
                                },
                            )}
                        >
                            <MainNav className='flex-1' isUserLoaded={isUserLoaded} />
                            <UserNav currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
