using TrashMobMobileApp.Enums;

namespace TrashMobMobileApp.StateContainers
{
    public class EventStateInformation
    {
        public Action<UserEventInteraction> UserEventInteractionAction { get; set; }
    }
}
