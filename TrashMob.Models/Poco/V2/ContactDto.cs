#nullable enable
namespace TrashMob.Models.Poco.V2
{
    using System;
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? OrganizationName { get; set; }
        public string? Title { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public int ContactType { get; set; }
        public string? Source { get; set; }
        public Guid? UserId { get; set; }
        public Guid? PartnerId { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
