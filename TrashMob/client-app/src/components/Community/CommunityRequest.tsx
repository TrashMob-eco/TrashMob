import * as React from 'react'
import CommunityRequestData from '../Models/CommunityRequestData';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { loadCaptchaEnginge, LoadCanvasTemplateNoReload, validateCaptcha } from 'react-simple-captcha';
import { getDefaultHeaders } from '../../store/AuthStore';
import OverlayTrigger from "react-bootstrap/OverlayTrigger";
import Tooltip from "react-bootstrap/Tooltip";
import * as ToolTips from "../../store/ToolTips";
import { Button, ButtonGroup, Col, Container, Form, Modal, Row } from 'react-bootstrap';
import * as Constants from '../Models/Constants';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerPointCollection from '../MapControllerPointCollection';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';
import UserData from '../Models/UserData';
import AddressData from '../Models/AddressData';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';

interface CommunityRequestProps extends RouteComponentProps<any> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const CommunityRequest: React.FC<CommunityRequestProps> = (props) => {
    const [contactName, setContactName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [phone, setPhone] = React.useState<string>();
    const [contactNameErrors, setContactNameErrors] = React.useState<string>("");
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [phoneErrors, setPhoneErrors] = React.useState<string>("");
    const [website, setWebsite] = React.useState<string>("");
    const [websiteErrors, setWebsiteErrors] = React.useState<string>("");
    const [show, setShow] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isLocationDataLoaded, setIsLocationDataLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [latitudeErrors, setLatitudeErrors] = React.useState<string>("");
    const [longitudeErrors, setLongitudeErrors] = React.useState<string>("");
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();

    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);

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

    // This will handle the submit form event.  
    function handleSave(event: any) {

        if (!isSaveEnabled) {
            return;
        }

        setIsSaveEnabled(false);
        event.preventDefault();

        const form = new FormData(event.target);

        var user_captcha_value = form.get("user_captcha_input")?.toString() ?? "";

        if (validateCaptcha(user_captcha_value) === true) {

            var communityRequestData = new CommunityRequestData();
            communityRequestData.contactName = contactName ?? "";
            communityRequestData.email = email ?? "";
            communityRequestData.phone = phone ?? "";
            communityRequestData.website = website ?? "";
            communityRequestData.city = city ?? "";
            communityRequestData.region = region ?? "";
            communityRequestData.country = country ?? "";
            communityRequestData.latitude = latitude ?? "";
            communityRequestData.longitude = longitude ?? "";

            var data = JSON.stringify(communityRequestData);

            const headers = getDefaultHeaders('POST');

            fetch('/api/CommunityRequest', {
                method: 'POST',
                body: data,
                headers: headers,
            }).then(() => {
                setTimeout(() => props.history.push("/"), 2000);
            });

            handleShow();
        }
        else {
            alert('Captcha Does Not Match');
        }
    }

    function validateForm() {
        if (contactNameErrors !== "" ||
            emailErrors !== "" ||
            websiteErrors !== "" ||
            phoneErrors !== "" ||
            contactName === "" ||
            email === "" ||
            website === "" ||
            phone === "" ||
            latitudeErrors !== "" ||
            longitudeErrors !== "") {
            setIsSaveEnabled(false);
        }
        else {
            setIsSaveEnabled(true);
        }
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        setIsSaveEnabled(false);
        props.history.push("/");
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


    function handleContactNameChanged(val: string) {
        if (val.length <= 0 || val.length > 64) {
            setContactNameErrors("Please enter a valid contact name.");
        }
        else {
            setContactNameErrors("");
            setContactName(val);
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
        if (val.length <= 0 || val.length > 1000) {
            setWebsiteErrors("Website cannot be empty and cannot be more than 1000 characters long.");
        }
        else {
            setWebsiteErrors("");
            setWebsite(val);
        }

        validateForm();
    }

    React.useEffect(() => {
        loadCaptchaEnginge(6, 'white', 'black');
    }, []);


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

    function renderContactNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestContactName}</Tooltip>
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestEmail}</Tooltip>
    }

    function renderPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestPhone}</Tooltip>
    }

    function renderWebsiteToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestWebsite}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestPostalCode}</Tooltip>
    }

    function renderLatitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestLatitude}</Tooltip>
    }

    function renderLongitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRequestLongitude}</Tooltip>
    }

    function handleAttendanceChanged() {
        // Do nothing
    }

    function handleDetailsSelected(e: any) {
        // Do nothing
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
        <Container>
            <Row className="gx-2 py-5" lg={2}>
                <Col lg={4} className="d-flex">
                    <div className="bg-white py-2 px-5 shadow-sm rounded">
                        <h2 className="color-primary mt-4 mb-5">Get your community started with TrashMob.eco!</h2>
                        <p>
                            Please help us to onboard your community to TrashMob.eco. We'll need a few pieces of information about your community so we can get started!
                        </p>
                    </div>
                </Col>
                <Col lg={{ span: 7, offset: 1 }}>
                    <div className="bg-white p-5 shadow-sm rounded">
                        <Form onSubmit={handleSave} >

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderContactNameToolTip}>
                                    <Form.Label className="control-label">Contact Name:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={contactName} maxLength={parseInt('64')} onChange={(val) => handleContactNameChanged(val.target.value)} required placeholder="Enter Contact Name" />
                                <span style={{ color: "red" }}>{contactNameErrors}</span>
                            </Form.Group>

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                    <Form.Label className="control-label">Email:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required placeholder="Enter Email" />
                                <span style={{ color: "red" }}>{emailErrors}</span>
                            </Form.Group >

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPhoneToolTip}>
                                    <Form.Label className="control-label">Phone:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={phone} maxLength={parseInt('64')} onChange={(val) => handlePhoneChanged(val.target.value)} required placeholder="Enter Phone" />
                                <span style={{ color: "red" }}>{phoneErrors}</span>
                            </Form.Group >

                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderWebsiteToolTip}>
                                    <Form.Label className="control-label">Website:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={website} maxLength={parseInt('2048')} onChange={(val) => handleWebsiteChanged(val.target.value)} required placeholder="Enter Website" />
                                <span style={{ color: "red" }}>{websiteErrors}</span>
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
                                            <RegionDropdown
                                                country={country ?? ""}
                                                value={region ?? ""}
                                                onChange={(val) => selectRegion(val)} />
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
                                        <Form.Control type="text" name="latitude" value={latitude} onChange={(val) => handleLatitudeChanged(val.target.value)} />
                                        <span style={{ color: "red" }}>{latitudeErrors}</span>
                                    </Form.Group>
                                </Col>
                                <Col>
                                    <Form.Group>
                                        <OverlayTrigger placement="top" overlay={renderLongitudeToolTip}>
                                            <Form.Label className="control-label" htmlFor="Longitude">Longitude:</Form.Label>
                                        </OverlayTrigger >
                                        <Form.Control type="text" name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)} />
                                        <span style={{ color: "red" }}>{longitudeErrors}</span>
                                    </Form.Group>
                                </Col>
                            </Form.Row>
                            <Form.Row>
                                <Form.Label>Click on the map to set the location for your Partner. The location fields above will be automatically populated.</Form.Label>
                            </Form.Row>
                            <Form.Row>
                                <AzureMapsProvider>
                                    <>
                                        <MapControllerPointCollection center={center} multipleEvents={[]} isEventDataLoaded={isLocationDataLoaded} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} eventName="Your City" latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} myAttendanceList={[]} isUserEventDataLoaded={true} onAttendanceChanged={handleAttendanceChanged} onDetailsSelected={handleDetailsSelected} history={props.history} location={props.location} match={props.match} />
                                    </>
                                </AzureMapsProvider>
                            </Form.Row>
                            <Form.Group>
                                <LoadCanvasTemplateNoReload className="border" />
                            </Form.Group>
                            <Form.Group className="required">
                                <Form.Label className="control-label">CAPTCHA Value:</Form.Label>
                                <Form.Control type="text" required name="user_captcha_input" placeholder="Enter Captcha" />
                            </Form.Group >
                            <Form.Group className="form-group d-flex justify-content-end">
                                <ButtonGroup className="justify-content-between">
                                    <Button id="contactFormCancelBtn" className="action mr-2" onClick={(e) => handleCancel(e)}>Cancel</Button>
                                    <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Submit</Button>
                                </ButtonGroup>
                            </Form.Group >
                        </Form >
                    </div>
                </Col>
            </Row >
            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Confirmation</Modal.Title>
                </Modal.Header>
                <Modal.Body className="text-center">
                    <b>Onboarding process has begun. We'll copy you in on all emails we send to your city so you can stay in the loop!</b>
                    <br />
                    <small>You'll now be redirected to the TrashMob.eco home page...</small>
                </Modal.Body>
            </Modal>
        </Container >
    )
}

export default withRouter(CommunityRequest);