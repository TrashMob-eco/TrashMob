import { Guid } from 'guid-typescript';

class ContactRequestData {
    id: string = Guid.createEmpty().toString();

    email: string = '';

    name: string = '';

    message: string = '';

    recaptchaToken: string = '';
}

export default ContactRequestData;
