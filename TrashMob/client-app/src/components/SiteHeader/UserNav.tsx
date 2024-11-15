import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
    ChevronDown,
    CircleUserRound,
    CircleHelp,
    Gauge,
    IdCard,
    LogOut,
    Map,
    Plus,
    User,
    UserRoundX,
} from 'lucide-react';
import { Link } from 'react-router-dom';
import { cn } from '@/lib/utils';
import UserData from '../Models/UserData';
import { getApiConfig, getB2CPolicies, msalClient } from '@/store/AuthStore';
import React from 'react';

interface UserNavProps {
    currentUser: UserData;
    isUserLoaded: boolean;
    className?: string;
}

export const UserNav = (props: UserNavProps) => {
    const { currentUser, isUserLoaded, className } = props;

    function signIn(e: React.MouseEvent) {
        e.preventDefault();
        const apiConfig = getApiConfig();

        msalClient.loginRedirect({
            scopes: apiConfig.b2cScopes,
        });
    }

    function signOut(e: React.MouseEvent) {
        e.preventDefault();
        const logoutRequest = {
            account: msalClient.getActiveAccount(),
        };

        msalClient.logout(logoutRequest);
    }

    function profileEdit(e: React.MouseEvent) {
        e.preventDefault();

        const account = msalClient.getAllAccounts()[0];
        const policy = getB2CPolicies();
        const scopes = getApiConfig();

        const request = {
            account,
            authority: policy.authorities.profileEdit.authority,
            scopes: scopes.b2cScopes,
        };

        msalClient.acquireTokenRedirect(request);
    }

    return (
        <div className={cn('flex flex-row item-center basis-full lg:basis-0', className)}>
            {!isUserLoaded && (
                <Button variant='outline' className='text-primary border-primary' onClick={signIn}>
                    Sign in
                </Button>
            )}
            {isUserLoaded ? (
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant='outline'>
                            <CircleUserRound size={32} />
                            {currentUser.userName || 'Welcome'}
                            <ChevronDown />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent>
                        <DropdownMenuItem asChild>
                            <Link to='/mydashboard'>
                                <Gauge />
                                <span>My Dashboard</span>
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to='/manageeventdashboard'>
                                <Plus />
                                <span>Add Event</span>
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuItem onClick={profileEdit}>
                            <User />
                            <span>Update my profile</span>
                        </DropdownMenuItem>
                        <DropdownMenuItem asChild>
                            <Link to='/locationpreference'>
                                <Map />
                                <span>My Location Preference</span>
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
                        <DropdownMenuItem asChild>
                            <Link to='/deletemydata'>
                                <UserRoundX />
                                <span>Delete my account</span>
                            </Link>
                        </DropdownMenuItem>
                        <DropdownMenuSeparator />
                        <DropdownMenuItem onClick={signOut}>
                            <LogOut />
                            <span>Sign out</span>
                        </DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            ) : null}
            <Button variant='outline' size='icon' className='!ml-3 !mr-2 border-primary' asChild>
                <Link to='/help'>
                    <CircleHelp />
                </Link>
            </Button>
        </div>
    );
};
