import SocialMediaAccountTypeData from "../components/Models/SocialMediaAccountTypeData";
import { getDefaultHeaders } from "./AuthStore";

export function getSocialMediaAccountType(socialMediaAccountTypeList: SocialMediaAccountTypeData[], socialMediaAccountTypeId: any): string {
    if (socialMediaAccountTypeList === null || socialMediaAccountTypeList.length === 0) {
        socialMediaAccountTypeList = getSocialMediaAccountTypes();
    }

    var socialMediaAccountType = socialMediaAccountTypeList.find(et => et.id === socialMediaAccountTypeId)
    if (socialMediaAccountType)
        return socialMediaAccountType.name;
    return "Unknown";
}

function getSocialMediaAccountTypes() : SocialMediaAccountTypeData[] {
    const headers = getDefaultHeaders('GET');

    fetch('/api/socialMediaAccounttypes', {
        method: 'GET',
        headers: headers
    })
        .then(response => response.json() as Promise<Array<any>>)
        .then(data => {
            return data;
        });
    return [];
}
