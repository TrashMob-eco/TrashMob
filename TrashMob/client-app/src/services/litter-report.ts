import LitterReportData from '@/components/Models/LitterReportData';
import { ApiService } from '.';

export type GetLitterReports_Response = LitterReportData[];
export const GetLitterReports = () => ({
    key: ['/litterreport', 'list'],
    service: async () =>
        ApiService('public').fetchData<GetLitterReports_Response>({
            url: '/litterreport',
            method: 'get',
        }),
});

export type GetLitterReport_Params = { litterReportId: string };
export type GetLitterReport_Response = LitterReportData;
export const GetLitterReport = (params: GetLitterReport_Params) => ({
    key: ['/litterreport', params.litterReportId],
    service: async () =>
        ApiService('public').fetchData<GetLitterReport_Response>({
            url: `/litterreport/${params.litterReportId}`,
            method: 'get',
        }),
});

export type GetNewLitterReports_Response = LitterReportData[];
export const GetNewLitterReports = () => ({
    key: ['/litterreport', 'new'],
    service: async () =>
        ApiService('public').fetchData<GetNewLitterReports_Response>({
            url: '/litterreport/new',
            method: 'get',
        }),
});

export type GetNotCancelledLitterReports_Response = LitterReportData[];
export const GetNotCancelledLitterReports = () => ({
    key: ['/litterreport', 'notcancelled'],
    service: async () =>
        ApiService('public').fetchData<GetNotCancelledLitterReports_Response>({
            url: '/litterreport/notcancelled',
            method: 'get',
        }),
});

export type GetUserLitterReports_Params = { userId: string };
export type GetUserLitterReports_Response = LitterReportData[];
export const GetUserLitterReports = (params: GetUserLitterReports_Params) => ({
    key: ['/litterreport', 'user', params.userId],
    service: async () =>
        ApiService('protected').fetchData<GetUserLitterReports_Response>({
            url: `/litterreport/userlitterreports/${params.userId}`,
            method: 'get',
        }),
});

export type GetLitterReportImage_Params = { litterImageId: string; imageSize: 'Thumb' | 'Reduced' | 'Raw' };
export type GetLitterReportImage_Response = string;
export const GetLitterReportImage = (params: GetLitterReportImage_Params) => ({
    key: ['/litterreport', 'image', params.litterImageId, params.imageSize],
    service: async () =>
        ApiService('public').fetchData<GetLitterReportImage_Response>({
            url: `/litterreport/image/${params.litterImageId}/${params.imageSize}`,
            method: 'get',
        }),
});
