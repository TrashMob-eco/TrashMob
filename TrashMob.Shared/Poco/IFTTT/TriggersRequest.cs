namespace TrashMob.Shared.Poco.IFTTT
{
    public class TriggersRequest
    {
        public string trigger_identity { get; set; }

        public object triggerFields { get; set; }

        public int limit { get; set; } = 50;

        public object user { get; set; }

        public object ifttt_source { get; set; }
    }
}
