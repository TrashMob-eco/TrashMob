// Sponsors API service

import { ApiService } from '.';
import SponsorData from '../components/Models/SponsorData';

// ============================================================================
// Sponsor Operations
// ============================================================================

export type GetSponsors_Params = { partnerId: string };
export type GetSponsors_Response = SponsorData[];
export const GetSponsors = (params: GetSponsors_Params) => ({
    key: ['/communities/', params.partnerId, '/sponsors'],
    service: async () =>
        ApiService('protected').fetchData<GetSponsors_Response>({
            url: `/communities/${params.partnerId}/sponsors`,
            method: 'get',
        }),
});

export type GetSponsor_Params = { partnerId: string; sponsorId: string };
export type GetSponsor_Response = SponsorData;
export const GetSponsor = (params: GetSponsor_Params) => ({
    key: ['/communities/', params.partnerId, '/sponsors/', params.sponsorId],
    service: async () =>
        ApiService('protected').fetchData<GetSponsor_Response>({
            url: `/communities/${params.partnerId}/sponsors/${params.sponsorId}`,
            method: 'get',
        }),
});

export type CreateSponsor_Params = { partnerId: string };
export type CreateSponsor_Body = SponsorData;
export type CreateSponsor_Response = SponsorData;
export const CreateSponsor = () => ({
    key: ['/communities/sponsors', 'create'],
    service: async (params: CreateSponsor_Params, body: CreateSponsor_Body) =>
        ApiService('protected').fetchData<CreateSponsor_Response, CreateSponsor_Body>({
            url: `/communities/${params.partnerId}/sponsors`,
            method: 'post',
            data: body,
        }),
});

export type UpdateSponsor_Params = { partnerId: string; sponsorId: string };
export type UpdateSponsor_Body = SponsorData;
export type UpdateSponsor_Response = SponsorData;
export const UpdateSponsor = () => ({
    key: ['/communities/sponsors', 'update'],
    service: async (params: UpdateSponsor_Params, body: UpdateSponsor_Body) =>
        ApiService('protected').fetchData<UpdateSponsor_Response, UpdateSponsor_Body>({
            url: `/communities/${params.partnerId}/sponsors/${params.sponsorId}`,
            method: 'put',
            data: body,
        }),
});

export type DeactivateSponsor_Params = { partnerId: string; sponsorId: string };
export type DeactivateSponsor_Response = void;
export const DeactivateSponsor = () => ({
    key: ['/communities/sponsors', 'deactivate'],
    service: async (params: DeactivateSponsor_Params) =>
        ApiService('protected').fetchData<DeactivateSponsor_Response>({
            url: `/communities/${params.partnerId}/sponsors/${params.sponsorId}`,
            method: 'delete',
        }),
});
