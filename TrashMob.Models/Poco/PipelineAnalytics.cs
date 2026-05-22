#nullable enable

namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;

    public class PipelineAnalytics
    {
        public List<PipelineStageStat> StageCounts { get; set; } = [];
        public int TotalProspects { get; set; }

        // Outreach metrics
        public int TotalEmailsSent { get; set; }
        public int TotalEmailsOpened { get; set; }
        public int TotalEmailsClicked { get; set; }
        public int TotalEmailsBounced { get; set; }
        public double OpenRate { get; set; }
        public double ClickRate { get; set; }
        public double BounceRate { get; set; }

        // Conversion metrics
        public int ConvertedCount { get; set; }
        public double ConversionRate { get; set; }
        public double AverageDaysInPipeline { get; set; }

        // Breakdown by prospect type
        public List<ProspectTypeStat> TypeBreakdown { get; set; } = [];

        // Project 60 Phase 4: per-user touchpoint tally — total of ProspectActivity +
        // ProspectOutreachEmail rows that user created in the window. Used to gauge
        // volunteer effort against the pipeline (and as the foundation for a future
        // salesperson hire's billable-attempts report).
        public List<UserTouchpointStat> TouchpointsByUserLast30Days { get; set; } = [];
        public List<UserTouchpointStat> TouchpointsByUserLast90Days { get; set; } = [];
    }

    public class PipelineStageStat
    {
        public int Stage { get; set; }
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ProspectTypeStat
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
        public int ConvertedCount { get; set; }
    }

    public class UserTouchpointStat
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ActivityCount { get; set; }
        public int OutreachEmailCount { get; set; }
        public int TotalTouchpoints { get; set; }
    }
}
