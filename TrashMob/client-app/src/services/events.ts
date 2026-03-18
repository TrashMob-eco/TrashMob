// eventtypes
// Events
// events
// eventsummaries
// EventAttendees

import { ApiService } from '.';
import { PagedResponse } from '../lib/api-errors';
import DisplayEventSummary from '../components/Models/DisplayEventSummary';
import EventAttendeeData from '../components/Models/EventAttendeeData';
import EventData from '../components/Models/EventData';
import EventSummaryData from '../components/Models/EventSummaryData';
import EventTypeData from '../components/Models/EventTypeData';
import UserData from '../components/Models/UserData';

export type GetEventTypes_Response = EventTypeData[];
export const GetEventTypes = () => ({
    key: ['/eventtypes'],
    service: async () =>
        ApiService('public').fetchData<GetEventTypes_Response>({
            url: '/v2/lookups/event-types',
            method: 'get',
        }),
});

export type GetFilteredEvents_Params = {
    city?: string;
    region?: string;
    country?: string;
    startDate?: string;
    endDate?: string;
    createdByUserId?: string;
};
export type GetFilteredEvents_Response = PagedResponse<EventData>;

export const GetFilteredEvents = (params: GetFilteredEvents_Params) => ({
    key: ['/events/filteredevents', params],
    service: () => {
        return ApiService('public').fetchData<GetFilteredEvents_Response>({
            url: '/v2/events/pagedfilteredevents',
            method: 'post',
            data: params,
        });
    },
});

export type GetAllEvents_Response = PagedResponse<EventData>;
export const GetAllEvents = () => ({
    key: ['/events'],
    service: async () =>
        ApiService('protected').fetchData<GetAllEvents_Response>({
            url: '/v2/events?pageSize=100',
            method: 'get',
        }),
});

export type GetAllActiveEvents_Response = EventData[];
export const GetAllActiveEvents = () => ({
    key: ['/Events/active'],
    service: async () =>
        ApiService('public').fetchData<GetAllActiveEvents_Response>({
            url: '/v2/events/active',
            method: 'get',
        }),
});

export type GetAllCompletedEvents_Response = EventData[];
export const GetAllCompletedEvents = () => ({
    key: ['/Events/completed'],
    service: async () =>
        ApiService('public').fetchData<GetAllCompletedEvents_Response>({
            url: '/v2/events/completed',
            method: 'get',
        }),
});

export type GetAllNotCancelledEvents_Response = EventData[];
export const GetAllNotCancelledEvents = () => ({
    key: ['/Events/notcanceled'],
    service: async () =>
        ApiService('public').fetchData<GetAllNotCancelledEvents_Response>({
            url: '/v2/events/notcanceled',
            method: 'get',
        }),
});

export type GetAllEventsBeingAttendedByUser_Params = { userId: string };
export type GetAllEventsBeingAttendedByUser_Response = EventData[];
export const GetAllEventsBeingAttendedByUser = (params: GetAllEventsBeingAttendedByUser_Params) => ({
    key: ['/events/eventsuserisattending/', params],
    service: async () =>
        ApiService('protected').fetchData<GetAllEventsBeingAttendedByUser_Response>({
            url: `/v2/events/eventsuserisattending/${params.userId}`,
            method: 'get',
        }),
});

export type GetEventsSummaries_Params = {
    country: string;
    region: string;
    city: string;
    postalCode: string;
};
export type GetEventsSummaries_Response = DisplayEventSummary[];
export const GetEventsSummaries = (params: GetEventsSummaries_Params) => ({
    key: ['/eventsummaries', params],
    service: async () =>
        ApiService('public').fetchData<GetEventsSummaries_Response>({
            url: `/v2/events/summaries?country=${params.country}&region=${params.region}&city=${params.city}&postalCode=${params.postalCode}`,
            method: 'get',
        }),
});

export type GetEventById_Params = { eventId: string };
export type GetEventById_Response = EventData;
export const GetEventById = (params: GetEventById_Params) => ({
    key: ['/Events/', params],
    service: async () =>
        ApiService('public').fetchData<GetEventById_Response>({
            url: `/v2/events/${params.eventId}`,
            method: 'get',
        }),
});

export type CreateEvent_Body = EventData;
export type CreateEvent_Response = EventData;
export const CreateEvent = () => ({
    key: ['/Events', 'create'],
    service: async (body: CreateEvent_Body) =>
        ApiService('protected').fetchData<CreateEvent_Response, CreateEvent_Body>({
            url: '/v2/events',
            method: 'post',
            data: body,
        }),
});

export type UpdateEvent_Body = EventData;
export type UpdateEvent_Response = unknown;
export const UpdateEvent = () => ({
    key: ['/Events', 'update'],
    service: async (body: UpdateEvent_Body) =>
        ApiService('protected').fetchData<UpdateEvent_Response, UpdateEvent_Body>({
            url: '/v2/events',
            method: 'put',
            data: body,
        }),
});

export type DeleteEvent_Body = { eventId: string; cancellationReason: string };
export type DeleteEvent_Response = EventData;
export const DeleteEvent = () => ({
    key: ['/Events', 'delete'],
    service: async (body: DeleteEvent_Body) =>
        ApiService('protected').fetchData<DeleteEvent_Response, DeleteEvent_Body>({
            url: '/v2/events',
            method: 'delete',
            data: body,
        }),
});

export type GetEventSummaryById_Params = { eventId: string };
export type GetEventSummaryById_Response = EventSummaryData;
export const GetEventSummaryById = (params: GetEventSummaryById_Params) => ({
    key: ['/eventsummaries/', params],
    service: async () =>
        ApiService('public').fetchData<GetEventSummaryById_Response>({
            url: `/v2/events/${params.eventId}/summary`,
            method: 'get',
        }),
});

export type CreateEventSummary_Body = EventSummaryData;
export type CreateEventSummary_Response = unknown;
export const CreateEventSummary = () => ({
    key: ['/eventsummaries', 'create'],
    service: async (body: CreateEventSummary_Body) =>
        ApiService('protected').fetchData<CreateEventSummary_Response, CreateEventSummary_Body>({
            url: `/v2/events/${body.eventId}/summary`,
            method: 'post',
            data: body,
        }),
});

export type UpdateEventSummary_Body = EventSummaryData;
export type UpdateEventSummary_Response = unknown;
export const UpdateEventSummary = () => ({
    key: ['/eventsummaries', 'update'],
    service: async (body: UpdateEventSummary_Body) =>
        ApiService('protected').fetchData<UpdateEventSummary_Response, UpdateEventSummary_Body>({
            url: `/v2/events/${body.eventId}/summary`,
            method: 'put',
            data: body,
        }),
});

export type GetEventAttendeeCount_Params = { eventId: string };
export type GetEventAttendeeCount_Response = { eventId: string; count: number };
export const GetEventAttendeeCount = (params: GetEventAttendeeCount_Params) => ({
    key: ['/eventattendees', params.eventId, 'count'],
    service: async () =>
        ApiService('public').fetchData<GetEventAttendeeCount_Response>({
            url: `/v2/events/${params.eventId}/attendees/count`,
            method: 'get',
        }),
});

export type GetEventAttendees_Params = { eventId: string };
export type GetEventAttendees_Response = PagedResponse<EventAttendeeData>;
export const GetEventAttendees = (params: GetEventAttendees_Params) => ({
    key: ['/eventattendees', params.eventId],
    service: async () =>
        ApiService('protected').fetchData<GetEventAttendees_Response>({
            url: `/v2/events/${params.eventId}/attendees?pageSize=100`,
            method: 'get',
        }),
});

export type AddEventAttendee_Body = EventAttendeeData;
export type AddEventAttendee_Response = unknown;
export const AddEventAttendee = () => ({
    key: ['/EventAttendees', 'add'],
    service: async (body: AddEventAttendee_Body) =>
        ApiService('protected').fetchData<AddEventAttendee_Response, AddEventAttendee_Body>({
            url: `/v2/events/${body.eventId}/attendees`,
            method: 'post',
            data: body,
        }),
});

export type DeleteEventAttendee_Params = { eventId: string; userId: string };
export type DeleteEventAttendee_Response = unknown;
export const DeleteEventAttendee = () => ({
    key: ['/EventAttendees/', 'delete'],
    service: async (params: DeleteEventAttendee_Params) =>
        ApiService('protected').fetchData<DeleteEventAttendee_Response>({
            url: `/v2/events/${params.eventId}/attendees/${params.userId}`,
            method: 'delete',
        }),
});

export type GetUserEvents_Params = { userId: string };
export type GetUserEvents_Response = EventData[];
export const GetUserEvents = (params: GetUserEvents_Params) => ({
    key: ['/events/userevents/', params, '/false'],
    service: async () =>
        ApiService('protected').fetchData<GetUserEvents_Response>({
            url: `/v2/events/userevents/${params.userId}/false`,
            method: 'get',
        }),
});

// Event Leads
export type GetEventLeads_Params = { eventId: string };
export type GetEventLeads_Response = UserData[];
export const GetEventLeads = (params: GetEventLeads_Params) => ({
    key: ['/eventattendees', params.eventId, 'leads'],
    service: async () =>
        ApiService('protected').fetchData<GetEventLeads_Response>({
            url: `/v2/events/${params.eventId}/attendees/leads`,
            method: 'get',
        }),
});

export type PromoteToLead_Params = { eventId: string; userId: string };
export type PromoteToLead_Response = EventAttendeeData;
export const PromoteToLead = () => ({
    key: ['/eventattendees', 'promote'],
    service: async (params: PromoteToLead_Params) =>
        ApiService('protected').fetchData<PromoteToLead_Response>({
            url: `/v2/events/${params.eventId}/attendees/${params.userId}/promote`,
            method: 'put',
        }),
});

export type DemoteFromLead_Params = { eventId: string; userId: string };
export type DemoteFromLead_Response = EventAttendeeData;
export const DemoteFromLead = () => ({
    key: ['/eventattendees', 'demote'],
    service: async (params: DemoteFromLead_Params) =>
        ApiService('protected').fetchData<DemoteFromLead_Response>({
            url: `/v2/events/${params.eventId}/attendees/${params.userId}/demote`,
            method: 'put',
        }),
});

/**
 * Verifies an attendee's waiver status at check-in (event leads/admins only).
 */
export type VerifyAttendeeWaiverStatus_Params = { eventId: string; userId: string };
export type VerifyAttendeeWaiverStatus_Response = { hasValidWaiver: boolean };
export const VerifyAttendeeWaiverStatus = (params: VerifyAttendeeWaiverStatus_Params) => ({
    key: ['/eventattendees', params.eventId, 'attendees', params.userId, 'waiver-status'],
    service: async () =>
        ApiService('protected').fetchData<VerifyAttendeeWaiverStatus_Response>({
            url: `/v2/events/${params.eventId}/attendees/${params.userId}/waiver-status`,
            method: 'get',
        }),
});
