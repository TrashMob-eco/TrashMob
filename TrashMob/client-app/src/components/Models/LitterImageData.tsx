import { Guid } from 'guid-typescript';

class LitterImageData {
    id: string = Guid.createEmpty().toString();

    litterReportId: string = '';

    azureBlobURL: string = '';

    streetAddress: string = '';

    city: string = '';

    region: string = '';

    country: string = '';

    postalCode: string = '';

    latitude: number | null = null;

    longitude: number | null = null;

    // isCancelled: string = '';
}

export default LitterImageData;
