namespace TrashMobMobileApp.StateContainers
{
    using TrashMobMobileApp.Enums;

    public class EventStateInformation
    {
        public Action<UserEventInteraction> UserEventInteractionAction { get; set; }

        public string EventId { get; set; }
    }
}
