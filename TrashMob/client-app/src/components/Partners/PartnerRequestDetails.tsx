import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { getApiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, Col, Container, Form, Row } from 'react-bootstrap';
import PartnerRequestData from '../Models/PartnerRequestData';
import UserData from '../Models/UserData';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerSinglePointNoEvents from '../MapControllerSinglePointNoEvent';
import PartnerRequestStatusData from '../Models/PartnerRequestStatusData';
import PartnerTypeData from '../Models/PartnerTypeData';
import { getPartnerRequestStatus } from '../../store/partnerRequestStatusHelper';
import { getPartnerType } from '../../store/partnerTypeHelper';
import { Guid } from 'guid-typescript';
import PhoneInput from 'react-phone-input-2'

export interface PartnerRequestDetailsMatchParams {
    partnerRequestId: string;
}

interface PartnerRequestDetailsParams extends RouteComponentProps<PartnerRequestDetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerRequestDetails: React.FC<PartnerRequestDetailsParams> = (props) => {
    const [partnerRequestId, setPartnerRequestId] = React.useState<string>("");
    const [name, setName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [website, setWebsite] = React.useState<string>();
    const [phone, setPhone] = React.useState<string>();
    const [notes, setNotes] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [partnerRequestStatusId, setPartnerRequestStatusId] = React.useState<number>(0);
    const [partnerTypeId, setPartnerTypeId] = React.useState<number>(0);

    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [partnerRequestStatusList, setPartnerRequestStatusList] = React.useState<PartnerRequestStatusData[]>([]);
    const [partnerTypeList, setPartnerTypeList] = React.useState<PartnerTypeData[]>([]);
    const [isPartnerRequestDataLoaded, setIsPartnerRequestDataLoaded] = React.useState<boolean>(false);
    const [isPartnerRequestIdReady, setIsPartnerRequestIdReady] = React.useState<boolean>();
    const [loadedPartnerRequestId, setLoadedPartnerRequestId] = React.useState<string | undefined>(props.match?.params["partnerRequestId"]);

    React.useEffect(() => {
        var partId = loadedPartnerRequestId;
        if (!partId) {
            setPartnerRequestId(Guid.createEmpty().toString());
            setLoadedPartnerRequestId(Guid.createEmpty().toString())
        }
        else {
            setPartnerRequestId(partId);
        }

        setIsPartnerRequestIdReady(true);

    }, [loadedPartnerRequestId]);

    React.useEffect(() => {

        if (props.isUserLoaded) {

            setIsPartnerRequestDataLoaded(false);
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            const request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerRequestStatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerRequestStatusData[]>)
                    .then(data => {
                        setPartnerRequestStatusList(data);
                    })
                    .then(() => {
                        fetch('/api/partnertypes', {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<PartnerTypeData[]>)
                            .then(data => {
                                setPartnerTypeList(data)
                            })
                            .then(() => {
                                fetch('/api/partnerrequests/' + partnerRequestId, {
                                    method: 'GET',
                                    headers: headers
                                })
                                    .then(response => response.json() as Promise<PartnerRequestData>)
                                    .then(data => {
                                        setName(data.name);
                                        setEmail(data.email);
                                        setWebsite(data.website);
                                        setPhone(data.phone);
                                        setStreetAddress(data.streetAddress);
                                        setCity(data.city);
                                        setRegion(data.region);
                                        setCountry(data.country);
                                        setPostalCode(data.postalCode);
                                        setLatitude(data.latitude ?? 0);
                                        setLongitude(data.longitude ?? 0);
                                        setPartnerRequestStatusId(data.partnerRequestStatusId);
                                        setPartnerTypeId(data.partnerTypeId);
                                        setNotes(data.notes);
                                        setCreatedDate(data.createdDate ? new Date(data.createdDate) : new Date());
                                        setLastUpdatedDate(data.lastUpdatedDate ? new Date(data.lastUpdatedDate) : new Date());
                                        setIsPartnerRequestDataLoaded(true);
                                    });
                            })
                    })
            });
        }
        MapStore.getOption().then(opts => {
            setMapOptions(opts);
            setIsMapKeyLoaded(true);
        })

        if ("geolocation" in navigator) {
            navigator.geolocation.getCurrentPosition(position => {
                var point = new data.Position(position.coords.longitude, position.coords.latitude);
                setCenter(point)
            });
        } else {
            console.log("Not Available");
        }
    }, [props.currentUser, props.isUserLoaded, partnerRequestId]);

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/mydashboard");
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestName}</Tooltip>
    }

    function renderPartnerRequestStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestStatus}</Tooltip>
    }

    function renderPartnerTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerType}</Tooltip>
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestEmail}</Tooltip>
    }

    function renderWebsiteToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestWebsite}</Tooltip>
    }

    function renderPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPhone}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestNotes}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestRegion}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestLastUpdatedDate}</Tooltip>
    }

    function handleLocationChange(point: data.Position) {
        // Do nothing. This is a read-only form
    }

    // Returns the HTML Form to the render() method.  
    function renderDetailsForm() {
        return (
            <Container>
                <Row className="gx-2 py-5" lg={2}>
                    <Col lg={4} className="d-flex">
                        <div className="bg-white py-2 px-5 shadow-sm rounded">
                            <h2 className="color-primary mt-4 mb-5">Partner request sent</h2>
                            <p>
                                This partner request has been sent!
                            </p>
                        </div>
                    </Col>
                    <Col lg={8}>
                        <div className="bg-white p-5 shadow-sm rounded">
                            <h2 className="color-primary mt-4 mb-5">Partner Request</h2>
                            <Form>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Partner Name</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="name" value={name} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderPartnerTypeToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Partner Type</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="partnerType" value={getPartnerType(partnerTypeList, partnerTypeId)} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderPartnerRequestStatusToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Request Status</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="requestStatus" value={getPartnerRequestStatus(partnerRequestStatusList, partnerRequestStatusId)} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Email</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="email" value={email} />
                                        </Form.Group >
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderPhoneToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Phone</Form.Label>
                                            </OverlayTrigger>
                                            <PhoneInput
                                                value={phone}
                                                disabled
                                            />
                                        </Form.Group >
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderWebsiteToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5">Website</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="website" value={website} />
                                        </Form.Group >
                                    </Col>
                                </Form.Row>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                                        <Form.Label className="control-label font-weight-bold h5">Notes</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="notes" value={notes} />
                                </Form.Group >
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="streetAddress" value={streetAddress} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="city" value={city} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="postalCode" value={postalCode} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="Region">Region</Form.Label>
                                            </OverlayTrigger >
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="region" value={region} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="Country">Country</Form.Label>
                                            </OverlayTrigger >
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="country" value={country ?? ""} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <AzureMapsProvider>
                                        <>
                                            <MapControllerSinglePointNoEvents center={center} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={false} />
                                        </>
                                    </AzureMapsProvider>
                                </Form.Row>
                                <Form.Group className="form-group">
                                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                </Form.Group >
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="createdDate">Created Date</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="createdDate" value={createdDate.toString()} />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                                <Form.Label className="control-label font-weight-bold h5" htmlFor="lastUpdatedDate">Last Updated Date</Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="lastUpdatedDate" value={lastUpdatedDate.toString()} />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                            </Form >
                        </div>
                    </Col>
                </Row>
            </Container>
        )
    }
    var contents = isPartnerRequestDataLoaded && isPartnerRequestIdReady && partnerRequestId
        ? renderDetailsForm()
        : <p><em>Loading...</em></p>;

    return <div>
        <hr />
        {contents}
    </div>;

}

export default withRouter(PartnerRequestDetails);