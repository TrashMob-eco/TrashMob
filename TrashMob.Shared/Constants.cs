namespace TrashMob.Shared
{
    public static class Constants
    {
        public const string TrashMobReadScope = "TrashMob.Read";
        public const string TrashMobWriteScope = "TrashMob.Writes";
        public const string TrashMobEmailAddress = "info@trashmob.eco";
        public const string TrashMobEmailName = "TrashMob Information";
    }

    public enum EventStatusEnum
    {
        Active = 1,
        Full = 2,
        Canceled = 3,
        Complete = 4,
    }
}
