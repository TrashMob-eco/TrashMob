import { Badge } from '@/components/ui/badge';

/**
 * Ten pipeline stages defined by Project 63 for the municipal sales pipeline.
 * Numeric values must match `PipelineStageEnum` on the backend.
 */
export const PIPELINE_STAGES = [
    { value: 0, label: 'Identified' },
    { value: 1, label: 'Researched' },
    { value: 2, label: 'Contacted' },
    { value: 3, label: 'Follow-up needed' },
    { value: 4, label: 'Responded' },
    { value: 5, label: 'Discovery in progress' },
    { value: 6, label: 'Meeting requested' },
    { value: 7, label: 'Meeting scheduled' },
    { value: 8, label: 'Not a fit' },
    { value: 9, label: 'Future follow-up' },
] as const;

/**
 * Constrained municipality types defined by Project 63 (`MunicipalityTypeEnum`).
 * Wire format is the enum name string (e.g. `"City"`). Anything from before the
 * enum migration that did not map is bucketed to `Other` with the original string
 * preserved in `typeRaw`.
 */
export const MUNICIPALITY_TYPES = ['City', 'Town', 'County', 'RegionalAgency', 'SpecialDistrict', 'Other'] as const;

/**
 * Sales-priority ranking defined by Project 63 (`ProspectPriorityEnum`).
 */
export const PROSPECT_PRIORITIES = [
    { value: 1, label: 'High' },
    { value: 2, label: 'Medium' },
    { value: 3, label: 'Low' },
] as const;

/**
 * Activity-type strings the Weekly Report categorizes (Project 63 Phase 2).
 * Match `ProspectActivityTypeEnum` names on the backend — case-insensitive on
 * read, but new activities should log with these exact strings.
 *
 * Legacy strings like `EmailSent` / `Reply` are still accepted on write (the
 * ActivityType column is free-form), but they won't be counted in the weekly
 * report categories.
 */
export const ACTIVITY_TYPES = [
    'Outreach',
    'FollowUp',
    'ResponseReceived',
    'MeetingRequested',
    'MeetingScheduled',
    'MeetingHeld',
    'Note',
] as const;

export function getPipelineStageLabel(stage: number): string {
    return PIPELINE_STAGES.find((s) => s.value === stage)?.label ?? 'Unknown';
}

export function getPriorityLabel(priority: number | null | undefined): string {
    if (priority == null) return '';
    return PROSPECT_PRIORITIES.find((p) => p.value === priority)?.label ?? '';
}

interface PipelineStageBadgeProps {
    stage: number;
}

export const PipelineStageBadge = ({ stage }: PipelineStageBadgeProps) => {
    const label = getPipelineStageLabel(stage);

    switch (stage) {
        case 0: // Identified
        case 1: // Researched
            return <Badge variant='secondary'>{label}</Badge>;
        case 2: // Contacted
        case 3: // Follow-up needed
            return <Badge variant='outline'>{label}</Badge>;
        case 4: // Responded
        case 5: // Discovery in progress
        case 6: // Meeting requested
        case 7: // Meeting scheduled
            return <Badge variant='success'>{label}</Badge>;
        case 8: // Not a fit
            return <Badge variant='destructive'>{label}</Badge>;
        case 9: // Future follow-up
            return <Badge variant='outline'>{label}</Badge>;
        default:
            return <Badge variant='outline'>{label}</Badge>;
    }
};

interface PriorityBadgeProps {
    priority: number | null | undefined;
}

/**
 * Renders a sales-priority pill. Renders nothing when priority is null.
 */
export const PriorityBadge = ({ priority }: PriorityBadgeProps) => {
    if (priority == null) return null;

    switch (priority) {
        case 1: // High
            return <Badge variant='destructive'>High</Badge>;
        case 2: // Medium
            return <Badge>Medium</Badge>;
        case 3: // Low
            return <Badge variant='outline'>Low</Badge>;
        default:
            return <Badge variant='outline'>{priority}</Badge>;
    }
};
