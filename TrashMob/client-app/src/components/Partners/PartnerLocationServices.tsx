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
    const [partnerLocationServices, setPartnerLocationServices] = React.useState<PartnerLocationServiceData[]>([]);
    const [isPartnerLocationServicesDataLoaded, setIsPartnerServicesDataLoaded] = React.useState<boolean>(false);
    const [serviceTypeId, setServiceTypeId] = React.useState<number>(0);
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);

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

                fetch('/api/partnerlocationservices/' + props.partnerLocationId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerLocationServiceData[]>)
                    .then(data => {
                        setPartnerLocationServices(data);
                        setIsPartnerServicesDataLoaded(true);
                    });
            });
        }
    }, [props.partnerLocationId, props.isUserLoaded])

    function removeService(serviceTypeId: number) {
        if (!window.confirm("Please confirm that you want to remove service type: '" + getServiceType(serviceTypeList, serviceTypeId) + "' from this Partner Location?"))
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

                fetch('/api/partnerlocationservices/' + props.partnerLocationId + '/' + serviceTypeId, {
                    method: 'DELETE',
                    headers: headers,
                })
            });
        }
    }

    function handleAddServiceType() {

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        var partnerLocationService = new PartnerLocationServiceData();
        partnerLocationService.partnerLocationId = props.partnerLocationId;
        partnerLocationService.serviceTypeId = serviceTypeId ?? 0;
        partnerLocationService.notes = notes;
        partnerLocationService.createdByUserId = props.currentUser.id
        partnerLocationService.lastUpdatedByUserId = props.currentUser.id

        var data = JSON.stringify(partnerLocationService);

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocationservices', {
                method: 'POST',
                headers: headers,
                body: data,
            })
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
                        </tr>
                    </thead>
                    <tbody>
                        {services.map(service =>
                            <tr key={service.serviceTypeId}>
                                <td>{getServiceType(serviceTypeList, service.serviceTypeId)}</td>
                                <td>{service.notes}</td>
                                <td>
                                    <Button className="action" onClick={() => removeService(service.serviceTypeId)}>Remove Service</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderAddPartnerLocationService() {
        return (
            <div>
                <Form onSubmit={handleAddServiceType}>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderServiceTypeToolTip}>
                                    <Form.Label className="control-label" htmlFor="ServiceType">Service Type:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select data-val="true" name="serviceTypeId" defaultValue={serviceTypeId} onChange={(val) => selectServiceType(val.target.value)} required>
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
                        <Button className="action" onClick={() => handleAddServiceType()}>Add Service</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {props.partnerLocationId === Guid.EMPTY && <p> <em>Partner Location must be created first.</em></p>}
                {!isPartnerLocationServicesDataLoaded && props.partnerLocationId !== Guid.EMPTY && <p><em>Loading...</em></p>}
                {isPartnerLocationServicesDataLoaded && renderPartnerLocationServicesTable(partnerLocationServices)}
                {renderAddPartnerLocationService()}
            </div>
        </>
    );
}
