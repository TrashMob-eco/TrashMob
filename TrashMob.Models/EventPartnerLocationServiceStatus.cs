#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of a partner location service request for an event.
    /// </summary>
    public class EventPartnerLocationServiceStatus : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventPartnerLocationServiceStatus"/> class.
        /// </summary>
        public EventPartnerLocationServiceStatus()
        {
            EventPartnerLocationServices = new HashSet<EventPartnerLocationService>();
        }

        /// <summary>
        /// Gets or sets the collection of event partner location services with this status.
        /// </summary>
        public virtual ICollection<EventPartnerLocationService> EventPartnerLocationServices { get; set; }
    }
}