import * as React from 'react'
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, Col, Form } from 'react-bootstrap';
import PartnerRequestData from '../Models/PartnerRequestData';
import UserData from '../Models/UserData';
import * as Constants from '../Models/Constants';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import AddressData from '../Models/AddressData';
import MapControllerSinglePointNoEvents from '../MapControllerSinglePointNoEvent';

interface PartnerRequestDetails extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerRequestDetails: React.FC<PartnerRequestDetails> = (props) => {
    const [name, setName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [website, setWebsite] = React.useState<string>();
    const [phone, setPhone] = React.useState<string>();
    const [notes, setNotes] = React.useState<string>("");
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [websiteErrors, setWebsiteErrors] = React.useState<string>("");
    const [phoneErrors, setPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [latitudeErrors, setLatitudeErrors] = React.useState<string>("");
    const [longitudeErrors, setLongitudeErrors] = React.useState<string>("");
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);

    React.useEffect(() => {

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
    }, [props.currentUser, props.isUserLoaded]);

    function validateForm() {
        if (nameErrors !== "" ||
            notesErrors !== "" ||
            emailErrors !== "" ||
            websiteErrors !== "" ||
            phoneErrors !== "" ||
            latitudeErrors !== "" ||
            longitudeErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);

        var partnerRequestData = new PartnerRequestData();
        partnerRequestData.name = name ?? "";
        partnerRequestData.email = email ?? "";
        partnerRequestData.phone = phone ?? "";
        partnerRequestData.website = website ?? "";
        partnerRequestData.partnerRequestStatusId = 1;
        partnerRequestData.notes = notes ?? "";
        partnerRequestData.city = city ?? "";
        partnerRequestData.region = region ?? "";
        partnerRequestData.country = country ?? "";
        partnerRequestData.latitude = latitude ?? "";
        partnerRequestData.longitude = longitude ?? "";
        partnerRequestData.createdByUserId = props.currentUser.id;
        partnerRequestData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(partnerRequestData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/PartnerRequests', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then(() => {
                props.history.push("/");
            })
        });
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push("/");
    }

    function handleNameChanged(val: string) {
        if (name === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }

        validateForm();
    }

    function handleEmailChanged(val: string) {
        var pattern = new RegExp(Constants.RegexEmail);

        if (!pattern.test(val)) {
            setEmailErrors("Please enter valid email address.");
        }
        else {
            setEmailErrors("");
            setEmail(val);
        }

        validateForm();
    }

    function handleWebsiteChanged(val: string) {
        var pattern = new RegExp(Constants.RegexWebsite);

        if (!pattern.test(val)) {
            setWebsiteErrors("Please enter valid website.");
        }
        else {
            setWebsiteErrors("");
            setWebsite(val);
        }

        validateForm();
    }

    function handlePhoneChanged(val: string) {
        var pattern = new RegExp(Constants.RegexPhoneNumber);

        if (!pattern.test(val)) {
            setPhoneErrors("Please enter a valid phone number.");
        }
        else {
            setPhoneErrors("");
            setPhone(val);
        }

        validateForm();
    }

    function handleNotesChanged(val: string) {
        if (val.length < 0 || val.length > 1000) {
            setNotesErrors("Notes cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setNotesErrors("");
            setNotes(val);
        }

        validateForm();
    }

    function handleCityChanged(val: string) {
        setCity(val);

        validateForm();
    }

    function selectCountry(val: string) {
        setCountry(val);

        validateForm();
    }

    function selectRegion(val: string) {
        setRegion(val);

        validateForm();
    }

    function handlePostalCodeChanged(val: string) {
        setPostalCode(val);
        validateForm();
    }

    function handleLatitudeChanged(val: string) {
        try {
            if (val) {
                var floatVal = parseFloat(val);

                if (floatVal < -90 || floatVal > 90) {
                    setLatitudeErrors("Latitude must be => -90 and <= 90");
                }
                else {
                    setLatitude(floatVal);
                    setLatitudeErrors("");
                }
            }
            else {
                setLatitudeErrors("Latitude must be => -90 and <= 90");
            }
        }
        catch {
            setLatitudeErrors("Latitude must be a valid number.");
        }

        validateForm();
    }

    function handleLongitudeChanged(val: string) {
        try {
            if (val) {
                var floatVal = parseFloat(val);

                if (floatVal < -180 || floatVal > 180) {
                    setLongitudeErrors("Longitude must be >= -180 and <= 180");
                }
                else {
                    setLongitude(floatVal);
                    setLongitudeErrors("");
                }
            }
            else {
                setLongitudeErrors("Longitude must be >= -180 and <= 180");
            }
        }
        catch {
            setLongitudeErrors("Longitude must be a valid number");
        }

        validateForm();
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestName}</Tooltip>
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPrimaryEmail}</Tooltip>
    }

    function renderWebsiteToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestSecondaryEmail}</Tooltip>
    }

    function renderPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPrimaryPhone}</Tooltip>
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestNotes}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPostalCode}</Tooltip>
    }

    function renderLatitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestLatitude}</Tooltip>
    }

    function renderLongitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestLongitude}</Tooltip>
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
                        setCity(data.addresses[0].address.municipality);
                        setCountry(data.addresses[0].address.country);
                        setRegion(data.addresses[0].address.countrySubdivisionName);
                        setPostalCode(data.addresses[0].address.postalCode);
                        validateForm();
                    })
            })
    }

    return (
        <div className="container-fluid card">
            <h1>Become a Partner!</h1>
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                <Form.Label className="control-label">Partner Name:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{nameErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                <Form.Label className="control-label">Email:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{emailErrors}</span>
                        </Form.Group >
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderWebsiteToolTip}>
                                <Form.Label className="control-label">Website:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={website} maxLength={parseInt('64')} onChange={(val) => handleWebsiteChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{websiteErrors}</span>
                        </Form.Group >
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderPhoneToolTip}>
                                <Form.Label className="control-label">Phone:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control type="text" defaultValue={phone} maxLength={parseInt('64')} onChange={(val) => handlePhoneChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{phoneErrors}</span>
                        </Form.Group >
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
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                <Form.Label className="control-label" htmlFor="City">City:</Form.Label>
                            </OverlayTrigger >
                            <Form.Control type="text" name="city" value={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                <Form.Label className="control-label" htmlFor="PostalCode">Postal Code:</Form.Label>
                            </OverlayTrigger >
                            <Form.Control type="text" name="postalCode" value={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                <Form.Label className="control-label" htmlFor="Country">Country:</Form.Label>
                            </OverlayTrigger >
                            <div>
                                <CountryDropdown name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                            </div>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                <Form.Label className="control-label" htmlFor="Region">Region:</Form.Label>
                            </OverlayTrigger >
                            <div>
                                <span>{region}</span>
                            </div>
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLatitudeToolTip}>
                                <Form.Label className="control-label" htmlFor="Latitude">Latitude:</Form.Label>
                            </OverlayTrigger>
                            <span>{latitude}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLongitudeToolTip}>
                                <Form.Label className="control-label" htmlFor="Longitude">Longitude:</Form.Label>
                            </OverlayTrigger >
                            <span>{region}</span>
                            <Form.Control type="text" name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)}  />
                            <span style={{ color: "red" }}>{longitudeErrors}</span>
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
                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                    <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                </Form.Group >
            </Form >
        </div>
    )
}

export default withRouter(PartnerRequestDetails);