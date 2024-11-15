import * as React from 'react';

import { RouteComponentProps } from 'react-router-dom';
import { Col, Container, Row } from 'react-bootstrap';
import { useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import EmailTemplateData from '../Models/EmailTemplateData';
import { GetAdminEmailTemplates } from '../../services/admin';
import { Services } from '../../config/services.config';

interface AdminEmailTemplatesPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const AdminEmailTemplates: React.FC<AdminEmailTemplatesPropsType> = (props) => {
    const [emailList, setEmailList] = React.useState<EmailTemplateData[]>([]);
    const [isEmailDataLoaded, setIsEmailDataLoaded] = React.useState<boolean>(false);

    const getAdminEmailTemplates = useQuery({
        queryKey: GetAdminEmailTemplates().key,
        queryFn: GetAdminEmailTemplates().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    React.useEffect(() => {
        if (props.isUserLoaded) {
            getAdminEmailTemplates.refetch().then((res) => {
                setEmailList(res.data?.data || []);
                setIsEmailDataLoaded(true);
            });
        }
    }, [props.isUserLoaded]);

    function renderEmailTable(emails: EmailTemplateData[]) {
        return (
            <div>
                <h2 className='color-primary mt-4 mb-5'>Email Templates</h2>
                <table className='table table-striped' aria-labelledby='tableLabel'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Content</th>
                        </tr>
                    </thead>
                    <tbody>
                        {emails
                            .sort((a, b) => (a.name > b.name ? 1 : -1))
                            .map((email) => (
                                <tr key={email.name}>
                                    <td>{email.name}</td>
                                    <td>
                                        <div
                                            dangerouslySetInnerHTML={{
                                                __html: email.content,
                                            }}
                                        />
                                    </td>
                                </tr>
                            ))}
                    </tbody>
                </table>
            </div>
        );
    }

    const contents = isEmailDataLoaded ? (
        renderEmailTable(emailList)
    ) : (
        <p>
            <em>Loading...</em>
        </p>
    );

    return (
        <Container>
            <Row className='gx-2 py-5' lg={2}>
                <Col lg={12}>
                    <div className='bg-white p-5 shadow-sm rounded'>{contents}</div>
                </Col>
            </Row>
        </Container>
    );
};
