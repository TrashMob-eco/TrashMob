import * as React from 'react'
import { apiConfig, getDefaultHeaders, msalClient } from '../../store/AuthStore';
import EventMediaData from '../Models/EventMediaData';
import MediaTypeData from '../Models/MediaTypeData';
import UserData from '../Models/UserData';

export interface ManageEventMediaProps {
    eventId: string;
    isUserLoaded: boolean;
    currentUser: UserData;
}

export const ManageEventMedia: React.FC<ManageEventMediaProps> = (props) => {
    const [eventMediaUrl, setEventMediaUrl] = React.useState<string>("");
    const [eventMediaTypeId, setEventMediaTypeId] = React.useState<number>(0);
    const [eventMediaUsageTypeId, setEventMediaUsageTypeId] = React.useState<number>(0);
    const [mediaTypeList, setMediaTypeList] = React.useState<MediaTypeData[]>([]);

    React.useEffect(() => {
        const headers = getDefaultHeaders('GET');

        fetch('/api/mediatypes', {
            method: 'GET',
            headers: headers,
        })
            .then(response => response.json() as Promise<Array<any>>)
            .then(data => {
                setMediaTypeList(data);
            }).then(() => {

                fetch('/api/eventmedias/' + props.eventId, {
                    method: 'GET',
                    headers: headers,
                })
                    .then(response => response.json() as Promise<Array<EventMediaData>>)
                    .then(data => {
                        if (data && data.length > 0) {
                            setEventMediaUrl(data[0].mediaUrl);
                            setEventMediaTypeId(data[0].mediaTypeId);
                            setEventMediaUsageTypeId(data[0].mediaUsageTypeId);
                        }
                    });
            });
    })

    // This will handle the submit form event.  
    function handleSave(event: any) {
        event.preventDefault();

        // PUT request for Edit Event.  
        const account = msalClient.getAllAccounts()[0];

        var request = {
            scopes: apiConfig.b2cScopes,
            account: account
        };

        return msalClient.acquireTokenSilent(request).then(tokenResponse => {
            const headers = getDefaultHeaders('POST');
            headers.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

            var eventMediaData = new EventMediaData();
            eventMediaData.mediaUrl = eventMediaUrl ?? "";
            eventMediaData.eventId = props.eventId;
            eventMediaData.createdByUserId = props.currentUser.id;
            eventMediaData.mediaTypeId = eventMediaTypeId ?? 0;
            eventMediaData.mediaUsageTypeId = eventMediaUsageTypeId ?? 0;

            var evtmediadatas: EventMediaData[] = [];
            evtmediadatas.push(eventMediaData);
            var evtmediadata = JSON.stringify(evtmediadatas);

            if (eventMediaData.mediaUrl !== "") {
                const headers2 = getDefaultHeaders('PUT');
                headers2.append('Authorization', 'BEARER ' + tokenResponse.accessToken);

                fetch('/api/eventmedias/' + props.currentUser.id, {
                    method: 'PUT',
                    headers: headers2,
                    body: evtmediadata,
                })
            }
            else {
                Promise.resolve()
            }
        })
    }

    function selectMediaType(val: string) {
        setEventMediaTypeId(parseInt(val));
        // Todo, change this default
        setEventMediaUsageTypeId(1);
    }

    function handleEventMediaUrlChanged(val: string) {
        setEventMediaUrl(val);
    }

    return (
        <>
            <div>
                <p><em>Feature under construction</em></p>
            </div>
        </>
    );
}
