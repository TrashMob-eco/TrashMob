import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Button } from 'react-bootstrap';
import PartnerData from '../Models/PartnerData';

interface AdminPartnersPropsType extends RouteComponentProps {
    partnerList: PartnerData[];
    isPartnerDataLoaded: boolean;
    onPartnerListChanged: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminPartners: React.FC<AdminPartnersPropsType> = (props) => {

    // Handle Delete request for an partner
    function handleDelete(id: string, name: string) {
        if (!window.confirm("Do you want to delete partner with name: " + name))
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

                fetch('api/partners/' + id, {
                    method: 'delete',
                    headers: headers
                }).then(() => { props.onPartnerListChanged(); });
            });
        }
    }

    function renderPartnersTable(partners: PartnerData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partners.map(partner => {
                            return (
                                <tr key={partner.id.toString()}>
                                    <td>{partner.name}</td>
                                    <td>
                                        <Button className="action" onClick={() => handleDelete(partner.id, partner.name)}>Delete Partner</Button>
                                    </td>
                                </tr>)
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    let contents = props.isPartnerDataLoaded
        ? renderPartnersTable(props.partnerList)
        : <p><em>Loading...</em></p>;

    return (
        <div>
            <h1 id="tableLabel">Partners</h1>
            {contents}
        </div>
    );
}

