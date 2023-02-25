namespace TrashMob.Poco
{
    using System;

    public class ActiveDirectoryUpdateUserProfileRequest
    {
        public Guid objectId { get; set; }

        public string userName { get; set; }

        public string givenName { get; set; }
    }
}
