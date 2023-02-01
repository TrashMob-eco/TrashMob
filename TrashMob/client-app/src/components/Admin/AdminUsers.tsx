import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Col, Container, Dropdown, Row } from 'react-bootstrap';
import { XSquare } from 'react-bootstrap-icons';

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
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {

                if (!validateToken(tokenResponse.idTokenClaims)) {
                    return;
                }

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
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {

                if (!validateToken(tokenResponse.idTokenClaims)) {
                    return;
                }

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

    const userActionDropdownList = (userId: string, userName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => handleDelete(userId, userName)}><XSquare />Delete User</Dropdown.Item>
            </>
        )
    }
    function renderUsersTable(users: UserData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Users</h2>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>UserName</th>
                            <th>Email</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Postal Code</th>
                            <th>Actions</th>
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
                                    <td className="btn py-0">
                                        <Dropdown role="menuitem">
                                            <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                            <Dropdown.Menu id="share-menu">
                                                {userActionDropdownList(user.id, user.userName)}
                                            </Dropdown.Menu>
                                        </Dropdown>
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
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={12}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {contents}
                    </div>
                </Col>
            </Row>
        </Container >
    );
}

