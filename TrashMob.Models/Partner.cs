#nullable disable

namespace TrashMob.Models
{
    public class Partner : KeyedModel
    {
        public Partner()
        {
            PartnerContacts = new HashSet<PartnerContact>();
            PartnerDocuments = new HashSet<PartnerDocument>();
            PartnerSocialMediaAccounts = new HashSet<PartnerSocialMediaAccount>();
            PartnerLocations = new HashSet<PartnerLocation>();
            PartnerAdmins = new HashSet<PartnerAdmin>();
            PartnerAdminInvitations = new HashSet<PartnerAdminInvitation>();
        }

        public string Name { get; set; }

        public int PartnerStatusId { get; set; }

        public int PartnerTypeId { get; set; }

        public string Website { get; set; }

        public string PublicNotes { get; set; }

        public string PrivateNotes { get; set; }

        public virtual ICollection<PartnerContact> PartnerContacts { get; set; }

        public virtual ICollection<PartnerDocument> PartnerDocuments { get; set; }

        public virtual ICollection<PartnerLocation> PartnerLocations { get; set; }

        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccounts { get; set; }

        public virtual ICollection<PartnerAdmin> PartnerAdmins { get; set; }

        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitations { get; set; }

        public virtual PartnerStatus PartnerStatus { get; set; }

        public virtual PartnerType PartnerType { get; set; }
    }
}