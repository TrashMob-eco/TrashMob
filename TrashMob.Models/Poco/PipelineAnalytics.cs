#nullable enable

namespace TrashMob.Models.Poco
{
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
}
