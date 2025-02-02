import * as React from 'react';

import { Col, Container, Dropdown, Row } from 'react-bootstrap';
import { XSquare } from 'react-bootstrap-icons';
import { useMutation, useQuery } from '@tanstack/react-query';
import PartnerData from '../Models/PartnerData';
import UserData from '../Models/UserData';
import { DeletePartnerById, GetPartners } from '../../services/partners';
import { Services } from '../../config/services.config';

interface AdminPartnersPropsType {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const AdminPartners: React.FC<AdminPartnersPropsType> = (props) => {
    const [partnerList, setPartnerList] = React.useState<PartnerData[]>([]);
    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);

    const getPartners = useQuery({
        queryKey: GetPartners().key,
        queryFn: GetPartners().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const deletePartnerById = useMutation({
        mutationKey: DeletePartnerById().key,
        mutationFn: DeletePartnerById().service,
    });

    React.useEffect(() => {
        if (props.isUserLoaded) {
            getPartners.refetch().then((res) => {
                setPartnerList(res.data?.data || []);
                setIsPartnerDataLoaded(true);
            });
        }
    }, [props.isUserLoaded]);

    // Handle Delete request for an partner
    function handleDelete(id: string, name: string) {
        if (!window.confirm(`Are you sure you want to delete partner with name: ${name}`)) return;

        deletePartnerById.mutateAsync({ id }).then(() => {
            getPartners.refetch().then((res) => {
                setPartnerList(res.data?.data || []);
                setIsPartnerDataLoaded(true);
            });
        });
    }

    const partnerActionDropdownList = (partnerId: string, partnerName: string) => (
        <Dropdown.Item onClick={() => handleDelete(partnerId, partnerName)}>
            <XSquare />
            Delete Partner
        </Dropdown.Item>
    );

    function renderPartnersTable(partners: PartnerData[]) {
        return (
            <div>
                <h2 className='color-primary mt-4 mb-5'>Partners</h2>
                <table className='table table-striped' aria-labelledby='tableLabel'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partners.map((partner) => (
                            <tr key={partner.id.toString()}>
                                <td>{partner.name}</td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {partnerActionDropdownList(partner.id, partner.name)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    }

    const contents = isPartnerDataLoaded ? (
        renderPartnersTable(partnerList)
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
