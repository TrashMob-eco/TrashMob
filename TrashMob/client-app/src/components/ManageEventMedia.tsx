import * as React from 'react'
import UserData from './Models/UserData';

export interface ManageEventMediaProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventMedia: React.FC<ManageEventMediaProps> = () => {
 
    return (
        <>
            <div>
                <p><em>Feature under construction</em></p>
            </div>
        </>
    );
}
