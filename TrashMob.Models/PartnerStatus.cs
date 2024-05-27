#nullable disable

namespace TrashMob.Models
{
    public class PartnerStatus : LookupModel
    {
        public PartnerStatus()
        {
            Partners = new HashSet<Partner>();
        }

        public virtual ICollection<Partner> Partners { get; set; }
    }
}