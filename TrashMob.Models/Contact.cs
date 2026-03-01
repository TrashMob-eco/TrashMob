#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a contact in the CRM system — any person or organization the nonprofit interacts with.
    /// </summary>
    public class Contact : KeyedModel
    {
        public Contact()
        {
            Donations = [];
            ContactNotes = [];
            ContactContactTags = [];
            Pledges = [];
            GrantsAsFunderContact = [];
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string OrganizationName { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public int ContactType { get; set; }

        public string Source { get; set; }

        public Guid? UserId { get; set; }

        public Guid? PartnerId { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; }

        public virtual User User { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual ICollection<Donation> Donations { get; set; }

        public virtual ICollection<ContactNote> ContactNotes { get; set; }

        public virtual ICollection<ContactContactTag> ContactContactTags { get; set; }

        public virtual ICollection<Pledge> Pledges { get; set; }

        public virtual ICollection<Grant> GrantsAsFunderContact { get; set; }
    }
}
