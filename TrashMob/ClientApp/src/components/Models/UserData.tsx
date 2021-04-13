import { Guid } from "guid-typescript";

class UserData {
    id: string = Guid.createEmpty().toString();
    uniqueId: string = "";
    tenantId: string = "";
    userName: string = "";
    givenName: string = "";
    surname: string = "";
    city: string = "";
    region: string = "";
    country: string = "";
    postalCode: string = "";
    email: string = "";
    dateAgreedToPrivacyPolicy: Date = new Date();
    privacyPolicyVersion: string = "";
    dateAgreedToTermsOfService: Date = new Date();
    termsOfServiceVersion: string = "";
    memberSince: Date = new Date();
}

export default UserData;