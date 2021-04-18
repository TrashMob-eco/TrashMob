import * as msal from "@azure/msal-browser";
import { Guid } from "guid-typescript";
import AgreeToPolicies from "../components/AgreeToPolicies";
import UserData from "../components/Models/UserData";
import { CurrentPrivacyPolicyVersion } from "../components/PrivacyPolicy";
import { CurrentTermsOfServiceVersion } from "../components/TermsOfService";
import { apiConfig, defaultHeaders, msalClient } from "./AuthStore";

const user: UserData = {
    id: Guid.createEmpty().toString(),
    dateAgreedToPrivacyPolicy: new Date(),
    dateAgreedToTermsOfService: new Date(),
    memberSince: new Date(),
    privacyPolicyVersion: "",
    termsOfServiceVersion: "",
    nameIdentifier: "",
    userName: "",
    city: "",
    country: "",
    email: "",
    givenName: "",
    postalCode: "",
    region: "",
    surname: "",
};

export function cacheUser(user: UserData) {
    localStorage.setItem('currentUser', JSON.stringify(user));
}

export function getUserFromCache() {
    var userString = localStorage.getItem('currentUser');

    if (userString) {
        return JSON.parse(userString);
    }

    return null;
}

export function verifyAccount(result: msal.AuthenticationResult) {

    const account = msalClient.getAllAccounts()[0];

    var request = {
        scopes: apiConfig.b2cScopes,
        account: account
    };

    msalClient.acquireTokenSilent(request).then(tokenResponse => {
        const headers = defaultHeaders('PUT');
        headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

        user.nameIdentifier = result.idTokenClaims["sub"];
        user.nameIdentifier = result.idTokenClaims["sub"];
        user.userName = result.account?.username ?? "";
        user.city = result.account?.idTokenClaims["city"] ?? "";
        user.region = result.account?.idTokenClaims["region"] ?? "";
        user.country = result.account?.idTokenClaims["country"] ?? "";
        user.postalCode = result.account?.idTokenClaims["postalCode"] ?? "";
        user.givenName = result.account?.idTokenClaims["given_name"] ?? "";
        user.surname = result.account?.idTokenClaims["family_name"] ?? "";
        user.email = result.account?.idTokenClaims["emails"][0] ?? "";

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
                    cacheUser(user)
                }

                //if (user.dateAgreedToPrivacyPolicy < CurrentPrivacyPolicyVersion.versionDate || user.dateAgreedToTermsOfService < CurrentTermsOfServiceVersion.versionDate || user.termsOfServiceVersion === "" || user.privacyPolicyVersion === "") {
                //    return AgreeToPolicies;
                // }
            });
    });
}

export function updateAgreements(tosVersion: string, privacyVersion: string) {

    const account = msalClient.getAllAccounts()[0];

    var request = {
        scopes: apiConfig.b2cScopes,
        account: account
    };

    msalClient.acquireTokenSilent(request).then(tokenResponse => {
        const headers = defaultHeaders('GET');
        headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

        fetch('api/Users/' + getUserFromCache().id, {
            method: 'GET',
            headers: headers,
            body: JSON.stringify(user)
        })
            .then(response => response.json() as Promise<UserData> | null)
            .then(user => {
                user.dateAgreedToPrivacyPolicy = new Date();
                user.dateAgreedToTermsOfService = new Date();
                user.termsOfServiceVersion = tosVersion;
                user.privacyPolicyVersion = privacyVersion;
                fetch('api/Users', {
                    method: 'PUT',
                    headers: headers,
                    body: JSON.stringify(user)
                })
                    .then(response => response.json() as Promise<UserData> | null)
                    .then(data => cacheUser(data ?? new UserData()));
            })
    })
}

export function clearUserCache() {
    user.id = Guid.createEmpty().toString();
    user.nameIdentifier = "";
    user.userName = "";
    user.dateAgreedToPrivacyPolicy = new Date();
    user.privacyPolicyVersion = "";
    user.dateAgreedToTermsOfService = new Date();
    user.termsOfServiceVersion = "";
    user.memberSince = new Date();
    cacheUser(user);
}
