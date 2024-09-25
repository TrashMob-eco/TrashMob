import { Guid } from 'guid-typescript';

class UserData {
    id: string = Guid.createEmpty().toString();

    userName: string = '';

    city: string = '';

    region: string = '';

    country: string = '';

    postalCode: string = '';

    email: string = '';

    dateAgreedToTrashMobWaiver: Date = new Date();

    trashMobWaiverVersion: string = '';

    memberSince: Date = new Date();

    latitude: number = 0;

    longitude: number = 0;

    prefersMetric: boolean = false;

    travelLimitForLocalEvents: number = 0;

    isSiteAdmin: boolean = false;
}

export default UserData;
