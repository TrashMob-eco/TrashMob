import * as React from 'react'
import { RouteComponentProps } from 'react-router-dom';
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import PartnerLocationData from '../Models/PartnerLocationData';

export interface PartnerLocationsDataProps extends RouteComponentProps {
    partnerId: string;
    partnerLocations: PartnerLocationData[];
    isPartnerLocationDataLoaded: boolean;
    onPartnerLocationsUpdated: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocations: React.FC<PartnerLocationsDataProps> = (props) => {

    const [locationName, setLocationName] = React.useState<string>("");

    function removeLocation(locationId: string, name: string) {
        if (!window.confirm("Please confirm that you want to remove Location with name: '" + name + "' as a location from this Partner?"))
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

                fetch('/api/partnerlocations/' + props.partnerId + '/' + locationId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        props.onPartnerLocationsUpdated()
                    });
            });
        }
    }

    function handleLocationNameChanged(locationName: string) {
        setLocationName(locationName);
    }

    function renderPartnerLocationNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationName}</Tooltip>
    }

    function handleAddLocation(event: any) {

    }        

    function renderPartnerLocationsTable(locations: PartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>City</th>
                        </tr>
                    </thead>
                    <tbody>
                        {locations.map(location => 
                                    <tr key={location.id.toString()}>
                                        <td>{location.name}</td>
                                        <td>{location.city}</td>
                                        <td>
                                            <Button className="action" onClick={() => removeLocation(location.id, location.name)}>Remove Location</Button>
                                        </td>
                                    </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderAddLocation() {
        return (
            <div>
                <Form onSubmit={handleAddLocation}>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderPartnerLocationNameToolTip}>
                                    <Form.Label htmlFor="LocationName">Location Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="locationName" defaultValue={locationName} onChange={val => handleLocationNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                            </Form.Group>
                        </Col>
                        <Button className="action" onClick={(e) => handleAddLocation(e)}>Search</Button>
                    </Form.Row>
                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {!props.isPartnerLocationDataLoaded && <p><em>Loading...</em></p>}
                {props.isPartnerLocationDataLoaded && props.location && renderPartnerLocationsTable(props.partnerLocations)}
                {renderAddLocation()};
            </div>
        </>
    );
}