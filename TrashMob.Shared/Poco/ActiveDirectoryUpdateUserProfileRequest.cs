namespace TrashMob.Shared.Poco
{
    using System;

    public class ActiveDirectoryUpdateUserProfileRequest
    {
        public Guid objectId { get; set; }

        public string userName { get; set; }
    }
}