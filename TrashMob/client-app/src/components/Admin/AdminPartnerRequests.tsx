import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import PartnerRequestData from '../Models/PartnerRequestData';

interface AdminPartnerRequestsPropsType extends RouteComponentProps {
    partnerRequestList: PartnerRequestData[];
    isPartnerRequestDataLoaded: boolean;
    onPartnerRequestListChanged: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminPartnerRequests: React.FC<AdminPartnerRequestsPropsType> = (props) => {

    // Handle approve request for a partner  
    function handleApprove(id: string, name: string) {
        if (!window.confirm("Do you want to approve partner with name: " + name))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('PUT');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('api/partnerrequests/' + id, {
                    method: 'put',
                    headers: headers
                }).then(() => { props.onPartnerRequestListChanged(); });
            });
        }
    }

    function renderPartnerRequestsTable(partnerRequests: PartnerRequestData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Primary Email</th>
                            <th>Secondary Email</th>
                            <th>Primary Phone</th>
                            <th>Secondary Phone</th>
                            <th>Notes</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partnerRequests.map(partnerRequest => {
                            return (
                                <tr key={partnerRequest.id.toString()}>
                                    <td>{partnerRequest.name}</td>
                                    <td>{partnerRequest.primaryEmail}</td>
                                    <td>{partnerRequest.secondaryEmail}</td>
                                    <td>{partnerRequest.primaryPhone}</td>
                                    <td>{partnerRequest.secondaryPhone}</td>
                                    <td>
                                        <Button className="action" onClick={() => handleApprove(partnerRequest.id, partnerRequest.name)}>Delete Partner</Button>
                                    </td>
                                </tr>)
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    let contents = props.isPartnerRequestDataLoaded
        ? renderPartnerRequestsTable(props.partnerRequestList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel">Partner Requests</h1>
            {contents}
        </div>
    );
}

