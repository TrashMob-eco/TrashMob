#nullable disable

namespace TrashMob.Models
{
    public class SiteMetric : KeyedModel
    {
        public DateTimeOffset ProcessedTime { get; set; }

        public string MetricType { get; set; }

        public long MetricValue { get; set; }
    }
}