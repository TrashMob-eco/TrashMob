import { FC, useEffect, useState } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { CurrentTrashMobWaiverVersion } from './Waivers';

export interface WaiversReturnProps extends RouteComponentProps {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
};

const WaiversReturn: FC<WaiversReturnProps> = ({ currentUser, isUserLoaded, onUserUpdated, history }) => {

    const [isSigned, setIsSigned] = useState<boolean>(false);

    useEffect(() => {
        if (!isUserLoaded || !currentUser) {
            return;
        }

        var envelopeId = sessionStorage.getItem("envelopeId");

        // var targetUrl = sessionStorage.getItem("targetUrl");

        if (envelopeId) {

            const account = msalClient.getAllAccounts()[0];

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {

                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/docusign/' + currentUser.id + '/' + envelopeId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<string>)
                    .then(envelopeStatus => {
                        if (envelopeStatus === "completed") {
                            fetch('/api/users/' + currentUser.id, {
                                method: 'GET',
                                headers: headers,
                            })
                                .then(response => response.json() as Promise<UserData>)
                                .then(user => {
                                    if (user) {
                                        user.dateAgreedToTrashMobWaiver = new Date();
                                        user.trashMobWaiverVersion = CurrentTrashMobWaiverVersion.versionId;
                                        fetch('/api/Users/', {
                                            method: 'PUT',
                                            headers: headers,
                                            body: JSON.stringify(user)
                                        })
                                            .then(response => response.json() as Promise<UserData>)
                                            .then(_ => {
                                                onUserUpdated();
                                                // Todo: figure out how to make the history.push wait for the onUserUpdated to complete firing. Since the user is not updated before
                                                // the history redirect, the waiver is brought back up.
                                                // For now, just redirect to the home page.
                                                history.push("/");

                                            //    if (!targetUrl || targetUrl === "") {
                                            //        history.push("/");
                                            //    }
                                            //    else {
                                            //        sessionStorage.setItem("targetUrl", "")
                                            //        history.push(targetUrl);
                                            //    }
                                            })
                                    }
                                })
                        }
                        else {
                            setIsSigned(true);
                        }
                    })
            });
        }
        else {
            setIsSigned(true);
        }
    }, [isUserLoaded, currentUser, history, onUserUpdated]);

    const renderIncomplete = () => {
        return (
            <div>
                <h1>Waiver Signing Incomplete</h1>
                <p>
                    The waiver signing process did not complete successfully. Please try again.
                </p>
            </div>
        );
    }

    const contents = isSigned
        ? renderIncomplete()
        : <p><em>Loading...</em></p>;

    return (
        <div className="container-fluid card">
            {contents}
        </div>
    );
}

export default WaiversReturn;
