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
import * as Constants from '../Models/Constants';
import AddressData from '../Models/AddressData';
import MapControllerSinglePointNoEvent from '../MapControllerSinglePointNoEvent';

export interface PartnerLocationsDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerLocations: React.FC<PartnerLocationsDataProps> = (props) => {

    const [partnerLocationId, setPartnerLocationId] = React.useState<string>(Guid.createEmpty().toString());
    const [locationName, setLocationName] = React.useState<string>("");
    const [locationNameErrors, setLocationNameErrors] = React.useState<string>("");
    const [notes, setNotes] = React.useState<string>();
    const [notesErrors, setNotesErrors] = React.useState<string>();
    const [isPartnerLocationActive, setIsPartnerLocationActive] = React.useState<boolean>(true);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>();
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [primaryEmail, setPrimaryEmail] = React.useState<string>("");
    const [secondaryEmail, setSecondaryEmail] = React.useState<string>("");
    const [primaryPhone, setPrimaryPhone] = React.useState<string>("");
    const [secondaryPhone, setSecondaryPhone] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [createdByUserId, setCreatedByUserId] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [primaryEmailErrors, setPrimaryEmailErrors] = React.useState<string>("");
    const [secondaryEmailErrors, setSecondaryEmailErrors] = React.useState<string>("");
    const [primaryPhoneErrors, setPrimaryPhoneErrors] = React.useState<string>("");
    const [secondaryPhoneErrors, setSecondaryPhoneErrors] = React.useState<string>("");
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [partnerLocations, setPartnerLocations] = React.useState<PartnerLocationData[]>([]);
    const [isPartnerLocationDataLoaded, setIsPartnerLocationDataLoaded] = React.useState<boolean>(false);

    React.useEffect(() => {

        if (props.isUserLoaded) {
            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);


                fetch('/api/partnerlocations/' + props.partnerId, {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerLocationData[]>)
                    .then(data => {
                        setPartnerLocations(data);
                        setIsPartnerLocationDataLoaded(true);
                    });
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
    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

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
            });
        }
    }

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

    function handleNotesChanged(notes: string) {
        if (notes === "") {
            setNotesErrors("Notes cannot be empty.");
        }
        else {
            setNotes(notes);
            setNotesErrors("");
        }

        validateForm();
    }

    function handlePrimaryEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setPrimaryEmailErrors("Please enter valid email address.");
        }
        else {
            setPrimaryEmailErrors("");
            setPrimaryEmail(val);
        }

        validateForm();
    }

    function handleSecondaryEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setSecondaryEmailErrors("Please enter valid email address.");
        }
        else {
            setSecondaryEmailErrors("");
            setSecondaryEmail(val);
        }

        validateForm();
    }

    function handlePrimaryPhoneChanged(val: string) {
        var pattern = new RegExp(Constants.RegexPhoneNumber);

        if (!pattern.test(val)) {
            setPrimaryPhoneErrors("Please enter a valid phone number.");
        }
        else {
            setPrimaryPhoneErrors("");
            setPrimaryPhone(val);
        }

        validateForm();
    }

    function handleSecondaryPhoneChanged(val: string) {
        var pattern = new RegExp(Constants.RegexPhoneNumber);

        if (!pattern.test(val)) {
            setSecondaryPhoneErrors("Please enter a valid phone number.");
        }
        else {
            setSecondaryPhoneErrors("");
            setSecondaryPhone(val);
        }

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

    function renderPrimaryEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPrimaryEmail}</Tooltip>
    }

    function renderSecondaryEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationSecondaryEmail}</Tooltip>
    }

    function renderPrimaryPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationPrimaryPhone}</Tooltip>
    }

    function renderSecondaryPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationSecondaryPhone}</Tooltip>
    }

    function renderIsPartnerLocationActiveToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationIsPartnerLocationActive}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLocationNotes}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLastUpdatedDate}</Tooltip>
    }


    function addLocation() {
        setIsEditOrAdd(true);
    }

    function editLocation(locationId: string) {
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocations/' + props.partnerId + '/' + locationId, {
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
                    setPrimaryEmail(data.primaryEmail);
                    setSecondaryEmail(data.secondaryEmail);
                    setPrimaryPhone(data.primaryPhone);
                    setSecondaryPhone(data.secondaryPhone);
                    setIsPartnerLocationActive(data.isActive);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(data.createdDate);
                    setLastUpdatedDate(data.lastUpdatedDate);
                    setNotes(data.notes);
                    setIsEditOrAdd(true);
                });
        });
    }

    function validateForm() {
        if (notes === "" ||
            notesErrors !== "" ||
            primaryEmail === "" ||
            primaryEmailErrors !== "" ||
            secondaryEmail === "" ||
            secondaryEmailErrors !== "" ||
            primaryPhone === "" ||
            primaryPhoneErrors !== "" ||
            secondaryPhone === "" ||
            secondaryPhoneErrors !== "") {
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
        partnerLocationData.latitude = latitude ?? "";
        partnerLocationData.longitude = longitude ?? "";
        partnerLocationData.primaryEmail = primaryEmail ?? "";
        partnerLocationData.secondaryEmail = secondaryEmail ?? "";
        partnerLocationData.primaryPhone = primaryPhone ?? "";
        partnerLocationData.secondaryPhone = secondaryPhone ?? "";
        partnerLocationData.isActive = isPartnerLocationActive;
        partnerLocationData.notes = notes ?? "";
        partnerLocationData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerLocationData.createdDate = createdDate;
        partnerLocationData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(partnerLocationData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            var method = "PUT";

            if (partnerLocationData.id === Guid.EMPTY) {
                method = "POST";
            }

            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/partnerlocations', {
                method: method,
                body: data,
                headers: headers,
            })
                .then(() => {
                    setIsEditOrAdd(false);
                });
        });
    }

    function renderPartnerLocationsTable(locations: PartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>City</th>
                            <th>Region</th>
                            <th>Country</th>
                        </tr>
                    </thead>
                    <tbody>
                        {locations.map(location =>
                            <tr key={location.id.toString()}>
                                <td>{location.name}</td>
                                <td>{location.city}</td>
                                <td>{location.region}</td>
                                <td>{location.country}</td>
                                <td>
                                    <Button className="action" onClick={() => editLocation(location.id)}>Edit Location</Button>
                                    <Button className="action" onClick={() => removeLocation(location.id, location.name)}>Remove Location</Button>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button className="action" onClick={() => addLocation()}>Add Location</Button>
            </div>
        );
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

    function renderEditLocation() {
        return (
            <div>
                <Form onSubmit={handleSave}>
                    <Form.Row>
                        <input type="hidden" name="Id" value={partnerLocationId.toString()} />
                    </Form.Row>
                    <Button disabled={!isSaveEnabled} className="action" onClick={(e) => handleSave(e)}>Save</Button>
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
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPrimaryEmailToolTip}>
                                    <Form.Label className="control-label">Primary Email:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={primaryEmail} maxLength={parseInt('64')} onChange={(val) => handlePrimaryEmailChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{primaryEmailErrors}</span>
                            </Form.Group >
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderSecondaryEmailToolTip}>
                                    <Form.Label className="control-label">Secondary Email:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={secondaryEmail} maxLength={parseInt('64')} onChange={(val) => handleSecondaryEmailChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{secondaryEmailErrors}</span>
                            </Form.Group >
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
                                    onChange={(e) => setIsPartnerLocationActive(e.currentTarget.checked)}
                                >
                                    Is Active
                                </ToggleButton>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPrimaryPhoneToolTip}>
                                    <Form.Label className="control-label">Primary Phone:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={primaryPhone} maxLength={parseInt('64')} onChange={(val) => handlePrimaryPhoneChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{primaryPhoneErrors}</span>
                            </Form.Group >
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderSecondaryPhoneToolTip}>
                                    <Form.Label className="control-label">Secondary Phone:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={secondaryPhone} maxLength={parseInt('64')} onChange={(val) => handleSecondaryPhoneChanged(val.target.value)} required />
                                <span style={{ color: "red" }}>{secondaryPhoneErrors}</span>
                            </Form.Group >
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
                        <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                            <Form.Label className="control-label">Notes:</Form.Label>
                        </OverlayTrigger>
                        <Form.Control as="textarea" defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} required />
                        <span style={{ color: "red" }}>{notesErrors}</span>
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
                                <span>{createdDate.toString()}</span>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                    <Form.Label className="control-label" htmlFor="lastUpdatedDate">Last Updated Date:</Form.Label>
                                </OverlayTrigger>
                                <span>{lastUpdatedDate.toString()}</span>
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
                {isPartnerLocationDataLoaded && partnerLocations && renderPartnerLocationsTable(partnerLocations)}
                {isEditOrAdd && renderEditLocation()}
            </div>
        </>
    );
}