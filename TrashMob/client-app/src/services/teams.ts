// Teams API service

import { ApiService } from '.';
import EventData from '../components/Models/EventData';
import TeamData from '../components/Models/TeamData';
import TeamEventData from '../components/Models/TeamEventData';
import TeamMemberData from '../components/Models/TeamMemberData';

// ============================================================================
// Team Operations
// ============================================================================

export type GetPublicTeams_Params = {
    latitude?: number;
    longitude?: number;
    radiusMiles?: number;
};
export type GetPublicTeams_Response = TeamData[];
export const GetPublicTeams = (params?: GetPublicTeams_Params) => ({
    key: ['/teams', params],
    service: async () => {
        const queryParams = new URLSearchParams();
        if (params?.latitude !== undefined) queryParams.append('latitude', params.latitude.toString());
        if (params?.longitude !== undefined) queryParams.append('longitude', params.longitude.toString());
        if (params?.radiusMiles !== undefined) queryParams.append('radiusMiles', params.radiusMiles.toString());
        const queryString = queryParams.toString();
        return ApiService('public').fetchData<GetPublicTeams_Response>({
            url: `/teams${queryString ? `?${queryString}` : ''}`,
            method: 'get',
        });
    },
});

export type GetTeamById_Params = { teamId: string };
export type GetTeamById_Response = TeamData;
export const GetTeamById = (params: GetTeamById_Params) => ({
    key: ['/teams/', params.teamId],
    service: async () =>
        ApiService('public').fetchData<GetTeamById_Response>({
            url: `/teams/${params.teamId}`,
            method: 'get',
        }),
});

export type GetMyTeams_Response = TeamData[];
export const GetMyTeams = () => ({
    key: ['/teams/my'],
    service: async () =>
        ApiService('protected').fetchData<GetMyTeams_Response>({
            url: '/teams/my',
            method: 'get',
        }),
});

export type GetTeamsILead_Response = TeamData[];
export const GetTeamsILead = () => ({
    key: ['/teams/my/leading'],
    service: async () =>
        ApiService('protected').fetchData<GetTeamsILead_Response>({
            url: '/teams/my/leading',
            method: 'get',
        }),
});

export type CheckTeamName_Params = { name: string; excludeTeamId?: string };
export type CheckTeamName_Response = boolean;
export const CheckTeamName = (params: CheckTeamName_Params) => ({
    key: ['/teams/check-name', params],
    service: async () => {
        const queryParams = new URLSearchParams({ name: params.name });
        if (params.excludeTeamId) queryParams.append('excludeTeamId', params.excludeTeamId);
        return ApiService('public').fetchData<CheckTeamName_Response>({
            url: `/teams/check-name?${queryParams.toString()}`,
            method: 'get',
        });
    },
});

export type CreateTeam_Body = TeamData;
export type CreateTeam_Response = TeamData;
export const CreateTeam = () => ({
    key: ['/teams', 'create'],
    service: async (body: CreateTeam_Body) =>
        ApiService('protected').fetchData<CreateTeam_Response, CreateTeam_Body>({
            url: '/teams',
            method: 'post',
            data: body,
        }),
});

export type UpdateTeam_Body = TeamData;
export type UpdateTeam_Response = TeamData;
export const UpdateTeam = () => ({
    key: ['/teams', 'update'],
    service: async (body: UpdateTeam_Body) =>
        ApiService('protected').fetchData<UpdateTeam_Response, UpdateTeam_Body>({
            url: `/teams/${body.id}`,
            method: 'put',
            data: body,
        }),
});

export type DeactivateTeam_Params = { teamId: string };
export type DeactivateTeam_Response = void;
export const DeactivateTeam = () => ({
    key: ['/teams', 'deactivate'],
    service: async (params: DeactivateTeam_Params) =>
        ApiService('protected').fetchData<DeactivateTeam_Response>({
            url: `/teams/${params.teamId}`,
            method: 'delete',
        }),
});

// ============================================================================
// Team Member Operations
// ============================================================================

export type GetTeamMembers_Params = { teamId: string };
export type GetTeamMembers_Response = TeamMemberData[];
export const GetTeamMembers = (params: GetTeamMembers_Params) => ({
    key: ['/teams/', params.teamId, '/members'],
    service: async () =>
        ApiService('public').fetchData<GetTeamMembers_Response>({
            url: `/teams/${params.teamId}/members`,
            method: 'get',
        }),
});

export type GetTeamLeads_Params = { teamId: string };
export type GetTeamLeads_Response = TeamMemberData[];
export const GetTeamLeads = (params: GetTeamLeads_Params) => ({
    key: ['/teams/', params.teamId, '/members/leads'],
    service: async () =>
        ApiService('public').fetchData<GetTeamLeads_Response>({
            url: `/teams/${params.teamId}/members/leads`,
            method: 'get',
        }),
});

export type JoinTeam_Params = { teamId: string };
export type JoinTeam_Response = TeamMemberData;
export const JoinTeam = () => ({
    key: ['/teams/members', 'join'],
    service: async (params: JoinTeam_Params) =>
        ApiService('protected').fetchData<JoinTeam_Response>({
            url: `/teams/${params.teamId}/members/join`,
            method: 'post',
        }),
});

export type AddTeamMember_Params = { teamId: string; userId: string };
export type AddTeamMember_Response = TeamMemberData;
export const AddTeamMember = () => ({
    key: ['/teams/members', 'add'],
    service: async (params: AddTeamMember_Params) =>
        ApiService('protected').fetchData<AddTeamMember_Response>({
            url: `/teams/${params.teamId}/members/${params.userId}`,
            method: 'post',
        }),
});

export type RemoveTeamMember_Params = { teamId: string; userId: string };
export type RemoveTeamMember_Response = void;
export const RemoveTeamMember = () => ({
    key: ['/teams/members', 'remove'],
    service: async (params: RemoveTeamMember_Params) =>
        ApiService('protected').fetchData<RemoveTeamMember_Response>({
            url: `/teams/${params.teamId}/members/${params.userId}`,
            method: 'delete',
        }),
});

export type PromoteToTeamLead_Params = { teamId: string; userId: string };
export type PromoteToTeamLead_Response = TeamMemberData;
export const PromoteToTeamLead = () => ({
    key: ['/teams/members', 'promote'],
    service: async (params: PromoteToTeamLead_Params) =>
        ApiService('protected').fetchData<PromoteToTeamLead_Response>({
            url: `/teams/${params.teamId}/members/${params.userId}/promote`,
            method: 'put',
        }),
});

export type DemoteFromTeamLead_Params = { teamId: string; userId: string };
export type DemoteFromTeamLead_Response = TeamMemberData;
export const DemoteFromTeamLead = () => ({
    key: ['/teams/members', 'demote'],
    service: async (params: DemoteFromTeamLead_Params) =>
        ApiService('protected').fetchData<DemoteFromTeamLead_Response>({
            url: `/teams/${params.teamId}/members/${params.userId}/demote`,
            method: 'put',
        }),
});

// ============================================================================
// Team Event Operations
// ============================================================================

export type GetTeamEvents_Params = { teamId: string };
export type GetTeamEvents_Response = EventData[];
export const GetTeamEvents = (params: GetTeamEvents_Params) => ({
    key: ['/teams/', params.teamId, '/events'],
    service: async () =>
        ApiService('public').fetchData<GetTeamEvents_Response>({
            url: `/teams/${params.teamId}/events`,
            method: 'get',
        }),
});

export type GetTeamUpcomingEvents_Params = { teamId: string };
export type GetTeamUpcomingEvents_Response = EventData[];
export const GetTeamUpcomingEvents = (params: GetTeamUpcomingEvents_Params) => ({
    key: ['/teams/', params.teamId, '/events/upcoming'],
    service: async () =>
        ApiService('public').fetchData<GetTeamUpcomingEvents_Response>({
            url: `/teams/${params.teamId}/events/upcoming`,
            method: 'get',
        }),
});

export type GetTeamPastEvents_Params = { teamId: string };
export type GetTeamPastEvents_Response = EventData[];
export const GetTeamPastEvents = (params: GetTeamPastEvents_Params) => ({
    key: ['/teams/', params.teamId, '/events/past'],
    service: async () =>
        ApiService('public').fetchData<GetTeamPastEvents_Response>({
            url: `/teams/${params.teamId}/events/past`,
            method: 'get',
        }),
});

export type LinkEventToTeam_Params = { teamId: string; eventId: string };
export type LinkEventToTeam_Response = TeamEventData;
export const LinkEventToTeam = () => ({
    key: ['/teams/events', 'link'],
    service: async (params: LinkEventToTeam_Params) =>
        ApiService('protected').fetchData<LinkEventToTeam_Response>({
            url: `/teams/${params.teamId}/events/${params.eventId}`,
            method: 'post',
        }),
});

export type UnlinkEventFromTeam_Params = { teamId: string; eventId: string };
export type UnlinkEventFromTeam_Response = void;
export const UnlinkEventFromTeam = () => ({
    key: ['/teams/events', 'unlink'],
    service: async (params: UnlinkEventFromTeam_Params) =>
        ApiService('protected').fetchData<UnlinkEventFromTeam_Response>({
            url: `/teams/${params.teamId}/events/${params.eventId}`,
            method: 'delete',
        }),
});
