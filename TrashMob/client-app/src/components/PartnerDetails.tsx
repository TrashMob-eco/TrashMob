import * as React from 'react'
import { RouteComponentProps } from 'react-router';
import EventData from './Models/EventData';
import UserData from './Models/UserData';
import { getDefaultHeaders } from '../store/AuthStore';
import { getEventType } from '../store/eventTypeHelper';
import { data } from 'azure-maps-control';
import * as MapStore from '../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapController from './MapController';
import { Col, Form } from 'react-bootstrap';
import PartnerData from './Models/PartnerData';
import PartnerLocationData from './Models/PartnerLocationData';

export interface PartnerDetailsMatchParams {
    partnerId: string;
}

export interface PartnerDetailsProps extends RouteComponentProps<PartnerDetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerDetails: React.FC<PartnerDetailsProps> = (props) => {
    const [isDataLoaded, setIsDataLoaded] = React.useState<boolean>(false);
    const [partnerId, setPartnerId] = React.useState<string>(props.match.params["partnerId"]);
    const [partnerName, setPartnerName] = React.useState<string>("New Partner");
    const [notes, setNotes] = React.useState<string>();
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [partnerLocationList, setPartnerLocationList] = React.useState<PartnerLocationData[]>([]);
    const [currentUser, setCurrentUser] = React.useState<UserData>(props.currentUser);
    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(props.isUserLoaded);

    React.useEffect(() => {

        const headers = getDefaultHeaders('GET');

        if (partnerId != null) {

            fetch('/api/Partners/' + partnerId, {
                method: 'GET',
                headers: headers
            })
                .then(response => response.json() as Promise<PartnerData>)
                .then(partnerData => {
                    setPartnerId(partnerData.id);
                    setPartnerName(partnerData.name);
                    setNotes(partnerData.notes);
                    setIsDataLoaded(true);
                })
                .then(() => {
                    fetch('/api/Partners/' + partnerId, {
                        method: 'GET',
                        headers: headers
                    })
                        .then(response => response.json() as Promise<PartnerLocationData[]>)
                        .then(partnerLocationData => {
                            setPartnerLocationList(partnerLocationData);
                        })
                })
        }

        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })
    }, [partnerId]);

    React.useEffect(() => {
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.isUserLoaded])

    function handleLocationChange(point: data.Position) {
        // do nothing
    }

    function renderPartnerLocations(partners: PartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>City</th>
                            <th>Country</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partners.map(partner =>
                            <tr key={partner.id.toString()}>
                                <td>{partner.city}</td>
                                <td>{partner.country}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderPartner() {

        return (
            <div>
                <Form>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Name">Name:</Form.Label>
                                <Form.Control disabled type="text" defaultValue={partnerName} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <Form.Label htmlFor="Notes">Notes:</Form.Label>
                                <Form.Control disabled as="textarea" defaultValue={notes} rows={5} cols={5} />
                            </Form.Group>
                        </Col>
                    </Form.Row>
                </Form >
                <div>
                    <h2>Locations</h2>
                    {renderPartnerLocations(partnerLocationList)}
                </div>
                <div>
                    <AzureMapsProvider>
                        <>
                            <MapController center={center} multipleEvents={[]} isEventDataLoaded={isDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName={partnerName} latitude={0} longitude={0} onLocationChange={handleLocationChange} currentUser={currentUser} isUserLoaded={isUserLoaded} />
                        </>
                    </AzureMapsProvider>
                </div>
            </div>
        )
    }

    let contents = isDataLoaded
        ? renderPartner()
        : <p><em>Loading...</em></p>;

    return <div>
        <h3>Partner Details</h3>
        <hr />
        {contents}
    </div>;
}
