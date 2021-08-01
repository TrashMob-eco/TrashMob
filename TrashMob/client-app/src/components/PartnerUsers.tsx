import * as React from 'react'
import { RouteComponentProps } from 'react-router-dom';
import UserData from './Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import PartnerUserData from './Models/PartnerUserData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import * as ToolTips from "../store/ToolTips";

export interface PartnerUsersDataProps extends RouteComponentProps {
    partnerId: string;
    partnerUsers: PartnerUserData[];
    isPartnerUserDataLoaded: boolean;
    onPartnerUsersUpdated: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerUsers: React.FC<PartnerUsersDataProps> = (props) => {

    const [userName, setUserName] = React.useState<string>("");

    function removeUser(userId: string, userName: string) {
        if (!window.confirm("Do you want to remove user with userName: " + userName))
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
                    .then(() => {
                        props.onPartnerUsersUpdated()
                    });
            });
        }
    }

    function handleSearch(event: any) {
        
        if (userName == "")
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
                    if (!window.confirm("Do you want to add user with userName: " + userName + " and email address: " + data.email))
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
        setUserName(userName);
    }

    function renderPartnerUserNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPrimaryEmail}</Tooltip>
    }

    function renderPartnerUsersTable(partnerUsers: PartnerUserData[]) {
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
                        {partnerUsers.map(partnerUser =>
                            <tr key={partnerUser.id.toString()}>
                                <td>{partnerUser.userName}</td>
                                <td>{partnerUser.email}</td>
                                <td>
                                    <Button className="action" onClick={() => removeUser(partnerUser.id, partnerUser.userName)}>Remove User</Button>
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
                <Form onSubmit={handleSearch}>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderPartnerUserNameToolTip}>
                                    <Form.Label htmlFor="UserName">User Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="username" defaultValue={userName} onChange={val => handleUserNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={(e) => handleSearch(e)}>Search</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {!props.isPartnerUserDataLoaded && <p><em>Loading...</em></p>}
                {props.isPartnerUserDataLoaded && renderPartnerUsersTable(props.partnerUsers)}
                renderAddUser();
            </div>
        </>
    );
}