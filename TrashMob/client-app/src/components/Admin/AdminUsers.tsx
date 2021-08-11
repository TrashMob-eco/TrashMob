import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';

interface AdminUsersPropsType extends RouteComponentProps {
    userList: UserData[];
    isUserDataLoaded: boolean;
    onUserListChanged: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminUsers: React.FC<AdminUsersPropsType> = (props) => {

    // Handle Delete request for an event  
    function handleDelete(id: string, name: string) {
        if (!window.confirm("Do you want to delete user with name: " + name))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('api/users/' + id, {
                    method: 'delete',
                    headers: headers
                }).then(() => { props.onUserListChanged(); });
            });
        }
    }

    function renderUsersTable(users: UserData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>UserName</th>
                            <th>Email</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user => {
                            return (
                                <tr key={user.id.toString()}>
                                    <td>{user.userName}</td>
                                    <td>{user.email}</td>
                                    <td>{user.city}</td>
                                    <td>{user.region}</td>
                                    <td>{user.country}</td>
                                    <td>{user.postalCode}</td>
                                    <td>
                                        <Button className="action" onClick={() => handleDelete(user.id, user.userName)}>Delete User</Button>
                                    </td>
                                </tr>)
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    let contents = props.isUserDataLoaded
        ? renderUsersTable(props.userList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel" >Users</h1>
            {contents}
        </div>
    );
}

