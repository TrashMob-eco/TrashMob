namespace TrashMob.Shared.Poco
{
    using System;

    public class ActiveDirectoryNewUserRequest
    {
        public string email { get; set; }

        public Guid objectId { get; set; }

        public string userName { get; set; }
    }
}