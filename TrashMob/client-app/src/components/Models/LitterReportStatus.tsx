export enum LitterReportStatusEnum {
    New = 1,
    Assigned = 2,
    Cleaned = 3,
    Cancelled = 4,
}

export const LitterReportStatusLabels: Record<LitterReportStatusEnum, string> = {
    [LitterReportStatusEnum.New]: 'New',
    [LitterReportStatusEnum.Assigned]: 'Assigned',
    [LitterReportStatusEnum.Cleaned]: 'Cleaned',
    [LitterReportStatusEnum.Cancelled]: 'Cancelled',
};

export const LitterReportStatusColors: Record<LitterReportStatusEnum, string> = {
    [LitterReportStatusEnum.New]: 'bg-red-500',
    [LitterReportStatusEnum.Assigned]: 'bg-yellow-500',
    [LitterReportStatusEnum.Cleaned]: 'bg-green-500',
    [LitterReportStatusEnum.Cancelled]: 'bg-gray-500',
};
