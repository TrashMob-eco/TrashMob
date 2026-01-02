import { Guid } from 'guid-typescript';
import LitterImageData from './LitterImageData';

class LitterReportData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    description: string = '';

    litterReportStatusId: number = 1;

    litterImages: LitterImageData[] = [];

    createdByUserId: string = '';

    createdDate: Date | null = null;

    lastUpdatedByUserId: string = '';

    lastUpdatedDate: Date | null = null;
}

export default LitterReportData;
