import { Badge } from '@/components/ui/badge';
import { createElement } from 'react';

export const CONTACT_TYPES = [
    { value: 1, label: 'Individual' },
    { value: 2, label: 'Organization' },
    { value: 3, label: 'Foundation' },
] as const;

export const NOTE_TYPES = [
    { value: 1, label: 'Call' },
    { value: 2, label: 'Email' },
    { value: 3, label: 'Meeting' },
    { value: 4, label: 'Letter' },
    { value: 5, label: 'Other' },
    { value: 6, label: 'Appeal' },
] as const;

export const DONATION_TYPES = [
    { value: 1, label: 'Cash' },
    { value: 2, label: 'Check' },
    { value: 3, label: 'Credit Card' },
    { value: 4, label: 'In-Kind' },
    { value: 5, label: 'Matching Gift' },
] as const;

export function getContactTypeLabel(type: number): string {
    return CONTACT_TYPES.find((t) => t.value === type)?.label ?? 'Unknown';
}

export function getNoteTypeLabel(type: number): string {
    return NOTE_TYPES.find((t) => t.value === type)?.label ?? 'Unknown';
}

export const RECURRING_FREQUENCIES = [
    { value: 1, label: 'One-Time' },
    { value: 2, label: 'Monthly' },
    { value: 3, label: 'Quarterly' },
    { value: 4, label: 'Annually' },
] as const;

export const PLEDGE_STATUSES = [
    { value: 1, label: 'Active' },
    { value: 2, label: 'Fulfilled' },
    { value: 3, label: 'Lapsed' },
    { value: 4, label: 'Cancelled' },
] as const;

export function getDonationTypeLabel(type: number): string {
    return DONATION_TYPES.find((t) => t.value === type)?.label ?? 'Unknown';
}

export function getRecurringFrequencyLabel(frequency: number): string {
    return RECURRING_FREQUENCIES.find((f) => f.value === frequency)?.label ?? 'Unknown';
}

export function getPledgeStatusLabel(status: number): string {
    return PLEDGE_STATUSES.find((s) => s.value === status)?.label ?? 'Unknown';
}

export const ContactTypeBadge = ({ type }: { type: number }) => {
    const label = getContactTypeLabel(type);
    switch (type) {
        case 1:
            return createElement(Badge, null, label);
        case 2:
            return createElement(Badge, { variant: 'secondary' }, label);
        case 3:
            return createElement(Badge, { variant: 'outline' }, label);
        default:
            return createElement(Badge, { variant: 'outline' }, label);
    }
};

export const NoteTypeBadge = ({ type }: { type: number }) => {
    const label = getNoteTypeLabel(type);
    return createElement(Badge, { variant: 'secondary' }, label);
};

export const DonationTypeBadge = ({ type }: { type: number }) => {
    const label = getDonationTypeLabel(type);
    return createElement(Badge, { variant: 'outline' }, label);
};

export const PledgeStatusBadge = ({ status }: { status: number }) => {
    const label = getPledgeStatusLabel(status);
    switch (status) {
        case 1:
            return createElement(Badge, { variant: 'success' }, label);
        case 2:
            return createElement(Badge, null, label);
        case 3:
            return createElement(Badge, { variant: 'destructive' }, label);
        case 4:
            return createElement(Badge, { variant: 'secondary' }, label);
        default:
            return createElement(Badge, { variant: 'outline' }, label);
    }
};

export const GRANT_STATUSES = [
    { value: 1, label: 'Prospect' },
    { value: 2, label: 'LOI Submitted' },
    { value: 3, label: 'Application Submitted' },
    { value: 4, label: 'Awarded' },
    { value: 5, label: 'Declined' },
    { value: 6, label: 'Reporting' },
    { value: 7, label: 'Renewal' },
    { value: 8, label: 'Closed' },
] as const;

export function getGrantStatusLabel(status: number): string {
    return GRANT_STATUSES.find((s) => s.value === status)?.label ?? 'Unknown';
}

export const GrantStatusBadge = ({ status }: { status: number }) => {
    const label = getGrantStatusLabel(status);
    switch (status) {
        case 1:
            return createElement(Badge, { variant: 'outline' }, label);
        case 2:
        case 3:
            return createElement(Badge, { variant: 'secondary' }, label);
        case 4:
            return createElement(Badge, { variant: 'success' }, label);
        case 5:
            return createElement(Badge, { variant: 'destructive' }, label);
        case 6:
            return createElement(Badge, null, label);
        case 7:
            return createElement(Badge, { variant: 'secondary' }, label);
        case 8:
            return createElement(Badge, { variant: 'outline' }, label);
        default:
            return createElement(Badge, { variant: 'outline' }, label);
    }
};
