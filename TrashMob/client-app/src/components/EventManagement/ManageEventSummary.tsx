import * as React from 'react'
import UserData from '../Models/UserData';

export interface ManagerEventSummaryProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventSummary: React.FC<ManagerEventSummaryProps> = (props) => {
    return (
        <>
            <div>
                <p><em>Feature under construction</em></p>
            </div>
        </>
    );
}
