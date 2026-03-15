import { Guid } from 'guid-typescript';
import LitterImageData from './LitterImageData';

class LitterReportData {
    id: string = Guid.createEmpty().toString();

    name: string = '';

    description: string = '';

    litterReportStatusId: number = 1;

    /**
     * V2 API returns the image collection as `images` (camelCase from C# `Images`).
     * This is the canonical field that matches the v2 DTO JSON shape.
     */
    images: LitterImageData[] = [];

    /**
     * @deprecated Use `images` instead. Kept for backward compatibility with existing
     * component code that references `litterImages`. Components should migrate to `images`.
     */
    litterImages: LitterImageData[] = [];

    createdByUserId: string = '';

    createdDate: Date | null = null;

    lastUpdatedByUserId: string = '';

    lastUpdatedDate: Date | null = null;

    createdByUserName: string = '';
}

export default LitterReportData;
