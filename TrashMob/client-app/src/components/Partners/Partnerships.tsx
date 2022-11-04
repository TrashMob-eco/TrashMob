import * as React from 'react'
import { FC } from 'react'
import { Button, Col, Container, Form, Image, OverlayTrigger, Row, Tooltip } from 'react-bootstrap';
import globes from '../assets/gettingStarted/globes.png';
import heroImg from '../assets/partnerships/whatIsPartnerships.svg';
import Safetykits from '../assets/partnerships/Safetykits.svg';
import Supplies from '../assets/partnerships/Supplies.svg';
import TrashDisposal from '../assets/partnerships/TrashDisposal.svg';
import Garbage from '../assets/partnerships/garbage.svg';

import { Link } from 'react-router-dom';
import PartnerRequestData from '../Models/PartnerRequestData';
import * as Constants from '../Models/Constants';
import * as ToolTips from "../../store/ToolTips";
import { AzureMapsProvider } from 'react-azure-maps';
import MapControllerSinglePointNoEvents from '../MapControllerSinglePointNoEvent';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import AddressData from '../Models/AddressData';
import { getKey } from '../../store/MapStore';

export const Partnerships: FC<any> = (props) => {
    const [name, setName] = React.useState<string>();
    const [partnerTypeList, setPartnerTypeList] = React.useState<[]>([]);
    const [partnerTypeId, setPartnerTypeId] = React.useState<number>(0);
    const [email, setEmail] = React.useState<string>();
    const [website, setWebsite] = React.useState<string>();
    const [phone, setPhone] = React.useState<string>();
    const [address, setAddress] = React.useState<string>();
    const [type, setType] = React.useState<string>();
    const [notes, setNotes] = React.useState<string>("");
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [emailErrors, setEmailErrors] = React.useState<string>("");
    const [websiteErrors, setWebsiteErrors] = React.useState<string>("");
    const [phoneErrors, setPhoneErrors] = React.useState<string>("");
    const [notesErrors, setNotesErrors] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [center, setCenter] = React.useState<any>();
    const [mapOptions, setMapOptions] = React.useState<any>();
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isPartnerTypeDataLoaded, setIsPartnerTypeDataLoaded] = React.useState<boolean>(false);
    const [title, setTitle] = React.useState<string>("Apply to become a partner");
    function validateForm() {
        if (nameErrors !== "" ||
            notesErrors !== "" ||
            emailErrors !== "" ||
            websiteErrors !== "" ||
            phoneErrors !== "") {
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
        partnerRequestData.partnerRequestStatusId = Constants.PartnerRequestStatusSent;
        partnerRequestData.notes = notes ?? "";
        partnerRequestData.streetAddress = streetAddress ?? "";
        partnerRequestData.city = city ?? "";
        partnerRequestData.region = region ?? "";
        partnerRequestData.country = country ?? "";
        partnerRequestData.latitude = latitude ?? 0;
        partnerRequestData.longitude = longitude ?? 0;
        partnerRequestData.createdByUserId = "0";
        partnerRequestData.partnerTypeId = partnerTypeId;
        partnerRequestData.isBecomeAPartnerRequest = (true)

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
                // props.history.push("/");
            })
        });
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        // props.history.push("/");
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

    function handleAddressChanged(val: string) {
        setAddress(val);
    }

    function handleCityChanged(val: string) {
        setCity(val);
    }

    function handlePascalCodeChanged(val: string) {
        setPostalCode(val);
    }

    function handleCountryChanged(val: string) {
        setCountry(val);
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

    function selectPartnerType(val: string) {
        setPartnerTypeId(parseInt(val));
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestName}</Tooltip>
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

    function renderStreetAddressToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestStreetAddress}</Tooltip>
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

    function handleLocationChange(point: any) {
        // In an Azure Map point, the longitude is the first position, and latitude is second
        setLatitude(point[1]);
        setLongitude(point[0]);
        var locationString = point[1] + ',' + point[0]
        var headers = getDefaultHeaders('GET');

        getKey()
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
    return (
        <>
            <Container fluid className='bg-grass mb-5'>
                <Row className="text-center pt-0">
                    <Col md={7} className="d-flex flex-column justify-content-center pr-5">
                        <h1 className="font-weight-bold">Partnerships</h1>
                        <p className="font-weight-bold">Connecting you to nearby services.</p>
                    </Col>
                    <Col md={5}>
                        <Image src={globes} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>
            <Container className='py-5'>
                <Row>
                    <Col sm={7}>
                        <h1 className='fw-600'>What are partnerships?</h1>
                        <h4>Partnering with local cities and businesses can connect TrashMob event attendees and creators with the supplies and services they need.</h4>
                        <p className='para'>Partners can include cities, local businesses, and branches/locations of larger companies. Services can include trash hauling and disposal locations, and supplies can include buckets, grabber tools, and safety equipment.
                            Looking for supplies and services for your next event? Invite a partnership from your city! Have supplies and services to offer? Become a partner!</p>
                        <Button variant="primary" className='text-center mt-4 para px-3 h-49'>
                            Request a partnership
                        </Button>
                        <Button variant="outline" className='text-center ml-3 mt-4 para px-3 h-49'>
                            Become a partner
                        </Button>
                    </Col>
                    <Col sm={5}>
                        <Image src={heroImg} alt="globes" className="mt-0 h-100" />
                    </Col>
                </Row>
            </Container>

            <Container fluid className='bg-white text-center py-5'>
                <h1 className='fw-600'>Benefits of partnerships</h1>
                <h4>Services and supplies offered can include:</h4>
                <Row className='w-50 mt-5 mx-auto'>
                    <Col>
                        <Image src={Safetykits} alt="Safetykits" className="mt-0" />
                        <h4>Safety kits</h4>
                    </Col>
                    <Col>
                        <Image src={Supplies} alt="Supplies" className="mt-0" />
                        <h4>Supplies</h4>
                    </Col>
                    <Col>
                        <Image src={TrashDisposal} alt="TrashDisposal" className="mt-0" />
                        <h4>Trash disposal</h4>
                    </Col>
                </Row>
            </Container>

            <Container className='py-5'>
                <Row>
                    <Col sm={7}>
                        <h1 className='fw-600'>Making the most out of partnerships</h1>
                        <h4>After setting a location for an event, recommended partners and their services will be revealed depending on the event’s radius and range of the partner’s supplies.</h4>
                        <p className='para'>After selecting one or more of the services, it will go to the partner for confirmation. Once approved, event attendees will see the services and supplies list when they register for the event. Following the instructions given by the partner, both event creators and attendees can access the supplied services and resources. Note that supplied services from a given partner may vary by location/branch.
                            If there is no partner listed, request a partnership for your area and the TrashMob team will reach out to onboard the partner if they’re interested.
                            Have supplies and services to offer? Become a partner yourself!</p>
                        <Button variant="primary" className='text-center mt-4 para px-3 h-49'>
                            Request a partnership
                        </Button>
                        <Button variant="outline" className='text-center ml-3 mt-4 para px-3 h-49'>
                            Become a partner
                        </Button>
                    </Col>
                    <Col sm={5}>
                        <Image src={Garbage} alt="globes" className="h-100 mt-0" />
                    </Col>
                </Row>
            </Container>

            <div className='w-100 bg-white'>
                <Container className='py-5'>
                    <div className='d-flex justify-content-between align-items-center'>
                        <h1 className='fw-600 m-0'>Invite a partner</h1>
                        <Button variant="outline" className='text-center m-0 para px-3 h-49 fw-600'>
                            Become a partner
                        </Button>
                    </div>
                    <Form onSubmit={handleSave} className="mt-5 p-4 directorCard" >
                        <Form.Row>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderNameToolTip}>
                                        <Form.Label className="control-label h5">Partner Name:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={name} maxLength={parseInt('64')} onChange={(val) => handleNameChanged(val.target.value)} required />
                                    <span style={{ color: "red" }}>{nameErrors}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderPartnerTypeToolTip}>
                                        <Form.Label className="control-label h5" htmlFor="PartnerType">Type:</Form.Label>
                                    </OverlayTrigger>
                                    <div className='d-flex h-60'>
                                        <div className='d-flex w-100 align-items-center'>
                                            <input type="radio" className='m-0' defaultValue={type} name="type" onChange={(val) => setType(val.target.value)} required />
                                            <label className="control-label m-0 ml-2">City or government entity</label>
                                        </div>
                                        <div className='d-flex w-100 align-items-center'>
                                            <input type="radio" className='m-0' defaultValue={type} name="type" onChange={(val) => setType(val.target.value)} required />
                                            <label className="control-label m-0 ml-2">Business</label>
                                        </div>

                                        {/* <select data-val="true" className='w-100 bg-white border-0' name="partnerTypeId" defaultValue={partnerTypeId} onChange={(val) => selectPartnerType(val.target.value)} required>
                                            <option value="">-- Select Partner Type --</option>
                                            {[1, 2, 3].map((partnerType, ind) =>
                                                <option key={ind} value={partnerType}>{partnerType}</option>
                                            )}
                                        </select> */}
                                    </div>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderEmailToolTip}>
                                        <Form.Label className="control-label h5">Email:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={email} maxLength={parseInt('64')} onChange={(val) => handleEmailChanged(val.target.value)} required />
                                    <span style={{ color: "red" }}>{emailErrors}</span>
                                </Form.Group >
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderWebsiteToolTip}>
                                        <Form.Label className="control-label h5">Website:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={website} maxLength={parseInt('1024')} onChange={(val) => handleWebsiteChanged(val.target.value)} />
                                    <span style={{ color: "red" }}>{websiteErrors}</span>
                                </Form.Group >
                            </Col>
                        </Form.Row>
                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPhoneToolTip}>
                                        <Form.Label className="control-label h5">Phone:</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={phone} maxLength={parseInt('64')} onChange={(val) => handlePhoneChanged(val.target.value)} />
                                    <span style={{ color: "red" }}>{phoneErrors}</span>
                                </Form.Group >
                            </Col>
                        </Form.Row>

                        <Form.Row>
                            <AzureMapsProvider>
                                <>
                                    <MapControllerSinglePointNoEvents center={center} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props?.currentUser} isUserLoaded={props?.isUserLoaded} isDraggable={true} />
                                </>
                            </AzureMapsProvider>
                        </Form.Row>

                        <Form.Row>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderStreetAddressToolTip}>
                                        <Form.Label className="control-label h5" htmlFor="StreetAddress">Street Address:</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={address} maxLength={parseInt('64')} onChange={(val) => handleAddressChanged(val.target.value)} />
                                    <span>{streetAddress}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                        <Form.Label className="control-label h5" htmlFor="City">City:</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={city} maxLength={parseInt('64')} onChange={(val) => handleCityChanged(val.target.value)} />
                                    <span>{city}</span>
                                </Form.Group>
                            </Col>
                            <Col>
                                <Form.Group>
                                    <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                        <Form.Label className="control-label h5" htmlFor="PostalCode">Postal Code</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={postalCode} maxLength={parseInt('64')} onChange={(val) => handlePascalCodeChanged(val.target.value)} />
                                    <span>{postalCode}</span>
                                </Form.Group>
                            </Col>
                        </Form.Row>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderNotesToolTip}>
                                <Form.Label className="control-label h5">Notes:</Form.Label>
                            </OverlayTrigger>
                            <Form.Control as="textarea" className='border-0 bg-light h-60 para' defaultValue={notes} maxLength={parseInt('2048')} rows={5} cols={5} onChange={(val) => handleNotesChanged(val.target.value)} required />
                            <span style={{ color: "red" }}>{notesErrors}</span>
                        </Form.Group >
                        {/* <Form.Row>
                            <Col>
                                <Form.Group className="required">
                                    <OverlayTrigger placement="top" overlay={renderCountryToolTip}>
                                        <Form.Label className="control-label" htmlFor="Country">Country:</Form.Label>
                                    </OverlayTrigger >
                                    <Form.Control type="text" className='border-0 bg-light h-60 para' defaultValue={country} maxLength={parseInt('64')} onChange={(val) => handleCountryChanged(val.target.value)} />
                                    <div>
                                        <span>{country}</span>
                                    </div>
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
                        </Form.Row> */}
                        {/* <Form.Row>
                            <Form.Label>Click on the map to set the location for your Partner. The location fields above will be automatically populated.</Form.Label>
                        </Form.Row> */}

                        <Form.Group className="form-group d-flex justify-content-end">
                            <Button disabled={!isSaveEnabled} type="submit" className="action btn-default px-3 h-49">Submit</Button>
                            {/* <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button> */}
                        </Form.Group >
                    </Form >
                </Container>
            </div>



            {/* <Container className="py-5">
                <h2 className='font-weight-bold'>What are partnerships?</h2>

                <p>Partnering with local cities and businesses can connect TrashMob event attendees and creators with the supplies and services they need.</p>

                <p>Partners can include cities, local businesses, and branches/locations of larger companies. Services can include trash hauling and disposal locations, and supplies can include buckets, grabber tools, and safety equipment.</p>

                <p>Looking for supplies and services for your next event? Request a partnership from your city! Have supplies and services to offer? Become a partner!</p>

                <h2 className='font-weight-bold'>Benefits of partnerships?</h2>

                <p>Services and supplies offered can include</p>

                <ul>
                    <li>Trash Hauling</li>
                    <li>Trash Disposal</li>
                    <li>Supplies</li>
                    <li>Starter Kits</li>
                </ul>

                <h2 className='font-weight-bold'>Making the most of partnerships</h2>

                <p>After setting a location for an event, recommended partners and their services will be revealed depending on the event’s radius and range of the partner’s supplies.</p>

                <p>After selecting one or more of the services, it will go to the partner for confirmation. Once approved, event attendees will see the services and supplies list when they register for the event. Following the instructions given by the partner, both event creators and attendees can access the supplied services and resources.</p>

                <p>If there is no partner listed, request a partnership for your area and the TrashMob team will reach out to onboard the partner if they’re interested.</p>

                <p>Have supplies and services to offer? Become a partner yourself!</p>
            </Container>
            <Container fluid className="bg-white">
                <Row className="text-center pt-5">
                    <Col md>
                        <div className="d-flex flex-column">
                            <div className="px-5 mb-5">
                                <Link className="mt-2 btn btn-primary" to="/requestapartner" role="button">Request a partnership</Link>
                            </div>
                            <div className="px-5 mb-5">
                                <Link className="mt-2 btn btn-primary" to="/becomeapartner" role="button">Become a partner</Link>
                            </div>
                        </div>
                    </Col>
                </Row>
            </Container> */}
        </>
    );
}
