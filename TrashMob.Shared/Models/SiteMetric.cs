#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class SiteMetric : KeyedModel
    {
        public SiteMetric()
        {
        }

        public DateTimeOffset ProcessedTime { get; set; }

        public string MetricType { get; set; }

        public long MetricValue { get; set; }
    }
}
