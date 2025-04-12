#nullable disable

namespace TrashMob.Models
{
    public class JobOpportunity : KeyedModel
    {
        public JobOpportunity()
        {
        }

        public string Title { get; set; }

        public string TagLine { get; set; }

        public string FullDescription { get; set; }

        public bool IsActive { get; set; }
    }
}