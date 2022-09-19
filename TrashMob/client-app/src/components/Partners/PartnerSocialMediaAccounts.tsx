import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import SocialMediaAccountData from '../Models/SocialMediaAccountData';
import SocialMediaAccountTypeData from '../Models/SocialMediaAccountTypeData';

export interface PartnerSocialMediaAccountsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerSocialMediaAccounts: React.FC<PartnerSocialMediaAccountsDataProps> = (props) => {

    const [accountName, setAccountName] = React.useState<string>("");
    const [socialMediaAccounts, setSocialMediaAccounts] = React.useState<SocialMediaAccountData[]>([]);
    const [isSocialMediaAccountsDataLoaded, setIsSocialMediaAccountDataLoaded] = React.useState<boolean>(false);
    const [socialMediaAccountTypeId, setSocialMediaAccountTypeId] = React.useState<number>(0);
    const [socialMediaAccountTypeList, setSocialMediaAccountTypeList] = React.useState<SocialMediaAccountTypeData[]>([]);

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

                fetch('/api/partnersocialmediaaccounts/' + props.partnerId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<SocialMediaAccountData[]>)
                    .then(data => {
                        setSocialMediaAccounts(data);
                        setIsSocialMediaAccountDataLoaded(true);
                    });
            });
        }
    }, [props.partnerId, props.isUserLoaded])

    function removeAccount(userId: string, accountName: string) {
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

                fetch('/api/partnersocialmediaaccounts/' + props.partnerId + '/' + userId, {
                    method: 'DELETE',
                    headers: headers,
                })
            });
        }
    }

    function handleAddSocialMediaAccount() {
        
        if (accountName === "")
            return;

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var socialMediaAccountData = new SocialMediaAccountData();
        socialMediaAccountData.accountIdentifier = accountName;
        socialMediaAccountData.socialMediaAccountTypeId = socialMediaAccountTypeId ?? 0;

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnersocialmediaaccounts/' + props.partnerId, {
                  method: 'POST',
                  headers: headers,
                 })
        });
    }

    function handleAccountNameChanged(accountName: string) {
        setAccountName(accountName);
    }


    function selectSocialMediaAccountType(val: string) {
        setSocialMediaAccountTypeId(parseInt(val));
    }

    function renderSocialMediaAccountNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountName}</Tooltip>
    }

    function renderSocialMediaAccountTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.SocialMediaAccountType}</Tooltip>
    }

    function renderPartnerSocialMediaAccountsTable(accounts: SocialMediaAccountData[]) {
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
                        {accounts.map(account => 
                                    <tr key={account.id.toString()}>
                                        <td>{account.accountIdentifier}</td>
                                        <td>{account.socialMediaAccountTypeId}</td>
                                        <td>
                                            <Button className="action" onClick={() => removeAccount(account.id, account.accountIdentifier)}>Remove Account</Button>
                                        </td>
                                    </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderAddPartnerSocialMediaAccount() {
        return (
            <div>
                <Form onSubmit={handleAddSocialMediaAccount}>
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
                                    <select data-val="true" name="mediaTypeId" defaultValue={socialMediaAccountTypeId} onChange={(val) => selectSocialMediaAccountType(val.target.value)} required>
                                        <option value="">-- Select Media Type --</option>
                                        {socialMediaAccountTypeList.map(type =>
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={() => handleAddSocialMediaAccount()}>Add Account</Button>
                    </Form.Row>
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
                {renderAddPartnerSocialMediaAccount()}
            </div>
        </>
    );
}
