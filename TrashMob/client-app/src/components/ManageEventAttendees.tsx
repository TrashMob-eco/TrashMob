import * as React from 'react'
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import UserData from './Models/UserData';

export interface ManageEventAttendeesProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventAttendees: React.FC<ManageEventAttendeesProps> = (props) => {
    const [eventAttendees, setEventAttendees] = React.useState<UserData[]>([]);
    const [isEventAttendeeDataLoaded, setIsEventAttendeeDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {
        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/eventattendees/' + props.eventId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<UserData[]>)
                    .then(data => {
                        setEventAttendees(data);
                        setIsEventAttendeeDataLoaded(true);
                    });
            });
        }
    }, [props.eventId, props.isUserLoaded])

    function renderEventAttendeesTable(users: UserData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>User Name</th>
                            <th>City</th>
                            <th>Country</th>
                            <th>Member Since</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user =>
                            <tr key={user.id.toString()}>
                                <td>{user.userName ? user.userName : user.sourceSystemUserName}</td>
                                <td>{user.city}</td>
                                <td>{user.country}</td>
                                <td>{new Date(user.memberSince).toLocaleDateString()}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    return (
        <>
            <div>
                {!isEventAttendeeDataLoaded && <p><em>Loading...</em></p>}
                {isEventAttendeeDataLoaded && renderEventAttendeesTable(eventAttendees)}
            </div>
        </>
    );
}
