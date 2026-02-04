import SocialMediaAccountTypeData from '../components/Models/SocialMediaAccountTypeData';
import { GetSocialMediaAccountTypes } from '../services/social-media';

export function getSocialMediaAccountType(
    socialMediaAccountTypeList: SocialMediaAccountTypeData[],
    socialMediaAccountTypeId: number,
): string {
    const socialMediaAccountType = socialMediaAccountTypeList.find((et) => et.id === socialMediaAccountTypeId);
    return socialMediaAccountType ? socialMediaAccountType.name : 'Unknown';
}

export async function getSocialMediaAccountTypeAsync(socialMediaAccountTypeId: number): Promise<string> {
    const socialMediaAccountTypeList = await getSocialMediaAccountTypes();
    return getSocialMediaAccountType(socialMediaAccountTypeList, socialMediaAccountTypeId);
}

async function getSocialMediaAccountTypes(): Promise<SocialMediaAccountTypeData[]> {
    const result = await GetSocialMediaAccountTypes()
        .service()
        .then((res) => res.data)
        .catch(() => []);
    return result;
}
