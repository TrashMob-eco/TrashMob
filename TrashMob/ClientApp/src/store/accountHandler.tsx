import * as msal from "@azure/msal-browser";
import { Guid } from "guid-typescript";
import { AgreeToPolicies } from "../components/ConfirmTosPopup";
import { CurrentPrivacyPolicyVersion } from "../components/PrivacyPolicy";
import { CurrentTermsOfServiceVersion } from "../components/TermsOfService";

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

            if (user.dateAgreedToPrivacyPolicy < CurrentPrivacyPolicyVersion.versionDate || user.dateAgreedToTermsOfService < CurrentTermsOfServiceVersion.versionDate || user.termsOfServiceVersion === "" || user.privacyPolicyVersion === "") {
                // todo: fix this so this popup works.
               // return AgreeToPolicies;
            }
        });

}

export function updateAgreements(tosVersion: string, privacyVersion: string) {

    const headers = new Headers();
    headers.append("Allow", 'GET');
    headers.append("Accept", 'application/json');
    headers.append("Content-Type", 'application/json');

    user.dateAgreedToPrivacyPolicy = new Date();
    user.dateAgreedToTermsOfService = new Date();
    user.termsOfServiceVersion = tosVersion;
    user.privacyPolicyVersion = privacyVersion;

    fetch('api/Users', {
        method: 'PUT',
        headers: headers,
        body: JSON.stringify(user)
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
