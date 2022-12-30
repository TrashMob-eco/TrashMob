import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Dropdown, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import { Guid } from 'guid-typescript';
import ServiceTypeData from '../Models/ServiceTypeData';
import { getServiceType } from '../../store/serviceTypeHelper';
import PartnerLocationServiceData from '../Models/PartnerLocationServiceData';
import { Pencil, XSquare } from 'react-bootstrap-icons';

export interface PartnerLocationServicesDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocationServices: React.FC<PartnerLocationServicesDataProps> = (props) => {

    const [notes, setNotes] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [partnerLocationServices, setPartnerLocationServices] = React.useState<PartnerLocationServiceData[]>([]);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>(Guid.EMPTY);
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isPartnerLocationServicesDataLoaded, setIsPartnerLocationServicesDataLoaded] = React.useState<boolean>(false);
    const [serviceTypeId, setServiceTypeId] = React.useState<number>(0);
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);
    const [isEdit, setIsEdit] = React.useState<boolean>(false);
    const [isAdd, setIsAdd] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);

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
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerlocationservices/getbypartnerlocation/' + props.partnerLocationId, {
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
        setNotes("");
        setServiceTypeId(0);
        setNotes("");
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsAdd(true);
        setIsAddEnabled(false);
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setNotes("");
        setServiceTypeId(0);
        setNotes("");
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        setIsAdd(false);
        setIsEdit(false);
        setIsAddEnabled(true);
    }

    function editService(serviceTypeId: number) {
        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocationservices/' + props.partnerLocationId + '/' + serviceTypeId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerLocationServiceData>)
                .then(data => {
                    setServiceTypeId(data.serviceTypeId);
                    setNotes(data.notes);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(new Date(data.createdDate));
                    setLastUpdatedDate(new Date(data.lastUpdatedDate));
                    setIsAddEnabled(false);
                    setIsEdit(true);
                });
        });
    }

    function removeService(serviceTypeId: number) {
        if (!window.confirm("Please confirm that you want to remove service type: '" + getServiceType(serviceTypeList, serviceTypeId) + "' from this Partner?"))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('DELETE');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerlocationservices/' + props.partnerLocationId + '/' + serviceTypeId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        setIsPartnerLocationServicesDataLoaded(false);

                        fetch('/api/partnerlocationservices/getbypartnerlocation/' + props.partnerLocationId, {
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

    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var method = "PUT";

        if (createdByUserId === Guid.EMPTY) {
            method = "POST";

            // We need to prevent an additional add of an existing service type
            if (partnerLocationServices.find(obj => obj.serviceTypeId === serviceTypeId)) {
                window.alert("Adding more than one instance of an existing service type is not allowed.")
                return;
            }
        }

        var partnerService = new PartnerLocationServiceData();
        partnerService.partnerLocationId = props.partnerLocationId;
        partnerService.serviceTypeId = serviceTypeId ?? 0;
        partnerService.notes = notes;
        partnerService.createdByUserId = createdByUserId;

        var data = JSON.stringify(partnerService);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocationservices', {
                method: method,
                headers: headers,
                body: data,
            })
                .then(() => {
                    setIsAdd(false);
                    setIsEdit(false);
                    setIsPartnerLocationServicesDataLoaded(false);

                    fetch('/api/partnerlocationservices/getbypartnerlocation/' + props.partnerLocationId, {
                        method: 'GET',
                        headers: headers,
                    })
                        .then(response => response.json() as Promise<PartnerLocationServiceData[]>)
                        .then(data => {
                            setPartnerLocationServices(data);
                            setIsPartnerLocationServicesDataLoaded(true);
                            setIsEdit(false);
                            setIsAdd(false);
                            setIsAddEnabled(true);
                        });
                });
        });
    }

    function validateForm() {
        if (notes === "" ||
            notesErrors !== "" ||
            serviceTypeId === 0) {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function handleNotesChanged(val: string) {
        if (val === "") {
            setNotesErrors("Notes cannot be blank.");
        }
        else {
            setNotesErrors("");
            setNotes(val);
        }

        validateForm();
    }

    function selectServiceType(val: string) {
        setServiceTypeId(parseInt(val));
        validateForm();
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerServiceNotes}</Tooltip>
    }

    function renderServiceTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerServiceType}</Tooltip>
    }

    const locationServiceActionDropdownList = (serviceTypeId: number) => {
        return (
            <>
                <Dropdown.Item onClick={() => editService(serviceTypeId)}><Pencil />Edit Service</Dropdown.Item>
                <Dropdown.Item onClick={() => removeService(serviceTypeId)}><XSquare />Remove Service</Dropdown.Item>
            </>
        )
    }

    function renderPartnerLocationServicesTable(services: PartnerLocationServiceData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Partner Location Services</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Service Type</th>
                            <th>Notes</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {services.map(service =>
                            <tr key={service.serviceTypeId}>
                                <td>{getServiceType(serviceTypeList, service.serviceTypeId)}</td>
                                <td>{service.notes}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {locationServiceActionDropdownList(service.serviceTypeId)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addService()}>Add Service</Button>
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
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="ServiceType">Service Type</Form.Label>
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
                                    <Form.Label className="control-label font-weight-bold h5" htmlFor="serviceType">Notes</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="notes" defaultValue={notes} onChange={val => handleNotesChanged(val.target.value)} maxLength={parseInt('64')} required />
                                <span style={{ color: "red" }}>{notesErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="createdDate">Created Date</Form.Label>
                                <Form.Control type="text" disabled defaultValue={createdDate ? createdDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="lastUpdatedDate">Last Updated Date</Form.Label>
                                <Form.Control type="text" disabled defaultValue={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                            </Form.Group>
                        </Col>
                        <Button disabled={!isSaveEnabled} type="submit" className="btn btn-default">Save</Button>
                        <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <div className="bg-white p-5 shadow-sm rounded">
            {props.partnerLocationId === Guid.EMPTY && <p> <em>Partner location must be created first.</em></p>}
            {!isPartnerLocationServicesDataLoaded && props.partnerLocationId !== Guid.EMPTY && <p><em>Loading...</em></p>}
            {isPartnerLocationServicesDataLoaded && partnerLocationServices && renderPartnerLocationServicesTable(partnerLocationServices)}
            {(isEdit || isAdd) && renderAddPartnerService()}
        </div>
    );
}
