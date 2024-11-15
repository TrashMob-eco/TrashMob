import * as React from 'react';
import { Dropdown } from 'react-bootstrap';
import { Guid } from 'guid-typescript';
import { CheckSquare, XSquare } from 'react-bootstrap-icons';
import { useMutation, useQuery } from '@tanstack/react-query';
import UserData from '../Models/UserData';
import * as Constants from '../Models/Constants';
import EventPartnerLocationServiceStatusData from '../Models/EventPartnerLocationServiceStatusData';
import { getEventPartnerLocationServiceStatus } from '../../store/eventPartnerLocationServiceStatusHelper';
import DisplayPartnerLocationEventData from '../Models/DisplayPartnerLocationEventServiceData';
import DisplayPartnerLocationEventServiceData from '../Models/DisplayPartnerLocationEventServiceData';
import ServiceTypeData from '../Models/ServiceTypeData';
import { getServiceType } from '../../store/serviceTypeHelper';
import { Services } from '../../config/services.config';
import {
    GetEventPartnerLocationServiceStatuses,
    GetPartnerLocationEventServicesByLocationId,
    GetPartnerLocationEventServicesByUserId,
    UpdateEventPartnerLocationServices,
} from '../../services/locations';
import { GetServiceTypes } from '../../services/services';

export interface PartnerLocationEventRequestsDataProps {
    partnerLocationId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const PartnerLocationEventRequests: React.FC<PartnerLocationEventRequestsDataProps> = (props) => {
    const [isPartnerLocationEventDataLoaded, setIsPartnerLocationEventDataLoaded] = React.useState<boolean>(false);
    const [eventPartnerStatusList, setEventPartnerStatusList] = React.useState<EventPartnerLocationServiceStatusData[]>(
        [],
    );
    const [serviceTypeList, setServiceTypeList] = React.useState<ServiceTypeData[]>([]);
    const [partnerLocationEvents, setPartnerLocationEvents] = React.useState<DisplayPartnerLocationEventData[]>([]);

    const getEventPartnerLocationServiceStatuses = useQuery({
        queryKey: GetEventPartnerLocationServiceStatuses().key,
        queryFn: GetEventPartnerLocationServiceStatuses().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getServiceTypes = useQuery({
        queryKey: GetServiceTypes().key,
        queryFn: GetServiceTypes().service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerLocationEventServicesByUserId = useQuery({
        queryKey: GetPartnerLocationEventServicesByUserId({
            userId: props.currentUser.id,
        }).key,
        queryFn: GetPartnerLocationEventServicesByUserId({
            userId: props.currentUser.id,
        }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false,
    });

    const getPartnerLocationEventServicesByLocationId = useMutation({
        mutationKey: GetPartnerLocationEventServicesByLocationId().key,
        mutationFn: GetPartnerLocationEventServicesByLocationId().service,
    });

    const updateEventPartnerLocationServices = useMutation({
        mutationKey: UpdateEventPartnerLocationServices().key,
        mutationFn: UpdateEventPartnerLocationServices().service,
    });

    React.useEffect(() => {
        if (props.isUserLoaded && props.partnerLocationId) {
            getEventPartnerLocationServiceStatuses
                .refetch()
                .then((locationServiceStatusesRes) => {
                    setEventPartnerStatusList(locationServiceStatusesRes.data?.data || []);
                    if (props.partnerLocationId !== Guid.EMPTY) {
                        getPartnerLocationEventServicesByLocationId
                            .mutateAsync({
                                locationId: props.partnerLocationId,
                            })
                            .then((res) => {
                                setPartnerLocationEvents(res.data || []);
                            });
                    } else {
                        getPartnerLocationEventServicesByUserId.refetch().then((res) => {
                            setPartnerLocationEvents(res.data?.data || []);
                        });
                    }
                })
                .then(() => {
                    getServiceTypes.refetch().then((serviceTypesRes) => {
                        setServiceTypeList(serviceTypesRes.data?.data || []);
                        setIsPartnerLocationEventDataLoaded(true);
                    });
                });
        }
    }, [props.partnerLocationId, props.isUserLoaded, props.currentUser.id]);

    // This will handle the submit form event.
    function handleRequestPartnerAssistance(
        eventId: string,
        partnerLocationId: string,
        serviceTypeId: number,
        eventPartnerLocationServiceStatusId: number,
    ) {
        updateEventPartnerLocationServices
            .mutateAsync({
                eventId,
                partnerLocationId,
                serviceTypeId,
                acceptDecline:
                    eventPartnerLocationServiceStatusId === Constants.EventPartnerLocationServiceStatusAccepted
                        ? 'accept'
                        : 'decline',
            })
            .then(() => {
                getPartnerLocationEventServicesByLocationId
                    .mutateAsync({ locationId: partnerLocationId })
                    .then((res) => {
                        setPartnerLocationEvents(res.data || []);
                        setIsPartnerLocationEventDataLoaded(true);
                    });
            });
    }

    const eventPartnerServiceRequestActionDropdownList = (partnerEvent: DisplayPartnerLocationEventServiceData) => (
        <>
            <Dropdown.Item
                onClick={() =>
                    handleRequestPartnerAssistance(
                        partnerEvent.eventId,
                        partnerEvent.partnerLocationId,
                        partnerEvent.serviceTypeId,
                        Constants.EventPartnerLocationServiceStatusAccepted,
                    )
                }
            >
                <CheckSquare />
                Accept Partner Assistance Request
            </Dropdown.Item>
            <Dropdown.Item
                onClick={() =>
                    handleRequestPartnerAssistance(
                        partnerEvent.eventId,
                        partnerEvent.partnerLocationId,
                        partnerEvent.serviceTypeId,
                        Constants.EventPartnerLocationServiceStatusDeclined,
                    )
                }
            >
                <XSquare />
                Decline Partner Assistance Request
            </Dropdown.Item>
        </>
    );

    function renderPartnerLocationEventServicesTable(
        partnerLocationEventServices: DisplayPartnerLocationEventServiceData[],
    ) {
        return (
            <div>
                <table className='table table-striped' aria-labelledby='tableLabel' width='100%'>
                    <thead>
                        <tr>
                            <th>Location Name</th>
                            <th>Event Name</th>
                            <th>Event Date</th>
                            <th>Event Address</th>
                            <th>Service Type</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {partnerLocationEventServices.map((partnerEvent) => (
                            <tr key={partnerEvent.eventId.toString()}>
                                <td>{partnerEvent.partnerLocationName}</td>
                                <td>{partnerEvent.eventName}</td>
                                <td>{new Date(partnerEvent.eventDate).toDateString()}</td>
                                <td>
                                    {partnerEvent.eventStreetAddress},{partnerEvent.eventCity}
                                </td>
                                <td>{getServiceType(serviceTypeList, partnerEvent.serviceTypeId)}</td>
                                <td>
                                    {getEventPartnerLocationServiceStatus(
                                        eventPartnerStatusList,
                                        partnerEvent.eventPartnerLocationStatusId,
                                    )}
                                </td>
                                <td className='btn py-0'>
                                    <Dropdown role='menuitem'>
                                        <Dropdown.Toggle id='share-toggle' variant='outline' className='h-100 border-0'>
                                            ...
                                        </Dropdown.Toggle>
                                        <Dropdown.Menu id='share-menu'>
                                            {eventPartnerServiceRequestActionDropdownList(partnerEvent)}
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

    return (
        <div className='bg-white p-5 shadow-sm rounded'>
            {!isPartnerLocationEventDataLoaded && props.partnerLocationId !== Guid.EMPTY && (
                <p>
                    <em>Loading...</em>
                </p>
            )}
            {isPartnerLocationEventDataLoaded && partnerLocationEvents.length === 0 ? (
                <p>
                    {' '}
                    <em>There are no event requests for this location.</em>
                </p>
            ) : null}
            {isPartnerLocationEventDataLoaded && partnerLocationEvents.length !== 0
                ? renderPartnerLocationEventServicesTable(partnerLocationEvents)
                : null}
        </div>
    );
};
