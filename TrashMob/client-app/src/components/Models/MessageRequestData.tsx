import { Guid } from "guid-typescript";

class MessageRequestData {
    id: string = Guid.createEmpty().toString();
    userName: string = "";
    message: string = "";
}

export default MessageRequestData;