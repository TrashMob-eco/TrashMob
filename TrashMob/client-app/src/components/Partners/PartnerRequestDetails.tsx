import * as React from 'react';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import OverlayTrigger from 'react-bootstrap/OverlayTrigger';
import Tooltip from 'react-bootstrap/Tooltip';
import { Button, Col, Container, Form, Row } from 'react-bootstrap';
import { data } from 'azure-maps-control';
import { Guid } from 'guid-typescript';
import PhoneInput from 'react-phone-input-2';
import { useMutation, useQuery } from '@tanstack/react-query';
import * as ToolTips from '../../store/ToolTips';
import UserData from '../Models/UserData';
import PartnerRequestStatusData from '../Models/PartnerRequestStatusData';
import PartnerTypeData from '../Models/PartnerTypeData';
import { getPartnerRequestStatus } from '../../store/partnerRequestStatusHelper';
import { getPartnerType } from '../../store/partnerTypeHelper';
import { GetPartnerRequestById, GetPartnerRequestStatuses, GetPartnerTypes } from '../../services/partners';
import { Services } from '../../config/services.config';
import { APIProvider, Marker } from '@vis.gl/react-google-maps';
import { useGetGoogleMapApiKey } from '../../hooks/useGetGoogleMapApiKey';
import { GoogleMap } from '../Map/GoogleMap';

export interface PartnerRequestDetailsMatchParams {
    partnerRequestId: string;
}

interface PartnerRequestDetailsParams extends RouteComponentProps<PartnerRequestDetailsMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerRequestDetails: React.FC<PartnerRequestDetailsParams> = (props) => {
    const [partnerRequestId, setPartnerRequestId] = React.useState<string>('');
    const [name, setName] = React.useState<string>();
    const [email, setEmail] = React.useState<string>();
    const [website, setWebsite] = React.useState<string>();
    const [phone, setPhone] = React.useState<string>();
    const [notes, setNotes] = React.useState<string>('');
    const [latitude, setLatitude] = React.useState<number>(0);
    const [longitude, setLongitude] = React.useState<number>(0);
    const [streetAddress, setStreetAddress] = React.useState<string>();
    const [city, setCity] = React.useState<string>();
    const [country, setCountry] = React.useState<string>('');
    const [region, setRegion] = React.useState<string>();
    const [postalCode, setPostalCode] = React.useState<string>();
    const [createdDate, setCreatedDate] = React.useState<Date>(new Date());
    const [lastUpdatedDate, setLastUpdatedDate] = React.useState<Date>(new Date());
    const [partnerRequestStatusId, setPartnerRequestStatusId] = React.useState<number>(0);
    const [partnerTypeId, setPartnerTypeId] = React.useState<number>(0);
    const [partnerRequestStatusList, setPartnerRequestStatusList] = React.useState<PartnerRequestStatusData[]>([]);
    const [partnerTypeList, setPartnerTypeList] = React.useState<PartnerTypeData[]>([]);
    const [isPartnerRequestDataLoaded, setIsPartnerRequestDataLoaded] = React.useState<boolean>(false);
    const [isPartnerRequestIdReady, setIsPartnerRequestIdReady] = React.useState<boolean>();
    const [loadedPartnerRequestId, setLoadedPartnerRequestId] = React.useState<string | undefined>(
        props.match?.params.partnerRequestId,
    );

    const [partnerType, setPartnerType] = React.useState('Unknown');
    const [partnerRequestStatus, setPartnerRequestStatus] = React.useState('Unknown');

    React.useEffect(() => {
        setPartnerType(getPartnerType(partnerTypeList, partnerTypeId));
    }, [partnerTypeList, partnerTypeId]);

    React.useEffect(() => {
        setPartnerRequestStatus(getPartnerRequestStatus(partnerRequestStatusList, partnerRequestStatusId));
    }, [partnerRequestStatusList, partnerRequestStatusId]);

    const getPartnerRequestStatuses = useQuery({
        queryKey: GetPartnerRequestStatuses().key,
        queryFn: GetPartnerRequestStatuses().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerTypes = useQuery({
        queryKey: GetPartnerTypes().key,
        queryFn: GetPartnerTypes().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerRequestById = useMutation({
        mutationKey: GetPartnerRequestById().key,
        mutationFn: GetPartnerRequestById().service,
    });

    React.useEffect(() => {
        const partId = loadedPartnerRequestId;
        if (!partId) {
            setPartnerRequestId(Guid.createEmpty().toString());
            setLoadedPartnerRequestId(Guid.createEmpty().toString());
        } else {
            setPartnerRequestId(partId);
        }

        setIsPartnerRequestIdReady(true);
    }, [loadedPartnerRequestId]);

    React.useEffect(() => {
        if (props.isUserLoaded) {
            setIsPartnerRequestDataLoaded(false);
            getPartnerRequestStatuses.refetch().then((partnerRequestStatusesRes) => {
                setPartnerRequestStatusList(partnerRequestStatusesRes.data?.data || []);
                getPartnerTypes.refetch().then((partnerTypesRes) => {
                    setPartnerTypeList(partnerTypesRes.data?.data || []);
                    getPartnerRequestById.mutateAsync({ id: partnerRequestId }).then((partnerRequestByIdRes) => {
                        setName(partnerRequestByIdRes.data.name);
                        setEmail(partnerRequestByIdRes.data.email);
                        setWebsite(partnerRequestByIdRes.data.website);
                        setPhone(partnerRequestByIdRes.data.phone);
                        setStreetAddress(partnerRequestByIdRes.data.streetAddress);
                        setCity(partnerRequestByIdRes.data.city);
                        setRegion(partnerRequestByIdRes.data.region);
                        setCountry(partnerRequestByIdRes.data.country);
                        setPostalCode(partnerRequestByIdRes.data.postalCode);
                        setLatitude(partnerRequestByIdRes.data.latitude ?? 0);
                        setLongitude(partnerRequestByIdRes.data.longitude ?? 0);
                        setPartnerRequestStatusId(partnerRequestByIdRes.data.partnerRequestStatusId);
                        setPartnerTypeId(partnerRequestByIdRes.data.partnerTypeId);
                        setNotes(partnerRequestByIdRes.data.notes);
                        setCreatedDate(
                            partnerRequestByIdRes.data.createdDate
                                ? new Date(partnerRequestByIdRes.data.createdDate)
                                : new Date(),
                        );
                        setLastUpdatedDate(
                            partnerRequestByIdRes.data.lastUpdatedDate
                                ? new Date(partnerRequestByIdRes.data.lastUpdatedDate)
                                : new Date(),
                        );
                        setIsPartnerRequestDataLoaded(true);
                    });
                });
            });
        }
        
    }, [props.currentUser, props.isUserLoaded, partnerRequestId]);

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        props.history.push('/mydashboard');
    }

    function renderNameToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestName}</Tooltip>;
    }

    function renderPartnerRequestStatusToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestStatus}</Tooltip>;
    }

    function renderPartnerTypeToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerType}</Tooltip>;
    }

    function renderEmailToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestEmail}</Tooltip>;
    }

    function renderWebsiteToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestWebsite}</Tooltip>;
    }

    function renderPhoneToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestPhone}</Tooltip>;
    }

    function renderNotesToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestNotes}</Tooltip>;
    }

    function renderCountryToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestCountry}</Tooltip>;
    }

    function renderRegionToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestRegion}</Tooltip>;
    }

    function renderCreatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestCreatedDate}</Tooltip>;
    }

    function renderLastUpdatedDateToolTip(props: any) {
        return <Tooltip {...props}>{ToolTips.PartnerRequestLastUpdatedDate}</Tooltip>;
    }

    // Returns the HTML Form to the render() method.
    function renderDetailsForm() {
        return (
            <Container>
                <Row className='gx-2 py-5' lg={2}>
                    <Col lg={4} className='d-flex'>
                        <div className='bg-white py-2 px-5 shadow-sm rounded'>
                            <h2 className='color-primary mt-4 mb-5'>Partner request sent</h2>
                            <p>This partner request has been sent!</p>
                        </div>
                    </Col>
                    <Col lg={8}>
                        <div className='bg-white p-5 shadow-sm rounded'>
                            <h2 className='color-primary mt-4 mb-5'>Partner Request</h2>
                            <Form>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderNameToolTip}>
                                                <Form.Label className='control-label font-weight-bold h5'>
                                                    Partner Name
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='name'
                                                value={name}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderPartnerTypeToolTip}>
                                                <Form.Label className='control-label font-weight-bold h5'>
                                                    Partner Type
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='partnerType'
                                                value={partnerType}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderPartnerRequestStatusToolTip}>
                                                <Form.Label className='control-label font-weight-bold h5'>
                                                    Request Status
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='requestStatus'
                                                value={partnerRequestStatus}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderEmailToolTip}>
                                                <Form.Label className='control-label font-weight-bold h5'>
                                                    Email
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='email'
                                                value={email}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderPhoneToolTip}>
                                                <Form.Label className='control-label font-weight-bold h5'>
                                                    Phone
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <PhoneInput value={phone} disabled />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderWebsiteToolTip}>
                                                <Form.Label className='control-label font-weight-bold h5'>
                                                    Website
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='website'
                                                value={website}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Group>
                                    <OverlayTrigger placement='top' overlay={renderNotesToolTip}>
                                        <Form.Label className='control-label font-weight-bold h5'>Notes</Form.Label>
                                    </OverlayTrigger>
                                    <Form.Control
                                        type='text'
                                        className='border-0 bg-light h-60 p-18'
                                        disabled
                                        name='notes'
                                        value={notes}
                                    />
                                </Form.Group>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <Form.Label className='control-label font-weight-bold h5' htmlFor='City'>
                                                Street Address
                                            </Form.Label>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='streetAddress'
                                                value={streetAddress}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <Form.Label className='control-label font-weight-bold h5' htmlFor='City'>
                                                City
                                            </Form.Label>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='city'
                                                value={city}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <Form.Label
                                                className='control-label font-weight-bold h5'
                                                htmlFor='PostalCode'
                                            >
                                                Postal Code
                                            </Form.Label>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='postalCode'
                                                value={postalCode}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderRegionToolTip}>
                                                <Form.Label
                                                    className='control-label font-weight-bold h5'
                                                    htmlFor='Region'
                                                >
                                                    Region
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='region'
                                                value={region}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderCountryToolTip}>
                                                <Form.Label
                                                    className='control-label font-weight-bold h5'
                                                    htmlFor='Country'
                                                >
                                                    Country
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='country'
                                                value={country ?? ''}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                                <Form.Row>
                                    <GoogleMap 
                                        defaultCenter={{ lat: latitude, lng: longitude }}
                                        defaultZoom={11}
                                    >
                                        <Marker 
                                            position={{ lat: latitude, lng: longitude }}
                                            draggable={false}
                                        />
                                    </GoogleMap>
                                </Form.Row>
                                <Form.Group className='form-group'>
                                    <Button className='action' onClick={(e) => handleCancel(e)}>
                                        Cancel
                                    </Button>
                                </Form.Group>
                                <Form.Row>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderCreatedDateToolTip}>
                                                <Form.Label
                                                    className='control-label font-weight-bold h5'
                                                    htmlFor='createdDate'
                                                >
                                                    Created Date
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='createdDate'
                                                value={createdDate.toString()}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col>
                                        <Form.Group>
                                            <OverlayTrigger placement='top' overlay={renderLastUpdatedDateToolTip}>
                                                <Form.Label
                                                    className='control-label font-weight-bold h5'
                                                    htmlFor='lastUpdatedDate'
                                                >
                                                    Last Updated Date
                                                </Form.Label>
                                            </OverlayTrigger>
                                            <Form.Control
                                                type='text'
                                                className='border-0 bg-light h-60 p-18'
                                                disabled
                                                name='lastUpdatedDate'
                                                value={lastUpdatedDate.toString()}
                                            />
                                        </Form.Group>
                                    </Col>
                                </Form.Row>
                            </Form>
                        </div>
                    </Col>
                </Row>
            </Container>
        );
    }
    const contents =
        isPartnerRequestDataLoaded && isPartnerRequestIdReady && partnerRequestId ? (
            renderDetailsForm()
        ) : (
            <p>
                <em>Loading...</em>
            </p>
        );

    return (
        <div>
            <hr />
            {contents}
        </div>
    );
};

const PartnerRequestDetailsWrapper = (props: PartnerRequestDetailsParams) => {
    const { data: googleApiKey, isLoading } = useGetGoogleMapApiKey()

    if (isLoading) return null;

    return (
        <APIProvider apiKey={googleApiKey || ''}>
            <PartnerRequestDetails {...props} />
        </APIProvider>
    );
};


export default withRouter(PartnerRequestDetailsWrapper);
