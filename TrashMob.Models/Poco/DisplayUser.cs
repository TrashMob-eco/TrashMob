namespace TrashMob.Models.Poco
{
    using System;

    public class DisplayUser
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public DateTimeOffset? MemberSince { get; set; }
    }
}