import SocialMediaAccountTypeData from '../components/Models/SocialMediaAccountTypeData';
import { GetSocialMediaAccountTypes } from '../services/social-media';

export function getSocialMediaAccountType(
    socialMediaAccountTypeList: SocialMediaAccountTypeData[],
    socialMediaAccountTypeId: any,
): string {
    const socialMediaAccountType = socialMediaAccountTypeList.find((et) => et.id === socialMediaAccountTypeId);
    return socialMediaAccountType ? socialMediaAccountType.name : 'Unknown';
}

export async function getSocialMediaAccountTypeAsync(socialMediaAccountTypeId: any): Promise<string> {
    const socialMediaAccountTypeList = await getSocialMediaAccountTypes();
    return getSocialMediaAccountType(socialMediaAccountTypeList, socialMediaAccountTypeId);
}

async function getSocialMediaAccountTypes(): Promise<SocialMediaAccountTypeData[]> {
    const result = await GetSocialMediaAccountTypes()
        .service()
        .then((res) => res.data)
        .catch((err) => []);
    return result;
}
