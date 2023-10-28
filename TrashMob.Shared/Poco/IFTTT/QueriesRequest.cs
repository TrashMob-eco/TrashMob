namespace TrashMob.Shared.Poco.IFTTT
{
    public class QueriesRequest
    {
        public string trigger_identity { get; set; }

        public object triggerFields { get; set; }

        public int limit { get; set; }

        public object user { get; set; }

        public object ifttt_source { get; set; }
    }
}
