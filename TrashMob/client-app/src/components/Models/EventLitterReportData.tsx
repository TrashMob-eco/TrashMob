import LitterReportData from './LitterReportData';

export interface EventLitterReportData {
    eventId: string;
    litterReportId: string;
    notes: string;
    createdByUserId: string;
    createdDate: Date;
    lastUpdatedByUserId: string;
    lastUpdatedDate: Date;
}

export interface FullEventLitterReportData {
    eventId: string;
    litterReportId: string;
    litterReport: LitterReportData;
}

export default EventLitterReportData;
