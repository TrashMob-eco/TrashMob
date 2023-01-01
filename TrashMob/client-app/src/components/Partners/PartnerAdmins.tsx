import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Container, Dropdown, Form, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import PartnerAdminInvitationData from '../Models/PartnerAdminInvitationData';
import * as Constants from '../Models/Constants';
import { Guid } from 'guid-typescript';
import { getInvitationStatus } from '../../store/invitationStatusHelper';
import InvitationStatusData from '../Models/InvitationStatusData';
import { Envelope, XSquare } from 'react-bootstrap-icons';

export interface PartnerAdminsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerAdmins: React.FC<PartnerAdminsDataProps> = (props) => {

    const [userEmail, setUserEmail] = React.useState<string>("");
    const [administrators, setAdministrators] = React.useState<UserData[]>([]);
    const [isPartnerAdminDataLoaded, setIsPartnerAdminDataLoaded] = React.useState<boolean>(false);
    const [userEmailErrors, setUserEmailErrors] = React.useState<string>("");
    const [partnerAdminInvitations, setPartnerAdminInvitations] = React.useState<PartnerAdminInvitationData[]>([]);
    const [isPartnerAdminInvitationsDataLoaded, setIsPartnerAdminInvitationsDataLoaded] = React.useState<boolean>(false);
    const [invitationStatusList, setInvitationStatusList] = React.useState<InvitationStatusData[]>([]);
    const [isSendEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/invitationstatuses', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setInvitationStatusList(data);
            });

        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

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
                    })
                    .then(() => {
                        setIsPartnerAdminInvitationsDataLoaded(false);
                        var getHeaders = getDefaultHeaders("GET");
                        getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                        fetch('/api/partneradmininvitations/' + props.partnerId, {
                            method: 'GET',
                            headers: getHeaders,
                        })
                            .then(response => response.json() as Promise<PartnerAdminInvitationData[]>)
                            .then(data => {
                                setPartnerAdminInvitations(data);
                                setIsPartnerAdminInvitationsDataLoaded(true);
                                setIsEditOrAdd(false);
                                setIsAddEnabled(true);
                            });
                    });
            });
        }
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    function removeUser(userId: string, email: string) {
        if (!window.confirm("Please confirm that you want to remove user with email: '" + email + "' as a user from this Partner?"))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partneradmins/' + props.partnerId + '/' + userId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        fetch('/api/partneradmins/' + props.partnerId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<UserData[]>)
                            .then(data => {
                                setAdministrators(data);
                                setIsPartnerAdminDataLoaded(true);
                            })
                    });
            });
        }
    }

    function handleResendInvite(invitationId: string, email: string) {

        if (!window.confirm("Please confirm you want to resend invite to user with Email: '" + email + "'")) {
            return;
        }
        else {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('POST');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
                fetch('/api/partneradmininvitations/resend/' + invitationId, {
                    method: 'POST',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerAdminInvitationsDataLoaded(false);
                        var getHeaders = getDefaultHeaders("GET");
                        getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                        fetch('/api/partneradmininvitations/getbypartner/' + props.partnerId, {
                            method: 'GET',
                            headers: getHeaders,
                        })
                            .then(response => response.json() as Promise<PartnerAdminInvitationData[]>)
                            .then(data => {
                                setPartnerAdminInvitations(data);
                                setIsPartnerAdminInvitationsDataLoaded(true);
                                setIsEditOrAdd(false);
                                setIsAddEnabled(true);
                            });
                    });
            });
        }
    }

    function handleCancelInvite(invitationId: string, email: string) {

        if (!window.confirm("Please confirm you want to cancel invite for user with Email: '" + email + "'")) {
            return;
        }
        else {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
                fetch('/api/partneradmininvitations/' + invitationId, {
                    method: 'POST',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerAdminInvitationsDataLoaded(false);
                        var getHeaders = getDefaultHeaders("GET");
                        getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                        fetch('/api/partneradmininvitations/getbypartner/' + props.partnerId, {
                            method: 'GET',
                            headers: getHeaders,
                        })
                            .then(response => response.json() as Promise<PartnerAdminInvitationData[]>)
                            .then(data => {
                                setPartnerAdminInvitations(data);
                                setIsPartnerAdminInvitationsDataLoaded(true);
                                setIsEditOrAdd(false);
                                setIsAddEnabled(true);
                            });
                    });
            });
        }
    }

    function handleSendInvite(event: any) {

        event.preventDefault();

        if (!isSendEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        if (!window.confirm("Please confirm you want to send an invitation to be an Administator for this Partner to: " + userEmail)) {
            return;
        }
        else {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            var partnerAdminInvitationData = new PartnerAdminInvitationData();
            partnerAdminInvitationData.partnerId = props.partnerId;
            partnerAdminInvitationData.email = userEmail ?? "";
            partnerAdminInvitationData.invitationStatusId = 1;

            var data = JSON.stringify(partnerAdminInvitationData);

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('POST');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);
                fetch('/api/partneradmininvitations', {
                    method: 'POST',
                    body: data,
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerAdminInvitationsDataLoaded(false);
                        var getHeaders = getDefaultHeaders("GET");
                        getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                        fetch('/api/partneradmininvitations/' + props.partnerId, {
                            method: 'GET',
                            headers: getHeaders,
                        })
                            .then(response => response.json() as Promise<PartnerAdminInvitationData[]>)
                            .then(data => {
                                setPartnerAdminInvitations(data);
                                setIsPartnerAdminInvitationsDataLoaded(true);
                                setIsEditOrAdd(false);
                                setIsAddEnabled(true);
                            });
                    });
            });
        }
    }

    function addAdmin() {
        setUserEmail("");
        setIsAddEnabled(false);
        setIsEditOrAdd(true);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setUserEmail("");
        setIsSaveEnabled(false);
        setIsAddEnabled(true);
    }

    function validateForm() {
        if (userEmail === "" ||
            userEmailErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function handleUserEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setUserEmailErrors("Please enter valid email address.");
        }
        else {
            setUserEmailErrors("");
            setUserEmail(val);
        }

        validateForm();
    }

    function renderPartnerUserNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerUserNameSearch}</Tooltip>
    }

    const adminActionDropdownList = (userId: string, userName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => removeUser(userId, userName)}><XSquare />Remove Admin</Dropdown.Item>
            </>
        )
    }

    const inviteActionDropdownList = (invitationId: string, invitationEmail: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => handleResendInvite(invitationId, invitationEmail)}><Envelope />Resend Invite</Dropdown.Item>
                <Dropdown.Item onClick={() => handleCancelInvite(invitationId, invitationEmail)}><XSquare />Cancel Invite</Dropdown.Item>
            </>
        )
    }

    function renderUsersTable(users: UserData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Current Admins</h2>
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
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {adminActionDropdownList(user.id, user.userName)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addAdmin()}>Send Invite to New Admin</Button>
            </div>
        );
    }

    function renderInvitationsTable(invitations: PartnerAdminInvitationData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Pending Invitations</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Email</th>
                            <th>Status</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {invitations.map(invitation =>
                            <tr key={invitation.id}>
                                <td>{invitation.email}</td>
                                <td>{getInvitationStatus(invitationStatusList, invitation.invitationStatusId)}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {inviteActionDropdownList(invitation.id, invitation.email)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderSendInvite() {
        return (
            <div>
                <Form onSubmit={handleSendInvite}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPartnerUserNameToolTip}>
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="UserName">Enter the Email to Send Invitation to</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="userEmail" defaultValue={userEmail} onChange={val => handleUserEmailChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Form.Group className="form-group">
                            <Button disabled={!isSendEnabled} type="submit" className="action btn-default">Send</Button>
                            <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                        </Form.Group >
                    </Form.Row>
                </Form>
            </div>
        );
    }

    var partnerAdminContents = isPartnerAdminDataLoaded && props.partnerId !== Guid.EMPTY
        ? renderUsersTable(administrators)
        : <p><em>Loading...</em></p>;

    var partnerAdminInvitationsContents = isPartnerAdminInvitationsDataLoaded && props.partnerId !== Guid.EMPTY
        ? renderInvitationsTable(partnerAdminInvitations)
        : <p><em>Loading...</em></p>;

    return <div>
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Edit Partner Admins</h2>
                        <p>
                            This page allows you to add more administrators to this partner so you can share the load of maintaining the configuration of the partner. You can invite new
                            administrators by clicking the Invite Administrator button, and entering their email address into the text box and clicking "Send Invitation."
                        </p>
                        <p>
                            The email address you set will be sent an invite to join TrashMob.eco if they are not already a user. Once they have joined TrashMob.eco and are logged in,
                            they will see an invitation in their Dashboard. They can Accept or Decline the invitation from there.
                        </p>
                    </div>
                </Col>
                <Col lg={8}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                        {partnerAdminContents}
                        {partnerAdminInvitationsContents}
                        {isEditOrAdd && renderSendInvite()}
                    </div>
                </Col>
            </Row>
        </Container>
    </div>;
}