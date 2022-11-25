import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, ToggleButton, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import SocialMediaAccountTypeData from '../Models/SocialMediaAccountTypeData';
import PartnerSocialMediaAccountData from '../Models/PartnerSocialMediaAccountData';
import { getSocialMediaAccountType } from '../../store/socialMediaAccountTypeHelper';

export interface PartnerSocialMediaAccountsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerSocialMediaAccounts: React.FC<PartnerSocialMediaAccountsDataProps> = (props) => {

    const [socialMediaAccountId, setSocialMediaAccountId] = React.useState<string>(Guid.EMPTY);
    const [accountName, setAccountName] = React.useState<string>("");
    const [isActive, setIsActive] = React.useState<boolean>(true);
    const [socialMediaAccounts, setSocialMediaAccounts] = React.useState<PartnerSocialMediaAccountData[]>([]);
    const [isSocialMediaAccountsDataLoaded, setIsSocialMediaAccountDataLoaded] = React.useState<boolean>(false);
    const [socialMediaAccountTypeId, setSocialMediaAccountTypeId] = React.useState<number>(0);
    const [accountNameErrors, setAccountNameErrors] = React.useState<string>("");
    const [socialMediaAccountTypeList, setSocialMediaAccountTypeList] = React.useState<SocialMediaAccountTypeData[]>([]);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/socialmediaaccounttypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setSocialMediaAccountTypeList(data);
            });

        if (props.isUserLoaded && props.partnerId && props.partnerId !== Guid.EMPTY) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnersocialmediaaccounts/getbypartner/' + props.partnerId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerSocialMediaAccountData[]>)
                    .then(data => {
                        setSocialMediaAccounts(data);
                        setIsSocialMediaAccountDataLoaded(true);
                    });
            });
        }
    }, [props.partnerId, props.isUserLoaded])

    function addSocialMediaAccount() {
        setAccountName("");
        setSocialMediaAccountTypeId(0);
        setIsEditOrAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setAccountName("");
        setSocialMediaAccountTypeId(0);
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsEditOrAdd(false);
        setIsAddEnabled(true);
    }

    function editAccount(partnerAccountId: string) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnersocialmediaaccounts/' + partnerAccountId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerSocialMediaAccountData>)
                .then(data => {
                    setSocialMediaAccountId(data.id);
                    setAccountName(data.accountIdentifier);
                    setSocialMediaAccountTypeId(data.socialMediaAccountTypeId);
                    setIsActive(data.isActive);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(new Date(data.createdDate));
                    setLastUpdatedDate(new Date(data.lastUpdatedDate));
                    setIsEditOrAdd(true);
                    setIsAddEnabled(false);
                });
        });
    }

    function removeAccount(accountId: string, accountName: string) {
        if (!window.confirm("Please confirm that you want to remove social media account with name: '" + accountName + "' as a social media account from this Partner?"))
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

                fetch('/api/partnersocialmediaaccounts/' + accountId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsSocialMediaAccountDataLoaded(false);

                        fetch('/api/partnersocialmediaaccounts/getbypartner/' + props.partnerId, {
                        method: 'GET',
                        headers: headers,
                    })
                        .then(response => response.json() as Promise<PartnerSocialMediaAccountData[]>)
                        .then(data => {
                            setSocialMediaAccounts(data);
                            setIsSocialMediaAccountDataLoaded(true);
                        });
                })
            });
        }
    }

    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var socialMediaAccountData = new PartnerSocialMediaAccountData();
        socialMediaAccountData.id = socialMediaAccountId;
        socialMediaAccountData.partnerId = props.partnerId;
        socialMediaAccountData.accountIdentifier = accountName;
        socialMediaAccountData.isActive = isActive;
        socialMediaAccountData.socialMediaAccountTypeId = socialMediaAccountTypeId ?? 0;
        socialMediaAccountData.createdByUserId = createdByUserId;

        var data = JSON.stringify(socialMediaAccountData);

        var method = "PUT";

        if (socialMediaAccountId === Guid.EMPTY) {
            method = "POST";
        }

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnersocialmediaaccounts', {
                method: method,
                headers: headers,
                body: data,
            })
                .then(() => {
                    setIsEditOrAdd(false);
                    setIsSocialMediaAccountDataLoaded(false);
                    var getHeaders = getDefaultHeaders("GET");
                    getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    fetch('/api/partnersocialmediaaccounts/getbypartner/' + props.partnerId, {
                        method: 'GET',
                        headers: getHeaders,
                    })
                        .then(response => response.json() as Promise<PartnerSocialMediaAccountData[]>)
                        .then(data => {
                            setSocialMediaAccounts(data);
                            setIsSocialMediaAccountDataLoaded(true);
                            setIsEditOrAdd(false);
                            setIsAddEnabled(true);
                        });
                });
        });
    }

    function validateForm() {
        if (accountName === "" ||
            accountNameErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function handleAccountNameChanged(val: string) {
        if (val === "") {
            setAccountNameErrors("Name cannot be blank.");
        }
        else {
            setAccountNameErrors("");
            setAccountName(val);
        }

        validateForm();
    }

    function handleIsActiveChanged(active: boolean) {
        setIsActive(active);
        validateForm();
    }

    function selectSocialMediaAccountType(val: string) {
        setSocialMediaAccountTypeId(parseInt(val));
        validateForm();
    }

    function renderSocialMediaAccountNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountName}</Tooltip>
    }

    function renderSocialMediaAccountTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountType}</Tooltip>
    }

    function renderIsActiveToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountIsActive}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerSocialMediaAccountCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerSocialMediaAccountLastUpdatedDate}</Tooltip>
    }

    function renderPartnerSocialMediaAccountsTable(accounts: PartnerSocialMediaAccountData[]) {
        return (
            <div>
                <p>This page allows you to add a list of social media accounts you would like to have tagged when you approve a partnership request to both help spread the word about what TrashMob.eco users are
                    doing within your community, and how your organization is helping your community. This feature is still in development, but adding the information when you set things up now will help
                    when this feature fully launches.
                </p>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Account Name</th>
                            <th>Account Type</th>
                            <th>Is Active</th>
                        </tr>
                    </thead>
                    <tbody>
                        {accounts.map(account =>
                            <tr key={account.id.toString()}>
                                <td>{account.accountIdentifier}</td>
                                <td>{getSocialMediaAccountType(socialMediaAccountTypeList, account.socialMediaAccountTypeId)}</td>
                                <td>{account.isActive === true ? "Yes" : "No"} </td>
                                <td>
                                    <Button className="action" onClick={() => editAccount(account.id)}>Edit</Button>
                                    <Button className="action" onClick={() => removeAccount(account.id, account.accountIdentifier)}>Remove Account</Button>
                                 </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addSocialMediaAccount()}>Add SocialMediaAccount</Button>
            </div>
        );
    }

    function renderAddPartnerSocialMediaAccount() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderSocialMediaAccountNameToolTip}>
                                    <Form.Label className="control-label" htmlFor="AccountName">AccountName</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="username" defaultValue={accountName} onChange={val => handleAccountNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderSocialMediaAccountTypeToolTip}>
                                    <Form.Label className="control-label" htmlFor="SocialMediaAccountType">Social Media Account Type:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select data-val="true" name="socialMediaAccountTypeId" defaultValue={socialMediaAccountTypeId} onChange={(val) => selectSocialMediaAccountType(val.target.value)} required>
                                        <option value="">-- Select Media Type --</option>
                                        {socialMediaAccountTypeList.map(type =>
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderIsActiveToolTip}>
                                    <ToggleButton
                                        type="checkbox"
                                        variant="outline-dark"
                                        checked={isActive}
                                        value="1"
                                        onChange={(e) => handleIsActiveChanged(e.currentTarget.checked)}
                                    >
                                        Is Active
                                    </ToggleButton>
                                </OverlayTrigger >
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Group className="form-group">
                        <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                        <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                    </Form.Group >
                    <Form.Group className="form-group">
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                <Form.Label className="control-label">Created Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control type="text" disabled defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label">Last Updated Date:</Form.Label>
                            </OverlayTrigger>
                            <Form.Group>
                                <Form.Control type="text" disabled defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                    </Form.Group >
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {props.partnerId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                {!isSocialMediaAccountsDataLoaded && props.partnerId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isSocialMediaAccountsDataLoaded && renderPartnerSocialMediaAccountsTable(socialMediaAccounts)}
                {isEditOrAdd && renderAddPartnerSocialMediaAccount()}
            </div>
        </>
    );
}
