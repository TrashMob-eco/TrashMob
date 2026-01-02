namespace TrashMobJobs
{
    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; } = new();

        public bool IsPastDue { get; set; }
    }
}