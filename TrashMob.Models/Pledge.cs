#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a multi-payment pledge from a contact.
    /// </summary>
    public class Pledge : KeyedModel
    {
        public Pledge()
        {
            Donations = [];
        }

        public Guid ContactId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }

        public int Frequency { get; set; }

        public int Status { get; set; }

        public string Notes { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Donation> Donations { get; set; }
    }
}
