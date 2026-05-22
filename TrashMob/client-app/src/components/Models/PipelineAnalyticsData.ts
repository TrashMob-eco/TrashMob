export class PipelineStageStat {
    stage: number = 0;
    label: string = '';
    count: number = 0;
}

export class ProspectTypeStat {
    type: string = '';
    count: number = 0;
    convertedCount: number = 0;
}

export class UserTouchpointStat {
    userId: string = '';
    userName: string = '';
    activityCount: number = 0;
    outreachEmailCount: number = 0;
    totalTouchpoints: number = 0;
}

class PipelineAnalyticsData {
    stageCounts: PipelineStageStat[] = [];
    totalProspects: number = 0;
    totalEmailsSent: number = 0;
    totalEmailsOpened: number = 0;
    totalEmailsClicked: number = 0;
    totalEmailsBounced: number = 0;
    openRate: number = 0;
    clickRate: number = 0;
    bounceRate: number = 0;
    convertedCount: number = 0;
    conversionRate: number = 0;
    averageDaysInPipeline: number = 0;
    typeBreakdown: ProspectTypeStat[] = [];
    touchpointsByUserLast30Days: UserTouchpointStat[] = [];
    touchpointsByUserLast90Days: UserTouchpointStat[] = [];
}

export default PipelineAnalyticsData;
