namespace TrashMob.Poco
{
    using System;

    public partial class DisplayUser
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public DateTimeOffset? MemberSince { get; set; }
    }
}
