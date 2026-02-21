import { Badge } from '@/components/ui/badge';

export const PIPELINE_STAGES = [
    { value: 0, label: 'New' },
    { value: 1, label: 'Contacted' },
    { value: 2, label: 'Responded' },
    { value: 3, label: 'Interested' },
    { value: 4, label: 'Onboarding' },
    { value: 5, label: 'Active' },
    { value: 6, label: 'Declined' },
] as const;

export const PROSPECT_TYPES = ['Municipality', 'Nonprofit', 'HOA', 'CivicOrg', 'Other'] as const;

export const ACTIVITY_TYPES = ['EmailSent', 'EmailOpened', 'EmailClicked', 'Reply', 'StatusChange', 'Note'] as const;

export function getPipelineStageLabel(stage: number): string {
    return PIPELINE_STAGES.find((s) => s.value === stage)?.label ?? 'Unknown';
}

interface PipelineStageBadgeProps {
    stage: number;
}

export const PipelineStageBadge = ({ stage }: PipelineStageBadgeProps) => {
    const label = getPipelineStageLabel(stage);

    switch (stage) {
        case 0:
            return <Badge>{label}</Badge>;
        case 1:
            return <Badge variant='secondary'>{label}</Badge>;
        case 2:
            return <Badge variant='outline'>{label}</Badge>;
        case 3:
        case 4:
        case 5:
            return <Badge variant='success'>{label}</Badge>;
        case 6:
            return <Badge variant='destructive'>{label}</Badge>;
        default:
            return <Badge variant='outline'>{label}</Badge>;
    }
};
