namespace TrashMobMobileApp.Authentication
{
    public class UserContext
    {
        public string EmailAddress { get; internal set; }

        public string GivenName { get; internal set; }

        public bool IsLoggedOn { get; internal set; }
                
        public string AccessToken { get; internal set; }        
    }
}