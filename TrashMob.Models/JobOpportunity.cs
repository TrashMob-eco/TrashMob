#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a job or volunteer opportunity within the TrashMob organization.
    /// </summary>
    public class JobOpportunity : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobOpportunity"/> class.
        /// </summary>
        public JobOpportunity()
        {
        }

        /// <summary>
        /// Gets or sets the title of the job opportunity.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a short tagline or summary of the opportunity.
        /// </summary>
        public string TagLine { get; set; }

        /// <summary>
        /// Gets or sets the full description of the job opportunity.
        /// </summary>
        public string FullDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the job opportunity is currently active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}