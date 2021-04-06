import * as msal from "@azure/msal-browser";
import { Guid } from "guid-typescript";
import { User } from "oidc-client";

export function verifyAccount(result: msal.AuthenticationResult) {

    const headers = new Headers();
    const bearer = `Bearer ${result.accessToken}`;
    headers.append("Authorization", bearer);
    headers.append("Allow", 'GET');
    headers.append("Accept", 'application/json');
    headers.append("Content-Type", 'application/json');

    var userData = new UserData();
    userData.uniqueId = result.uniqueId;
    userData.tenantId = result.tenantId;
    userData.userName = result.account?.username ?? "";

    fetch('api/Users', {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(userData)
    })
        .then(response => response.json() as Promise<UserData> | null);
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
