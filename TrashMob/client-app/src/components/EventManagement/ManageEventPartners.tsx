import * as React from 'react';
import { Button, Container, Dropdown } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { Link } from 'react-router';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import * as Constants from '../Models/Constants';
import DisplayEventPartnerLocationData from '../Models/DisplayEventPartnerLocationData';
import EventPartnerLocationServiceStatusData from '../Models/EventPartnerLocationServiceStatusData';
import { getEventPartnerLocationServiceStatus } from '../../store/eventPartnerLocationServiceStatusHelper';
import { getServiceType } from '../../store/serviceTypeHelper';
import ServiceTypeData from '../Models/ServiceTypeData';
import EventPartnerLocationServiceData from '../Models/EventPartnerLocationServiceData';
import DisplayEventPartnerLocationServiceData from '../Models/DisplayEventPartnerLocationServiceData';
import { GetServiceTypes } from '../../services/services';
import { Services } from '../../config/services.config';
import {
    CreateEventPartnerLocationService,
    DeleteEventPartnerLocationService,
    GetEventPartnerLocationServices,
    GetEventPartnerLocationServicesByLocationId,
    GetEventPartnerLocationServiceStatuses,
} from '../../services/locations';
import { Eye } from 'lucide-react';

export interface ManageEventPartnersProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
    isEventComplete: boolean;
}

export const ManageEventPartners: React.FC<ManageEventPartnersProps> = (props) => {
    const [isEventPartnerLocationDataLoaded, setIsEventPartnerLocationDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerLocations, setEventPartnerLocations] = React.useState<DisplayEventPartnerLocationData[]>([]);
    const [eventPartnerLocationServices, setEventPartnerLocationServices] = React.useState<
        DisplayEventPartnerLocationServiceData[]
    >([]);
    const [isPartnerLocationServicesDataLoaded, setIsPartnerLocationServicesDataLoaded] =
        React.useState<boolean>(false);
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);
    const [serviceStatusList, setServiceStatusList] = React.useState<EventPartnerLocationServiceStatusData[]>([]);
    const [partnerLocationId, setPartnerLocationId] = React.useState<string>(Guid.EMPTY);
    const [partnerLocationName, setPartnerLocationName] = React.useState<string>('');

    const getServiceTypes = useQuery({
        queryKey: GetServiceTypes().key,
        queryFn: GetServiceTypes().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventPartnerLocationServiceStatuses = useQuery({
        queryKey: GetEventPartnerLocationServiceStatuses().key,
        queryFn: GetEventPartnerLocationServiceStatuses().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventPartnerLocationServices = useQuery({
        queryKey: GetEventPartnerLocationServices({ eventId: props.eventId }).key,
        queryFn: GetEventPartnerLocationServices({ eventId: props.eventId }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getEventPartnerLocationServicesByLocationId = useMutation({
        mutationKey: GetEventPartnerLocationServicesByLocationId().key,
        mutationFn: GetEventPartnerLocationServicesByLocationId().serviceAsync,
    });

    const createEventPartnerLocationService = useMutation({
        mutationKey: CreateEventPartnerLocationService().key,
        mutationFn: CreateEventPartnerLocationService().service,
    });

    const deleteEventPartnerLocationService = useMutation({
        mutationKey: DeleteEventPartnerLocationService().key,
        mutationFn: DeleteEventPartnerLocationService().service,
    });

    React.useEffect(() => {
        getServiceTypes.refetch().then((serviceTypesRes) => {
            setServiceTypeList(serviceTypesRes.data?.data || []);
            getEventPartnerLocationServiceStatuses.refetch().then((res) => {
                setServiceStatusList(res.data?.data || []);
            });
        });

        if (props.isUserLoaded && props.eventId && props.eventId !== Guid.EMPTY) {
            getEventPartnerLocationServices.refetch().then((res) => {
                setEventPartnerLocations(res.data?.data || []);
                setIsEventPartnerLocationDataLoaded(true);
            });
        }
    }, [props.eventId, props.isUserLoaded]);

    function OnEventPartnerLocationsUpdated() {
        getEventPartnerLocationServices.refetch().then((res) => {
            setEventPartnerLocations(res.data?.data || []);
            setIsEventPartnerLocationDataLoaded(true);
        });
    }

    function handleViewPartnerServices(locationId: string, partnerLocName: string) {
        getEventPartnerLocationServicesByLocationId.mutateAsync({ eventId: props.eventId, locationId }).then((res) => {
            setPartnerLocationId(locationId);
            setPartnerLocationName(partnerLocName);
            setEventPartnerLocationServices(res.data);
            setIsPartnerLocationServicesDataLoaded(true);
        });
    }

    // This will handle Cancel button click event.
    function handleCancel(event: any) {
        event.preventDefault();
        setPartnerLocationId(Guid.EMPTY);
        setPartnerLocationName('');
        setIsPartnerLocationServicesDataLoaded(false);
    }

    //  This will handle the submit form event.
    function requestService(serviceTypeId: number, partnerLocName: string) {
        const body = new EventPartnerLocationServiceData();
        body.eventId = props.eventId;
        body.partnerLocationId = partnerLocationId;
        body.serviceTypeId = serviceTypeId;
        body.eventPartnerLocationServiceStatusId = Constants.EventPartnerLocationServiceStatusRequested;

        createEventPartnerLocationService.mutateAsync(body).then(() => {
            OnEventPartnerLocationsUpdated();
            handleViewPartnerServices(partnerLocationId, partnerLocName);
        });
    }

    function removeServiceRequest(serviceTypeId: number, partnerLocName: string) {
        deleteEventPartnerLocationService
            .mutateAsync({
                eventId: props.eventId,
                partnerLocationId,
                serviceTypeId,
            })
            .then(() => {
                OnEventPartnerLocationsUpdated();
                handleViewPartnerServices(partnerLocationId, partnerLocName);
            });
    }

    const partnerActionDropdownList = (partnerLocationId: string, partnerLocName: string) => (
        <Dropdown.Item onClick={() => handleViewPartnerServices(partnerLocationId, partnerLocName)}>
            <Eye aria-hidden='true' color='#96ba00' size={24} className='mr-2' />
            View partner services
        </Dropdown.Item>
    );

    const partnerServiceActionDropdownList = (serviceTypeId: number, partnerLocName: string) => (
        <Dropdown.Item onClick={() => requestService(serviceTypeId, partnerLocName)}>
            <Eye aria-hidden='true' color='#96ba00' size={24} className='mr-2' />
            Request partner service
        </Dropdown.Item>
    );

    const existingPartnerServiceActionDropdownList = (serviceTypeId: number, partnerLocName: string) => (
        <Dropdown.Item onClick={() => removeServiceRequest(serviceTypeId, partnerLocName)}>
            <Eye aria-hidden='true' color='#96ba00' size={24} className='mr-2' />
            Remove service request
        </Dropdown.Item>
    );

    function renderEventPartnerLocationsTable(eventPartnerLocations: DisplayEventPartnerLocationData[]) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby='tableLabel' width='100%'>
                    <thead>
                        <tr>
                            <th className='h5 py-4'>Partner Name</th>
                            <th className='h5 py-4'>Location</th>
                            <th className='h5 py-4'>Notes</th>
                            <th className='h5 py-4'>Requested Services</th>
                            <th className='h5 py-4'>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {eventPartnerLocations.map((eventPartnerLocation) => (
                            <tr key={eventPartnerLocation.partnerLocationId.toString()}>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerName}</td>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerLocationName}</td>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerLocationNotes}</td>
                                <td className='color-grey p-18 py-3'>{eventPartnerLocation.partnerServicesEngaged}</td>
                                <td className='color-grey p-18 py-3'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {partnerActionDropdownList(
                                                eventPartnerLocation.partnerLocationId,
                                                eventPartnerLocation.partnerLocationName,
                                            )}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    }

    function renderEventPartnerLocationServicesTable(
        services: DisplayEventPartnerLocationServiceData[],
        partnerLocationName: string,
    ) {
        const availableServices = services.filter(
            (s) => s.isAdvanceNoticeRequired === false || props.isEventComplete === false,
        );

        return (
            <div>
                <div className='d-flex align-items-center justify-content-between'>
                    <h4 className='fw-600 color-primary my-4'>
                        Services Available from Partner: {partnerLocationName}
                    </h4>
                </div>
                <table className='table table-striped' aria-labelledby='tableLabel' width='100%'>
                    <thead>
                        <tr>
                            <th>Service Type</th>
                            <th>Notes</th>
                            <th>Request Status</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {availableServices.map((service) => (
                            <tr key={service.serviceTypeId}>
                                <td>{getServiceType(serviceTypeList, service.serviceTypeId)}</td>
                                <td>{service.partnerLocationServicePublicNotes}</td>
                                <td>
                                    {getEventPartnerLocationServiceStatus(
                                        serviceStatusList,
                                        service.eventPartnerLocationServiceStatusId,
                                    )}
                                </td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {service.eventPartnerLocationServiceStatusId === 0
                                                ? partnerServiceActionDropdownList(
                                                      service.serviceTypeId,
                                                      partnerLocationName,
                                                  )
                                                : existingPartnerServiceActionDropdownList(
                                                      service.serviceTypeId,
                                                      partnerLocationName,
                                                  )}
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                <Button className='action' onClick={(e: any) => handleCancel(e)}>
                    Cancel
                </Button>
            </div>
        );
    }

    function renderPartnerInvite() {
        return (
            <div className='d-flex flex-column align-items-start'>
                <p className='font-size-h5'>
                    Sorry, there are no registered partners in your area. Invite local government or business to join
                    TrashMob.eco as a partner!
                </p>
                <Link className='btn btn-primary banner-button' to='/inviteapartner'>
                    Invite a partner
                </Link>
            </div>
        );
    }

    return (
        <Container className='p-4 bg-white rounded my-5'>
            <div className='d-flex align-items-center justify-content-between'>
                <h4 className='fw-600 color-primary my-4'>Available Partners ({eventPartnerLocations?.length})</h4>
            </div>
            {props.eventId === Guid.EMPTY && (
                <p>
                    {' '}
                    <em>Event must be created first.</em>
                </p>
            )}
            {!isEventPartnerLocationDataLoaded && props.eventId !== Guid.EMPTY && (
                <p>
                    <em>Loading...</em>
                </p>
            )}
            {isEventPartnerLocationDataLoaded && eventPartnerLocations.length === 0 ? renderPartnerInvite() : null}
            {isEventPartnerLocationDataLoaded && eventPartnerLocations.length !== 0
                ? renderEventPartnerLocationsTable(eventPartnerLocations)
                : null}
            {isPartnerLocationServicesDataLoaded && eventPartnerLocationServices
                ? renderEventPartnerLocationServicesTable(eventPartnerLocationServices, partnerLocationName)
                : null}
        </Container>
    );
};
