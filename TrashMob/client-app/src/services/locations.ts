// pickuplocations

import { ApiService } from '.';
import DisplayEventPartnerLocationData from '../components/Models/DisplayEventPartnerLocationData';
import DisplayEventPartnerLocationServiceData from '../components/Models/DisplayEventPartnerLocationServiceData';
import EventPartnerLocationServiceData from '../components/Models/EventPartnerLocationServiceData';
import EventPartnerLocationServiceStatusData from '../components/Models/EventPartnerLocationServiceStatusData';
import PartnerLocationData from '../components/Models/PartnerLocationData';
import PartnerLocationServiceData from '../components/Models/PartnerLocationServiceData';
import PickupLocationData from '../components/Models/PickupLocationData';
import DisplayPartnerLocationEventData from '../components/Models/DisplayPartnerLocationEventServiceData';

export type GetHaulingPartnerLocation_Params = { eventId: string };
export type GetHaulingPartnerLocation_Response = PartnerLocationData;
export const GetHaulingPartnerLocation = (params: GetHaulingPartnerLocation_Params) => ({
    key: ['/eventpartnerlocationservices/gethaulingpartnerlocation/', params],
    service: async () =>
        ApiService('protected').fetchData<GetHaulingPartnerLocation_Response>({
            url: `/eventpartnerlocationservices/gethaulingpartnerlocation/${params.eventId}`,
            method: 'get',
        }),
});

export type GetEventPartnerLocationServices_Params = { eventId: string };
export type GetEventPartnerLocationServices_Response = DisplayEventPartnerLocationData[];
export const GetEventPartnerLocationServices = (params: GetEventPartnerLocationServices_Params) => ({
    key: ['/eventpartnerlocationservices', params],
    service: async () =>
        ApiService('protected').fetchData<GetEventPartnerLocationServices_Response>({
            url: `/eventpartnerlocationservices/${params.eventId}`,
            method: 'get',
        }),
});

export type GetEventPartnerLocationServicesByLocationId_Params = {
    eventId: string;
    locationId: string;
};
export type GetEventPartnerLocationServicesByLocationId_Response = DisplayEventPartnerLocationServiceData[];
export const GetEventPartnerLocationServicesByLocationId = () => ({
    key: ['/eventpartnerlocationservices', 'by location id'],
    service: async (params: GetEventPartnerLocationServicesByLocationId_Params) =>
        ApiService('protected').fetchData<GetEventPartnerLocationServicesByLocationId_Response>({
            url: `/eventpartnerlocationservices/${params.eventId}/${params.locationId}`,
            method: 'get',
        }),
});

export type GetEventPartnerLocationServiceStatuses_Response = EventPartnerLocationServiceStatusData[];
export const GetEventPartnerLocationServiceStatuses = () => ({
    key: ['/eventpartnerlocationservicestatuses'],
    service: async () =>
        ApiService('public').fetchData<GetEventPartnerLocationServiceStatuses_Response>({
            url: '/eventpartnerlocationservicestatuses',
            method: 'get',
        }),
});

export type CreateEventPartnerLocationService_Body = EventPartnerLocationServiceData;
export type CreateEventPartnerLocationService_Response = unknown;
export const CreateEventPartnerLocationService = () => ({
    key: ['/eventpartnerlocationservices', 'create'],
    service: async (body: CreateEventPartnerLocationService_Body) =>
        ApiService('protected').fetchData<
            CreateEventPartnerLocationService_Response,
            CreateEventPartnerLocationService_Body
        >({ url: '/eventpartnerlocationservices', method: 'post', data: body }),
});

export type DeleteEventPartnerLocationService_Params = {
    eventId: string;
    partnerLocationId: string;
    serviceTypeId: number;
};
export type DeleteEventPartnerLocationService_Response = unknown;
export const DeleteEventPartnerLocationService = () => ({
    key: ['/eventpartnerlocationservices', 'delete'],
    service: async (params: DeleteEventPartnerLocationService_Params) =>
        ApiService('protected').fetchData<DeleteEventPartnerLocationService_Response>({
            url: `/eventpartnerlocationservices/${params.eventId}/${params.partnerLocationId}/${params.serviceTypeId}`,
            method: 'delete',
        }),
});

export type GetEventPickupLocations_Params = { eventId: string };
export type GetEventPickupLocations_Response = PickupLocationData[];
export const GetEventPickupLocations = (params: GetEventPickupLocations_Params) => ({
    key: ['/pickuplocations/getbyevent/', params],
    service: async () =>
        ApiService('protected').fetchData<GetEventPickupLocations_Response>({
            url: `/pickuplocations/getbyevent/${params.eventId}`,
            method: 'get',
        }),
});

export type GetEventPickupLocationsByUser_Params = { userId: string };
export type GetEventPickupLocationsByUser_Response = PickupLocationData[];
export const GetEventPickupLocationsByUser = (params: GetEventPickupLocationsByUser_Params) => ({
    key: ['/pickupLocations/getbyuser/', params],
    service: async () =>
        ApiService('protected').fetchData<GetEventPickupLocationsByUser_Response>({
            url: `/pickupLocations/getbyuser/${params.userId}`,
            method: 'get',
        }),
});

export type GetEventPickupLocationById_Params = { locationId: string };
export type GetEventPickupLocationById_Response = PickupLocationData;
export const GetEventPickupLocationById = () => ({
    key: ['/pickuplocations/', 'getByLocationId'],
    service: async (params: GetEventPickupLocationById_Params) =>
        ApiService('protected').fetchData<GetEventPickupLocationById_Response>({
            url: `/pickuplocations/${params.locationId}`,
            method: 'get',
        }),
});

export type DeleteEventPickupLocationById_Params = { locationId: string };
export type DeleteEventPickupLocationById_Response = unknown;
export const DeleteEventPickupLocationById = () => ({
    key: ['/pickuplocations/', 'delete'],
    service: async (params: DeleteEventPickupLocationById_Params) =>
        ApiService('protected').fetchData<DeleteEventPickupLocationById_Params>({
            url: `/pickuplocations/${params.locationId}`,
            method: 'delete',
        }),
});

export type SubmitEventPickupLocations_Params = { eventId: string };
export type SubmitEventPickupLocations_Response = unknown;
export const SubmitEventPickupLocations = () => ({
    key: ['/pickuplocations/submit/'],
    service: async (params: SubmitEventPickupLocations_Params) =>
        ApiService('protected').fetchData<SubmitEventPickupLocations_Response>({
            url: `/pickuplocations/submit/${params.eventId}`,
            method: 'post',
        }),
});

export type CreateEventPickupLocation_Body = PickupLocationData;
export type CreateEventPickupLocation_Response = unknown;
export const CreateEventPickupLocation = () => ({
    key: ['/pickuplocations', 'create'],
    service: async (body: CreateEventPickupLocation_Body) =>
        ApiService('protected').fetchData<CreateEventPickupLocation_Response, CreateEventPickupLocation_Body>({
            url: '/pickuplocations',
            method: 'post',
            data: body,
        }),
});

export type UpdateEventPickupLocation_Body = PickupLocationData;
export type UpdateEventPickupLocation_Response = unknown;
export const UpdateEventPickupLocation = () => ({
    key: ['/pickuplocations', 'update'],
    service: async (body: UpdateEventPickupLocation_Body) =>
        ApiService('protected').fetchData<UpdateEventPickupLocation_Response, UpdateEventPickupLocation_Body>({
            url: '/pickuplocations',
            method: 'put',
            data: body,
        }),
});

export type GetPartnerLocations_Params = { locationId: string };
export type GetPartnerLocations_Response = PartnerLocationData;
export const GetPartnerLocations = (params: GetPartnerLocations_Params) => ({
    key: ['/partnerlocations/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerLocations_Response>({
            url: `/partnerlocations/${params.locationId}`,
            method: 'get',
        }),
});

export type CreatePartnerLocations_Body = PartnerLocationData;
export type CreatePartnerLocations_Response = PartnerLocationData;
export const CreatePartnerLocations = () => ({
    key: ['/partnerlocations', 'create'],
    service: async (body: CreatePartnerLocations_Body) =>
        ApiService('protected').fetchData<CreatePartnerLocations_Response, CreatePartnerLocations_Body>({
            url: '/partnerlocations',
            method: 'post',
            data: body,
        }),
});

export type UpdatePartnerLocations_Body = PartnerLocationData;
export type UpdatePartnerLocations_Response = PartnerLocationData;
export const UpdatePartnerLocations = () => ({
    key: ['/partnerlocations', 'update'],
    service: async (body: UpdatePartnerLocations_Body) =>
        ApiService('protected').fetchData<UpdatePartnerLocations_Response, UpdatePartnerLocations_Body>({
            url: '/partnerlocations',
            method: 'put',
            data: body,
        }),
});

export type DeletePartnerLocation_Params = { locationId: string };
export type DeletePartnerLocation_Response = PartnerLocationData[];
export const DeletePartnerLocation = () => ({
    key: ['/partnerlocations', 'delete'],
    service: async (params: DeletePartnerLocation_Params) =>
        ApiService('protected').fetchData<DeletePartnerLocation_Response>({
            url: `/partnerlocations/${params.locationId}`,
            method: 'delete',
        }),
});

export type GetLocationsByPartner_Params = { partnerId: string };
export type GetLocationsByPartner_Response = PartnerLocationData[];
export const GetLocationsByPartner = (params: GetLocationsByPartner_Params) => ({
    key: ['/partnerlocations/getbypartner/', params],
    service: async () =>
        ApiService('protected').fetchData<GetLocationsByPartner_Response>({
            url: `/partnerlocations/getbypartner/${params.partnerId}`,
            method: 'get',
        }),
});

export type GetPartnerLocationsServicesByLocationId_Params = {
    locationId: string;
};
export type GetPartnerLocationsServicesByLocationId_Response = PartnerLocationServiceData[];
export const GetPartnerLocationsServicesByLocationId = (params: GetPartnerLocationsServicesByLocationId_Params) => ({
    key: ['/partnerlocationservices/getbypartnerlocation/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerLocationsServicesByLocationId_Response>({
            url: `/partnerlocationservices/getbypartnerlocation/${params.locationId}`,
            method: 'get',
        }),
});

export type GetPartnerLocationServiceByLocationIdAndServiceType_Params = {
    locationId: string;
    serviceTypeId: number;
};
export type GetPartnerLocationServiceByLocationIdAndServiceType_Response = PartnerLocationServiceData;
export const GetPartnerLocationServiceByLocationIdAndServiceType = () => ({
    key: ['/partnerlocationservices/', 'get by location id and service type id'],
    service: async (params: GetPartnerLocationServiceByLocationIdAndServiceType_Params) =>
        ApiService('protected').fetchData<GetPartnerLocationServiceByLocationIdAndServiceType_Response>({
            url: `/partnerlocationservices/${params.locationId}/${params.serviceTypeId}`,
            method: 'get',
        }),
});

export type DeletePartnerLocationServiceByLocationIdAndServiceType_Params = {
    locationId: string;
    serviceTypeId: number;
};
export type DeletePartnerLocationServiceByLocationIdAndServiceType_Response = unknown;
export const DeletePartnerLocationServiceByLocationIdAndServiceType = () => ({
    key: ['/partnerlocationservices/', 'delete by location id and service type id'],
    service: async (params: DeletePartnerLocationServiceByLocationIdAndServiceType_Params) =>
        ApiService('protected').fetchData<DeletePartnerLocationServiceByLocationIdAndServiceType_Response>({
            url: `/partnerlocationservices/${params.locationId}/${params.serviceTypeId}`,
            method: 'delete',
        }),
});

export type CreateLocationService_Body = PartnerLocationServiceData;
export type CreateLocationService_Response = PartnerLocationServiceData[];
export const CreateLocationService = () => ({
    key: ['/partnerlocationservices', 'create'],
    service: async (body: CreateLocationService_Body) =>
        ApiService('protected').fetchData<CreateLocationService_Response, CreateLocationService_Body>({
            url: '/partnerlocations',
            method: 'post',
            data: body,
        }),
});

export type UpdateLocationService_Body = PartnerLocationServiceData;
export type UpdateLocationService_Response = PartnerLocationServiceData[];
export const UpdateLocationService = () => ({
    key: ['/partnerlocationservices', 'update'],
    service: async (body: UpdateLocationService_Body) =>
        ApiService('protected').fetchData<UpdateLocationService_Response, UpdateLocationService_Body>({
            url: '/partnerlocations',
            method: 'put',
            data: body,
        }),
});

export type GetPartnerLocationEventServicesByLocationId_Params = {
    locationId: string;
};
export type GetPartnerLocationEventServicesByLocationId_Response = DisplayPartnerLocationEventData[];
export const GetPartnerLocationEventServicesByLocationId = () => ({
    key: ['/partnerlocationeventservices/', 'by locationId'],
    service: async (params: GetPartnerLocationEventServicesByLocationId_Params) =>
        ApiService('protected').fetchData<GetPartnerLocationEventServicesByLocationId_Response>({
            url: `/partnerlocationeventservices/${params.locationId}`,
            method: 'get',
        }),
});

export type GetPartnerLocationEventServicesByUserId_Params = { userId: string };
export type GetPartnerLocationEventServicesByUserId_Response = DisplayPartnerLocationEventData[];
export const GetPartnerLocationEventServicesByUserId = (params: GetPartnerLocationEventServicesByUserId_Params) => ({
    key: ['/partnerlocationeventservices/getbyuser/', params],
    service: async () =>
        ApiService('protected').fetchData<GetPartnerLocationEventServicesByUserId_Response>({
            url: `/partnerlocationeventservices/getbyuser/${params.userId}`,
            method: 'get',
        }),
});

export type UpdateEventPartnerLocationServices_Params = {
    acceptDecline: 'accept' | 'decline';
    eventId: string;
    partnerLocationId: string;
    serviceTypeId: number;
};
export type UpdateEventPartnerLocationServices_Response = unknown;
export const UpdateEventPartnerLocationServices = () => ({
    key: ['/eventpartnerlocationservices/', 'update'],
    service: async (params: UpdateEventPartnerLocationServices_Params) =>
        ApiService('protected').fetchData<UpdateEventPartnerLocationServices_Response>({
            url: `/eventpartnerlocationservices/${params.acceptDecline}/${params.eventId}/${params.partnerLocationId}/${params.serviceTypeId}`,
            method: 'put',
        }),
});

export type PickupLocationMarkAsPickedUp_Params = { locationId: string };
export type PickupLocationMarkAsPickedUp_Response = unknown;
export const PickupLocationMarkAsPickedUp = () => ({
    key: ['/pickuplocations/markpickedup/'],
    service: async (params: PickupLocationMarkAsPickedUp_Params) =>
        ApiService('protected').fetchData<PickupLocationMarkAsPickedUp_Response>({
            url: `/pickuplocations/markpickedup/${params.locationId}`,
            method: 'post',
        }),
});
