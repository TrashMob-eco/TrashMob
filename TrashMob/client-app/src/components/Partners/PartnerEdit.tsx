import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import PartnerData from '../Models/PartnerData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import PartnerStatusData from '../Models/PartnerStatusData';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import * as MapStore from '../../store/MapStore';
import { data } from 'azure-maps-control';
import AddressData from '../Models/AddressData';
import MapControllerSinglePointNoEvent from '../MapControllerSinglePointNoEvent';
import PartnerTypeData from '../Models/PartnerTypeData';

export interface PartnerEditDataProps {
    partnerId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
};

export const PartnerEdit: React.FC<PartnerEditDataProps> = (props) => {

    const [isPartnerDataLoaded, setIsPartnerDataLoaded] = React.useState<boolean>(false);
    const [partnerStatusList, setPartnerStatusList] = React.useState<PartnerStatusData[]>([]);
    const [partnerTypeList, setPartnerTypeList] = React.useState<PartnerTypeData[]>([]);
    const [name, setName] = React.useState<string>("");
    const [website, setWebsite] = React.useState<string>("");
    const [partnerStatusId, setPartnerStatusId] = React.useState<number>(0);
    const [partnerTypeId, setPartnerTypeId] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>("");
    const [city, setCity] = React.useState<string>("");
    const [country, setCountry] = React.useState<string>("");
    const [region, setRegion] = React.useState<string>("");
    const [postalCode, setPostalCode] = React.useState<string>("");
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [nameErrors, setNameErrors] = React.useState<string>("");
    const [publicNotes, setPublicNotes] = React.useState<string>("");
    const [publicNotesErrors, setPublicNotesErrors] = React.useState<string>();
    const [privateNotes, setPrivateNotes] = React.useState<string>("");

    const [createdByUserId, setCreatedByUserId] = React.useState<string>("");
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [isSaveEnabled, setIsSaveEnabled] = React.useState<boolean>(false);
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));

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

                fetch('/api/partnerstatuses', {
                    method: 'GET',
                    headers: headers
                })
                    .then(response => response.json() as Promise<PartnerStatusData[]>)
                    .then(data => {
                        setPartnerStatusList(data)
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
                            .then(_ => {
                                setIsPartnerDataLoaded(false);

                                fetch('/api/partners/' + props.partnerId, {
                                    method: 'GET',
                                    headers: headers
                                })
                                    .then(response => response.json() as Promise<PartnerData>)
                                    .then(data => {
                                        setPartnerStatusId(data.partnerStatusId);
                                        setPartnerTypeId(data.partnerTypeId);
                                        setName(data.name);
                                        setPublicNotes(data.publicNotes);
                                        setPrivateNotes(data.privateNotes);
                                        setWebsite(data.website);
                                        setStreetAddress(data.streetAddress);
                                        setCity(data.city);
                                        setRegion(data.region);
                                        setCountry(data.country);
                                        setPostalCode(data.postalCode);
                                        setLatitude(data.latitude ?? 0);
                                        setLongitude(data.longitude ?? 0);
                                        setCreatedByUserId(data.createdByUserId);
                                        setCreatedDate(data.createdDate);
                                        setLastUpdatedDate(data.lastUpdatedDate);
                                        setIsPartnerDataLoaded(true);
                                    })
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

    }, [props.currentUser, props.isUserLoaded, props.partnerId]);

    function validateForm() {
        if (name === "" ||
            nameErrors !== "") {
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

        var partnerData = new PartnerData();
        partnerData.id = props.partnerId;
        partnerData.name = name ?? "";
        partnerData.website = website ?? "";
        partnerData.partnerStatusId = partnerStatusId ?? 2;
        partnerData.streetAddress = streetAddress;
        partnerData.city = city;
        partnerData.region = region;
        partnerData.country = country;
        partnerData.postalCode = postalCode;
        partnerData.latitude = latitude;
        partnerData.longitude = longitude;
        partnerData.publicNotes = publicNotes;
        partnerData.privateNotes = privateNotes;
        partnerData.partnerTypeId = partnerTypeId;
        partnerData.createdByUserId = createdByUserId ?? props.currentUser.id;
        partnerData.createdDate = createdDate;
        partnerData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(partnerData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Partners', {
                method: 'PUT',
                body: data,
                headers: headers,
            })
        });
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
    }

    function handleNameChanged(val: string) {
        if (val === "") {
            setNameErrors("Name cannot be blank.");
        }
        else {
            setNameErrors("");
            setName(val);
        }

        validateForm();
    }

    function handlePublicNotesChanged(notes: string) {
        if (notes === "") {
            setPublicNotesErrors("Notes cannot be empty.");
        }
        else {
            setPublicNotes(notes);
            setPublicNotesErrors("");
        }

        validateForm();
    }

    function handlePrivateNotesChanged(notes: string) {
        setPrivateNotes(notes);
        validateForm();
    }

    function handleWebsiteChanged(val: string) {
        setWebsite(val);        

        validateForm();
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

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerName}</Tooltip>
    }

    function renderWebsiteToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerWebsite}</Tooltip>
    }

    function renderPublicNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerPublicNotes}</Tooltip>
    }

    function renderPrivateNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerPrivateNotes}</Tooltip>
    }

    function renderPartnerStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerStatus}</Tooltip>
    }

    function renderPartnerTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerType}</Tooltip>
    }

    function renderStreetAddressToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerStreetAddress}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerPostalCode}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerLastUpdatedDate}</Tooltip>
    }

    function selectPartnerStatus(val: string) {
        setPartnerStatusId(parseInt(val));
    }

    function selectPartnerType(val: string) {
        setPartnerTypeId(parseInt(val));
    }

    // Returns the HTML Form to the render() method.  
    function renderCreateForm(statusList: Array<PartnerStatusData>, typeList: Array<PartnerTypeData>) {
        return (
            <div className="container-fluid card">
                <h1>Edit Partner</h1>
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
                            <Form.Group>
                                <OverlayTrigger placement="top" overlay={renderWebsiteToolTip}>
                                    <Form.Label className="control-label">Website:</Form.Label>
                                </OverlayTrigger>
                                <Form.Control type="text" defaultValue={website} maxLength={parseInt('1024')} onChange={(val) => handleWebsiteChanged(val.target.value)} />
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPartnerStatusToolTip}>
                                    <Form.Label className="control-label" htmlFor="Partner Status">Partner Status:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select data-val="true" name="partnerStatusId" defaultValue={partnerStatusId} onChange={(val) => selectPartnerStatus(val.target.value)} required>
                                        <option value="">-- Select Partner Status --</option>
                                        {statusList.map(status =>
                                            <option key={status.id} value={status.id}>{status.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderPartnerTypeToolTip}>
                                    <Form.Label className="control-label" htmlFor="PartnerType">Partner Type:</Form.Label>
                                </OverlayTrigger>
                                <div>
                                    <select data-val="true" name="partnerTypeId" defaultValue={partnerTypeId} onChange={(val) => selectPartnerType(val.target.value)} required>
                                        <option value="">-- Select Partner Type --</option>
                                        {typeList.map(partnerType =>
                                            <option key={partnerType.id} value={partnerType.id}>{partnerType.name}</option>
                                        )}
                                    </select>
                                </div>
                            </Form.Group>
                        </Col>
                    </Form.Row>
                    <Form.Row>
                        <Col>
                            <Form.Group className="required">
                                <OverlayTrigger placement="top" overlay={renderStreetAddressToolTip}>
                                    <Form.Label className="control-label" htmlFor="StreetAddress">Street Address:</Form.Label>
                                </OverlayTrigger >
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
                    <Form.Group className="form-group">
                        <Button disabled={!isSaveEnabled} type="submit" className="action btn-default">Save</Button>
                        <Button className="action" onClick={(e) => handleCancel(e)}>Cancel</Button>
                    </Form.Group >
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
                </Form >
            </div>
        )
    }

    var contents = isPartnerDataLoaded && props.partnerId
        ? renderCreateForm(partnerStatusList, partnerTypeList)
        : <p><em>Loading...</em></p>;

    return <div>
        <hr />
        {contents}
    </div>;
}