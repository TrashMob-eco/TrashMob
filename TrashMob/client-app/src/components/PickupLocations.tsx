import * as React from 'react'
import UserData from './Models/UserData';
import { Button, Col, Dropdown, Form, OverlayTrigger, ToggleButton, Tooltip } from 'react-bootstrap';
import { getApiConfig, getDefaultHeaders, msalClient } from './../store/AuthStore';
import * as ToolTips from ".././store/ToolTips";
import PartnerLocationData from './Models/PartnerLocationData';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as MapStore from '../store/MapStore';
import { data } from 'azure-maps-control';
import { Guid } from 'guid-typescript';
import AddressData from './Models/AddressData';
import MapControllerSinglePointNoEvent from './MapControllerSinglePointNoEvent';
import PickupLocationData from './Models/PickupLocationData';
import { Pencil, XSquare } from 'react-bootstrap-icons';
import PhoneInput from 'react-phone-input-2'
import { ManageEventPartners } from './EventManagement/ManageEventPartners';

export interface PickupLocationsDataProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PickupLocations: React.FC<PickupLocationsDataProps> = (props) => {

    const [pickupLocationId, setPickupLocationId] = React.useState<string>(Guid.EMPTY)
    const [haulingPartnerLocation, setHaulingPartnerLocation] = React.useState<PartnerLocationData>();
    const [notes, setNotes] = React.useState<string>("");
    const [hasBeenPickedUp, setHasBeenPickedUp] = React.useState<boolean>(true);
    const [hasBeenSubmitted, setHasBeenSubmitted] = React.useState<boolean>(false);
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
    const [pickupLocationsData, setPickupLocationsData] = React.useState<PickupLocationData[]>([]);
    const [isPickupLocationsDataLoaded, setIsPickupLocationsDataLoaded] = React.useState<boolean>(false);
    const [isPartnerLocationsDataLoaded, setIsPartnerLocationsDataLoaded] = React.useState<boolean>(false);
    const [isAddEnabled, setIsAddEnabled] = React.useState<boolean>(true);
    const [isSubmitEnabled, setIsSubmitEnabled] = React.useState<boolean>(false);
    const [isEditOrAdd, setIsEditOrAdd] = React.useState<boolean>(false);
    const [statusMessage, setStatusMessage] = React.useState<string>("Loading...");


    React.useEffect(() => {

        if (props.isUserLoaded && props.eventId && props.eventId !== Guid.EMPTY) {

            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/eventpartnerlocationservices/gethaulingpartnerlocation/' + props.eventId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => {
                        if (response.status === 200) {
                            return response.json() as Promise<PartnerLocationData>;
                        }
                        else {
                            throw Error("You must add a hauling partner and have it accepted before you can add pickup locations.");
                        }
                    })
                    .then(data => {
                        setHaulingPartnerLocation(data);
                        setIsPartnerLocationsDataLoaded(true);
                        fetch('/api/pickuplocations/getbyevent/' + props.eventId, {
                            method: 'GET',
                            headers: headers,
                        })
                            .then(response => response.json() as Promise<PickupLocationData[]>)
                            .then(data => {
                                setPickupLocationsData(data);
                                setIsPickupLocationsDataLoaded(true);

                                if (data.some(pl => pl.hasBeenSubmitted === false)) {
                                    setIsSubmitEnabled(true);
                                }
                            });
                    })
                    .catch((error) => {
                        setStatusMessage(error.message);
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
    }, [props.currentUser, props.eventId, props.isUserLoaded]);

    function handleNotesChanged(val: string) {
        setNotes(val);
        validateForm();
    }

    function handleHasBeenPickedUpChanged(val: boolean) {
        setHasBeenPickedUp(val);
        validateForm();
    }

    function renderStreetAddressToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationStreetAddress}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationPostalCode}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationNotes}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PickupLocationLastUpdatedDate}</Tooltip>
    }

    function validateForm() {
        if (country === "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    function addPickupLocation() {
        resetForm();
        setIsAddEnabled(false);
        setIsEditOrAdd(true);
    }

    function removePickupLocation(pickupLocationId: string) {
        if (!window.confirm("Please confirm that you want to remove this pickup location?"))
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

                fetch('/api/pickuplocations/' + pickupLocationId, {
                    method: 'DELETE',
                    headers: headers,
                })
                    .then(() => {
                        fetch('/api/pickuplocations/getbyevent/' + props.eventId, {
                            method: 'GET',
                            headers: headers
                        })
                            .then(response => response.json() as Promise<PickupLocationData[]>)
                            .then(data => {
                                resetForm();
                                setPickupLocationsData(data);
                                setIsPickupLocationsDataLoaded(true);

                                if (data.some(pl => !pl.hasBeenSubmitted)) {
                                    setIsSubmitEnabled(true);
                                }
                            })
                    });
            });
        }
    }

    function submitPickupLocations() {
        if (!window.confirm("Please confirm that you want to submit these pickup locations? Once submitted, they cannot be updated."))
            return;
        else {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {
                const headers = getDefaultHeaders('POST');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/pickuplocations/submit/' + props.eventId, {
                    method: 'POST',
                    headers: headers,
                })
                    .then(() => {
                        const getHeaders = getDefaultHeaders('GET');
                        fetch('/api/pickuplocations/getbyevent/' + props.eventId, {
                            method: 'GET',
                            headers: getHeaders
                        })
                            .then(response => response.json() as Promise<PickupLocationData[]>)
                            .then(data => {
                                resetForm();
                                setPickupLocationsData(data);
                                setIsPickupLocationsDataLoaded(true);
                                setIsSubmitEnabled(false);
                                setIsAddEnabled(true);
                                setIsEditOrAdd(false);
                            })
                    });
            });
        }
    }

    function handleSave(event: any) {

        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var partnerLocationData = new PickupLocationData();
        partnerLocationData.id = pickupLocationId;
        partnerLocationData.eventId = props.eventId;
        partnerLocationData.streetAddress = streetAddress ?? "";
        partnerLocationData.city = city ?? "";
        partnerLocationData.region = region ?? "";
        partnerLocationData.country = country ?? "";
        partnerLocationData.postalCode = postalCode ?? "";
        partnerLocationData.latitude = latitude ?? 0;
        partnerLocationData.longitude = longitude ?? 0;
        partnerLocationData.hasBeenSubmitted = hasBeenSubmitted;
        partnerLocationData.hasBeenPickedUp = hasBeenPickedUp;
        partnerLocationData.notes = notes ?? "";
        partnerLocationData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerLocationData.createdDate = createdDate;

        var data = JSON.stringify(partnerLocationData);

        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            var method = "PUT";

            if (pickupLocationId === Guid.EMPTY) {
                method = "POST";
            }

            const headers = getDefaultHeaders(method);
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/pickuplocations', {
                method: method,
                body: data,
                headers: headers,
            })
                .then(response => response.json() as Promise<PickupLocationData>)
                .then(() => {
                    fetch('/api/pickuplocations/getbyevent/' + props.eventId, {
                        method: 'GET',
                        headers: headers
                    })
                        .then(response => response.json() as Promise<PickupLocationData[]>)
                        .then(data => {
                            resetForm();
                            setPickupLocationsData(data);
                            setIsPickupLocationsDataLoaded(true);

                            if (data.some(pl => pl.hasBeenSubmitted === false)) {
                                setIsSubmitEnabled(true);
                            }
                            setIsAddEnabled(true);
                            setIsEditOrAdd(false);
                        })
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
                        setIsSaveEnabled(true);
                    })
            })
    }

    function editPickupLocation(locationId: string) {
        const account = msalClient.getAllAccounts()[0];
        var apiConfig = getApiConfig();

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('GET');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/pickuplocations/' + locationId, {
                method: 'GET',
                headers: headers,
            })
                .then(response => response.json() as Promise<PickupLocationData>)
                .then(data => {
                    setPickupLocationId(data.id);
                    setStreetAddress(data.streetAddress);
                    setCity(data.city);
                    setCountry(data.country);
                    setRegion(data.region);
                    setPostalCode(data.postalCode);
                    setLatitude(data.latitude);
                    setLongitude(data.longitude);
                    setHasBeenPickedUp(data.hasBeenPickedUp);
                    setHasBeenSubmitted(data.hasBeenSubmitted);
                    setNotes(data.notes);
                    setCreatedByUserId(data.createdByUserId);
                    setCreatedDate(new Date(data.createdDate));
                    setLastUpdatedDate(new Date(data.lastUpdatedDate));
                    setIsEditOrAdd(true);
                    setIsAddEnabled(false);
                });
        });
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        resetForm();
        setIsEditOrAdd(false);
        setIsAddEnabled(true);
        setIsSaveEnabled(false);
    }

    function resetForm() {
        setPickupLocationId(Guid.EMPTY);
        setStreetAddress("");
        setCity("");
        setRegion("");
        setCountry("");
        setPostalCode("");
        setLatitude(0);
        setLongitude(0);
        setNotes("");
        setHasBeenPickedUp(false);
        setCreatedByUserId(Guid.EMPTY);
        setCreatedDate(new Date());
        setLastUpdatedDate(new Date());
    }

    const pickupLocationActionDropdownList = (locationId: string) => {
        return (
            <>
                <Dropdown.Item onClick={() => editPickupLocation(locationId)}><Pencil />Edit Location</Dropdown.Item>
                <Dropdown.Item onClick={() => removePickupLocation(locationId)}><XSquare />Remove Location</Dropdown.Item>
            </>
        )
    }

    function renderPartnerLocationContacts() {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Hauling Partner Contacts</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                        </tr>
                    </thead>
                    <tbody>
                        {haulingPartnerLocation?.partnerLocationContacts.map(contact =>
                            <tr key={contact.id}>
                                <td>{contact.name}</td>
                                <td>{contact.email}</td>
                                <td><PhoneInput
                                    value={contact.phone}
                                    disabled
                                /></td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderPickupLocationsTable(locations: PickupLocationData[]) {
        return (
            <div>
                <h2 className="color-primary mt-4 mb-5">Pickup Locations</h2>
                <table className='table table-striped' aria-labelledby="tableLabel" width='100%'>
                    <thead>
                        <tr>
                            <th>Street Address</th>
                            <th>City</th>
                            <th>Submitted?</th>
                            <th>Picked Up?</th>
                            <th>Notes</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {locations.map(location =>
                            <tr key={location.id}>
                                <td>{location.streetAddress}</td>
                                <td>{location.city}</td>
                                <td>{location.hasBeenSubmitted ? 'Yes' : 'No'}</td>
                                <td>{location.hasBeenPickedUp ? 'Yes' : 'No'}</td>
                                <td>{location.notes}</td>
                                <td className="btn py-0">
                                    <Dropdown role="menuitem">
                                        <Dropdown.Toggle id="share-toggle" variant="outline" className="h-100 border-0">...</Dropdown.Toggle>
                                        <Dropdown.Menu id="share-menu">
                                            {pickupLocationActionDropdownList(location.id)}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Button disabled={!isAddEnabled} className="action" onClick={() => addPickupLocation()}>Add a pickup location</Button>
                <Button disabled={!isSubmitEnabled} className="action" onClick={() => submitPickupLocations()}>Submit locations</Button>
            </div>
        );
    }

    function renderEditLocation() {
        return (
            <Form onSubmit={handleSave}>
                <Form.Row>
                    <input type="hidden" name="Id" value={pickupLocationId} />
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <ToggleButton
                                type="checkbox"
                                variant="outline-dark"
                                checked={hasBeenPickedUp}
                                value="1"
                                onChange={(e) => handleHasBeenPickedUpChanged(e.currentTarget.checked)}
                            >
                                Picked Up?
                            </ToggleButton>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <ToggleButton
                                type="checkbox"
                                variant="outline-dark"
                                checked={hasBeenSubmitted}
                                value="1"
                            >
                                Submitted?
                            </ToggleButton>
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderStreetAddressToolTip}>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="StreetAddress">Street Address</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="streetAddress" value={streetAddress} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="City">City</Form.Label>
                            </OverlayTrigger >
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="city" value={city} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="PostalCode">Postal Code</Form.Label>
                            </OverlayTrigger >
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
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="country" value={country} />
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Group>
                    <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                        <Form.Label className="control-label font-weight-bold h5">Notes:</Form.Label>
                    </OverlayTrigger>
                    <Form.Control as="textarea" className='border-0 bg-light h-60 p-18' defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} />
                </Form.Group >
                <Button disabled={!isSaveEnabled} type="submit" className="btn btn-default">Save</Button>
                <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                <Form.Row>
                    <Form.Label>Click on the map to set the location for pickup. The location fields above will be automatically populated.</Form.Label>
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
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="createdDate">Created Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="createdDate" value={createdDate ? createdDate.toLocaleString() : ""} />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLastUpdatedDateToolTip}>
                                <Form.Label className="control-label font-weight-bold h5" htmlFor="lastUpdatedDate">Last Updated Date</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" className='border-0 bg-light h-60 p-18' disabled name="lastUpdatedDate" value={lastUpdatedDate ? lastUpdatedDate.toLocaleString() : ""} />
                        </Form.Group>
                    </Col>
                </Form.Row>
            </Form>
        );
    }

    function renderManageEventPartners() {
        return (
            <div>
                <p><em>{statusMessage}</em></p>
                <ManageEventPartners eventId={props.eventId} isUserLoaded={props.isUserLoaded} currentUser={props.currentUser} />
            </div>
        );
    }

    var pickupLocationsContents = isPickupLocationsDataLoaded && props.eventId !== Guid.EMPTY
        ? renderPickupLocationsTable(pickupLocationsData)
        : renderManageEventPartners()

    var partnerLocationsContents = isPartnerLocationsDataLoaded && props.eventId !== Guid.EMPTY
        ? renderPartnerLocationContacts()
        : ""

    return (
        <>
            <div>
                {pickupLocationsContents}
                {partnerLocationsContents}
                {isEditOrAdd && renderEditLocation()}
            </div>
        </>
    );
}