import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";

export interface PartnerAdminsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerAdmins: React.FC<PartnerAdminsDataProps> = (props) => {

    const [userEmail, setUserEmail] = React.useState<string>("");
    const [administrators, setAdministrators] = React.useState<UserData[]>([]);
    const [isPartnerAdminDataLoaded, setIsPartnerAdminDataLoaded] = React.useState<boolean>(false);

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


                fetch('/api/partneradmins/' + props.partnerId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<UserData[]>)
                    .then(data => {
                        setAdministrators(data);
                        setIsPartnerAdminDataLoaded(true);
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    function removeUser(userId: string, userName: string) {
        if (!window.confirm("Please confirm that you want to remove user with userName: '" + userName + "' as a user from this Partner?"))
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

                fetch('/api/partnerusers/' + props.partnerId + '/' + userId, {
                    method: 'DELETE',
                    headers: headers,
                })
            });
        }
    }

    function handleAddUser() {

        if (userEmail === "")
            return;

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/users/getUserByUserName/' + userEmail, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<UserData>)
                .then(data => {
                    if (!window.confirm("Please confirm you want to add user with userName: '" + userEmail + "' and email address: '" + data.email + "' as a user for this Partner."))
                        return;
                    else {
                        const headers = getDefaultHeaders('POST');
                        headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
                        fetch('/api/partnerusers/' + props.partnerId + "/" + data.id, {
                            method: 'POST',
                            headers: headers,
                        })
                    }
                });
        });
    }

    function handleUserNameChanged(userName: string) {
        setUserEmail(userName);
    }

    function renderPartnerUserNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerUserNameSearch}</Tooltip>
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
                            <tr key={user.id}>
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
                                <OverlayTrigger placement="top" overlay={renderPartnerUserNameToolTip}>
                                    <Form.Label className="control-label" htmlFor="UserName">Enter the User Name to Add:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="username" defaultValue={userEmail} onChange={val => handleUserNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={() => handleAddUser()}>Add User</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    var contents = isPartnerAdminDataLoaded && props.partnerId
        ? renderUsersTable(administrators)
        : <p><em>Loading...</em></p>;

    return <div>
        <hr />
        {contents}
        {renderAddUser()}
    </div>;
}