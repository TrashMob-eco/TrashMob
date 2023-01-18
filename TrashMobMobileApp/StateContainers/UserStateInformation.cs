namespace TrashMobMobileApp.StateContainers
{
    public class UserStateInformation
    {
        public Action OnSignOut { get; set; }
        public Stack<string> PageRouteStack { get; set; } = new Stack<string>();
        public int CurrentlyActiveTab { get; set; }
        public bool ShowFutureEvents { get; set; }
    }
}
