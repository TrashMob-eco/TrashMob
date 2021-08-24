import MediaTypeData from "../components/Models/MediaTypeData";
import EventTypeData from "../components/Models/MediaTypeData";
import { getDefaultHeaders } from "./AuthStore";

export function getMediaType(mediaTypeList: MediaTypeData[], mediaTypeId: any): string {
    if (mediaTypeList === null || mediaTypeList.length === 0) {
        mediaTypeList = getMediaTypes();
    }

    var mediaType = mediaTypeList.find(et => et.id === mediaTypeId)
    if (mediaType)
        return mediaType.name;
    return "Unknown";
}

function getMediaTypes() : EventTypeData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/mediatypes', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
