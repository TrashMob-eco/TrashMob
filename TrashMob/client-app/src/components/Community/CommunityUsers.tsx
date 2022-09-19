import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";

export interface CommunityUsersDataProps {
    communityId: string;
    users: UserData[];
    isUserDataLoaded: boolean;
    onCommunityUsersUpdated: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const CommunityUsers: React.FC<CommunityUsersDataProps> = (props) => {

    const [userName, setUserName] = React.useState<string>("");

    function removeUser(userId: string, userName: string) {
        if (!window.confirm("Please confirm that you want to remove user with userName: '" + userName + "' as a user from this Community?"))
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

                fetch('/api/communityusers/' + props.communityId + '/' + userId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        props.onCommunityUsersUpdated()
                    });
            });
        }
    }

    function handleAddUser() {
        
        if (userName === "")
            return;

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/users/getUserByUserName/' + userName, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<UserData>)
                .then(data => {
                    if (!window.confirm("Please confirm you want to add user with userName: '" + userName + "' and email address: '" + data.email + "' as a user for this Community."))
                        return;
                    else {
                        const headers = getDefaultHeaders('POST');
                        headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
                        fetch('/api/communityusers/' + props.communityId + "/" + data.id, {
                            method: 'POST',
                            headers: headers,
                        })
                    }
                });        
        });
    }

    function handleUserNameChanged(userName: string) {
        setUserName(userName);
    }

    function renderCommunityUserNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityUserNameSearch}</Tooltip>
    }

    function renderUsersTable(users: UserData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Username</th>
                            <th>Email</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map(user => 
                                    <tr key={user.id.toString()}>
                                        <td>{user.userName}</td>
                                        <td>{user.email}</td>
                                        <td>
                                            <Button className="action" onClick={() => removeUser(user.id, user.userName)}>Remove User</Button>
                                        </td>
                                    </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderAddUser() {
        return (
            <div>
                <Form onSubmit={handleAddUser}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderCommunityUserNameToolTip}>
                                    <Form.Label className="control-label" htmlFor="UserName">Search for User Name to Add:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="username" defaultValue={userName} onChange={val => handleUserNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={() => handleAddUser()}>Add User</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {!props.isUserDataLoaded && <p><em>Loading...</em></p>}
                {props.isUserDataLoaded && props.users && renderUsersTable(props.users)}
                {renderAddUser()}
            </div>
        </>
    );
}