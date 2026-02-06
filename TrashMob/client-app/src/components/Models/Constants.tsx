export const RegexPhoneNumber = /^[+]?[(]?[0-9]{3}[)]?[-\s.]?[0-9]{3}[-\s.]?[0-9]{4,6}$/im;
export const RegexEmail =
    /^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w -\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i;
export const RegexUserName = '^[a-zA-Z0-9_]*$';
export const RegexWebsite = /^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w.-]+)+[\w\-._~:/?#[\]@!$&'()*+,;=.]+$/;

export const PartnerRequestStatusSent = 1;
export const PartnerRequestStatusApproved = 2;
export const PartnerRequestStatusDeclined = 3;
export const PartnerRequestStatusPending = 4;

export const EventPartnerLocationServiceStatusNone = 0;
export const EventPartnerLocationServiceStatusRequested = 1;
export const EventPartnerLocationServiceStatusAccepted = 2;
export const EventPartnerLocationServiceStatusDeclined = 3;

export const ServiceTypeHauling = 1;

export const PartnerStatusActive = 1;
export const PartnerStatusInactive = 2;

export const EventStatusActive = 1;
export const EventStatusInactive = 2;

export const PartnerTypeGovernment = 1;
export const PartnerTypeBusiness = 2;

// Character limits for form fields (must match backend EF Core constraints)
export const MAX_EVENT_NAME_LENGTH = 256;
export const MAX_EVENT_DESC_LENGTH = 2048;
export const MAX_TEAM_NAME_LENGTH = 200;
export const MAX_TEAM_DESC_LENGTH = 2048;
export const MAX_LITTER_REPORT_NAME_LENGTH = 64;
export const MAX_LITTER_REPORT_DESC_LENGTH = 2048;
export const MAX_PARTNER_NAME_LENGTH = 128;
export const MAX_PARTNER_NOTES_LENGTH = 2048;
export const MAX_PARTNER_REQUEST_NOTES_LENGTH = 2048;
export const MAX_CONTACT_NAME_LENGTH = 64;
export const MAX_CONTACT_EMAIL_LENGTH = 64;
export const MAX_CONTACT_PHONE_LENGTH = 30;
export const MAX_CONTACT_NOTES_LENGTH = 2000;
export const MAX_MESSAGE_LENGTH = 2048;
export const MAX_USER_FEEDBACK_DESC_LENGTH = 4000;
