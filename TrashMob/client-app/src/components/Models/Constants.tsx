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
