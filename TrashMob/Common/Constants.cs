namespace TrashMob.Common
{
    public static class Constants
    {
        public const string TrashMobReadScope = "TrashMob.Read";
        public const string TrashMobWriteScope = "TrashMob.Writes";
    }

    public enum EventStatusEnum
    {
        Active = 1,
        Full = 2,
        Canceled = 3,
        Complete = 4,
    }
}
