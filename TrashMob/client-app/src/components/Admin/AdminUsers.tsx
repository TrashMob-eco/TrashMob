import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';

interface AdminUsersPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminUsers: React.FC<AdminUsersPropsType> = (props) => {

    const [userList, setUserList] = React.useState<UserData[]>([]);
    const [isUserDataLoaded, setIsUserDataLoaded] = React.useState<boolean>(false);

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

                // Load the User List
                fetch('/api/users', {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<UserData>>)
                    .then(data => {
                        setUserList(data);
                        setIsUserDataLoaded(true);
                    });
            })
        }
    }, [props.isUserLoaded])

    // Handle Delete request for a user  
    function handleDelete(id: string, name: string) {
        if (!window.confirm("Are you sure you want to delete user with name: " + name))
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
                }).then(() => {
                    const getHeaders = getDefaultHeaders('GET');
                    getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    // Load the User List
                    fetch('/api/users', {
                        method: 'GET',
                        headers: getHeaders,
                    })
                        .then(response => response.json() as Promise<Array<UserData>>)
                        .then(data => {
                            setUserList(data);
                            setIsUserDataLoaded(true);
                        });
                });
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

    let contents = isUserDataLoaded
        ? renderUsersTable(userList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel" >Users</h1>
            {contents}
        </div>
    );
}

