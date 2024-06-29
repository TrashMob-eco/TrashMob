import { FC, useEffect, useState } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import UserData from '../Models/UserData';
import { CurrentTrashMobWaiverVersion } from './Waivers';
import { useMutation, useQuery } from '@tanstack/react-query';
import { GetUsertDocusignByEnvelopeId } from '../../services/docusign';
import { Services } from '../../config/services.config';
import { GetUserById, UpdateUser } from '../../services/users';

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

    const getUserById = useQuery({
        queryKey: GetUserById({ userId: props.currentUser.id }).key,
        queryFn: GetUserById({ userId: props.currentUser.id }).service,
        staleTime: Services.CACHE.DISABLE,
        enabled: false
    })

    const getUsertDocusignByEnvelopeId = useMutation({
        mutationKey: GetUsertDocusignByEnvelopeId().key,
        mutationFn: GetUsertDocusignByEnvelopeId().service,
    })

    const updateUser = useMutation({
        mutationKey: UpdateUser().key,
        mutationFn: UpdateUser().service,
    })

    useEffect(() => {
        if (!props.isUserLoaded || !props.currentUser) return;

        let envId: string | null | undefined = loadedEnvelopeId;
        if (!envId) envId = sessionStorage.getItem("envelopeId");            
        else {
            setEnvelopeId(envId);
            setLoadedEnvelopeId(envId);
        }

        if (envId) {
            getUsertDocusignByEnvelopeId.mutateAsync({ userId: props.currentUser.id, envelopeId }).then((res) => {
                if (res.data !== "completed") setIsSigned(true);
                else {
                    getUserById.refetch().then(userRes => {
                        const user = userRes.data?.data;
                        if (user === null || user === undefined) return;
                        user.dateAgreedToTrashMobWaiver = new Date();
                        user.trashMobWaiverVersion = CurrentTrashMobWaiverVersion.versionId;
                        updateUser.mutateAsync(user).then(updatedUserRes => {
                            props.onUserUpdated();
                            // Todo: figure out how to make the history.push wait for the onUserUpdated to complete firing. Since the user is not updated before
                            // the history redirect, the waiver is brought back up.
                            // For now, just redirect to the home page.
                            props.history.push("/");
                            // if (!targetUrl || targetUrl === "") history.push("/");
                            // else {
                            //     sessionStorage.setItem("targetUrl", "")
                            //     history.push(targetUrl);
                            // }
                        })
                    })
                }
            })
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
