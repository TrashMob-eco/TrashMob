import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Col, Container, Dropdown, Row } from 'react-bootstrap';
import PartnerData from '../Models/PartnerData';
import { XSquare } from 'react-bootstrap-icons';

interface AdminPartnersPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminPartners: React.FC<AdminPartnersPropsType> = (props) => {
    const [partnerList, setPartnerList] = React.useState<PartnerData[]>([]);
    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);

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

                // Load the Partner List
                fetch('/api/partners', {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<PartnerData>>)
                    .then(data => {
                        setPartnerList(data);
                        setIsPartnerDataLoaded(true);
                    });
            })
        }
    }, [props.isUserLoaded])

    // Handle Delete request for an partner
    function handleDelete(id: string, name: string) {
        if (!window.confirm("Are you sure you want to delete partner with name: " + name))
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

                fetch('api/partners/' + id, {
                    method: 'delete',
                    headers: headers
                }).then(() => {
                    const getHeaders = getDefaultHeaders('GET');
                    getHeaders.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                    // Load the Partner List
                    fetch('/api/partners', {
                        method: 'GET',
                        headers: getHeaders,
                    })
                        .then(response => response.json() as Promise<Array<PartnerData>>)
                        .then(data => {
                            setPartnerList(data);
                            setIsPartnerDataLoaded(true);
                        });
                });
            });
        }
    }


    const partnerActionDropdownList = (partnerId: string, partnerName: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => handleDelete(partnerId, partnerName)}><XSquare />Delete Partner</Dropdown.Item>
            </>
        )
    }

    function renderPartnersTable(partners: PartnerData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Partners</h2>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partners.map(partner => {
                            return (
                                <tr key={partner.id.toString()}>
                                    <td>{partner.name}</td>
                                    <td className="btn py-0">
                                        <Dropdown role="menuitem">
                                            <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                            <Dropdown.Menu id="share-menu">
                                                {partnerActionDropdownList(partner.id, partner.name)}
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

    let contents = isPartnerDataLoaded
        ? renderPartnersTable(partnerList)
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
