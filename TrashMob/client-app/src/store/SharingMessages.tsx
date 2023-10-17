import EventData from "../components/Models/EventData"

export function getEventShareMessage(eventToShare: EventData, currUserId: string) {

    const eventDate = new Date(eventToShare.eventDate).toLocaleDateString("en-us", { year: "numeric", month: "2-digit", day: "2-digit" })
    const eventTime = new Date(eventToShare.eventDate).toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' })

    if (currUserId === eventToShare.createdByUserId) {
        return `Join my next {{TrashMob}} event on ${eventDate} at ${eventTime} in ${eventToShare.city}.\n` +
            `Sign up using the link for more details! Help me clean up ${eventToShare.city}!`
    }
    else {
        return `Join me at this {{TrashMob}} event on ${eventDate} at ${eventTime} in ${eventToShare.city}.\n` +
            `Sign up using the link for more details! Help me clean up ${eventToShare.city}!`
    }
}


export function getCancellationMessage(eventToShare: any, cancellationReason: string) {

    var eventDate = new Date(eventToShare.eventDate).toLocaleDateString("en-us", { year: "numeric", month: "2-digit", day: "2-digit" })

    var message = `Sorry everyone, we had to cancel our {{TrashMob}} event ${eventToShare.name} in #${eventToShare.city} on ${eventDate}. ${cancellationReason}.\n` +
        `Sign up using the link to get notified the next time we are having an event. Help us clean up the planet!`

    return message
}

export function getEventSummaryMessage(city: string, numAttendees: number, numBags: number) {
    var message = `We just finished a {{TrashMob}} event in ${city}. ${numAttendees} attendees picked up ${numBags} bags of #litter. ` +
        `Sign up using the link to get notified the next time we are having an event. Help us clean up the planet!`

    return message
}

export function getEventDetailsMessage(eventDate: Date, city: any, createdById: string, currUserId: string) {

    const eventDateFormatted = eventDate.toLocaleDateString("en-us", { year: "numeric", month: "2-digit", day: "2-digit" })
    const eventTime = eventDate.toLocaleTimeString("en-us", { hour12: true, hour: 'numeric', minute: '2-digit' })

    if (createdById === currUserId) {
        return `Join my next {{TrashMob}} event on ${eventDateFormatted} at ${eventTime} in ${city}.\n` +
            `Sign up using the link for more details! Help me clean up ${city}!`
    }
    else {
        return `Join me at this {{TrashMob}} event on ${eventDateFormatted} at ${eventTime} in ${city}.\n` +
            `Sign up using the link for more details! Help me clean up ${city}!`
    }
}

export const InvitationMessage = 'Interested in cleaning up the planet? Check out {{TrashMob}}! ' +
    'It\'s free and helps individuals and local orgs to connect with like-minded people to clean up their communities. Get started today by signing up using the link! '