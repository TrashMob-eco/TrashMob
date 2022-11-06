namespace TrashMob.Poco
{
    using System.Collections.Generic;

    public class ActiveDirectoryNewUserRequest
    {
        public string step { get; set; }

        public string client_id { get; set; }

        public string ui_locales { get; set; }

        public string email { get; set; }

        public List<Identity> identities { get; set; } = new List<Identity>();

        public string displayName { get; set; }

        public string givenName { get; set; }

        public string surname { get; set; }

    }

    public class Identity
    {
        public string signInType { get; set; }
        
        public string issuer { get; set; }
        
        public string issuerAssignedId { get; set; }
    }
}
