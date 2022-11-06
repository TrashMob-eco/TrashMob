namespace TrashMob.Poco
{
    using System.Collections.Generic;

    public class ActiveDirectoryNewUserRequest
    {
        public string Email { get; set; }

        public List<Identity> Identities { get; set; } = new List<Identity>();

        public string DisplayName { get; set; }

        public string ObjectId { get; set; }

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string Client_Id { get; set; }
    }

    public class Identity
    {
        public string signInType { get; set; }
        public string issuer { get; set; }
        public string issuerAssignedId { get; set; }

    }
}
