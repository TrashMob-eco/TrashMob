import { PartnerRequestForm, PartnerRequestMode } from '@/components/partner-requests/partner-request-form';

export const InviteAPartnerPage = () => {
    return <PartnerRequestForm mode={PartnerRequestMode.SEND} />;
};
