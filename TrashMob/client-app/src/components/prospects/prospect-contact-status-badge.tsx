import { Badge } from '@/components/ui/badge';

export const PROSPECT_CONTACT_STATUSES = [
    { value: 0, label: 'Active' },
    { value: 1, label: 'Wrong Person' },
    { value: 2, label: 'No Response' },
    { value: 3, label: 'Left Org' },
    { value: 4, label: 'Right Person' },
] as const;

export function getProspectContactStatusLabel(status: number): string {
    return PROSPECT_CONTACT_STATUSES.find((s) => s.value === status)?.label ?? 'Unknown';
}

interface ProspectContactStatusBadgeProps {
    status: number;
}

export const ProspectContactStatusBadge = ({ status }: ProspectContactStatusBadgeProps) => {
    const label = getProspectContactStatusLabel(status);

    switch (status) {
        case 0: // Active
            return <Badge>{label}</Badge>;
        case 1: // WrongPerson
            return <Badge variant='outline'>{label}</Badge>;
        case 2: // NoResponse
            return <Badge className='bg-yellow-100 text-yellow-800'>{label}</Badge>;
        case 3: // LeftOrganization
            return <Badge variant='secondary'>{label}</Badge>;
        case 4: // RightPerson
            return <Badge variant='success'>{label}</Badge>;
        default:
            return <Badge variant='outline'>{label}</Badge>;
    }
};
