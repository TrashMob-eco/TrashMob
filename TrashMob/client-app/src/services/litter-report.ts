import LitterReportData from "@/components/Models/LitterReportData";
import { ApiService } from ".";
import LitterImageData from "@/components/Models/LitterImageData";

export type GetLitterReports_Response = LitterReportData[];
export const GetLitterReports = () => ({
    key: ['/LitterReport', 'list'],
    service: async () =>
        ApiService('public').fetchData<GetLitterReports_Response>({
            url: '/litterreport',
            method: 'get',
        }),
});

export type CreateLitterReport_Body = LitterReportData;
export type CreateLitterReport_Response = unknown;
export const CreateLitterReport = () => ({
    key: ['/LitterReport', 'create'],
    service: async (body: CreateLitterReport_Body) =>
        ApiService('protected').fetchData<CreateLitterReport_Response, CreateLitterReport_Body>({
            url: '/litterreport',
            method: 'post',
            data: body,
        }),
});

export type UploadLitterImage_Body = { file: File, id: string, litterReportId?: string };
export type UploadLitterImage_Response = unknown;
export const UploadLitterImage = () => ({
    key: ['/LitterReport', 'image', 'upload'] as readonly string[],
    service: async (body: UploadLitterImage_Body) => {
        const formData = new FormData()
        const reportId = body.litterReportId
        const imageId = body.id
        formData.append("FormFile", body.file)
        formData.append("ParentId", imageId)
        formData.append("ImageType", "4")

        return ApiService('protected').fetchData<UploadLitterImage_Response, FormData>({
            url: `/litterreport/image/${imageId}`,
            method: 'post',
            data: formData
        })
    },
});