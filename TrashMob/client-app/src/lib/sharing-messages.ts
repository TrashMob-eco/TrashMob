import type TeamData from '@/components/Models/TeamData';
import type CommunityData from '@/components/Models/CommunityData';
import type EventData from '@/components/Models/EventData';
import type { NewsPostData } from '@/services/cms';
import type { ShareableContent } from '@/components/sharing';
import compact from 'lodash/compact';

/**
 * Creates ShareableContent for an event
 */
export function getEventShareableContent(event: EventData, currentUserId?: string): ShareableContent {
    const isCreator = currentUserId === event.createdByUserId;
    const eventDate = new Date(event.eventDate);
    const location = compact([event.city, event.region]).join(', ');

    return {
        type: 'event',
        title: event.name,
        description: isCreator
            ? `Join my next {{TrashMob}} event in ${event.city}. Sign up using the link for more details!`
            : `Join me at this {{TrashMob}} event in ${event.city}. Sign up using the link for more details!`,
        url: `${window.location.origin}/eventdetails/${event.id}`,
        location,
        date: eventDate,
    };
}

/**
 * Creates ShareableContent for a team
 */
export function getTeamShareableContent(team: TeamData): ShareableContent {
    const location = compact([team.city, team.region, team.country]).join(', ');

    return {
        type: 'team',
        title: team.name,
        description:
            `Check out ${team.name} on {{TrashMob}}! ` +
            (team.description
                ? team.description.substring(0, 100) + (team.description.length > 100 ? '...' : '')
                : 'Join this cleanup team and help make a difference in your community.'),
        url: `${window.location.origin}/teams/${team.id}`,
        imageUrl: team.logoUrl || undefined,
        location: location || undefined,
    };
}

/**
 * Creates ShareableContent for a community
 */
export function getCommunityShareableContent(community: CommunityData): ShareableContent {
    const location = compact([community.city, community.region, community.country]).join(', ');

    const truncatedNotes = community.publicNotes
        ? community.publicNotes.substring(0, 100) + (community.publicNotes.length > 100 ? '...' : '')
        : null;

    return {
        type: 'community',
        title: community.name,
        description:
            community.tagline ||
            truncatedNotes ||
            `Join the ${community.name} community on {{TrashMob}} and help keep your neighborhood clean!`,
        url: `${window.location.origin}/communities/${community.slug}`,
        imageUrl: community.logoUrl || community.bannerImageUrl || undefined,
        location: location || undefined,
    };
}

/**
 * Gets a shareable message for events with date/time details
 */
export function getEventShareMessage(event: EventData, currentUserId?: string): string {
    const isCreator = currentUserId === event.createdByUserId;
    const eventDate = new Date(event.eventDate).toLocaleDateString('en-us', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
    });
    const eventTime = new Date(event.eventDate).toLocaleTimeString('en-us', {
        hour12: true,
        hour: 'numeric',
        minute: '2-digit',
    });

    if (isCreator) {
        return (
            `Join my next {{TrashMob}} event on ${eventDate} at ${eventTime} in ${event.city}.\n` +
            `Sign up using the link for more details! Help me clean up ${event.city}!`
        );
    }
    return (
        `Join me at this {{TrashMob}} event on ${eventDate} at ${eventTime} in ${event.city}.\n` +
        `Sign up using the link for more details! Help me clean up ${event.city}!`
    );
}

/**
 * Gets a shareable message for teams
 */
export function getTeamShareMessage(team: TeamData): string {
    const location = compact([team.city, team.region]).join(', ');
    return (
        `Check out ${team.name} on {{TrashMob}}${location ? ` in ${location}` : ''}! ` +
        `Join this cleanup team and help make a difference in your community. Sign up using the link!`
    );
}

/**
 * Gets a shareable message for communities
 */
export function getCommunityShareMessage(community: CommunityData): string {
    const location = compact([community.city, community.region]).join(', ');
    return (
        `Check out the ${community.name} community on {{TrashMob}}${location ? ` in ${location}` : ''}! ` +
        `Join cleanup events and help keep our neighborhoods clean. Sign up using the link!`
    );
}

/**
 * Creates ShareableContent for event cancellation
 */
export function getEventCancellationShareableContent(event: EventData, cancellationReason: string): ShareableContent {
    const eventDate = new Date(event.eventDate).toLocaleDateString('en-us', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
    });

    return {
        type: 'event',
        title: `${event.name} - Cancelled`,
        description: `Sorry, the {{TrashMob}} event "${event.name}" in ${event.city} on ${eventDate} has been cancelled. ${cancellationReason}`,
        url: 'https://www.trashmob.eco',
        location: event.city,
    };
}

/**
 * Gets a shareable message for event cancellation
 */
export function getCancellationMessage(event: EventData, cancellationReason: string): string {
    const eventDate = new Date(event.eventDate).toLocaleDateString('en-us', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
    });

    return (
        `Sorry everyone, we had to cancel our {{TrashMob}} event ${event.name} in #${event.city} on ${eventDate}. ${cancellationReason}.\n` +
        'Sign up using the link to get notified the next time we are having an event. Help us clean up the planet!'
    );
}

/**
 * Creates ShareableContent for a news post
 */
export function getNewsPostShareableContent(post: NewsPostData, slug: string): ShareableContent {
    return {
        type: 'article',
        title: post.title,
        description: post.excerpt,
        url: `${window.location.origin}/news/${slug}`,
        imageUrl: post.coverImage?.data?.attributes?.url,
    };
}

/**
 * Gets a shareable message for a news post
 */
export function getNewsPostShareMessage(post: NewsPostData): string {
    const truncated = post.excerpt.length > 100 ? post.excerpt.substring(0, 100) + '...' : post.excerpt;
    return `Check out "${post.title}" on {{TrashMob}}! ${truncated}`;
}
