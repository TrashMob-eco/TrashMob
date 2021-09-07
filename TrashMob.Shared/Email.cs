
namespace TrashMob.Shared
{
    using System.Collections.Generic;

    public class EmailAddress
    {
        public string Name { get; set; }
        
        public string Email { get; set; }
    }

    public class Email
    {
        public List<EmailAddress> Addresses { get; } = new List<EmailAddress>();
        
        public string Subject { get; set; }
        
        public string Message { get; set; }
        
        public string HtmlMessage { get; set; }
    }
}
