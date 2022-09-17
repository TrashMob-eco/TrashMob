import * as React from 'react'
import UserData from '../Models/UserData';
import { Button, Col, Form, OverlayTrigger, Tooltip } from 'react-bootstrap';
import CommunityData from '../Models/CommunityData';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import * as ToolTips from "../../store/ToolTips";
import CommunityStatusData from '../Models/CommunityStatusData';
import AddressData from '../Models/AddressData';
import { getKey } from '../../store/MapStore';
import { CountryDropdown, RegionDropdown } from 'react-country-region-selector';
import { AzureMapsProvider, IAzureMapOptions } from 'react-azure-maps';
import MapControllerSinglePointNoEvent from '../MapControllerSinglePointNoEvent';
import { data } from 'azure-maps-control';
import * as MapStore from '../../store/MapStore';

export interface CommunityEditDataProps {
    community: CommunityData;
    communityStatusList: CommunityStatusData[];
    isCommunityDataLoaded: boolean;
    isUserLoaded: boolean;
    currentUser: UserData;
    onCommunityUpdated: any;
    onEditCanceled: any;
};

export const CommunityEdit: React.FC<CommunityEditDataProps> = (props) => {

    const [communityId, setCommunityId] = React.useState<string>(props.community.id);
    const [city, setCity] = React.useState<string>(props.community.city);
    const [region, setRegion] = React.useState<string>(props.community.region);
    const [country, setCountry] = React.useState<string>(props.community.country);
    const [postalCode, setPostalCode] = React.useState<string>(props.community.postalCode);
    const [communityStatusId, setCommunityStatusId] = React.useState<number>(props.community.communityStatusId);
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [latitudeErrors, setLatitudeErrors] = React.useState<string>("");
    const [longitudeErrors, setLongitudeErrors] = React.useState<string>("");
    const [center, setCenter] = React.useState<data.Position>(new data.Position(MapStore.defaultLongitude, MapStore.defaultLatitude));
    const [isMapKeyLoaded, setIsMapKeyLoaded] = React.useState<boolean>(false);
    const [mapOptions, setMapOptions] = React.useState<IAzureMapOptions>();
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
    }, [])

    function validateForm() {
        if (city === "" ||
            region === "" ||
            country === "") {
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

        var communityData = new CommunityData();
        communityData.id = props.community.id;
        communityData.city = city ?? "";
        communityData.region = region ?? "";
        communityData.country = country ?? "";
        communityData.postalCode = postalCode ?? "";
        communityData.latitude = latitude ?? "";
        communityData.longitude = longitude ?? "";
        communityData.communityStatusId = communityStatusId ?? 2;
        communityData.createdByUserId = props.community.createdByUserId ?? props.currentUser.id;
        communityData.createdDate = props.community.createdDate;
        communityData.lastUpdatedByUserId = props.currentUser.id;

        var data = JSON.stringify(communityData);

        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('PUT');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            fetch('/api/Communities', {
                method: 'PUT',
                body: data,
                headers: headers,
            })
                .then(() => {
                    props.onCommunityUpdated()
                });
        });
    }

    // This will handle Cancel button click event.  
    function handleCancel(event: any) {
        event.preventDefault();
        props.onEditCanceled();
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
            setLatitudeErrors("Invalid value specified for Latitude.");
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
            setLongitudeErrors("Invalid value specified for Longitude.")
        }

        validateForm();
    }


    function renderCommunityStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityStatus}</Tooltip>
    }

    function renderCityToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityCity}</Tooltip>
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityCountry}</Tooltip>
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityRegion}</Tooltip>
    }

    function renderPostalCodeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityPostalCode}</Tooltip>
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityCreatedDate}</Tooltip>
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityLastUpdatedDate}</Tooltip>
    }

    function renderLatitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityLatitude}</Tooltip>
    }

    function renderLongitudeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.CommunityLongitude}</Tooltip>
    }

    function selectCommunityStatus(val: string) {
        setCommunityStatusId(parseInt(val));
    }

    function handleLocationChange(point: data.Position) {
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
                        setCity(data.addresses[0].address.municipality);
                        setCountry(data.addresses[0].address.country);
                        setRegion(data.addresses[0].address.countrySubdivisionName);
                        setPostalCode(data.addresses[0].address.postalCode);
                    })
            }
            )

        validateForm();
    }

    return (
        <div className="container-fluid card">
            <h1>Edit Community</h1>
            <Form onSubmit={handleSave} >
                <Form.Row>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderCityToolTip}>
                                <Form.Label className="control-label" htmlFor="City">City:</Form.Label>
                            </OverlayTrigger >
                            <Form.Control className="control-label" disabled type="text" name="city" value={city} onChange={(val) => handleCityChanged(val.target.value)} maxLength={parseInt('256')} required />
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderPostalCodeToolTip}>
                                <Form.Label className="control-label" htmlFor="PostalCode">Postal Code:</Form.Label>
                            </OverlayTrigger >
                            <Form.Control type="text" disabled name="postalCode" value={postalCode} onChange={(val) => handlePostalCodeChanged(val.target.value)} maxLength={parseInt('25')} />
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
                                <CountryDropdown disabled name="country" value={country ?? ""} onChange={(val) => selectCountry(val)} />
                            </div>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group className="required">
                            <OverlayTrigger placement="top" overlay={renderRegionToolTip}>
                                <Form.Label className="control-label" htmlFor="Region">Region:</Form.Label>
                            </OverlayTrigger >
                            <div>
                                <RegionDropdown disabled
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
                            <Form.Control type="text" disabled name="latitude" value={latitude} onChange={(val) => handleLatitudeChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{latitudeErrors}</span>
                        </Form.Group>
                    </Col>
                    <Col>
                        <Form.Group>
                            <OverlayTrigger placement="top" overlay={renderLongitudeToolTip}>
                                <Form.Label className="control-label" htmlFor="Longitude">Longitude:</Form.Label>
                            </OverlayTrigger >
                            <Form.Control type="text" disabled name="longitude" value={longitude} onChange={(val) => handleLongitudeChanged(val.target.value)} />
                            <span style={{ color: "red" }}>{longitudeErrors}</span>
                        </Form.Group>
                    </Col>
                </Form.Row>
                <Form.Row>
                    <Form.Group>
                        <Button disabled={!isSaveEnabled} type="submit" className="btn btn-default">Save</Button>
                        <Button className="action" onClick={(e: any) => handleCancel(e)}>Cancel</Button>
                    </Form.Group>
                </Form.Row>
                <Form.Row>
                    <Form.Label>Search for location or click on the map to set the location for your event. The location fields above will be automatically populated.</Form.Label>
                </Form.Row>
                <Form.Row>
                    <AzureMapsProvider>
                        <>
                            <MapControllerSinglePointNoEvent center={center} mapOptions={mapOptions} isMapKeyLoaded={isMapKeyLoaded} latitude={latitude} longitude={longitude} onLocationChange={handleLocationChange} currentUser={props.currentUser} isUserLoaded={props.isUserLoaded} isDraggable={true} />
                        </>
                    </AzureMapsProvider>
                </Form.Row>
            </Form >
        </div>
    )
}