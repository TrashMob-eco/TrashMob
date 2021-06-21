import { Guid } from "guid-typescript";

class UserNotificationPreferenceData {
    id: string = Guid.createEmpty().toString();
    userId: string = Guid.createEmpty().toString();
    userNotificationTypeId: number = 0;
    isOptedOut: boolean = false;
}

export default UserNotificationPreferenceData;