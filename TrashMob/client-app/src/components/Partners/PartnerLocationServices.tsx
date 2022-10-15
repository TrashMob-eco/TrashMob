import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import ServiceTypeData from '../Models/ServiceTypeData';
import { getServiceType } from '../../store/serviceTypeHelper';
import PartnerLocationServiceData from '../Models/PartnerLocationServiceData';

export interface PartnerLocationServicesDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocationServices: React.FC<PartnerLocationServicesDataProps> = (props) => {

    const [notes, setNotes] = React.useState<string>("");
    const [partnerServices, setPartnerLocationServices] = React.useState<PartnerLocationServiceData[]>([]);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isPartnerLocationServicesDataLoaded, setIsPartnerLocationServicesDataLoaded] = React.useState<boolean>(false);
    const [serviceTypeId, setServiceTypeId] = React.useState<number>(0);
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);
    const [isEdit, setIsEdit] = React.useState<boolean>(false);
    const [isAdd, setIsAdd] = React.useState<boolean>(false);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        fetch('/api/servicetypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setServiceTypeList(data);
            });

        if (props.isUserLoaded && props.partnerLocationId && props.partnerLocationId !== Guid.EMPTY) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerservices/' + props.partnerLocationId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerLocationServiceData[]>)
                    .then(data => {
                        setPartnerLocationServices(data);
                        setIsPartnerLocationServicesDataLoaded(true);
                    });
            });
        }
    }, [props.partnerLocationId, props.isUserLoaded])

    function addService() {
        setIsAdd(true);
    }

    function editService(serviceTypeId: number) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerservices/' + props.partnerLocationId + '/' + serviceTypeId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerLocationServiceData>)
                .then(data => {
                    setServiceTypeId(data.serviceTypeId);
                    setNotes(data.notes);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(data.createdDate);
                    setLastUpdatedDate(data.lastUpdatedDate);
                    setIsEdit(true);
                });
        });
    }

    function removeService(serviceTypeId: number) {
        if (!window.confirm("Please confirm that you want to remove service type: '" + getServiceType(serviceTypeList, serviceTypeId) + "' from this Partner?"))
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

                fetch('/api/partnerservices/' + props.partnerLocationId + '/' + serviceTypeId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerLocationServicesDataLoaded(false);

                        fetch('/api/partnerservices/' + props.partnerLocationId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PartnerLocationServiceData[]>)
                            .then(data => {
                                setPartnerLocationServices(data);
                                setIsPartnerLocationServicesDataLoaded(true);
                            });
                    })
            });
        }
    }

    function handleSave() {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var method = "PUT";

        if (createdByUserId === Guid.EMPTY) {
            method = "POST";

            // We need to prevent an additional add of an existing service type
            if (partnerServices.find(obj => obj.serviceTypeId === serviceTypeId)) {
                window.alert("Adding more than one instance of an existing service type is not allowed.")
                return;
            }
        }

        var partnerService = new PartnerLocationServiceData();
        partnerService.partnerLocationId = props.partnerLocationId;
        partnerService.serviceTypeId = serviceTypeId ?? 0;
        partnerService.notes = notes;
        partnerService.createdByUserId = createdByUserId;
        partnerService.lastUpdatedByUserId = props.currentUser.id

        var data = JSON.stringify(partnerService);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerservices', {
                method: method,
                headers: headers,
                body: data,
            })
                .then(() => {
                    setIsAdd(false);
                    setIsEdit(false);
                    setIsPartnerLocationServicesDataLoaded(false);

                    fetch('/api/partnerservices/' + props.partnerLocationId, {
                        method: 'GET',
                        headers: headers,
                    })
                        .then(response => response.json() as Promise<PartnerLocationServiceData[]>)
                        .then(data => {
                            setPartnerLocationServices(data);
                            setIsPartnerLocationServicesDataLoaded(true);
                        });
                });
        });
    }

    function handleNotesChanged(notes: string) {
        setNotes(notes);
    }

    function selectServiceType(val: string) {
        setServiceTypeId(parseInt(val));
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerServiceNotes}</Tooltip>
    }

    function renderServiceTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerServiceType}</Tooltip>
    }

    function renderPartnerLocationServicesTable(services: PartnerLocationServiceData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Service Type</th>
                            <th>Notes</th>
                            <th>Created Date</th>
                            <th>Last Updated Date</th>
                        </tr>
                    </thead>
                    <tbody>
                        {services.map(service =>
                            <tr key={service.serviceTypeId}>
                                <td>{getServiceType(serviceTypeList, service.serviceTypeId)}</td>
                                <td>{service.notes}</td>
                                <td>{createdDate ? createdDate.toLocaleString() : ""}</td>
                                <td>{lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""}</td>
                                <td>
                                    <Button className="action" onClick={() => editService(service.serviceTypeId)}>Edit Service</Button>
                                    <Button className="action" onClick={() => removeService(service.serviceTypeId)}>Remove Service</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button className="action" onClick={() => addService()}>Add Service</Button>
            </div>
        );
    }

    function renderAddPartnerService() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderServiceTypeToolTip}>
                                    <Form.Label className="control-label" htmlFor="ServiceType">Service Type:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select disabled={isEdit} data-val="true" name="serviceTypeId" defaultValue={serviceTypeId} onChange={(val) => selectServiceType(val.target.value)} required>
                                        <option value="">-- Select Service Type --</option>
                                        {serviceTypeList.map(type =>
                                            <option key={type.id} value={type.id}>{type.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                                    <Form.Label className="control-label" htmlFor="serviceType">Notes</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="notes" defaultValue={notes} onChange={val => handleNotesChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label className="control-label" htmlFor="createdDate">Created Date</Form.Label>
                                <Form.Label defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date</Form.Label>
                                <Form.Label defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={() => handleSave()}>Save</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {props.partnerLocationId === Guid.EMPTY && <p> <em>Partner must be created first.</em></p>}
                {!isPartnerLocationServicesDataLoaded && props.partnerLocationId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isPartnerLocationServicesDataLoaded && renderPartnerLocationServicesTable(partnerServices)}
                {(isEdit || isAdd) && renderAddPartnerService()}
            </div>
        </>
    );
}
