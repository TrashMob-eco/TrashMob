import { useState } from 'react';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { HoverCard, HoverCardContent, HoverCardTrigger } from '@/components/ui/hover-card';
import { CircleUserRound, IdCard, LogOut, MapPin, User, UserRoundX } from 'lucide-react';
import { Link } from 'react-router';
import { cn } from '@/lib/utils';
import UserData from '../Models/UserData';
import { getApiConfig, getMsalClientInstance } from '@/store/AuthStore';
import { AgeGateDialog } from '@/components/AgeGate/AgeGateDialog';
import React from 'react';

interface UserNavProps {
    currentUser: UserData;
    isUserLoaded: boolean;
    className?: string;
}

export const UserNav = (props: UserNavProps) => {
    const { currentUser, isUserLoaded, className } = props;
    const [showAgeGate, setShowAgeGate] = useState(false);

    function signIn(e: React.MouseEvent) {
        e.preventDefault();
        const apiConfig = getApiConfig();

        getMsalClientInstance().loginRedirect({
            scopes: apiConfig.scopes,
        });
    }

    function handleCreateAccount(e: React.MouseEvent) {
        e.preventDefault();
        setShowAgeGate(true);
    }

    function handleAgeGateConfirm() {
        setShowAgeGate(false);
        const apiConfig = getApiConfig();
        getMsalClientInstance().loginRedirect({
            scopes: apiConfig.scopes,
        });
    }

    function signOut(e: React.MouseEvent) {
        e.preventDefault();
        const logoutRequest = {
            account: getMsalClientInstance().getActiveAccount(),
        };

        getMsalClientInstance().logout(logoutRequest);
    }

    return (
        <div className={cn('flex flex-row items-center gap-2 basis-full lg:basis-0', className)}>
            {!isUserLoaded && (
                <>
                    <Button variant='outline' className='text-current border-primary' onClick={signIn}>
                        Sign in
                    </Button>
                    <Button onClick={handleCreateAccount}>Create Account</Button>
                    <AgeGateDialog open={showAgeGate} onOpenChange={setShowAgeGate} onConfirm={handleAgeGateConfirm} />
                </>
            )}
            {isUserLoaded ? (
                <HoverCard openDelay={200} closeDelay={100}>
                    <DropdownMenu>
                        <HoverCardTrigger asChild>
                            <DropdownMenuTrigger asChild>
                                <Button
                                    variant='outline'
                                    size='icon'
                                    className='rounded-full h-10 w-10'
                                    aria-label={`Account menu for ${currentUser.userName}`}
                                >
                                    {currentUser.profilePhotoUrl ? (
                                        <img
                                            src={currentUser.profilePhotoUrl}
                                            alt={currentUser.userName}
                                            className='h-6 w-6 rounded-full object-cover'
                                            referrerPolicy='no-referrer'
                                        />
                                    ) : (
                                        <CircleUserRound size={24} />
                                    )}
                                </Button>
                            </DropdownMenuTrigger>
                        </HoverCardTrigger>
                        <HoverCardContent align='end' className='w-72'>
                            <div className='flex flex-col gap-2'>
                                <div className='flex items-center gap-3'>
                                    {currentUser.profilePhotoUrl ? (
                                        <img
                                            src={currentUser.profilePhotoUrl}
                                            alt={currentUser.userName}
                                            className='h-10 w-10 rounded-full object-cover'
                                            referrerPolicy='no-referrer'
                                        />
                                    ) : (
                                        <CircleUserRound size={40} className='text-muted-foreground' />
                                    )}
                                    <div className='flex flex-col overflow-hidden'>
                                        <span className='font-semibold truncate'>
                                            {currentUser.userName || 'Welcome'}
                                        </span>
                                        {currentUser.email ? (
                                            <span className='text-sm text-muted-foreground truncate'>
                                                {currentUser.email}
                                            </span>
                                        ) : null}
                                    </div>
                                </div>
                                {currentUser.city || currentUser.region ? (
                                    <div className='flex items-center gap-2 text-sm text-muted-foreground'>
                                        <MapPin size={14} />
                                        <span>
                                            {[currentUser.city, currentUser.region, currentUser.country]
                                                .filter(Boolean)
                                                .join(', ')}
                                        </span>
                                    </div>
                                ) : null}
                                {currentUser.isSiteAdmin ? (
                                    <span className='text-xs bg-primary/10 text-primary px-2 py-0.5 rounded-full w-fit'>
                                        Site Admin
                                    </span>
                                ) : null}
                            </div>
                        </HoverCardContent>
                        <DropdownMenuContent align='end'>
                            <DropdownMenuItem asChild>
                                <Link to='/myprofile'>
                                    <User />
                                    <span>Update my profile</span>
                                </Link>
                            </DropdownMenuItem>
                            {currentUser.isSiteAdmin ? (
                                <DropdownMenuItem asChild>
                                    <Link to='/siteadmin'>
                                        <IdCard />
                                        <span>Site Administration</span>
                                    </Link>
                                </DropdownMenuItem>
                            ) : null}
                            <DropdownMenuSeparator />
                            <DropdownMenuItem asChild>
                                <Link to='/deletemydata'>
                                    <UserRoundX />
                                    <span>Delete my account</span>
                                </Link>
                            </DropdownMenuItem>
                            <DropdownMenuItem onClick={signOut}>
                                <LogOut />
                                <span>Sign out</span>
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </HoverCard>
            ) : null}
        </div>
    );
};
