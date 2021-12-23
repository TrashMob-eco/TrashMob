import * as React from 'react'
import UserData from './Models/UserData';
import { apiConfig, getDefaultHeaders, msalClient } from '../store/AuthStore';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Button, Col, Form, ToggleButton, ToggleButtonGroup } from 'react-bootstrap';
import UserNotificationPreferenceData, { UserNotificationPreferenceDefaults } from './Models/UserNotificationPreferenceData';

interface NotificationPreferencesProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

const NotificationPreferences: React.FC<NotificationPreferencesProps> = (props) => {
    const userId = props.currentUser.id;
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [userNotificationPreferences, setUserNotificationPreferences] = React.useState<UserNotificationPreferenceData[]>(UserNotificationPreferenceDefaults);
    const [isOptedOutOfAllEmails, setIsOptedOutOfAllEmails] = React.useState<boolean>(false);

    React.useEffect(() => {

        if (props.isUserLoaded && !isDataLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/users/' + userId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<UserData>)
                    .then(data => {
                        setIsOptedOutOfAllEmails(data.isOptedOutOfAllEmails);
                        setIsDataLoaded(true);
                    });

                fetch('/api/usernotificationpreferences/' + userId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<UserNotificationPreferenceData[]>)
                    .then(data => {
                        if (data) {
                            var mergedPrefs: UserNotificationPreferenceData[] = userNotificationPreferences.map((userpref) => {
                                var loadedPref = data.find((p) => p.userNotificationTypeId === userpref.userNotificationTypeId);
                                if (loadedPref) {
                                    loadedPref.userFriendlyName = userpref.userFriendlyName;
                                    return loadedPref;
                                }

                                return userpref;
                            });

                            setUserNotificationPreferences(mergedPrefs);
                            setIsDataLoaded(true);
                        }
                    })
            });
        }

    }, [userId, userNotificationPreferences, props.isUserLoaded, isDataLoaded])

    function setIsOptedOut(userNotificationId: number) {
        const updatedPrefs: UserNotificationPreferenceData[] = userNotificationPreferences.map((item, index) => {
            if (item.userNotificationTypeId === userNotificationId)
                item.isOptedOut = !item.isOptedOut;
            return item;
        });

        setUserNotificationPreferences(updatedPrefs);
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        var userprefs: UserNotificationPreferenceData[] = userNotificationPreferences.map((userpref) => {
            var val = new UserNotificationPreferenceData();
            val.id = userpref.id;
            val.isOptedOut = userpref.isOptedOut;
            val.userId = userId;
            val.userNotificationTypeId = userpref.userNotificationTypeId;

            return val;
        });

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var usrprefdata = JSON.stringify(userprefs);

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {

            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/users/updateemailoptout/' + userId + '/' + isOptedOutOfAllEmails,
                {
                    method: 'PUT',
                    headers: headers,
                }).then(() => {
                    fetch('/api/usernotificationpreferences/' + userId, {
                        method: 'PUT',
                        headers: headers,
                        body: usrprefdata,
                    }).then(() => {
                        props.history.push("/");
                    });
                })
        });
    }

    return (
        !isDataLoaded ? <div>Loading</div> :
            <div>
                <h2>User Notification Preferences</h2>
                <div className="container-fluid" >
                    <Form onSubmit={handleSave} >
                        <Form.Row>
                            <h3>Email Notification Preferences</h3>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <Button type="submit" className="action btn-default">Save</Button>
                                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Form.Label>Email Notification</Form.Label>
                            <Col>
                                <Form.Group>
                                    <Form.Label>Check the box below to opt out of all email notifications</Form.Label>
                                    <ToggleButton
                                        type="checkbox"
                                        variant="outline-dark"
                                        checked={isOptedOutOfAllEmails}
                                        value="1"
                                        size="sm"
                                        onChange={(e) => setIsOptedOutOfAllEmails(e.currentTarget.checked)}
                                        block
                                    >Opt Out of All Emails</ToggleButton>
                                </Form.Group>
                            </Col>
                            <Col xs="2" >
                                <h4>- OR -</h4>
                            </Col>
                            <Col>
                                <Form.Label>Check the box below to opt out of certain types of email notifications</Form.Label>
                                <ToggleButtonGroup size="sm" type="checkbox" vertical>
                                    {userNotificationPreferences.map((pref) => (
                                        <Form.Group>
                                            <ToggleButton
                                                type="checkbox"
                                                variant="outline-dark"
                                                checked={pref.isOptedOut}
                                                value="1"
                                                onChange={(e) => setIsOptedOut(pref.userNotificationTypeId)}
                                            >{pref.userFriendlyName}</ToggleButton>
                                        </Form.Group>
                                    ))}
                                </ToggleButtonGroup>
                            </Col>
                        </Form.Row>
                    </Form >
                </div>
            </div >
    );
}

export default withRouter(NotificationPreferences);