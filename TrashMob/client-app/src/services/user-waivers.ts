import { ApiService } from '.';
import {
    WaiverVersionData,
    UserWaiverData,
    AcceptWaiverRequest,
    WaiverCheckResult,
} from '../components/Models/WaiverVersionData';
import { PaperWaiverUploadRequest, AttendeeWaiverStatus } from '../components/Models/PaperWaiverUpload';

/**
 * Gets waivers the current user needs to sign.
 */
export type GetRequiredWaivers_Params = { communityId?: string };
export type GetRequiredWaivers_Response = WaiverVersionData[];
export const GetRequiredWaivers = (params?: GetRequiredWaivers_Params) => ({
    key: ['/waivers/required', params],
    service: async () => {
        const url = params?.communityId ? `/waivers/required?communityId=${params.communityId}` : '/waivers/required';
        return ApiService('protected').fetchData<GetRequiredWaivers_Response>({
            url,
            method: 'get',
        });
    },
});

/**
 * Gets waivers required for a specific event.
 */
export type GetRequiredWaiversForEvent_Params = { eventId: string };
export type GetRequiredWaiversForEvent_Response = WaiverVersionData[];
export const GetRequiredWaiversForEvent = (params: GetRequiredWaiversForEvent_Params) => ({
    key: ['/waivers/required/event', params],
    service: async () =>
        ApiService('protected').fetchData<GetRequiredWaiversForEvent_Response>({
            url: `/waivers/required/event/${params.eventId}`,
            method: 'get',
        }),
});

/**
 * Gets all waivers signed by the current user.
 */
export type GetMyWaivers_Response = UserWaiverData[];
export const GetMyWaivers = () => ({
    key: ['/waivers/my'],
    service: async () =>
        ApiService('protected').fetchData<GetMyWaivers_Response>({
            url: '/waivers/my',
            method: 'get',
        }),
});

/**
 * Gets a specific user waiver by ID.
 */
export type GetUserWaiver_Params = { userWaiverId: string };
export type GetUserWaiver_Response = UserWaiverData;
export const GetUserWaiver = (params: GetUserWaiver_Params) => ({
    key: ['/waivers', params.userWaiverId],
    service: async () =>
        ApiService('protected').fetchData<GetUserWaiver_Response>({
            url: `/waivers/${params.userWaiverId}`,
            method: 'get',
        }),
});

/**
 * Accepts a waiver and creates a signed record.
 */
export type AcceptWaiver_Body = AcceptWaiverRequest;
export type AcceptWaiver_Response = UserWaiverData;
export const AcceptWaiver = () => ({
    key: ['/waivers/accept'],
    service: async (body: AcceptWaiver_Body) =>
        ApiService('protected').fetchData<AcceptWaiver_Response, AcceptWaiver_Body>({
            url: '/waivers/accept',
            method: 'post',
            data: body,
        }),
});

/**
 * Checks if the current user has valid waivers for an event.
 */
export type CheckWaiversForEvent_Params = { eventId: string };
export type CheckWaiversForEvent_Response = WaiverCheckResult;
export const CheckWaiversForEvent = (params: CheckWaiversForEvent_Params) => ({
    key: ['/waivers/check/event', params],
    service: async () =>
        ApiService('protected').fetchData<CheckWaiversForEvent_Response>({
            url: `/waivers/check/event/${params.eventId}`,
            method: 'get',
        }),
});

/**
 * Gets the download URL for a signed waiver PDF.
 */
export const getWaiverDownloadUrl = (userWaiverId: string): string => {
    return `/api/waivers/${userWaiverId}/download`;
};

/**
 * Uploads a paper waiver on behalf of a user.
 */
export type UploadPaperWaiver_Body = PaperWaiverUploadRequest;
export type UploadPaperWaiver_Response = UserWaiverData;
export const UploadPaperWaiver = () => ({
    key: ['/waivers/upload-paper'],
    service: async (body: UploadPaperWaiver_Body) => {
        const formData = new FormData();
        formData.append('FormFile', body.formFile);
        formData.append('UserId', body.userId);
        formData.append('WaiverVersionId', body.waiverVersionId);
        formData.append('SignerName', body.signerName);
        formData.append('DateSigned', body.dateSigned);
        if (body.eventId) {
            formData.append('EventId', body.eventId);
        }
        if (body.isMinor !== undefined) {
            formData.append('IsMinor', String(body.isMinor));
        }
        if (body.guardianName) {
            formData.append('GuardianName', body.guardianName);
        }
        if (body.guardianRelationship) {
            formData.append('GuardianRelationship', body.guardianRelationship);
        }
        return ApiService('protected').fetchData<UploadPaperWaiver_Response>({
            url: '/waivers/upload-paper',
            method: 'post',
            data: formData,
        });
    },
});

/**
 * Gets waiver status for all attendees of an event.
 */
export type GetEventAttendeeWaiverStatus_Params = { eventId: string };
export type GetEventAttendeeWaiverStatus_Response = AttendeeWaiverStatus[];
export const GetEventAttendeeWaiverStatus = (params: GetEventAttendeeWaiverStatus_Params) => ({
    key: ['/waivers/event', params.eventId, 'attendees'],
    service: async () =>
        ApiService('protected').fetchData<GetEventAttendeeWaiverStatus_Response>({
            url: `/waivers/event/${params.eventId}/attendees`,
            method: 'get',
        }),
});
