import * as React from 'react';

import { RouteComponentProps } from 'react-router-dom';
import { Col, Container, Dropdown, Row } from 'react-bootstrap';
import { CheckSquare, XSquare } from 'react-bootstrap-icons';
import PhoneInput from 'react-phone-input-2';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import PartnerRequestData from '../Models/PartnerRequestData';
import PartnerRequestStatusData from '../Models/PartnerRequestStatusData';
import { getPartnerRequestStatus } from '../../store/partnerRequestStatusHelper';
import * as Constants from '../Models/Constants';
import { Services } from '../../config/services.config';
import {
    ApprovePartnerRequest,
    DenyPartnerRequest,
    GetPartnerRequests,
    GetPartnerRequestStatuses,
} from '../../services/partners';

interface AdminPartnerRequestsPropsType extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const AdminPartnerRequests: React.FC<AdminPartnerRequestsPropsType> = (props) => {
    const [partnerRequestList, setPartnerRequestList] = React.useState<PartnerRequestData[]>([]);
    const [isPartnerRequestDataLoaded, setIsPartnerRequestDataLoaded] = React.useState<boolean>(false);
    const [partnerRequestStatusList, setPartnerRequestStatusList] = React.useState<PartnerRequestStatusData[]>([]);

    const getPartnerRequestStatuses = useQuery({
        queryKey: GetPartnerRequestStatuses().key,
        queryFn: GetPartnerRequestStatuses().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerRequests = useQuery({
        queryKey: GetPartnerRequests().key,
        queryFn: GetPartnerRequests().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const approvePartnerRequest = useMutation({
        mutationKey: ApprovePartnerRequest().key,
        mutationFn: ApprovePartnerRequest().service,
    });

    const denyPartnerRequest = useMutation({
        mutationKey: DenyPartnerRequest().key,
        mutationFn: DenyPartnerRequest().service,
    });

    React.useEffect(() => {
        if (props.isUserLoaded) {
            getPartnerRequestStatuses.refetch().then((partnerRequestStatusesRes) => {
                setPartnerRequestStatusList(partnerRequestStatusesRes.data?.data || []);
                getPartnerRequests.refetch().then((partnerRequestsRes) => {
                    setPartnerRequestList(partnerRequestsRes.data?.data || []);
                    setIsPartnerRequestDataLoaded(true);
                });
            });
        }
    }, [props.isUserLoaded]);

    // Handle approve request for a partner
    function handleApprove(id: string, name: string) {
        if (!window.confirm(`Do you want to approve partner with name: ${name}`)) return;

        approvePartnerRequest.mutateAsync({ id }).then(() => {
            getPartnerRequests.refetch().then((res) => {
                setPartnerRequestList(res.data?.data || []);
                setIsPartnerRequestDataLoaded(true);
            });
        });
    }

    // Handle approve request for a partner
    function handleDeny(id: string, name: string) {
        if (!window.confirm(`Do you want to deny partner with name: ${name}`)) return;

        denyPartnerRequest.mutateAsync({ id }).then(() => {
            getPartnerRequests.refetch().then((res) => {
                setPartnerRequestList(res.data?.data || []);
                setIsPartnerRequestDataLoaded(true);
            });
        });
    }

    const partnerRequestsActionDropdownList = (
        partnerRequestId: string,
        partnerRequestName: string,
        isBecomeAPartnerRequest: boolean,
        partnerRequestStatusId: number,
    ) => (
        <>
            <Dropdown.Item
                disabled={!isBecomeAPartnerRequest || partnerRequestStatusId !== Constants.PartnerRequestStatusPending}
                onClick={() => handleApprove(partnerRequestId, partnerRequestName)}
            >
                <CheckSquare />
                Approve Partner
            </Dropdown.Item>
            <Dropdown.Item
                disabled={!isBecomeAPartnerRequest || partnerRequestStatusId !== Constants.PartnerRequestStatusPending}
                onClick={() => handleDeny(partnerRequestId, partnerRequestName)}
            >
                <XSquare />
                Deny Partner
            </Dropdown.Item>
        </>
    );

    function renderPartnerRequestsTable(partnerRequests: PartnerRequestData[]) {
        return (
            <div>
                <h2 className='color-primary mt-4 mb-5'>Partner Requests</h2>
                <table className='table table-striped' aria-labelledby='tableLabel'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Website</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                            <th>Request Status</th>
                            <th>Is Become Partner Request</th>
                            <th>Notes</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partnerRequests.map((partnerRequest) => (
                            <tr key={partnerRequest.id.toString()}>
                                <td>{partnerRequest.name}</td>
                                <td>{partnerRequest.email}</td>
                                <td>
                                    <PhoneInput value={partnerRequest.phone} disabled />
                                </td>
                                <td>{partnerRequest.website}</td>
                                <td>{partnerRequest.city}</td>
                                <td>{partnerRequest.region}</td>
                                <td>{partnerRequest.country}</td>
                                <td>
                                    {getPartnerRequestStatus(
                                        partnerRequestStatusList,
                                        partnerRequest.partnerRequestStatusId,
                                    )}
                                </td>
                                <td>{partnerRequest.isBecomeAPartnerRequest}</td>
                                <td>{partnerRequest.notes}</td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {partnerRequestsActionDropdownList(
                                                partnerRequest.id,
                                                partnerRequest.name,
                                                partnerRequest.isBecomeAPartnerRequest,
                                                partnerRequest.partnerRequestStatusId,
                                            )}
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

    const contents = isPartnerRequestDataLoaded ? (
        renderPartnerRequestsTable(partnerRequestList)
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
