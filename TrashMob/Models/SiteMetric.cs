#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class SiteMetric
    {
        public SiteMetric()
        {
        }

        public Guid Id { get; set; }

        public DateTimeOffset ProcessedTime { get; set; }

        public string MetricType { get; set; }

        public long MetricValue { get; set; }
    }
}
