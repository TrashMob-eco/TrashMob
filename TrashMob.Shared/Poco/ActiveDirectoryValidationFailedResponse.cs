namespace TrashMob.Poco
{
    public class ActiveDirectoryValidationFailedResponse : ActiveDirectoryResponseBase
    {
        public string userMessage { get; set; }

        public string status { get; set; }
    }
}
