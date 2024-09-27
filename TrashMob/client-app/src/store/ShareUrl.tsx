import EventData from '../components/Models/EventData';

export function getTwitterUrl(eventData: EventData): string {
    const { host } = window.location;
    const eventUrl = `https://${host}/eventdetails/${eventData.id}`;
    const shareMessage = `Help clean up Planet Earth! Sign up for this TrashMob.eco event in ${
        eventData.city
    }, ${eventData.region} on ${new Date(eventData.eventDate).toLocaleDateString()}! via @TrashMobEco ${eventUrl}`;
    return `https://twitter.com/intent/tweet?text=${encodeURI(shareMessage)}&ref_src=twsrc%5Etfw`;
}

export function getFacebookUrl(eventId: string): string {
    const eventUrl = `https://www.trashmob.eco/eventdetails/${eventId}`;
    return `https://www.facebook.com/sharer/sharer.php?u=${encodeURI(eventUrl)}&amp;src=sdkpreparse`;
}
