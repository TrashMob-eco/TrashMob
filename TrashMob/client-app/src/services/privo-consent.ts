// PRIVO Consent API service

import { ApiService } from '.';
import ParentalConsentData from '../components/Models/ParentalConsentData';

// ============================================================================
// Adult Verification (Flow 1)
// ============================================================================

export type InitiateAdultVerification_Response = ParentalConsentData;
export const InitiateAdultVerification = () => ({
    key: ['/privo/verify'],
    service: async () =>
        ApiService('protected').fetchData<InitiateAdultVerification_Response>({
            url: '/v2/privo/verify',
            method: 'post',
        }),
});

// ============================================================================
// Parent-Initiated Child Consent (Flow 2)
// ============================================================================

export type InitiateChildConsent_Params = { dependentId: string };
export type InitiateChildConsent_Response = ParentalConsentData;
export const InitiateChildConsent = (params: InitiateChildConsent_Params) => ({
    key: ['/privo/consent/child', params.dependentId],
    service: async () =>
        ApiService('protected').fetchData<InitiateChildConsent_Response>({
            url: `/v2/privo/consent/child/${params.dependentId}`,
            method: 'post',
        }),
});

// ============================================================================
// Child-Initiated Consent (Flow 3)
// ============================================================================

export type InitiateChildInitiatedConsent_Body = {
    childFirstName: string;
    childEmail: string;
    childBirthDate: string;
    parentEmail: string;
};
export type InitiateChildInitiatedConsent_Response = ParentalConsentData;
export const InitiateChildInitiatedConsent = () => ({
    key: ['/privo/consent/child-initiated'],
    service: async (body: InitiateChildInitiatedConsent_Body) =>
        ApiService('public').fetchData<InitiateChildInitiatedConsent_Response, InitiateChildInitiatedConsent_Body>({
            url: '/v2/privo/consent/child-initiated',
            method: 'post',
            data: body,
        }),
});

// ============================================================================
// Verification Status
// ============================================================================

export type GetVerificationStatus_Response = ParentalConsentData;
export const GetVerificationStatus = () => ({
    key: ['/privo/status'],
    service: async () =>
        ApiService('protected').fetchData<GetVerificationStatus_Response>({
            url: '/v2/privo/status',
            method: 'get',
        }),
});

// ============================================================================
// Revoke Consent
// ============================================================================

export type RevokeConsent_Params = { consentId: string };
export const RevokeConsent = (params: RevokeConsent_Params) => ({
    key: ['/privo/consent', params.consentId, 'revoke'],
    service: async (reason: string) =>
        ApiService('protected').fetchData<void>({
            url: `/v2/privo/consent/${params.consentId}/revoke?reason=${encodeURIComponent(reason)}`,
            method: 'post',
        }),
});
