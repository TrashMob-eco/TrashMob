/**
 * A utility & helper function for event
 */

import EventData from '@/components/Models/EventData';
import moment from 'moment';

export const isCompletedEvent = (event: EventData) => {
    return moment(event.eventDate).add(event.durationHours, 'hours').add(event.durationMinutes, 'minutes').isBefore();
};
