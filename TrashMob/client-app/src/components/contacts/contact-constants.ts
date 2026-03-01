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

export function getDonationTypeLabel(type: number): string {
    return DONATION_TYPES.find((t) => t.value === type)?.label ?? 'Unknown';
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
