namespace TrashMobMobileApp.StateContainers
{
    public class UserStateInformation
    {
        public Action OnSignOut { get; set; }

        public Stack<string> PageRouteStack { get; set; } = new Stack<string>();
        
        public int CurrentlyActiveMainTab { get; set; }
        
        public int CurrentlyActiveEditEventTab { get; set; }

        public int CurrentlyActiveCompleteEventTab { get; set; }

        public bool ShowFutureEvents { get; set; }

        public bool HasToSignWaiver { get; set; } = false;
    }
}
