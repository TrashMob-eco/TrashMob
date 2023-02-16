import * as React from 'react'

import { RouteComponentProps } from 'react-router-dom';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { Col, Container, Row } from 'react-bootstrap';
import EmailTemplateData from '../Models/EmailTemplateData';

interface AdminEmailTemplatesPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const AdminEmailTemplates: React.FC<AdminEmailTemplatesPropsType> = (props) => {

    const [emailList, setEmailList] = React.useState<EmailTemplateData[]>([]);
    const [isEmailDataLoaded, setIsEmailDataLoaded] = React.useState<boolean>(false);

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

                // Load the User List
                fetch('/api/admin/emailtemplates', {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<EmailTemplateData>>)
                    .then(data => {
                        setEmailList(data);
                        setIsEmailDataLoaded(true);
                    });
            })
        }
    }, [props.isUserLoaded])

    function renderEmailTable(emails: EmailTemplateData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Email Templates</h2>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Content</th>
                        </tr>
                    </thead>
                    <tbody>
                        {emails.sort((a,b) => (a.name > b.name) ? 1: -1).map(email => {
                            return (
                                <tr key={email.name}>
                                    <td>{email.name}</td>
                                    <td><div dangerouslySetInnerHTML={{ __html: email.content }} /></td>
                                </tr>)
                        }
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    let contents = isEmailDataLoaded
        ? renderEmailTable(emailList)
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

