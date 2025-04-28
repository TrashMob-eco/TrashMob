import { PartnerRequestForm, PartnerRequestMode } from '@/components/partner-requests/partner-request-form';

export const BecomeAPartnerPage = () => {
    return <PartnerRequestForm mode={PartnerRequestMode.REQUEST} />;
};
