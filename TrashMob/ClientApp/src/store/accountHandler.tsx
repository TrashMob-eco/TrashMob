import * as msal from "@azure/msal-browser";
import { Guid } from "guid-typescript";

export const user: UserData = {
    id: Guid.createEmpty().toString(),
    dateAgreedToPrivacyPolicy: new Date(),
    dateAgreedToTermsOfService: new Date(),
    memberSince: new Date(),
    privacyPolicyVersion: "",
    tenantId: "",
    termsOfServiceVersion: "",
    uniqueId: "",
    userName: ""
};

export function verifyAccount(result: msal.AuthenticationResult) {

    const headers = new Headers();
    const bearer = `Bearer ${result.accessToken}`;
    headers.append("Authorization", bearer);
    headers.append("Allow", 'GET');
    headers.append("Accept", 'application/json');
    headers.append("Content-Type", 'application/json');

    user.uniqueId = result.uniqueId;
    user.tenantId = result.tenantId;
    user.userName = result.account?.username ?? "";

    fetch('api/Users', {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(user)
    })
        .then(response => response.json() as Promise<UserData> | null)
        .then(data => {
            if (data) {
                user.id = data.id;                
                user.dateAgreedToPrivacyPolicy = data.dateAgreedToPrivacyPolicy;
                user.dateAgreedToTermsOfService = data.dateAgreedToTermsOfService;
                user.memberSince = data.memberSince;
                user.privacyPolicyVersion = data.privacyPolicyVersion;
                user.termsOfServiceVersion = data.termsOfServiceVersion;
            }
        });
}

export class UserData {
    id: string = Guid.createEmpty().toString();
    uniqueId: string = "";
    tenantId: string = "";
    userName: string = "";
    dateAgreedToPrivacyPolicy: Date = new Date();
    privacyPolicyVersion: string = "";
    dateAgreedToTermsOfService: Date = new Date();
    termsOfServiceVersion: string = "";
    memberSince: Date = new Date();
}
