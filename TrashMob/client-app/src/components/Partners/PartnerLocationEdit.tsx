import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, ToggleButton, Tooltip } from 'react-bootstrap';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import PartnerLocationData from '../Models/PartnerLocationData';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as MapStore from '../../store/MapStore';
import { data } from 'azure-maps-control';
import { Guid } from 'guid-typescript';
import AddressData from '../Models/AddressData';
import MapControllerSinglePointNoEvent from '../MapControllerSinglePointNoEvent';

export interface PartnerLocationEditDataProps {
    partnerId: string;
    partnerLocationId: string;
    onCancel: any;
    onSave: any;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocationEdit: React.FC<PartnerLocationEditDataProps> = (props) => {

    const [partnerLocationId, setPartnerLocationId] = React.useState<string>(Guid.EMPTY);
    const [locationName, setLocationName] = React.useState<string>("");
    const [locationNameErrors, setLocationNameErrors] = React.useState<string>("");
    const [publicNotes, setPublicNotes] = React.useState<string>("");
    const [publicNotesErrors, setPublicNotesErrors] = React.useState<string>("");
    const [privateNotes, setPrivateNotes] = React.useState<string>("");
    const [isPartnerLocationActive, setIsPartnerLocationActive] = React.useState<boolean>(true);
    const [streetAddress, setStreetAddress] = React.useState<string>("");
    const [city, setCity] = React.useState<string>("");
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>("");
    const [postalCode, setPostalCode] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {

        if (props.isUserLoaded && props.partnerLocationId && props.partnerLocationId !== Guid.EMPTY) {

            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/partnerlocations/' + props.partnerLocationId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<PartnerLocationData>)
                    .then(data => {
                        setPartnerLocationId(data.id);
                        setLocationName(data.name);
                        setStreetAddress(data.streetAddress);
                        setCity(data.city);
                        setCountry(data.country);
                        setRegion(data.region);
                        setPostalCode(data.postalCode);
                        setLatitude(data.latitude);
                        setLongitude(data.longitude);
                        setIsPartnerLocationActive(data.isActive);
                        setCreatedByUserId(data.createdByUserId);
                        setCreatedDate(new Date(data.createdDate));
                        setLastUpdatedDate(new Date(data.lastUpdatedDate));
                        setPublicNotes(data.publicNotes);
                        setIsPartnerLocationDataLoaded(true);
                    });
            });
        }
        else {
            if (props.isUserLoaded && props.partnerLocationId === Guid.EMPTY) {
                setIsPartnerLocationDataLoaded(true);
            }
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
    }, [props.currentUser, props.partnerLocationId, props.isUserLoaded, props.partnerId]);

    function handleLocationNameChanged(locationName: string) {

        if (locationName === "") {
            setLocationNameErrors("Location Name cannot be empty.")
        }
        else {
            setLocationName(locationName);
            setLocationNameErrors("");
        }

        validateForm();
    }

    function handlePublicNotesChanged(notes: string) {
        if (notes === "") {
            setPublicNotesErrors("Public notes cannot be empty.");
        }
        else {
            setPublicNotes(notes);
            setPublicNotesErrors("");
        }

        validateForm();
    }

    function handleIsPartnerLocationActiveChanged(val: boolean) {
        setIsPartnerLocationActive(val);
        validateForm();
    }

    function handlePrivateNotesChanged(notes: string) {
        setPrivateNotes(notes);
        validateForm();
    }

    function renderPartnerLocationNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationName}</Tooltip>
    }

    function renderStreetAddressToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationStreetAddress}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPostalCode}</Tooltip>
    }

    function renderIsPartnerLocationActiveToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationIsPartnerLocationActive}</Tooltip>
    }

    function renderPublicNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPublicNotes}</Tooltip>
    }

    function renderPrivateNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPrivateNotes}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLastUpdatedDate}</Tooltip>
    }

    function validateForm() {
        if (publicNotes === "" ||
            publicNotesErrors !== "" ||
            country === "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var partnerLocationData = new PartnerLocationData();
        partnerLocationData.id = partnerLocationId;
        partnerLocationData.partnerId = props.partnerId;
        partnerLocationData.name = locationName ?? "";
        partnerLocationData.streetAddress = streetAddress ?? "";
        partnerLocationData.city = city ?? "";
        partnerLocationData.region = region ?? "";
        partnerLocationData.country = country ?? "";
        partnerLocationData.postalCode = postalCode ?? "";
        partnerLocationData.latitude = latitude ?? 0;
        partnerLocationData.longitude = longitude ?? 0;
        partnerLocationData.isActive = isPartnerLocationActive;
        partnerLocationData.publicNotes = publicNotes ?? "";
        partnerLocationData.privateNotes = privateNotes ?? "";
        partnerLocationData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerLocationData.createdDate = createdDate;

        var data = JSON.stringify(partnerLocationData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            var method = "PUT";

            if (partnerLocationId === Guid.EMPTY) {
                method = "POST";
            }

            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocations', {
                method: method,
                body: data,
                headers: headers,
            })
                .then(response => response.json() as Promise<PartnerLocationData>)
                .then(() => {
                    props.onSave();
                });
        });
    }

    function handleLocationChange(point: data.Position) {
        // In an Azure Map point, the longitude is the first position, and latitude is second
        setLatitude(point[1]);
        setLongitude(point[0]);
        var locationString = point[1] + ',' + point[0]
        var headers = getDefaultHeaders('GET');

        MapStore.getKey()
            .then(key => {
                fetch('https://atlas.microsoft.com/search/address/reverse/json?subscription-key=' + key + '&api-version=1.0&query=' + locationString, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<AddressData>)
                    .then(data => {
                        setStreetAddress(data.addresses[0].address.streetNameAndNumber);
                        setCity(data.addresses[0].address.municipality);
                        setCountry(data.addresses[0].address.country);
                        setRegion(data.addresses[0].address.countrySubdivisionName);
                        setPostalCode(data.addresses[0].address.postalCode);
                        validateForm();
                    })
            })
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setLocationName("");
        setStreetAddress("");
        setCity("");
        setRegion("");
        setCountry("");
        setPostalCode("");
        setLatitude(0);
        setLongitude(0);
        setPrivateNotes("");
        setPublicNotes("");
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
        props.onCancel();
    }

    function renderEditLocation() {
        return (
            <div>
                <p>
                    This page allows you set up a new partner location, or edit an existing one. 
                </p>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type="hidden" name="Id" value={partnerLocationId} />
                    </Form.Row>
                    <Button disabled={!isSaveEnabled} type="submit" className="btn btn-default">Save</Button>
                    <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPartnerLocationNameToolTip}>
                                    <Form.Label className="control-label" htmlFor="LocationName">Location Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" name="locationName" defaultValue={locationName} onChange={val => handleLocationNameChanged(val.target.value)} maxLength={parseInt('64')} required />
                                <span style={{ color: "red" }}>{locationNameErrors}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderIsPartnerLocationActiveToolTip}>
                                    <Form.Label className="control-label" htmlFor="IsPartnerLocationActive">Is Partner Location Active:</Form.Label>
                                </OverlayTrigger >
                                <ToggleButton
                                    type="checkbox"
                                    variant="outline-dark"
                                    checked={isPartnerLocationActive}
                                    value="1"
                                    onChange={(e) => handleIsPartnerLocationActiveChanged(e.currentTarget.checked)}
                                >
                                    Is Active
                                </ToggleButton>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderStreetAddressToolTip}>
                                    <Form.Label className="control-label" htmlFor="StreetAddress">Street Address:</Form.Label>
                                </OverlayTrigger>
                                <span>{streetAddress}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                    <Form.Label className="control-label" htmlFor="City">City:</Form.Label>
                                </OverlayTrigger >
                                <span>{city}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                    <Form.Label className="control-label" htmlFor="PostalCode">Postal Code:</Form.Label>
                                </OverlayTrigger >
                                <span>{postalCode}</span>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                    <Form.Label className="control-label" htmlFor="Country">Country:</Form.Label>
                                </OverlayTrigger >
                                <span>{country}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                    <Form.Label className="control-label" htmlFor="Region">Region:</Form.Label>
                                </OverlayTrigger >
                                <span>{region}</span>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Group className="required">
                        <OverlayTrigger placement="top" overlay={renderPublicNotesToolTip}>
                            <Form.Label className="control-label">Public Notes:</Form.Label>
                        </OverlayTrigger>
                        <Form.Control as="textarea" defaultValue={publicNotes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handlePublicNotesChanged(val.target.value)} required />
                        <span style={{ color: "red" }}>{publicNotesErrors}</span>
                    </Form.Group >
                    <Form.Group>
                        <OverlayTrigger placement="top" overlay={renderPrivateNotesToolTip}>
                            <Form.Label className="control-label">Private Notes:</Form.Label>
                        </OverlayTrigger>
                        <Form.Control as="textarea" defaultValue={privateNotes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handlePrivateNotesChanged(val.target.value)} />
                    </Form.Group >
                    <Form.Row>
                        <Form.Label>Click on the map to set the location for your Partner. The location fields above will be automatically populated.</Form.Label>
                    </Form.Row>
                    <Form.Row>
                        <AzureMapsProvider>
                            <>
                                <MapControllerSinglePointNoEvent center={center} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={true} />
                            </>
                        </AzureMapsProvider>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderCreatedDateToolTip}>
                                    <Form.Label className="control-label" htmlFor="createdDate">Created Date:</Form.Label>
                                </OverlayTrigger>
                                <span>{createdDate ? createdDate.toLocaleString() : ""}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                    <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date:</Form.Label>
                                </OverlayTrigger>
                                <span>{lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""}</span>
                            </Form.Group>
                        </Col>
                    </Form.Row>

                </Form>
            </div>
        );
    }

    return (
        <>
            <div>
                {!isPartnerLocationDataLoaded && <p><em>Loading...</em></p>}
                {isPartnerLocationDataLoaded && renderEditLocation()}
            </div>
        </>
    );
}