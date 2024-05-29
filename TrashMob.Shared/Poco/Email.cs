﻿namespace TrashMob.Shared.Poco
{
    using System.Collections.Generic;

    public class EmailAddress
    {
        public string Name { get; set; }

        public string Email { get; set; }
    }

    public class Email
    {
        public List<EmailAddress> Addresses { get; } = new();

        public string Subject { get; set; }

        public string Message { get; set; }

        public string HtmlMessage { get; set; }

        public object DynamicTemplateData { get; set; }

        public string TemplateId { get; set; }

        public int GroupId { get; set; }
    }
}