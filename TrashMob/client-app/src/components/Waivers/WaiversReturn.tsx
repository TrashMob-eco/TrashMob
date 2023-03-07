import { FC, useEffect, useState } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { getApiConfig, getDefaultHeaders, msalClient, validateToken } from '../../store/AuthStore';
import UserData from '../Models/UserData';
import { CurrentTrashMobWaiverVersion } from './Waivers';

export interface WaiversReturnMatchParams {
    envelopeId: string;
}

export interface WaiversReturnProps extends RouteComponentProps<WaiversReturnMatchParams> {
    isUserLoaded: boolean;
    currentUser: UserData;
    onUserUpdated: any;
};

const WaiversReturn: FC<WaiversReturnProps> = (props) => {

    const [envelopeId, setEnvelopeId] = useState<string>("");
    const [isSigned, setIsSigned] = useState<boolean>(false);
    const [loadedEnvelopeId, setLoadedEnvelopeId] = useState<string | undefined>(props.match.params["envelopeId"]);

    useEffect(() => {
        if (!props.isUserLoaded || !props.currentUser) {
            return;
        }

        var envId: string | null | undefined = loadedEnvelopeId;

        if (!envId) {
            envId = sessionStorage.getItem("envelopeId");            
        }
        else {
            setEnvelopeId(envId);
            setLoadedEnvelopeId(envId);
        }

        if (envId) {
            const account = msalClient.getAllAccounts()[0];
            var apiConfig = getApiConfig();

            var request = {
                scopes: apiConfig.b2cScopes,
                account: account
            };

            msalClient.acquireTokenSilent(request).then(tokenResponse => {

                if (!validateToken(tokenResponse.idTokenClaims)) {
                    return;
                }

                const headers = getDefaultHeaders('GET');
                headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/docusign/' + props.currentUser.id + '/' + envelopeId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<string>)
                    .then(envelopeStatus => {
                        if (envelopeStatus === "completed") {
                            fetch('/api/users/' + props.currentUser.id, {
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
                                                props.onUserUpdated();
                                                // Todo: figure out how to make the history.push wait for the onUserUpdated to complete firing. Since the user is not updated before
                                                // the history redirect, the waiver is brought back up.
                                                // For now, just redirect to the home page.
                                                props.history.push("/");

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
    }, [props.isUserLoaded, props.currentUser, props.history, props.onUserUpdated, envelopeId, loadedEnvelopeId, props]);

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
