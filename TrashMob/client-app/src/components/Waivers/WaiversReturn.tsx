import { FC, useEffect, useMemo } from 'react';
import { RouteComponentProps, useLocation } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { CurrentTrashMobWaiverVersion } from './Waivers';

export interface WaiversReturnProps extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
};

const WaiversReturn: FC<WaiversReturnProps> = ({ currentUser, isUserLoaded, onUserUpdated, history }) => {

    useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        var envelopeId = sessionStorage.getItem("envelopeId");
        var targetUrl = sessionStorage.getItem("targetUrl");

        if (envelopeId) {

            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {

                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/docusign/' + envelopeId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<string>)
                    .then(envelopeStatus => {
                        if (envelopeStatus === "signing_complete") {
                            fetch('/api/users/' + currentUser.id, {
                                method: 'GET',
                                headers: headers,
                            })
                                .then(response => response.json() as Promise<UserData>)
                                .then(user => {
                                    if (user) {
                                        user.dateAgreedToTrashMobWaiver = new Date();
                                        user.termsOfServiceVersion = CurrentTrashMobWaiverVersion.versionId;
                                        fetch('/api/Users/', {
                                            method: 'PUT',
                                            headers: headers,
                                            body: JSON.stringify(user)
                                        })
                                            .then(response => response.json() as Promise<UserData>)
                                            .then(_ => {
                                                onUserUpdated();
                                                if (!targetUrl || targetUrl === "") {
                                                    history.push("/");
                                                }
                                                else {
                                                    sessionStorage.setItem("targetUrl", "")
                                                    history.push(targetUrl);
                                                }
                                            })
                                    }
                                })
                        }
                })
            });
        }
    }, [isUserLoaded, currentUser]);

    return (
        <div className="container-fluid card">
            <h1>Waiver Signing Incomplete</h1>
            <p>
                The waiver signing process did not complete successfully. Please try again.
            </p>
        </div>
    );
}

export default WaiversReturn;
